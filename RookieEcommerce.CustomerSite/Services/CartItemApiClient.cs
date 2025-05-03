using RookieEcommerce.Application.Mappers;
using RookieEcommerce.SharedViewModels.CartDtos;

namespace RookieEcommerce.CustomerSite.Services
{
    public class CartItemApiClient(HttpClient httpClient)
    {
        public async Task AddCartItemAsync(CreateCartItemDto cartItem, Guid cartId)
        {
            var entity = CartMapper.CreateCartItemDtoToCartItem(cartItem);
            var response = await httpClient.PostAsJsonAsync($"api/v1/carts/{cartId}/items", entity);

            response.EnsureSuccessStatusCode();
        }
    }
}