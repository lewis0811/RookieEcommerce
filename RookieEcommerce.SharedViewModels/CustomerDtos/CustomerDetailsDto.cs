using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.SharedViewModels.CustomerDtos
{
    public class CustomerDetailsDto : BaseEntity
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}