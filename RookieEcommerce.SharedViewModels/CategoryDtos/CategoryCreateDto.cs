﻿using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.SharedViewModels.CategoryDtos
{
    public class CategoryCreateDto : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Guid? ParentCategoryId { get; set; }
    }
}