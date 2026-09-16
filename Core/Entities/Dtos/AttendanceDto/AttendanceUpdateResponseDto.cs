using System;
using Core.Entities;

namespace Core.Entities.Dtos.AttendanceDto
{
    public class AttendanceUpdateResponseDto : IDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public bool IsPresent { get; set; }
        public string Reason { get; set; }
    }
}
