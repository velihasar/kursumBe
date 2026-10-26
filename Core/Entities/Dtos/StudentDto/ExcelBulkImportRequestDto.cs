using System.Collections.Generic;
using Core.Entities;

namespace Core.Entities.Dtos.StudentDto
{
    public class ExcelBulkImportRequestDto : IDto
    {
        public int? TenantId { get; set; }
        public string? ImportMode { get; set; } = "SkipDuplicates"; // SkipDuplicates, UpdateExisting, etc.
        public List<ExcelImportStudentRowDto> Rows { get; set; } = new();
    }
}
