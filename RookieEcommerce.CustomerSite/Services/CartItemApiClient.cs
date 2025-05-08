using RookieEcommerce.Application.Mappers;
using RookieEcommerce.SharedViewModels.CartDtos;
using System.Linq.Dynamic.Core.Tokenizer;

namespace RookieEcommerce.CustomerSite.Services
{
    public class CartItemApiClient(HttpClient httpClient)
    {
        public async Task AddCartItemAsync(CreateCartItemDto cartItem, Guid cartId, string? token)
        {
            var entity = CartMapper.CreateCartItemDtoToCartItem(cartItem);

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.PostAsJsonAsync($"api/v1/carts/{cartId}/items", entity);

            response.EnsureSuccessStatusCode();
        }
    }
}