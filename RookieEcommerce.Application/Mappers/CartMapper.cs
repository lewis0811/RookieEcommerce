using Riok.Mapperly.Abstractions;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CartDtos;

namespace RookieEcommerce.Application.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    public static partial class CartMapper
    {
        public static partial CartDetailsDto CartToCartDetailsDto(Cart cart);

        public static partial CartItemDto CartItemToCartItemDto(CartItem cartItem);
        public static partial CartCreateDto CartToCartCreateDto(Cart cart);

        public static partial CartItemCreateDto CartItemToCartItemCreateDto(CartItem cartItem);
        public static partial CreateCartItemDto CreateCartItemDtoToCartItem (CreateCartItemDto cartItem);
    }
}