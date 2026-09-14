using System;
using Core.Entities;

namespace Core.Entities.Dtos.ParentDto
{
    public class ParentGetByIdDto : IDto
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
    }
}
