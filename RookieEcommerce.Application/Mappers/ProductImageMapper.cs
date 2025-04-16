using Riok.Mapperly.Abstractions;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductImageDtos;

namespace RookieEcommerce.Application.Mappers
{
    [Mapper]
    public static partial class ProductImageMapper
    {
        [MapperIgnoreSource(nameof(ProductImage.Product))]
        public static partial ProductImageDetailsDto ProductImageToProductImageDetailsDto(ProductImage productImage);

        public static partial List<ProductImageDetailsDto> ProductImageListToProductImageDeatilsDto(List<ProductImage> productImages);

        [MapperIgnoreSource(nameof(ProductImage.Product))]
        public static partial ProductImageCreateDto ProductImageToProductImageCreateDto(ProductImage productImage);
    }
}