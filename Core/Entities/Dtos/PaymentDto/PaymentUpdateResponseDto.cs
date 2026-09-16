using System;
using Core.Entities;

namespace Core.Entities.Dtos.PaymentDto
{
    public class PaymentUpdateResponseDto : IDto
    {
        public int Id { get; set; }
        public int? FeeDueId { get; set; }
        public int StudentId { get; set; }
        public int? ParentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public int PaymentType { get; set; }
        public string ReceiptNo { get; set; }
        public string TransactionId { get; set; }
        public string Notes { get; set; }
    }
}
