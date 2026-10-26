using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Dtos.StudentDto
{
    public class ExcelBulkImportResponseDto : IDto
    {
        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
        public List<string> ImportedStudentNames { get; set; } = new();
    }
}
