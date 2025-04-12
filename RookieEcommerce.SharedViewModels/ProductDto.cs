using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.SharedViewModels
{
    public class ProductDto : BaseEntity
    {
        public string Name { get; }
        public string Description { get; }
        public decimal Price { get; }
        public string CategoryName { get; }
        public ICollection<ProductImage> Images { get; }

        public ProductDto(Guid id, string name, string description, decimal price, string categoryName, ICollection<ProductImage> images, DateTime createdDate, DateTime? modifiedDate)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            CategoryName = categoryName;
            Images = images;
            ModifiedDate = modifiedDate;
            CreatedDate = createdDate;
        }
    } 
}
