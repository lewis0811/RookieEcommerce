using RookieEcommerce.Application.Mappers;
using RookieEcommerce.SharedViewModels.ProductDtos;
using RookieEcommerce.SharedViewModels.ProductRatingDtos;
using RookieEcommerce.SharedViewModels.ResponseDtos;
using System.Net.Http;

namespace RookieEcommerce.CustomerSite.Services
{
    public class ProductRatingApiClient(HttpClient httpClient)
    {
        public async Task<PaginationResponseDto<ProductRatingDetailsDto>> GetProductRatingsAsync()
        {
            var productRatings = await httpClient
                .GetFromJsonAsync<PaginationResponseDto<ProductRatingDetailsDto>>("api/v1/product-ratings?IsIncludedItems=true");
            return productRatings!;
        }

        public async Task<PaginationResponseDto<ProductRatingDetailsDto>> GetProductRatingsAsync(Guid productId)
        {
            var productRatings = await httpClient
                .GetFromJsonAsync<PaginationResponseDto<ProductRatingDetailsDto>>($"api/v1/product-ratings?productId={productId}&IsIncludedItems=true");
            return productRatings!;
        }

        public async Task<ProductRatingDetailsDto> GetProductRatingByIdAsync(Guid productRatingId)
        {
            var productRatings = await httpClient
                .GetFromJsonAsync<ProductRatingDetailsDto>($"api/v1/product-ratings/{productRatingId}?isIncludeItems=true");
            return productRatings!;
        }

        public async Task AddProductRatingAsync(CreateProductRatingDto dto)
        {
            var entity = ProductRatingMapper.CreateProductRatingDtoToProductRating(dto);
            var response = await httpClient.PostAsJsonAsync("api/v1/product-ratings", entity);

            response.EnsureSuccessStatusCode();
            //if (!response.IsSuccessStatusCode)
            //{
            //    throw new InvalidOperationException($"Failed to add product rating: {response.ReasonPhrase}");
            //}
        }
    }
}
