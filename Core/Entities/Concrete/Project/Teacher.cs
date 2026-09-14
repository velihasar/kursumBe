using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    public class Teacher : TenantEntity, IEntity
    {
        public int PersonId { get; set; }

        public DateTime StartDate { get; set; }

        public Person Person { get; set; } = null!;

        public ICollection<TeacherBranch> TeacherBranches { get; set; } = new List<TeacherBranch>();
    }
}
