using System;
using Core.Entities;

namespace Core.Entities.Dtos.EventDto
{
    public class EventUpdateResponseDto : IDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public DateTime StartDate { get; set; }
        public string Location { get; set; }
    }
}
