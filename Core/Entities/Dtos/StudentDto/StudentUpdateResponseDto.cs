using System;
using Core.Entities;

namespace Core.Entities.Dtos.StudentDto
{
    public class StudentUpdateResponseDto : IDto
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string StudentNumber { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
