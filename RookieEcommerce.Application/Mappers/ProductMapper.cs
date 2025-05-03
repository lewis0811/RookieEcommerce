using Riok.Mapperly.Abstractions;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductDtos;

namespace RookieEcommerce.Application.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    public static partial class ProductMapper
    {
        // GET
        public static partial List<ProductDetailsDto> ProductListToProductDetailsDto(List<Product> items);

        public static partial ProductDetailsDto ProductToProductDetailsDto(Product product);

        // POST
        public static partial ProductCreateDto ProductToProductCreateDto(Product product);
    }
}