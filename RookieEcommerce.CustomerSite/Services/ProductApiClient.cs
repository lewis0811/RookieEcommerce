using RookieEcommerce.SharedViewModels.ProductDtos;
using RookieEcommerce.SharedViewModels.ResponseDtos;

namespace RookieEcommerce.CustomerSite.Services
{
    public class ProductApiClient(HttpClient httpClient)
    {
        public async Task<PaginationResponseDto<ProductDetailsDto>> GetProductsAsync()
        {
            var products = await httpClient
                .GetFromJsonAsync<PaginationResponseDto<ProductDetailsDto>>("api/v1/products?IsIncludeItems=true");
            return products!;
        }
    }
}