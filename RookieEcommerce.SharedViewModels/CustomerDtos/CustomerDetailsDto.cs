using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.SharedViewModels.CustomerDtos
{
    public class CustomerDetailsDto : BaseEntity
    {
        public string IdentityUserId { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    } 
}