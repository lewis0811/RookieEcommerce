using Riok.Mapperly.Abstractions;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductVariantDtos;

namespace RookieEcommerce.Application.Mappers
{
    [Mapper]
    public static partial class ProductVariantMapper
    {
        [MapperIgnoreSource(nameof(ProductVariant.Product))]
        public static partial ProductVariantDetailsDto ProductVariantToProductVariantDetailsDto(ProductVariant productVariant);

        public static partial List<ProductVariantDetailsDto> ProductVariantListToProductVariantDetailsDto(List<ProductVariant> variants);

        [MapperIgnoreSource(nameof(ProductVariant.Product))]
        public static partial ProductVariantCreateDto ProductVariantToProductVariantCreateDto(ProductVariant variant);
    }
}