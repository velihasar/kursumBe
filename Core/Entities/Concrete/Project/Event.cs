using System;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    /// <summary>
    /// Kurum Etkinlikleri & Sınav Takvimi Entity
    /// </summary>
    public class Event :TenantEntity, IEntity
    {
        public string Title { get; set; } // Etkinlik / Sınav Başlığı
        public string Description { get; set; } // Detaylı Açıklama
        public string Category { get; set; } // Kategori (Deneme Sınavı, Veli Toplantısı, Atölye, Seminer vb.)
        public DateTime StartDate { get; set; } // Başlangıç Tarih ve Saati
        public DateTime? EndDate { get; set; } // Bitiş Tarih ve Saati
        public string Location { get; set; } // Etkinlik Yeri / Salon / Kampüs Bilgisi
        public string TargetAudience { get; set; } // Hedef Kitle Açıklaması (örn. 8. ve 12. Sınıflar, Tüm Veliler)
        public int TargetRole { get; set; } = 0; // 0: Herkes, 1: Veliler, 2: Öğrenciler, 3: Öğretmenler
        public int? Capacity { get; set; } // Kontenjan Sınırı (Varsa)
        public bool IsRegistrationRequired { get; set; } = false; // Ön Kayıt Zorunlu mu?
        public string ImageUrl { get; set; } // Etkinlik Afiş / Kapak Görseli URL
        public string Icon { get; set; } // İkon Tanımı (newspaper, people, code-slash, sparkles vb.)
        public int Status { get; set; } = 1; // 1: Planlandı, 2: Devam Ediyor, 3: Tamamlandı, 4: İptal Edildi
        public int? BranchId { get; set; } // Şubeye Özel Etkinlik (Opsiyonel)

        public virtual Branch Branch { get; set; }
    }
}
