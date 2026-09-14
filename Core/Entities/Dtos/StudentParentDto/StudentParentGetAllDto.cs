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
    }
}
