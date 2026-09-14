using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    public class Branch : TenantEntity, IEntity
    {

        public string Name { get; set; } = null!;

        public string? Address { get; set; }

        public string? Phone { get; set; }


        public ICollection<TenantUser> Users { get; set; } = new List<TenantUser>();

        public ICollection<StudentBranch> StudentBranches { get; set; } = new List<StudentBranch>();

        public ICollection<TeacherBranch> TeacherBranches { get; set; } = new List<TeacherBranch>();
    }
}
