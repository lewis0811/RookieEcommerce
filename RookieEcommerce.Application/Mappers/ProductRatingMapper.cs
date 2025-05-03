using Riok.Mapperly.Abstractions;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductRatingDtos;

namespace RookieEcommerce.Application.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    public static partial class ProductRatingMapper
    {
        public static partial List<ProductRatingDetailsDto> ProductRatingListToProductRatingDetailsDto(List<ProductRating> items);

        public static partial ProductRatingDetailsDto ProductRatingToProductRatingDetailsDto(ProductRating productRating);

        public static partial ProductRatingCreateDto ProductRatingToProductRatingCreateDto(ProductRating productRating);

        public static partial ProductRating CreateProductRatingDtoToProductRating(CreateProductRatingDto dto);
    }
}