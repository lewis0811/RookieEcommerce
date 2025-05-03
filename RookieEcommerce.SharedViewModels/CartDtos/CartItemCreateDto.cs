using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.SharedViewModels.CartDtos
{
    public class CartItemCreateDto : BaseEntity
    {
        public int Quantity { get; set; }
    }
}