using System;
using Core.Entities;

namespace Core.Entities.Dtos.StudentDto
{
    public class RegisterFullStudentRequestDto : IDto
    {
        public int? TenantId { get; set; }
        public int? ExistingPersonId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhotoUrl { get; set; }

        public string? StudentNumber { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        public int? BranchId { get; set; }

        // Optional Parent
        public bool IncludeParent { get; set; }
        public int? ExistingParentPersonId { get; set; }
        public string? ParentFirstName { get; set; }
        public string? ParentLastName { get; set; }
        public string? ParentPhone { get; set; }
        public string? ParentEmail { get; set; }
        public string? Relationship { get; set; } = "Anne";
        public bool IsPrimary { get; set; } = true;
    }
}
