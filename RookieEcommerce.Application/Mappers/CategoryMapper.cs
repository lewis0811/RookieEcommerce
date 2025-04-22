using Riok.Mapperly.Abstractions;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CategoryDtos;
using RookieEcommerce.SharedViewModels.ProductDtos;
using RookieEcommerce.SharedViewModels.ProductImageDtos;

namespace RookieEcommerce.Application.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    public static partial class CategoryMapper
    {
        // GET
        public static partial CategoryDetailsDto CategoryToCategoryDetailsDtoList(Category category);
        
        public static partial List<CategoryDetailsDto> CategoryListToCategoryDetailsDtoList(List<Category> categories);

        public static partial SubCategoriesDto CategoryToSubCategoriesDto(Category category);

        public static partial ProductsInCategoryDto ProductToProductsInCategoryDto(Product product);

        public static partial ProductImageDetailsDto ProductImageToProductImageDetailsDto(ProductImage productImage);

        // POST
        public static partial CategoryCreateDto CategoryToCategoryCreateDto(Category category);

    }
}