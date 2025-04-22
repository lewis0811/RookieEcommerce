using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductImageDtos;

namespace RookieEcommerce.SharedViewModels.ProductDtos
{
    public class ProductDetailsDto : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Details { get; set; }
        public Category? Category { get; set; }
        public ICollection<ProductImageDetailsDto>? Images { get; set; }
        public ICollection<ProductVariant>? Variants { get; set;}
    }
}
