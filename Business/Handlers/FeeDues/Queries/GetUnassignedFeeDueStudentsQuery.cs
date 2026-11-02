using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.FeeDueDto;
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

namespace Business.Handlers.FeeDues.Queries
{
    public class GetUnassignedFeeDueStudentsQuery : IRequest<IDataResult<UnassignedFeeDuesSummaryDto>>
    {
        public string Period { get; set; } // örn: "2026-11"
        public int? TenantId { get; set; }
        public int? CourseId { get; set; }
        public string Search { get; set; }

        public class GetUnassignedFeeDueStudentsQueryHandler : IRequestHandler<GetUnassignedFeeDueStudentsQuery, IDataResult<UnassignedFeeDuesSummaryDto>>
        {
            private readonly ICourseEnrollmentRepository _courseEnrollmentRepository;
            private readonly IFeeDueRepository _feeDueRepository;

            public GetUnassignedFeeDueStudentsQueryHandler(
                ICourseEnrollmentRepository courseEnrollmentRepository,
                IFeeDueRepository feeDueRepository)
            {
                _courseEnrollmentRepository = courseEnrollmentRepository;
                _feeDueRepository = feeDueRepository;
            }

            [PerformanceAspect(5)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<UnassignedFeeDuesSummaryDto>> Handle(GetUnassignedFeeDueStudentsQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                var now = DateTime.Now;
                string targetPeriod = !string.IsNullOrWhiteSpace(request.Period)
                    ? request.Period.Trim()
                    : $"{now.Year}-{now.Month:D2}";

                int year = now.Year;
                int month = now.Month;
                var periodParts = targetPeriod.Split('-');
                if (periodParts.Length == 2 && int.TryParse(periodParts[0], out int pYear) && int.TryParse(periodParts[1], out int pMonth))
                {
                    year = pYear;
                    month = Math.Clamp(pMonth, 1, 12);
                }

                // 1. İlgili döneme ait zaten oluşturulmuş tahakkukların CourseEnrollmentId'lerini al
                var existingDueQuery = _feeDueRepository.Query()
                    .Where(f => f.IsDeleted == false && f.Period == targetPeriod);

                if (targetTenantId > 0)
                {
                    existingDueQuery = existingDueQuery.Where(f => f.TenantId == targetTenantId);
                }

                var existingEnrollmentIds = await existingDueQuery
                    .Select(f => f.CourseEnrollmentId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                var existingSet = new HashSet<int>(existingEnrollmentIds);

                // 2. Aktif kurs kayıtlarını sorgula
                var enrollmentQuery = _courseEnrollmentRepository.Query()
                    .Include(e => e.Course)
                    .Include(e => e.Student).ThenInclude(s => s.Person)
                    .Include(e => e.Student).ThenInclude(s => s.Parents).ThenInclude(p => p.Parent).ThenInclude(pr => pr.Person)
                    .Where(e => e.IsDeleted == false && e.IsActive == true && e.Status == 1);

                if (targetTenantId > 0)
                {
                    enrollmentQuery = enrollmentQuery.Where(e => e.TenantId == targetTenantId);
                }

                if (request.CourseId.HasValue && request.CourseId.Value > 0)
                {
                    enrollmentQuery = enrollmentQuery.Where(e => e.CourseId == request.CourseId.Value);
                }

                var enrollments = await enrollmentQuery.ToListAsync(cancellationToken);

                // 3. Aidatı henüz oluşturulmamış ve ücreti > 0 olanları filtrele
                var unassignedList = new List<UnassignedFeeDueStudentDto>();

                foreach (var enrollment in enrollments)
                {
                    if (existingSet.Contains(enrollment.Id))
                        continue;

                    decimal suggestedAmount = 0;
                    if (enrollment.CustomMonthlyFee.HasValue && enrollment.CustomMonthlyFee.Value > 0)
                    {
                        suggestedAmount = enrollment.CustomMonthlyFee.Value;
                    }
                    else if (enrollment.Course != null && enrollment.Course.Price > 0)
                    {
                        suggestedAmount = enrollment.Course.Price;
                    }

                    if (suggestedAmount <= 0)
                        continue;

                    int dueDay = enrollment.DueDayOfMonth > 0 ? Math.Min(28, enrollment.DueDayOfMonth) : 15;
                    DateTime suggestedDueDate = new DateTime(year, month, dueDay);

                    var studentPerson = enrollment.Student?.Person;
                    string studentFullName = studentPerson != null
                        ? $"{studentPerson.FirstName} {studentPerson.LastName}".Trim()
                        : $"Öğrenci #{enrollment.StudentId}";

                    // Birincil veli bilgisi
                    var primaryParentRel = enrollment.Student?.Parents?.FirstOrDefault(p => p.IsPrimary)
                                          ?? enrollment.Student?.Parents?.FirstOrDefault();
                    var parentPerson = primaryParentRel?.Parent?.Person;
                    string parentFullName = parentPerson != null
                        ? $"{parentPerson.FirstName} {parentPerson.LastName}".Trim()
                        : null;
                    string parentPhone = parentPerson?.Phone;

                    // Arama filtresi backend'de uygulanır
                    if (!string.IsNullOrWhiteSpace(request.Search))
                    {
                        var search = request.Search.Trim().ToLower();
                        bool matchStudent = studentFullName.ToLower().Contains(search);
                        bool matchCourse = (enrollment.Course?.Name ?? "").ToLower().Contains(search);
                        bool matchParent = (parentFullName ?? "").ToLower().Contains(search);
                        bool matchNumber = (enrollment.Student?.StudentNumber ?? "").ToLower().Contains(search);

                        if (!matchStudent && !matchCourse && !matchParent && !matchNumber)
                            continue;
                    }

                    unassignedList.Add(new UnassignedFeeDueStudentDto
                    {
                        CourseEnrollmentId = enrollment.Id,
                        StudentId = enrollment.StudentId,
                        StudentName = studentFullName,
                        StudentNumber = enrollment.Student?.StudentNumber,
                        ParentName = parentFullName,
                        ParentPhoneNumber = parentPhone,
                        CourseId = enrollment.CourseId,
                        CourseName = enrollment.Course?.Name ?? "Kurs",
                        BranchId = enrollment.Course?.BranchId,
                        BranchName = null,
                        SuggestedAmount = suggestedAmount,
                        DueDayOfMonth = dueDay,
                        SuggestedDueDate = suggestedDueDate
                    });
                }

                var summary = new UnassignedFeeDuesSummaryDto
                {
                    Period = targetPeriod,
                    UnassignedCount = unassignedList.Count,
                    TotalEstimatedAmount = unassignedList.Sum(s => s.SuggestedAmount),
                    Students = unassignedList
                };

                return new SuccessDataResult<UnassignedFeeDuesSummaryDto>(summary);
            }
        }
    }
}

