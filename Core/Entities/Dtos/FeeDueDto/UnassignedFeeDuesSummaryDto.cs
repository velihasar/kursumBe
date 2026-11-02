using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Dtos.FeeDueDto
{
    public class UnassignedFeeDuesSummaryDto : IDto
    {
        public string Period { get; set; }
        public int UnassignedCount { get; set; }
        public decimal TotalEstimatedAmount { get; set; }
        public List<UnassignedFeeDueStudentDto> Students { get; set; } = new List<UnassignedFeeDueStudentDto>();
    }
}

