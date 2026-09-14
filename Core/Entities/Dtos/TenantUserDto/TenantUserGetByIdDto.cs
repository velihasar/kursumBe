using System;
using Core.Entities;

namespace Core.Entities.Dtos.TenantUserDto
{
    public class TenantUserGetByIdDto : IDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? BranchId { get; set; }
    }
}
