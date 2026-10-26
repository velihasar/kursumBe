using Core.Entities;

namespace Core.Entities.Dtos.AttendanceDto
{
    public class BulkAttendanceResponseDto : IDto
    {
        public int TotalProcessed { get; set; }
        public int CreatedCount { get; set; }
        public int UpdatedCount { get; set; }
        public string Message { get; set; }
    }
}
