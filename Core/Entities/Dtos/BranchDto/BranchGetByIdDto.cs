using System;
using Core.Entities;

namespace Core.Entities.Dtos.BranchDto
{
    public class BranchGetByIdDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}
