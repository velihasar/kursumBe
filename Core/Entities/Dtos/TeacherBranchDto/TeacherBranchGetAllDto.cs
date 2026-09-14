using System;
using Core.Entities;

namespace Core.Entities.Dtos.TeacherBranchDto
{
    public class TeacherBranchGetAllDto : IDto
    {
        public int TeacherId { get; set; }
        public int BranchId { get; set; }
        public int TenantId { get; set; }
        public bool IsActive { get; set; }
    }
}
