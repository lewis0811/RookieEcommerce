using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.SharedViewModels.CartDtos
{
    public class CartDetailsDto : BaseEntity
    {
        public Guid CustomerId { get; set; } // From Cart entity

        public List<CartItemDto>? Items { get; set; }

        // Calculated property for the total price of all items in the cart
        public decimal? TotalPrice => Items?.Sum(item => item.LineTotal);
    }
}