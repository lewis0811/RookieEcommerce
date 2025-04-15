namespace RookieEcommerce.SharedViewModels.CategoryDtos
{
    public class SubCategoriesDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
    }
}