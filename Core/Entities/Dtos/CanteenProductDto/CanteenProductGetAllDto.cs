using System;
using Core.Entities;

namespace Core.Entities.Dtos.CanteenProductDto
{
    public class CanteenProductGetAllDto : IDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Barcode { get; set; }
        public int? StockQuantity { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
