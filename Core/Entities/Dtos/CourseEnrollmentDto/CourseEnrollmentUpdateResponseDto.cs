using System;
using Core.Entities;

namespace Core.Entities.Dtos.CourseEnrollmentDto
{
    public class CourseEnrollmentUpdateResponseDto : IDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public decimal? CustomMonthlyFee { get; set; }
        public int DueDayOfMonth { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
    }
}
