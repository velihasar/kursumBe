using System;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    public class StudentParent :TenantEntity, IEntity
    {
        public int StudentId { get; set; }

        public int ParentId { get; set; }

        public string? Relationship { get; set; }

        public bool IsPrimary { get; set; }

        public Student Student { get; set; } = null!;

        public Parent Parent { get; set; } = null!;
    }
}
