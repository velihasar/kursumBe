using System;
using Core.Entities;

namespace Core.Entities.Dtos.FeeDueDto
{
    public class FeeDueGetAllDto : IDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public int CourseEnrollmentId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Period { get; set; }
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public DateTime DueDate { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
