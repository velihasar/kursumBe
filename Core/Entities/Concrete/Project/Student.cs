using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    public class Student : TenantEntity, IEntity
    {
        public int PersonId { get; set; }

        public string? StudentNumber { get; set; }

        public string? ParentAccessCode { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public Person Person { get; set; } = null!;

        public ICollection<StudentParent> Parents { get; set; } = new List<StudentParent>();

        public ICollection<StudentBranch> StudentBranches { get; set; } = new List<StudentBranch>();
    }
}
