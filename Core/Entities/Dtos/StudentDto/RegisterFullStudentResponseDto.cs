using Core.Entities;

namespace Core.Entities.Dtos.StudentDto
{
    public class RegisterFullStudentResponseDto : IDto
    {
        public int StudentId { get; set; }
        public int PersonId { get; set; }
        public string? StudentNumber { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public string? ParentFullName { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
