using System;
using Core.Entities;

namespace Core.Entities.Dtos.AnnouncementDto
{
    public class AnnouncementGetAllDto : IDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string Tag { get; set; }
        public string Icon { get; set; }
        public string ImageUrl { get; set; }
        public bool IsImportant { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int TargetAudience { get; set; }
        public int? BranchId { get; set; }
        public string BranchName { get; set; }
        public bool? IsActive { get; set; }
    }
}
