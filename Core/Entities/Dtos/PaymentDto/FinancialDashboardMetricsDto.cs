using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Dtos.PaymentDto
{
    public class FinancialDashboardMetricsDto : IDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalWalletBalance { get; set; }
        public decimal ThisMonthTotalExpected { get; set; }
        public decimal ThisMonthPaid { get; set; }
        public decimal ThisMonthRemaining { get; set; }
        public int CollectionRate { get; set; }
        public decimal OverdueTotalAmount { get; set; }
        public int OverdueDuesCount { get; set; }
        public List<RecentPaymentItemDto> RecentPayments { get; set; } = new();
    }
}
