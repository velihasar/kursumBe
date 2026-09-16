using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    // Aidat / Tahakkuk (Borç Kaydı)
    public class FeeDue : TenantEntity, IEntity
    {
        public int CourseEnrollmentId { get; set; } // Kurs kayıt ID
        public int StudentId { get; set; } // Öğrenci ID
        public string Period { get; set; } // Tahakkuk dönemi (ör. 2026-09)
        public string Title { get; set; } // Borç başlığı (ör. Eylül Aidatı)
        public decimal Amount { get; set; } // Toplam borç tutarı
        public decimal PaidAmount { get; set; } // Ödenen toplam tutar
        public decimal RemainingAmount { get; set; } // Kalan borç bakiyesi
        public DateTime DueDate { get; set; } // Son ödeme tarihi (Vade)
        public int Status { get; set; } = 0; // Durum (0: Ödenmedi, 1: Kısmi Ödendi, 2: Tam Ödendi)
        public string Description { get; set; } // Açıklama

        public virtual CourseEnrollment CourseEnrollment { get; set; }
        public virtual Student Student { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
