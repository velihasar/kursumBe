using System;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    public class Person : TenantEntity, IEntity
    {

        public int? UserId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public DateTime? DateOfBirth { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? PhotoUrl { get; set; }

        public Student? Student { get; set; }

        public Teacher? Teacher { get; set; }

        public Parent? Parent { get; set; }
    }
}
