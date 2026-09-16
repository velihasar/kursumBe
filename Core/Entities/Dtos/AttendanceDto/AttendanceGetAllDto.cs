using System;
using Core.Entities;

namespace Core.Entities.Dtos.AttendanceDto
{
    public class AttendanceGetAllDto : IDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public bool IsPresent { get; set; }
        public string Reason { get; set; }
        public bool? IsActive { get; set; }
    }
}
