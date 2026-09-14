using System;
using Core.Entities;

namespace Core.Entities.Dtos.TeacherDto
{
    public class TeacherUpdateResponseDto : IDto
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public DateTime StartDate { get; set; }
    }
}
