using System;
using Core.Entities;

namespace Core.Entities.Dtos.TenantDto
{
    public class TenantCreateResponseDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string LogoUrl { get; set; }
    }
}
