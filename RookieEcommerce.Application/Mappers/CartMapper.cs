using Riok.Mapperly.Abstractions;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CartDtos;

namespace RookieEcommerce.Application.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    public static partial class CartMapper
    {
        [MapperIgnoreSource(nameof(Cart.Customer))]
        public static partial CartDetailsDto CartToCartDetailsDto(Cart cart);

        [MapperIgnoreSource(nameof(CartItem.Cart))]
        [MapperIgnoreSource(nameof(CartItem.CartId))]
        [MapperIgnoreSource(nameof(CartItem.Product))]
        [MapperIgnoreSource(nameof(CartItem.ProductVariant))]
        [MapperIgnoreSource(nameof(CartItem.ProductId))]
        [MapperIgnoreSource(nameof(CartItem.ProductVariantId))]
        public static partial CartItemDto CartItemToCartItemDto(CartItem cartItem);
        
        [MapperIgnoreSource(nameof(Cart.Customer))]
        [MapperIgnoreSource(nameof(Cart.Items))]
        public static partial CartCreateDto CartToCartCreateDto(Cart cart);

        [MapperIgnoreSource(nameof(CartItem.Product))]
        [MapperIgnoreSource(nameof(CartItem.ProductVariant))]
        public static partial CartItemCreateDto CartItemToCartItemCreateDto(CartItem cartItem);
    
    }
}