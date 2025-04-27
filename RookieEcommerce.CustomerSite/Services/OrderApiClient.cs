using RookieEcommerce.SharedViewModels.OrderDtos;

namespace RookieEcommerce.CustomerSite.Services
{
    public class OrderApiClient(HttpClient httpClient)
    {
        public async Task<OrderDetailsDto> GetOrderItemAsync(Guid customerId)
        {
            var orderItem = await httpClient
                .GetFromJsonAsync<OrderDetailsDto>($"api/v1/orders/customer/{customerId}?isIncludeItems=true");

            return orderItem!;
        }

        public async Task CreateOrderAsync(CreateOrderDto dto)
        {
            var response = await httpClient.PostAsJsonAsync("api/v1/orders", dto);
            response.EnsureSuccessStatusCode();
        }
    }
}
