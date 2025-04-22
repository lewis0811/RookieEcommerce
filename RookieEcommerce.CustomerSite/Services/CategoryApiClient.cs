using RookieEcommerce.SharedViewModels.CategoryDtos;
using RookieEcommerce.SharedViewModels.ProductDtos;
using RookieEcommerce.SharedViewModels.ResponseDtos;

namespace RookieEcommerce.CustomerSite.Services
{
    public class CategoryApiClient(HttpClient httpClient)
    {
        public async Task<PaginationResponseDto<CategoryDetailsDto>> GetCategoriesAsync()
        {
            var categories = await httpClient
                .GetFromJsonAsync<PaginationResponseDto<CategoryDetailsDto>>("api/v1/categories?IsIncludeItems=true");
            return categories!;
        }
    }
}
