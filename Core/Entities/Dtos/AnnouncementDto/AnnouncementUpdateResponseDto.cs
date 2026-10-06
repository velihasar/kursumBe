using System;
using Core.Entities;

namespace Core.Entities.Dtos.AnnouncementDto
{
    public class AnnouncementUpdateResponseDto : IDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Author { get; set; }
        public string Tag { get; set; }
        public bool IsImportant { get; set; }
    }
}
