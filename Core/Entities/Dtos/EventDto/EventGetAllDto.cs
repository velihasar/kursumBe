using System;
using Core.Entities;

namespace Core.Entities.Dtos.EventDto
{
    public class EventGetAllDto : IDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Location { get; set; }
        public string TargetAudience { get; set; }
        public int TargetRole { get; set; }
        public int? Capacity { get; set; }
        public bool IsRegistrationRequired { get; set; }
        public string ImageUrl { get; set; }
        public string Icon { get; set; }
        public int Status { get; set; }
        public int? BranchId { get; set; }
        public string BranchName { get; set; }
        public bool? IsActive { get; set; }
    }
}
