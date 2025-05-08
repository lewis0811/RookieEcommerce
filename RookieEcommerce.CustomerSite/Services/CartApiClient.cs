using OpenIddict.Client.AspNetCore;
using RookieEcommerce.Application.Features.Carts.Commands;
using RookieEcommerce.SharedViewModels.CartDtos;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Text.Json;

namespace RookieEcommerce.CustomerSite.Services
{
    public class CartApiClient(HttpClient httpClient)
    {
        private sealed class CreateCartResponse
        {
            public Guid Id { get; init; } = Guid.Empty;
        }

        public async Task<CartDetailsDto?> GetCustomerCartAsync(Guid customerId, string? token)
        {
            CartDetailsDto? result = null;
            try
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                result = await httpClient.GetFromJsonAsync<CartDetailsDto>($"api/v1/carts?customer-id={customerId}&isIncludeItems=true");

            }
            catch (HttpRequestException)
            {
                return result;

            }
            return result;
        }

        public async Task<Guid?> CreateCustomerCartAsync(Guid customerId, string? token)
        {
            CreateCartCommand command = new CreateCartCommand { CustomerId = customerId };

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.PostAsJsonAsync($"api/v1/carts", command);

            if (response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.OK)
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

        public async Task RemoveCartItemAsync(Guid cartId, Guid cartItemId, string? token)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.DeleteAsync($"api/v1/carts/{cartId}/items/{cartItemId}");
            response.EnsureSuccessStatusCode();
        }
    }
}