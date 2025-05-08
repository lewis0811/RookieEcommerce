using RookieEcommerce.SharedViewModels.CategoryDtos;

namespace RookieEcommerce.CustomerSite.Services
{
    public class CategoryApiClient(HttpClient httpClient)
    {
        public async Task<List<CategoryDetailsDto>> GetCategoriesAsync(string? token)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                   new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var categories = await httpClient
                .GetFromJsonAsync<List<CategoryDetailsDto>>("api/v1/categories?IsIncludeItems=true");
            return categories!;
        }
    }
}