using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    public class Tenant : BaseEntity, IEntity
    {
        public string Name { get; set; } = null!;

        public string? Code { get; set; }

        public string? LogoUrl { get; set; }

        public ICollection<Branch> Branches { get; set; } = new List<Branch>();

        public ICollection<TenantUser> Users { get; set; } = new List<TenantUser>();

        public ICollection<Person> People { get; set; } = new List<Person>();

        public ICollection<Student> Students { get; set; } = new List<Student>();

        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

        public ICollection<Parent> Parents { get; set; } = new List<Parent>();
    }
}
