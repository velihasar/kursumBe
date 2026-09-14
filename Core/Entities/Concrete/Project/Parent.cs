using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    public class Parent : TenantEntity, IEntity
    {

        public int PersonId { get; set; }

        public Person Person { get; set; } = null!;

        public ICollection<StudentParent> Students { get; set; } = new List<StudentParent>();
    }
}
