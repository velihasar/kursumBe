using System;
using Core.Entities;

namespace Core.Entities.Dtos.ParentDto
{
    public class ParentCreateResponseDto : IDto
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
    }
}
