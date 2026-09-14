using System;
using Core.Entities;

namespace Core.Entities.Dtos.StudentBranchDto
{
    public class StudentBranchUpdateResponseDto : IDto
    {
        public int StudentId { get; set; }
        public int BranchId { get; set; }
        public int TenantId { get; set; }
    }
}
