using RookieEcommerce.Domain.Entities;
using RookieEcommerce.Domain.Enums;

namespace RookieEcommerce.SharedViewModels.ProductVariantDtos
{
    public class ProductVariantDetailsDto : BaseEntity
    {
        public Guid ProductId { get; set; }
        public PVariantType VariantType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
    }
}