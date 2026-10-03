using System;
using Core.Entities;

namespace Core.Entities.Dtos.StudentParentDto
{
    public class StudentParentGetAllDto : IDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ParentId { get; set; }
        public string Relationship { get; set; }
        public bool IsPrimary { get; set; }
        public string StudentName { get; set; }
        public string StudentNumber { get; set; }
        public string BranchName { get; set; }
        public string ParentName { get; set; }
        public string ParentPhone { get; set; }
        public string ParentEmail { get; set; }
    }
}
