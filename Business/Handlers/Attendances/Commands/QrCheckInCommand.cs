using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.AttendanceDto;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Attendances.Commands
{
    public class QrCheckInCommand : IRequest<IDataResult<QrCheckInResponseDto>>
    {
        public int StudentId { get; set; }
        public string QrCode { get; set; }
        public DateTime? CheckInTime { get; set; }
        public int? TenantId { get; set; }

        public class QrCheckInCommandHandler : IRequestHandler<QrCheckInCommand, IDataResult<QrCheckInResponseDto>>
        {
            private readonly IAttendanceRepository _attendanceRepository;
            private readonly ICourseEnrollmentRepository _courseEnrollmentRepository;
            private readonly ICourseRepository _courseRepository;
            private readonly IStudentRepository _studentRepository;
            private readonly IMediator _mediator;

            public QrCheckInCommandHandler(
                IAttendanceRepository attendanceRepository,
                ICourseEnrollmentRepository courseEnrollmentRepository,
                ICourseRepository courseRepository,
                IStudentRepository studentRepository,
                IMediator mediator)
            {
                _attendanceRepository = attendanceRepository;
                _courseEnrollmentRepository = courseEnrollmentRepository;
                _courseRepository = courseRepository;
                _studentRepository = studentRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<QrCheckInResponseDto>> Handle(QrCheckInCommand request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                var student = await _studentRepository.Query()
                    .Include(s => s.Person)
                    .FirstOrDefaultAsync(s => s.Id == request.StudentId && s.IsDeleted == false, cancellationToken);

                if (student == null)
                {
                    return new ErrorDataResult<QrCheckInResponseDto>("Öğrenci kaydı bulunamadı.");
                }

                if (targetTenantId <= 0)
                {
                    targetTenantId = student.TenantId;
                }

                // Get student's active enrollments with courses
                var enrollments = await _courseEnrollmentRepository.Query()
                    .Include(ce => ce.Course)
                    .Where(ce => ce.StudentId == request.StudentId && ce.Status == 1 && ce.IsDeleted == false)
                    .ToListAsync(cancellationToken);

                if (enrollments == null || !enrollments.Any())
                {
                    return new ErrorDataResult<QrCheckInResponseDto>("Öğrencinin kayıtlı olduğu aktif bir ders bulunamadı.");
                }

                var now = DateTime.Now;
                if (request.CheckInTime.HasValue)
                {
                    var dt = request.CheckInTime.Value;
                    now = dt.Kind == DateTimeKind.Utc ? dt.ToLocalTime() : dt;
                }
                var today = now.Date;
                var dayOfWeek = now.DayOfWeek;

                string turkishDay = dayOfWeek switch
                {
                    DayOfWeek.Monday => "Pazartesi",
                    DayOfWeek.Tuesday => "Salı",
                    DayOfWeek.Wednesday => "Çarşamba",
                    DayOfWeek.Thursday => "Perşembe",
                    DayOfWeek.Friday => "Cuma",
                    DayOfWeek.Saturday => "Cumartesi",
                    DayOfWeek.Sunday => "Pazar",
                    _ => ""
                };
                string englishDay = dayOfWeek.ToString();
                int dayNum = ((int)dayOfWeek == 0) ? 7 : (int)dayOfWeek;

                // Check courses scheduled for today
                var todayCourses = enrollments
                    .Select(e => e.Course)
                    .Where(c => c != null && c.IsDeleted == false && (
                        string.IsNullOrWhiteSpace(c.DaysOfWeek) ||
                        c.DaysOfWeek.ToLowerInvariant().Contains(turkishDay.ToLowerInvariant()) ||
                        c.DaysOfWeek.ToLowerInvariant().Contains(englishDay.ToLowerInvariant()) ||
                        c.DaysOfWeek.Contains(dayNum.ToString()) ||
                        c.DaysOfWeek.Contains(((int)dayOfWeek).ToString())
                    ))
                    .ToList();

                if (!todayCourses.Any())
                {
                    return new ErrorDataResult<QrCheckInResponseDto>($"Öğrencinin bugün ({turkishDay}) için kayıtlı bir ders programı bulunmamaktadır.");
                }

                Course matchingCourse = null;
                DateTime matchedLessonStart = today;
                DateTime matchedLessonEnd = today;

                foreach (var course in todayCourses)
                {
                    var startTs = ParseTime(course.StartTime) ?? new TimeSpan(now.Hour, 0, 0);
                    var endTs = ParseTime(course.EndTime) ?? startTs.Add(TimeSpan.FromHours(1.5));

                    var lessonStart = today.Add(startTs);
                    var lessonEnd = today.Add(endTs);

                    // 15 mins before lesson start, up to 60 mins after lesson end
                    var validWindowStart = lessonStart.AddMinutes(-15);
                    var validWindowEnd = lessonEnd.AddMinutes(60);

                    if (now >= validWindowStart && now <= validWindowEnd)
                    {
                        matchingCourse = course;
                        matchedLessonStart = lessonStart;
                        matchedLessonEnd = lessonEnd;
                        break;
                    }
                }

                // If no course in exact window, check if there's only 1 course today and user is within a broader day margin, or show info
                if (matchingCourse == null)
                {
                    var nextCourse = todayCourses.FirstOrDefault();
                    var startTs = ParseTime(nextCourse?.StartTime);
                    string timeInfo = startTs.HasValue ? $" Saat: {startTs.Value:hh\\:mm}" : "";
                    return new ErrorDataResult<QrCheckInResponseDto>(
                        $"Şu an geçerli bir ders saati aralığında değilsiniz. Bugün: '{nextCourse?.Name}'{timeInfo}. Yoklama ders başlangıcından 15 dk önce ile bitişinden 60 dk sonrasına kadar alınabilir."
                    );
                }

                string studentFullName = $"{student.Person?.FirstName} {student.Person?.LastName}".Trim();

                // Check if already checked in today for this course
                var existingAttendance = await _attendanceRepository.Query()
                    .FirstOrDefaultAsync(a => a.IsDeleted == false && a.StudentId == request.StudentId && a.CourseId == matchingCourse.Id && a.AttendanceDate.Date == today, cancellationToken);

                if (existingAttendance != null)
                {
                    if (existingAttendance.IsPresent)
                    {
                        var response = new QrCheckInResponseDto
                        {
                            AttendanceId = existingAttendance.Id,
                            StudentId = request.StudentId,
                            StudentName = studentFullName,
                            CourseId = matchingCourse.Id,
                            CourseName = matchingCourse.Name,
                            CheckInTime = existingAttendance.AttendanceDate,
                            Status = "Present",
                            Message = $"{studentFullName} - '{matchingCourse.Name}' dersi için zaten giriş yapılmış. ({existingAttendance.AttendanceDate:HH:mm})"
                        };
                        return new SuccessDataResult<QrCheckInResponseDto>(response, response.Message);
                    }
                    else
                    {
                        existingAttendance.IsPresent = true;
                        existingAttendance.AttendanceDate = now;
                        existingAttendance.Reason = "QR ile Giriş Yapıldı";
                        _attendanceRepository.Update(existingAttendance);
                        await _attendanceRepository.SaveChangesAsync();

                        var response = new QrCheckInResponseDto
                        {
                            AttendanceId = existingAttendance.Id,
                            StudentId = request.StudentId,
                            StudentName = studentFullName,
                            CourseId = matchingCourse.Id,
                            CourseName = matchingCourse.Name,
                            CheckInTime = now,
                            Status = "Present",
                            Message = $"{studentFullName} - '{matchingCourse.Name}' dersine giriş kaydedildi. ({now:HH:mm})"
                        };
                        return new SuccessDataResult<QrCheckInResponseDto>(response, response.Message);
                    }
                }

                // Create new Attendance record
                var newAttendance = new Attendance
                {
                    TenantId = targetTenantId,
                    CourseId = matchingCourse.Id,
                    StudentId = request.StudentId,
                    AttendanceDate = now,
                    IsPresent = true,
                    Reason = "QR ile Giriş Yapıldı",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedDate = now
                };

                _attendanceRepository.Add(newAttendance);
                await _attendanceRepository.SaveChangesAsync();

                var successResponse = new QrCheckInResponseDto
                {
                    AttendanceId = newAttendance.Id,
                    StudentId = request.StudentId,
                    StudentName = studentFullName,
                    CourseId = matchingCourse.Id,
                    CourseName = matchingCourse.Name,
                    CheckInTime = now,
                    Status = "Present",
                    Message = $"{studentFullName} - '{matchingCourse.Name}' dersine giriş başarıyla kaydedildi. ({now:HH:mm})"
                };

                return new SuccessDataResult<QrCheckInResponseDto>(successResponse, successResponse.Message);
            }

            private static TimeSpan? ParseTime(string timeStr)
            {
                if (string.IsNullOrWhiteSpace(timeStr)) return null;
                if (TimeSpan.TryParse(timeStr.Trim(), out var ts)) return ts;
                var parts = timeStr.Trim().Split(':', '.');
                if (parts.Length >= 2 && int.TryParse(parts[0], out int h) && int.TryParse(parts[1], out int m))
                {
                    return new TimeSpan(h, m, 0);
                }
                return null;
            }
        }
    }
}
