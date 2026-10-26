using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Dtos.TenantDto
{
    public class TenantDashboardSummaryDto : IDto
    {
        public int TenantId { get; set; }
        public string? TenantName { get; set; }
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalCourses { get; set; }
        public int TodayTotalAttendance { get; set; }
        public int TodayPresentCount { get; set; }
        public int TodayAbsentCount { get; set; }
        public int TodayLateCount { get; set; }
        public int TodayExcusedCount { get; set; }
        public int OverallAttendanceRate { get; set; }
        public List<RecentAttendanceActivityDto> RecentAttendances { get; set; } = new List<RecentAttendanceActivityDto>();
    }

    public class RecentAttendanceActivityDto : IDto
    {
        public int Id { get; set; }
        public string? StudentName { get; set; }
        public string? CourseName { get; set; }
        public string Status { get; set; }
        public string Time { get; set; }
        public DateTime AttendanceDate { get; set; }
    }
}
