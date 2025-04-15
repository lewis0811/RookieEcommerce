using Riok.Mapperly.Abstractions;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CategoryDtos;
using RookieEcommerce.SharedViewModels.ProductDtos;

namespace RookieEcommerce.Application.Mappers
{
    [Mapper]
    public static partial class CategoryMapper
    {
        public static partial CategoryDetailsDto CategoryToCategoryDetailsDtoList(Category categories);
        
        [MapperIgnoreSource(nameof(Category.ParentCategory))]
        [MapperIgnoreSource(nameof(Category.SubCategories))]
        [MapperIgnoreSource(nameof(Category.Products))]
        public static partial CategoryCreateDto CategoryToCategoryCreateDto(Category category);

        public static partial List<CategoryDetailsDto> CategoryListToDetailsDtoList(List<Category> categories);

        [MapperIgnoreSource(nameof(Category.SubCategories))]
        [MapperIgnoreSource(nameof(Category.Products))]
        [MapperIgnoreSource(nameof(Category.CreatedDate))]
        [MapperIgnoreSource(nameof(Category.ModifiedDate))]
        public static partial SubCategoriesDto CategoryToSubCategoriesDto(Category category);

        [MapperIgnoreSource(nameof(Product.Images))]
        [MapperIgnoreSource(nameof(Product.Variants))]
        public static partial ProductsInCategoryDto ProductToProductsInCategoryDto(Product product);

    }
}