using RookieEcommerce.SharedViewModels.ProductDtos;
using RookieEcommerce.SharedViewModels.ProductRatingDtos;
using RookieEcommerce.SharedViewModels.ResponseDtos;

namespace RookieEcommerce.CustomerSite.Models
{
    public class HomeProductDetailsViewModel
    {
        public PaginationResponseDto<ProductRatingDetailsDto> ProductRatings { get; set; } = new();
        public ProductDetailsDto ProductDetails { get; set; } = new();
        public CreateProductRatingDto CreateProductRating { get; set; } = new();
    }
}
