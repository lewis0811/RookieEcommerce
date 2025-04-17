using Riok.Mapperly.Abstractions;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductRatingDtos;

namespace RookieEcommerce.Application.Mappers
{
    [Mapper]
    public static partial class ProductRatingMapper
    {
        public static partial List<ProductRatingDetailsDto> ProductRatingListToProductRatingDetailsDto(List<ProductRating> items);
        
        [MapperIgnoreSource(nameof(Product))]
        [MapperIgnoreSource(nameof(Customer))]
        public static partial ProductRatingDetailsDto ProductRatingToProductRatingDetailsDto(ProductRating productRating);
        
        [MapperIgnoreSource(nameof(Product))]
        [MapperIgnoreSource(nameof(Customer))]
        public static partial ProductRatingCreateDto ProductRatingToProductRatingCreateDto(ProductRating productRating);
    }
}