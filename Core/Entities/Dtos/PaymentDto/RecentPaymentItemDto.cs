using System;
using Core.Entities;

namespace Core.Entities.Dtos.PaymentDto
{
    public class RecentPaymentItemDto : IDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentFullName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PaymentType { get; set; }
        public string? ReceiptNo { get; set; }
    }
}
