using System;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    public class TenantUser : TenantEntity, IEntity
    {

        public int UserId { get; set; }

        public int? BranchId { get; set; }

        public Branch? Branch { get; set; }
    }
}
