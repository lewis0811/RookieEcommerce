using Microsoft.EntityFrameworkCore;

namespace RookieEcommerce.Domain.Entities
{
    [Owned]
    public class Address : BaseEntity
    {
        public string? HouseNumber { get; set; }
        public string Street { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string CityProvince { get; set; } = string.Empty;
        public string Country { get; set; } = "Vietnam";
    }
}