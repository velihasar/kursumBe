using System;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    /// <summary>
    /// Kurum Duyuruları Entity
    /// </summary>
    public class Announcement :TenantEntity,  IEntity
    {
        public string Title { get; set; } // Duyuru Başlığı
        public string Summary { get; set; } // Kısa Özet
        public string Content { get; set; } // Detaylı İçerik Metni
        public string Author { get; set; } // Yayınlayan Birim / Yetkili (örn. Kurs Yönetimi, Rehberlik Servisi)
        public string Tag { get; set; } // Kategori Etiketi (Genel, Sınav, Akademik, Cüzdan, Tatil vb.)
        public string Icon { get; set; } // İkon Tanımı (örn. megaphone, wallet, bulb)
        public string ImageUrl { get; set; } // Kapak Görseli URL
        public bool IsImportant { get; set; } = false; // Önemli Duyuru mu?
        public bool IsPublished { get; set; } = true; // Yayın Durumu
        public DateTime? PublishDate { get; set; } // Yayınlanma Tarihi
        public DateTime? ExpireDate { get; set; } // Yayından Kaldırılma Tarihi
        public int TargetAudience { get; set; } = 0; // 0: Herkes, 1: Veliler, 2: Öğrenciler, 3: Öğretmenler
        public int? BranchId { get; set; } // Şubeye Özel Duyuru (Opsiyonel)

        public virtual Branch Branch { get; set; }
    }
}
