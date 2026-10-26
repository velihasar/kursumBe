using Core.Entities;

namespace Core.Entities.Dtos.PaymentDto
{
    public class GetFinancialDashboardMetricsRequestDto : IDto
    {
        public int? TenantId { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
    }
}
