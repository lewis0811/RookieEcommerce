using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductDtos;
using RookieEcommerce.SharedViewModels.ProductVariantDtos;

namespace RookieEcommerce.SharedViewModels.CartDtos
{
    public class CartItemDto : BaseEntity
    {
        public int Quantity { get; set; }
        public virtual ProductDetailsDto? Product { get; set; }
        public virtual ProductVariantDetailsDto? ProductVariant { get; set; }

        public decimal? LineTotal => ProductVariant != null
            ? ProductVariant.Price * Quantity
            : Product?.Price * Quantity;
    }
}