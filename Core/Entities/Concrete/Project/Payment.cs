using System;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    // Ödeme / Tahsilat Kaydı
    public class Payment : TenantEntity, IEntity
    {
        public int? FeeDueId { get; set; } // Kapatılan borç ID (Opsiyonel)
        public int StudentId { get; set; } // Öğrenci ID
        public int? ParentId { get; set; } // Ödemeyi yapan veli ID (Opsiyonel)
        public decimal Amount { get; set; } // Ödenen tutar
        public DateTime PaymentDate { get; set; } // Ödeme tarihi
        public int PaymentType { get; set; } // Ödeme yöntemi (1: Nakit, 2: Kart, 3: Havale, 4: POS)
        public string ReceiptNo { get; set; } // Makbuz / fiş no
        public string TransactionId { get; set; } // Banka/POS İşlem no
        public string Notes { get; set; } // Ödeme notları

        public virtual FeeDue FeeDue { get; set; }
        public virtual Student Student { get; set; }
        public virtual Parent Parent { get; set; }
    }
}
