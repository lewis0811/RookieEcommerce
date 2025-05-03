using RookieEcommerce.SharedViewModels.PaymentDtos;
using System.Text.Json;

namespace RookieEcommerce.CustomerSite.Services
{
    public class VnPayApiClient(HttpClient httpClient)
    {
        public sealed class VnPayReturnUrl
        { public string Url { get; set; } = ""; }

        public async Task<string> CreatePaymentUrlAsync(CreatePaymentDto dto)
        {
            var response = await httpClient.PostAsJsonAsync($"api/v1/vnpay/payment-url", dto);
            if (response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                try
                {
                    var result = await response.Content.ReadFromJsonAsync<string>();
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

            return String.Empty;
        }
    }
}