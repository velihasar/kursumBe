using System;
using Core.Entities;

namespace Core.Entities.Dtos.StudentDto
{
    public class ExcelImportStudentRowDto : IDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? StudentNumber { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }

        // Parent Information
        public string? ParentFirstName { get; set; }
        public string? ParentLastName { get; set; }
        public string? ParentPhone { get; set; }
        public string? ParentEmail { get; set; }
        public string? Relationship { get; set; } = "Veli";
        public bool IsPrimary { get; set; } = true;
    }
}
