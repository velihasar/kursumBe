using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Dtos.StudentWalletDto
{
    public class StudentWalletGetByIdDto : IDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentNumber { get; set; }
        public decimal Balance { get; set; }
        public decimal TotalDeposited { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastTransactionDate { get; set; }
        public bool? IsActive { get; set; }
        public List<StudentWalletTransactionGetAllDto> Transactions { get; set; } = new List<StudentWalletTransactionGetAllDto>();
    }
}
