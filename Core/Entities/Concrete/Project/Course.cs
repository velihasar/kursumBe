using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    // Kurs / Ders Tanımı
    public class Course : TenantEntity, IEntity
    {
        public string Name { get; set; } // Kurs adı (ör. 8. Sınıf Matematik)
        public string Code { get; set; } // Kurs kodu (ör. MAT-8A)
        public string Description { get; set; } // Kurs açıklaması
        public decimal Price { get; set; } // Standart kurs ücreti
        public int FeeType { get; set; } // Ödeme tipi (1: Aylık, 2: Toplam)
        public int? Capacity { get; set; } // Öğrenci kapasitesi
        public string DaysOfWeek { get; set; } // Ders günleri (ör. Pazartesi,Çarşamba)
        public string StartTime { get; set; } // Başlangıç saati (ör. 14:00)
        public string EndTime { get; set; } // Bitiş saati (ör. 16:00)
        public int? TeacherId { get; set; } // Öğretmen ID

        public virtual Teacher Teacher { get; set; }
        public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }
    }
}
