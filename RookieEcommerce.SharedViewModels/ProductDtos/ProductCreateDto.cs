using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.SharedViewModels.ProductDtos
{
    public class ProductCreateDto : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Guid? CategoryId { get; set; }
        public string? Details { get; set; }
    }
}