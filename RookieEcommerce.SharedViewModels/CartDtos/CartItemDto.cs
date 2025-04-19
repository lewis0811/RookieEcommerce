using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductDtos;

namespace RookieEcommerce.SharedViewModels.CartDtos
{
    public class CartItemDto : BaseEntity
    {
        public int Quantity { get; set; }
        public virtual ProductDetailsDto Product { get; set; } = new();
        public virtual ProductVariant? ProductVariant { get; set; } = new();

        public decimal LineTotal => ProductVariant != null
            ? ProductVariant.Price * Quantity
            : Product.Price * Quantity;
    }
}