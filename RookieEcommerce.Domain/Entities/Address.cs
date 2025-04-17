using Microsoft.EntityFrameworkCore;

namespace RookieEcommerce.Domain.Entities
{
    [Owned]
    public class Address
    {
        public string StreetAddress { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string CityProvince { get; set; } = string.Empty;
    }
}