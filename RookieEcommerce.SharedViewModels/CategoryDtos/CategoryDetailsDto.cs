using RookieEcommerce.SharedViewModels.ProductDtos;

namespace RookieEcommerce.SharedViewModels.CategoryDtos
{
    public class CategoryDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        
        public ICollection<SubCategoriesDto> SubCategories { get; set; } = [];

        public ICollection<ProductsInCategoryDto> Products { get; set; } = [];
    }
}