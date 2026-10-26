using Core.Entities;

namespace Core.Entities.Dtos.AttendanceDto
{
    public class BulkAttendanceItemDto : IDto
    {
        public int? Id { get; set; }
        public int StudentId { get; set; }
        public bool IsPresent { get; set; }
        public string? Reason { get; set; }
    }
}
