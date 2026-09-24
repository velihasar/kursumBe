using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    // Öğrenci Cüzdan / Bakiye Hesabı (Kantin, Su Dolabı vb. Harcamalar İçin)
    public class StudentWallet : TenantEntity, IEntity
    {
        public int StudentId { get; set; }
        public decimal Balance { get; set; } = 0; // Güncel Bakiye
        public decimal TotalDeposited { get; set; } = 0; // Toplam Yüklenen
        public decimal TotalSpent { get; set; } = 0; // Toplam Harcanan
        public DateTime? LastTransactionDate { get; set; }

        public virtual Student Student { get; set; }
        public virtual ICollection<StudentWalletTransaction> Transactions { get; set; } = new List<StudentWalletTransaction>();
    }
}
