using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    // Kurs Kaydı (Öğrencinin kurstaki durumu ve özel anlaşma şartları)
    public class CourseEnrollment : TenantEntity, IEntity
    {
        public int StudentId { get; set; } // Öğrenci ID
        public int CourseId { get; set; } // Kurs ID
        public DateTime EnrollmentDate { get; set; } // Kayıt tarihi
        public decimal? CustomMonthlyFee { get; set; } // Öğrenciye özel aylık ücret (İndirimli vb.)
        public int DueDayOfMonth { get; set; } = 1; // Her ayın kaçıncı günü ödeme yapılacak?
        public int Status { get; set; } = 1; // Durum (1: Aktif, 2: Donduruldu, 0: Pasif)
        public string Notes { get; set; } // Kayıt notları

        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<FeeDue> FeeDues { get; set; }
    }
}
