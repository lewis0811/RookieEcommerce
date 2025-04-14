﻿namespace RookieEcommerce.SharedViewModels.CategoryDtos
{
    public class CategorySummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
    }
}