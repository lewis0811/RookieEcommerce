using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CategoryDtos;
using RookieEcommerce.SharedViewModels.ProductImageDtos;
using RookieEcommerce.SharedViewModels.ProductVariantDtos;

namespace RookieEcommerce.SharedViewModels.ProductDtos
{
    public class ProductDetailsDto : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Details { get; set; }
        public CategoryDetailsDto? Category { get; set; }
        public ICollection<ProductImageDetailsDto>? Images { get; set; }
        public ICollection<ProductVariantDetailsDto>? Variants { get; set; }
    }
}