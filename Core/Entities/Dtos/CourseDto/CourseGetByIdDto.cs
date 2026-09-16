using System;
using Core.Entities;

namespace Core.Entities.Dtos.CourseDto
{
    public class CourseGetByIdDto : IDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int FeeType { get; set; }
        public int? Capacity { get; set; }
        public string DaysOfWeek { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int? TeacherId { get; set; }
        public string TeacherName { get; set; }
        public bool? IsActive { get; set; }
    }
}
