using System;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    public class StudentBranch :TenantEntity, IEntity
    {
        public int StudentId { get; set; }

        public int BranchId { get; set; }

        public Student Student { get; set; } = null!;

        public Branch Branch { get; set; } = null!;
    }
}
