using System;
using Core.Entities;

namespace Core.Entities.Dtos.FeeDueDto
{
    public class UnassignedFeeDueStudentDto : IDto
    {
        public int CourseEnrollmentId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentNumber { get; set; }
        public string ParentName { get; set; }
        public string ParentPhoneNumber { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int? BranchId { get; set; }
        public string BranchName { get; set; }
        public decimal SuggestedAmount { get; set; }
        public int DueDayOfMonth { get; set; }
        public DateTime SuggestedDueDate { get; set; }
    }
}

