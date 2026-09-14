using System;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    public class TeacherBranch : TenantEntity,IEntity
    {
        public int TeacherId { get; set; }

        public int BranchId { get; set; }

        public Teacher Teacher { get; set; } = null!;

        public Branch Branch { get; set; } = null!;
    }
}
