using RookieEcommerce.Domain.Entities;
using RookieEcommerce.Domain.Enums;

namespace RookieEcommerce.SharedViewModels
{
    public class ProductVariantDto : BaseEntity
    {
        public Guid ProductId { get; set; }
        public PVariantType VariantType { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }

        public ProductVariantDto(Guid id, Guid productId, PVariantType variantType, string name, string sku, int stockQuantity, decimal price, DateTime createDate, DateTime? modifiedDate)
        {
            Id = id;
            ProductId = productId;
            VariantType = variantType;
            Name = name;
            Sku = sku;
            StockQuantity = stockQuantity;
            Price = price;
            CreatedDate = createDate;
            ModifiedDate = modifiedDate;
        }
    }
}