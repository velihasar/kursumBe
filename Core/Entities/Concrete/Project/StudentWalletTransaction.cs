using System;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    // Cüzdan Hareketleri (Bakiye Yükleme ve Dolap/Kantin Harcamaları)
    public class StudentWalletTransaction : TenantEntity, IEntity
    {
        public int StudentWalletId { get; set; }
        public int StudentId { get; set; }
        public int TransactionType { get; set; } // 1: Bakiye Yükleme (Deposit), 2: Harcama (Expense/Purchase), 3: İade/Düzeltme (Refund)
        public decimal Amount { get; set; } // İşlem tutarı
        public decimal BalanceBefore { get; set; } // İşlem öncesi bakiye
        public decimal BalanceAfter { get; set; } // İşlem sonrası bakiye
        public string Category { get; set; } // Örn: "Su / İçecek", "Kantin / Dolap", "Spor Ekipmanı", "Nakit Yükleme", "Kredi Kartı"
        public string Description { get; set; } // Örn: "0.5L Su", "Veliden nakit bakiye yükleme"
        public int? PaymentType { get; set; } // Yükleme türü (1: Nakit, 2: Kredi Kartı, 3: Havale/EFT)
        public string ReceiptNo { get; set; } // Fiş / Makbuz No
        public DateTime TransactionDate { get; set; } // İşlem Tarihi

        public int? CanteenProductId { get; set; }
        public virtual StudentWallet StudentWallet { get; set; }
        public virtual Student Student { get; set; }
        public virtual CanteenProduct CanteenProduct { get; set; }
    }
}
