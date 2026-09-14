using System;
using Core.Entities;
using Newtonsoft.Json;

namespace Core.Entities.Concrete
{
    /// <summary>
    /// Kullanıcının push notification alacağı cihaz bilgilerini tutar.
    /// </summary>
    public class UserDevice : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        /// <summary>
        /// Firebase FCM cihaz token'ı
        /// </summary>
        public string DeviceToken { get; set; }

        /// <summary>
        /// ios / android gibi platform bilgisi
        /// </summary>
        public string Platform { get; set; }

        public string AppVersion { get; set; }

        /// <summary>
        /// Cihaz şu an aktif mi?
        /// </summary>
        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }

		// Navigation properties
		[JsonIgnore]
		public virtual User User { get; set; }
    }
}
