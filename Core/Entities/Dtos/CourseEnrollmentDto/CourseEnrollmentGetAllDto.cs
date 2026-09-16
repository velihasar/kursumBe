using System;
using Core.Entities;

namespace Core.Entities.Dtos.CourseEnrollmentDto
{
    public class CourseEnrollmentGetAllDto : IDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public decimal? CustomMonthlyFee { get; set; }
        public int DueDayOfMonth { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public bool? IsActive { get; set; }
    }
}
