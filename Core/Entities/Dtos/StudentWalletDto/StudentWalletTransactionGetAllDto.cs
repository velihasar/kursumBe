using System;
using Core.Entities;

namespace Core.Entities.Dtos.StudentWalletDto
{
    public class StudentWalletTransactionGetAllDto : IDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public int StudentWalletId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentNumber { get; set; }
        public int TransactionType { get; set; } // 1: Bakiye Yükleme (Deposit), 2: Harcama (Spend), 3: İade (Refund)
        public string TransactionTypeName { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public string Category { get; set; } // Su, Meşrubat, Kantin, Ekipman, vb.
        public string Description { get; set; }
        public int? PaymentType { get; set; } // 1: Nakit, 2: Kredi Kartı, 3: Havale/EFT
        public string PaymentTypeName { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
