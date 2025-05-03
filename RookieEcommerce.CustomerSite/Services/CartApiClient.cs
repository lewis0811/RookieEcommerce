using RookieEcommerce.SharedViewModels.CartDtos;
using System.Text.Json;

namespace RookieEcommerce.CustomerSite.Services
{
    public class CartApiClient(HttpClient httpClient)
    {
        private sealed class CreateCartResponse
        {
            public Guid Id { get; init; } = Guid.Empty;
        }

        public async Task<CartDetailsDto?> GetCustomerCartAsync(Guid customerId)
        {
            var result = await httpClient.GetFromJsonAsync<CartDetailsDto>($"api/v1/carts?customer-id={customerId}&isIncludeItems=true");
            return result;
        }

        public async Task<Guid?> CreateCustomerCartAsync(Guid customerId)
        {
            var response = await httpClient.PostAsJsonAsync($"api/v1/carts", customerId);

            if (response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                try
                {
                    var result = await response.Content.ReadFromJsonAsync<CreateCartResponse>();
                    return result?.Id;
                }
                catch (JsonException jsonEx)
                {
                    throw new JsonException($"Json deserialize error: {jsonEx.Message}");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error: {ex.Message}");
                }
            }

            return null;
        }

        public async Task RemoveCartItemAsync(Guid cartId, Guid cartItemId)
        {
            var response = await httpClient.DeleteAsync($"api/v1/carts/{cartId}/items/{cartItemId}");
            response.EnsureSuccessStatusCode();
        }
    }
}