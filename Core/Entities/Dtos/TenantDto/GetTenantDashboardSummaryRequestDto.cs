using Core.Entities;

namespace Core.Entities.Dtos.TenantDto
{
    public class GetTenantDashboardSummaryRequestDto : IDto
    {
        public int? TenantId { get; set; }
    }
}
