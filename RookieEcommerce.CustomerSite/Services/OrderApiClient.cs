using RookieEcommerce.Application.Features.Orders.Commands;
using RookieEcommerce.SharedViewModels.OrderDtos;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Text.Json;

namespace RookieEcommerce.CustomerSite.Services
{
    public class OrderApiClient(HttpClient httpClient)
    {
        public async Task<OrderDetailsDto?> GetOrderDetailAsync(Guid customerId, string? token)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            OrderDetailsDto? order = null;
            try
            {
                order = await httpClient
                    .GetFromJsonAsync<OrderDetailsDto>($"api/v1/orders/customer/{customerId}?isIncludeItems=true");

            }
            catch (HttpRequestException)
            {
                return order;
            }

            return order;
        }

        public async Task<OrderCreateDto> CreateOrderAsync(CreateOrderDto dto, string? token)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.PostAsJsonAsync("api/v1/orders", dto);
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                try
                {
                    var result = await response.Content.ReadFromJsonAsync<OrderCreateDto>();
                    return result!;
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

            return new();
        }

        public async Task UpdateOrderAsync(string orderId, string transactionId, string? token)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var command = new UpdateOrderCommand { OrderId = Guid.Parse(orderId), PaymentStatus = Domain.Enums.PaymentStatus.Succeed, TransactionId = transactionId };
            var response = await httpClient.PutAsJsonAsync($"api/v1/orders/{orderId}", command);
            response.EnsureSuccessStatusCode();
        }
    }
}