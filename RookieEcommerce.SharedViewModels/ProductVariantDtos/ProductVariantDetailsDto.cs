using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.SharedViewModels.ProductVariantDtos
{
    public class ProductVariantDetailsDto : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string VariantType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
    }
}