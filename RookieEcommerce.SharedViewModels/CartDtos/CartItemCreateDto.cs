using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductDtos;

namespace RookieEcommerce.SharedViewModels.CartDtos
{
    public class CartItemCreateDto : BaseEntity
    {
        public int Quantity { get; set; }
    }
}