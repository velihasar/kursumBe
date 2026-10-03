using Core.Entities;
using System;

namespace Core.Entities.Dtos.AttendanceDto
{
    public class QrCheckInResponseDto : IDto
    {
        public int AttendanceId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public DateTime CheckInTime { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
