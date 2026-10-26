using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.PaymentDto;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Payments.Queries
{
    public class GetFinancialDashboardMetricsQuery : IRequest<IDataResult<FinancialDashboardMetricsDto>>
    {
        public GetFinancialDashboardMetricsRequestDto Model { get; set; } = new();

        public class GetFinancialDashboardMetricsQueryHandler : IRequestHandler<GetFinancialDashboardMetricsQuery, IDataResult<FinancialDashboardMetricsDto>>
        {
            private readonly IPaymentRepository _paymentRepository;
            private readonly IFeeDueRepository _feeDueRepository;
            private readonly IStudentWalletRepository _studentWalletRepository;

            public GetFinancialDashboardMetricsQueryHandler(
                IPaymentRepository paymentRepository,
                IFeeDueRepository feeDueRepository,
                IStudentWalletRepository studentWalletRepository)
            {
                _paymentRepository = paymentRepository;
                _feeDueRepository = feeDueRepository;
                _studentWalletRepository = studentWalletRepository;
            }

            [PerformanceAspect(5)]
            [CacheAspect(5)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<FinancialDashboardMetricsDto>> Handle(GetFinancialDashboardMetricsQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                int targetTenantId = userTenantId > 0 ? userTenantId : (request.Model.TenantId ?? 0);

                var paymentsBase = _paymentRepository.Query().Where(p => p.IsDeleted == false);
                var feeDuesBase = _feeDueRepository.Query().Where(f => f.IsDeleted == false);
                var walletsBase = _studentWalletRepository.Query().Where(w => w.IsDeleted == false);

                if (targetTenantId > 0)
                {
                    paymentsBase = paymentsBase.Where(p => p.TenantId == targetTenantId);
                    feeDuesBase = feeDuesBase.Where(f => f.TenantId == targetTenantId);
                    walletsBase = walletsBase.Where(w => w.TenantId == targetTenantId);
                }

                // 1. Total Revenue
                decimal totalRevenue = await paymentsBase.SumAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0m;

                // 2. Total Wallet Balance
                decimal totalWalletBalance = await walletsBase.SumAsync(w => (decimal?)w.Balance, cancellationToken) ?? 0m;

                // 3. This Month calculations
                int targetYear = request.Model.Year ?? DateTime.Now.Year;
                int targetMonth = request.Model.Month ?? DateTime.Now.Month;
                string currentYearMonth = $"{targetYear}-{targetMonth:D2}";

                var thisMonthDuesList = await feeDuesBase
                    .Where(f => (f.Period != null && f.Period.StartsWith(currentYearMonth)) || (f.DueDate.Year == targetYear && f.DueDate.Month == targetMonth))
                    .ToListAsync(cancellationToken);

                var duesForCalc = thisMonthDuesList.Count > 0 ? thisMonthDuesList : await feeDuesBase.ToListAsync(cancellationToken);

                decimal thisMonthTotalExpected = duesForCalc.Sum(f => f.Amount);
                decimal thisMonthPaid = duesForCalc.Sum(f => f.PaidAmount);
                decimal thisMonthRemaining = duesForCalc.Sum(f => f.RemainingAmount > 0 ? f.RemainingAmount : Math.Max(0, f.Amount - f.PaidAmount));

                int collectionRate = thisMonthTotalExpected > 0
                    ? Math.Min(100, (int)Math.Round((thisMonthPaid / thisMonthTotalExpected) * 100))
                    : (totalRevenue > 0 ? 100 : 0);

                // 4. Overdue Dues
                var today = DateTime.Today;
                var overdueList = await feeDuesBase
                    .Where(f => f.DueDate < today && f.Status != 2 && (f.RemainingAmount > 0 || f.Amount > f.PaidAmount))
                    .ToListAsync(cancellationToken);

                decimal overdueTotalAmount = overdueList.Sum(f => f.RemainingAmount > 0 ? f.RemainingAmount : Math.Max(0, f.Amount - f.PaidAmount));
                int overdueDuesCount = overdueList.Count;

                // 5. Recent Payments (Top 5)
                var recentPaymentsRaw = await paymentsBase
                    .Include(p => p.Student).ThenInclude(s => s.Person)
                    .OrderByDescending(p => p.PaymentDate)
                    .Take(5)
                    .ToListAsync(cancellationToken);

                var recentPayments = recentPaymentsRaw.Select(p => new RecentPaymentItemDto
                {
                    Id = p.Id,
                    StudentId = p.StudentId,
                    StudentFullName = p.Student?.Person != null ? $"{p.Student.Person.FirstName} {p.Student.Person.LastName}".Trim() : "Bilinmeyen Öğrenci",
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    PaymentType = p.PaymentType switch
                    {
                        1 => "Nakit",
                        2 => "Kredi Kartı",
                        3 => "Havale / EFT",
                        4 => "POS / Çek",
                        _ => "Diğer"
                    },
                    ReceiptNo = p.ReceiptNo
                }).ToList();

                var result = new FinancialDashboardMetricsDto
                {
                    TotalRevenue = totalRevenue,
                    TotalWalletBalance = totalWalletBalance,
                    ThisMonthTotalExpected = thisMonthTotalExpected,
                    ThisMonthPaid = thisMonthPaid,
                    ThisMonthRemaining = thisMonthRemaining,
                    CollectionRate = collectionRate,
                    OverdueTotalAmount = overdueTotalAmount,
                    OverdueDuesCount = overdueDuesCount,
                    RecentPayments = recentPayments
                };

                return new SuccessDataResult<FinancialDashboardMetricsDto>(result);
            }
        }
    }
}
