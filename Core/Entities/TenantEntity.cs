using System;
using Core.Entities.Concrete.Project;

namespace Core.Entities
{
    public abstract class TenantEntity : BaseEntity, IEntity
    {
        public int TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
    }
}
