using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.FeeDues.Commands
{
    public class GenerateMonthlyFeeDuesCommand : IRequest<IDataResult<int>>
    {
        public int? TenantId { get; set; }
        public string Period { get; set; } // ör. "2026-10", boş ise mevcut ay

        public class GenerateMonthlyFeeDuesCommandHandler : IRequestHandler<GenerateMonthlyFeeDuesCommand, IDataResult<int>>
        {
            private readonly IFeeDueRepository _feeDueRepository;
            private readonly ICourseEnrollmentRepository _courseEnrollmentRepository;
            private readonly IMediator _mediator;

            public GenerateMonthlyFeeDuesCommandHandler(
                IFeeDueRepository feeDueRepository,
                ICourseEnrollmentRepository courseEnrollmentRepository,
                IMediator mediator)
            {
                _feeDueRepository = feeDueRepository;
                _courseEnrollmentRepository = courseEnrollmentRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<int>> Handle(GenerateMonthlyFeeDuesCommand request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();
                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                var now = DateTime.Now;
                string targetPeriod = !string.IsNullOrWhiteSpace(request.Period)
                    ? request.Period.Trim()
                    : $"{now.Year}-{now.Month:D2}";

                // Dönem parçalarını parse et (YYYY-MM)
                int year = now.Year;
                int month = now.Month;
                var periodParts = targetPeriod.Split('-');
                if (periodParts.Length == 2 && int.TryParse(periodParts[0], out int pYear) && int.TryParse(periodParts[1], out int pMonth))
                {
                    year = pYear;
                    month = Math.Clamp(pMonth, 1, 12);
                }

                // 1. Tüm aktif kurs kayıtlarını getir
                var enrollmentQuery = _courseEnrollmentRepository.Query()
                    .Include(e => e.Course)
                    .Include(e => e.Student)
                    .Where(e => e.IsDeleted == false && e.IsActive == true && e.Status == 1); // 1: Aktif kayıt

                if (targetTenantId > 0)
                {
                    enrollmentQuery = enrollmentQuery.Where(e => e.TenantId == targetTenantId);
                }

                var activeEnrollments = await enrollmentQuery.ToListAsync(cancellationToken);

                // 2. Mevcut döneme ait zaten oluşturulmuş tahakkukları al (Çift kayıt engelleme)
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

                var existingSet = new System.Collections.Generic.HashSet<int>(existingEnrollmentIds);

                int createdCount = 0;
                string monthName = new DateTime(year, month, 1).ToString("MMMM", new System.Globalization.CultureInfo("tr-TR"));

                foreach (var enrollment in activeEnrollments)
                {
                    // Zaten bu dönem için aidatı var ise atla
                    if (existingSet.Contains(enrollment.Id))
                        continue;

                    // Aylık kurs mu kontrolü (FeeType == 1: Aylık veya fiyatı olan)
                    decimal dueAmount = 0;
                    if (enrollment.CustomMonthlyFee.HasValue && enrollment.CustomMonthlyFee.Value > 0)
                    {
                        dueAmount = enrollment.CustomMonthlyFee.Value;
                    }
                    else if (enrollment.Course != null && enrollment.Course.Price > 0)
                    {
                        dueAmount = enrollment.Course.Price;
                    }

                    // Tutar 0 ise atla
                    if (dueAmount <= 0)
                        continue;

                    // Vade günü hesabı (DueDayOfMonth veya ayın 15'i)
                    int dueDay = enrollment.DueDayOfMonth > 0 ? Math.Min(28, enrollment.DueDayOfMonth) : 15;
                    DateTime dueDate = new DateTime(year, month, dueDay);

                    string courseName = enrollment.Course?.Name ?? "Kurs";
                    string title = $"{monthName} {year} Aidatı - {courseName}";

                    var feeDue = new FeeDue
                    {
                        TenantId = enrollment.TenantId,
                        CourseEnrollmentId = enrollment.Id,
                        StudentId = enrollment.StudentId,
                        Period = targetPeriod,
                        Title = title,
                        Amount = dueAmount,
                        PaidAmount = 0,
                        RemainingAmount = dueAmount,
                        DueDate = dueDate,
                        Status = 0, // 0: Ödenmedi
                        Description = $"{enrollment.EnrollmentDate:dd.MM.yyyy} tarihli kurs kaydına istinaden otomatik oluşturuldu.",
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = userId > 0 ? userId : null,
                        CreatedDate = DateTime.Now
                    };

                    _feeDueRepository.Add(feeDue);
                    existingSet.Add(enrollment.Id);
                    createdCount++;
                }

                if (createdCount > 0)
                {
                    await _feeDueRepository.SaveChangesAsync();
                }

                return new SuccessDataResult<int>(createdCount, $"{targetPeriod} dönemi için {createdCount} adet aidat tahakkuku oluşturuldu.");
            }
        }
    }
}
