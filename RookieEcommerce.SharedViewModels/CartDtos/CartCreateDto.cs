using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.SharedViewModels.CartDtos
{
    public class CartCreateDto : BaseEntity
    {
        public Guid CustomerId { get; set; }
    }
}