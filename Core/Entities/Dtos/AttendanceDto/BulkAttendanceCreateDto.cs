using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Dtos.AttendanceDto
{
    public class BulkAttendanceCreateDto : IDto
    {
        public int? TenantId { get; set; }
        public int CourseId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public List<BulkAttendanceItemDto> Records { get; set; } = new List<BulkAttendanceItemDto>();
    }
}
