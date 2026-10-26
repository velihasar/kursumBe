using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.TenantDto;
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

namespace Business.Handlers.Tenants.Queries
{
    public class GetTenantDashboardSummaryQuery : IRequest<IDataResult<TenantDashboardSummaryDto>>
    {
        public GetTenantDashboardSummaryRequestDto Model { get; set; } = new GetTenantDashboardSummaryRequestDto();

        public class GetTenantDashboardSummaryQueryHandler : IRequestHandler<GetTenantDashboardSummaryQuery, IDataResult<TenantDashboardSummaryDto>>
        {
            private readonly ITenantRepository _tenantRepository;
            private readonly IStudentRepository _studentRepository;
            private readonly ITeacherRepository _teacherRepository;
            private readonly ICourseRepository _courseRepository;
            private readonly IAttendanceRepository _attendanceRepository;
            private readonly IMediator _mediator;

            public GetTenantDashboardSummaryQueryHandler(
                ITenantRepository tenantRepository,
                IStudentRepository studentRepository,
                ITeacherRepository teacherRepository,
                ICourseRepository courseRepository,
                IAttendanceRepository attendanceRepository,
                IMediator mediator)
            {
                _tenantRepository = tenantRepository;
                _studentRepository = studentRepository;
                _teacherRepository = teacherRepository;
                _courseRepository = courseRepository;
                _attendanceRepository = attendanceRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<TenantDashboardSummaryDto>> Handle(GetTenantDashboardSummaryQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                int targetTenantId = userTenantId > 0 ? userTenantId : (request.Model?.TenantId ?? 0);

                if (targetTenantId <= 0)
                {
                    // SuperAdmin için ilk aktif kurumu seçebiliriz
                    var firstTenant = await _tenantRepository.Query()
                        .FirstOrDefaultAsync(t => t.IsDeleted == false && t.IsActive == true, cancellationToken);
                    if (firstTenant != null)
                    {
                        targetTenantId = firstTenant.Id;
                    }
                    else
                    {
                        return new ErrorDataResult<TenantDashboardSummaryDto>("Geçerli bir kurum bulunamadı.");
                    }
                }

                var tenant = await _tenantRepository.GetAsync(t => t.Id == targetTenantId && t.IsDeleted == false);
                string tenantName = tenant?.Name ?? "Kurum";

                // Count metrics in DB
                int totalStudents = await _studentRepository.Query()
                    .CountAsync(s => s.TenantId == targetTenantId && s.IsDeleted == false, cancellationToken);

                int totalTeachers = await _teacherRepository.Query()
                    .CountAsync(t => t.TenantId == targetTenantId && t.IsDeleted == false, cancellationToken);

                int totalCourses = await _courseRepository.Query()
                    .CountAsync(c => c.TenantId == targetTenantId && c.IsDeleted == false && c.IsActive == true, cancellationToken);

                // Today's attendances
                var todayStart = DateTime.Today;
                var todayEnd = todayStart.AddDays(1);

                var todayAttendances = await _attendanceRepository.Query()
                    .Where(a => a.TenantId == targetTenantId && a.IsDeleted == false && a.AttendanceDate >= todayStart && a.AttendanceDate < todayEnd)
                    .Select(a => new { a.IsPresent, a.Reason })
                    .ToListAsync(cancellationToken);

                int todayPresent = 0;
                int todayAbsent = 0;
                int todayLate = 0;
                int todayExcused = 0;

                foreach (var a in todayAttendances)
                {
                    if (a.IsPresent)
                    {
                        todayPresent++;
                    }
                    else
                    {
                        string reason = (a.Reason ?? string.Empty).ToLower();
                        if (reason.Contains("izin") || reason.Contains("rapor")) todayExcused++;
                        else if (reason.Contains("geç") || reason.Contains("gecik")) todayLate++;
                        else todayAbsent++;
                    }
                }

                int todayTotal = todayAttendances.Count;

                // Overall rate
                int totalAll = await _attendanceRepository.Query()
                    .CountAsync(a => a.TenantId == targetTenantId && a.IsDeleted == false, cancellationToken);

                int presentAll = await _attendanceRepository.Query()
                    .CountAsync(a => a.TenantId == targetTenantId && a.IsDeleted == false && a.IsPresent, cancellationToken);

                int overallRate = totalAll > 0 ? (int)Math.Round(((double)presentAll / totalAll) * 100) : 100;

                // Recent 5 attendances
                var recentList = await _attendanceRepository.Query()
                    .Include(a => a.Course)
                    .Include(a => a.Student).ThenInclude(s => s.Person)
                    .Where(a => a.TenantId == targetTenantId && a.IsDeleted == false)
                    .OrderByDescending(a => a.AttendanceDate)
                    .ThenByDescending(a => a.Id)
                    .Take(5)
                    .ToListAsync(cancellationToken);

                var recentDtos = recentList.Select(a =>
                {
                    string status = a.IsPresent ? "Geldi" : "Gelmedi";
                    string r = (a.Reason ?? string.Empty).ToLower();
                    if (r.Contains("izin") || r.Contains("rapor")) status = "İzinli";
                    else if (r.Contains("geç") || r.Contains("gecik")) status = "Geç Kaldı";

                    var studentName = a.Student?.Person != null
                        ? $"{a.Student.Person.FirstName} {a.Student.Person.LastName}".Trim()
                        : (a.Student != null ? $"Öğrenci #{a.StudentId}" : "Öğrenci");

                    return new RecentAttendanceActivityDto
                    {
                        Id = a.Id,
                        StudentName = studentName,
                        CourseName = a.Course?.Name ?? "Ders",
                        Status = status,
                        Time = a.AttendanceDate.ToString("HH:mm"),
                        AttendanceDate = a.AttendanceDate
                    };
                }).ToList();

                var dto = new TenantDashboardSummaryDto
                {
                    TenantId = targetTenantId,
                    TenantName = tenantName,
                    TotalStudents = totalStudents,
                    TotalTeachers = totalTeachers,
                    TotalCourses = totalCourses,
                    TodayTotalAttendance = todayTotal,
                    TodayPresentCount = todayPresent,
                    TodayAbsentCount = todayAbsent,
                    TodayLateCount = todayLate,
                    TodayExcusedCount = todayExcused,
                    OverallAttendanceRate = overallRate,
                    RecentAttendances = recentDtos
                };

                return new SuccessDataResult<TenantDashboardSummaryDto>(dto);
            }
        }
    }
}
