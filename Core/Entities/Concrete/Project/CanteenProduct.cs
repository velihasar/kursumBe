using System;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    // Kantin / Dolap Satış Ürünleri ve Fiyat Listesi (Su, Meşrubat, Atıştırmalık, Ekipman vb.)
    public class CanteenProduct : TenantEntity, IEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; } // Su & İçecek, Meşrubat, Kantin & Atıştırmalık, Ekipman & Malzeme vb.
        public string Barcode { get; set; }
        public int? StockQuantity { get; set; }
        public string Icon { get; set; } // Emoji simgesi: 💧, 🧃, 🍫, 🥋, 🥛, 🥪, ✨
        public string Description { get; set; }
    }
}
