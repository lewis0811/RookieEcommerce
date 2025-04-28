using RookieEcommerce.SharedViewModels.ProductDtos;
using RookieEcommerce.SharedViewModels.ResponseDtos;
using System.Text;
using System.Web;

namespace RookieEcommerce.CustomerSite.Services
{
    public class ProductApiClient(HttpClient httpClient)
    {
        public async Task<PaginationResponseDto<ProductDetailsDto>> GetProductsAsync(string? sortOrder, double? minPrice, double? maxPrice, Guid? categoryId, int? pageNumber)
        {
            var queryBuilder = new StringBuilder("api/v1/products?IsIncludeItems=true");

            if (!string.IsNullOrEmpty(sortOrder))
            {
                queryBuilder.Append($"&SortBy={HttpUtility.UrlEncode(sortOrder)}");
            }
            if (minPrice.HasValue)
            {
                queryBuilder.Append($"&MinPrice={minPrice.Value}");
            }
            if (maxPrice.HasValue)
            {
                queryBuilder.Append($"&MaxPrice={maxPrice.Value}");
            }
            if (categoryId.HasValue)
            {
                queryBuilder.Append($"&CategoryId={categoryId.Value}");
            }
            if(pageNumber.HasValue)
            {
                queryBuilder.Append($"&PageNumber={pageNumber.Value}");
            }

            string apiUrl = queryBuilder.ToString();

            var products = await httpClient
                .GetFromJsonAsync<PaginationResponseDto<ProductDetailsDto>>(apiUrl);

            return products!;
        }

        public async Task<ProductDetailsDto> GetProductByIdAsync(Guid productId)
        {
            var product = await httpClient
                .GetFromJsonAsync<ProductDetailsDto>($"api/v1/products/{productId}?isIncludeItems=true");
            return product!;
        }
    }
}