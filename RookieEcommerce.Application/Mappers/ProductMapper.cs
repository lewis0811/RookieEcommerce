using Riok.Mapperly.Abstractions;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductDtos;

namespace RookieEcommerce.Application.Mappers
{
    [Mapper]
    public static partial class ProductMapper
    {
        // GET
        public static partial List<ProductDetailsDto> ProductListToProductDetailsDto(List<Product> items);
        [MapProperty(nameof(Product.Category.Name), nameof(ProductDetailsDto.CategoryName))]
        public static partial ProductDetailsDto ProductToProductDetailsDto(Product product);

        // POST
        [MapperIgnoreSource(nameof(Product.Variants))]
        [MapperIgnoreSource(nameof(Product.Category))]
        [MapperIgnoreSource(nameof(Product.Images))]
        public static partial ProductCreateDto ProductToProductCreateDto(Product product);

        private static string? MapCategoryName(Category? category)
    => category?.Name;
    }
}