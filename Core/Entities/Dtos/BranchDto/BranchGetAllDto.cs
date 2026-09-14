using System;
using Core.Entities;

namespace Core.Entities.Dtos.BranchDto
{
    public class BranchGetAllDto : IDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool? IsActive { get; set; }
    }
}
