using System;
using Core.Entities;

namespace Core.Entities.Dtos.PersonDto
{
    public class PersonGetAllDto : IDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
    }
}
