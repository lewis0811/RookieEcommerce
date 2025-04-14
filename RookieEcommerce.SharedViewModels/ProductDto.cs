using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.SharedViewModels
{
    public class ProductDto : BaseEntity
    {
        public string Name { get; }
        public string Description { get; }
        public decimal Price { get; }
        public string? Details { get;} 
        public string? CategoryName { get; }
        public Guid? CategoryId { get; }
        public ICollection<ProductImage>? Images { get; }

        public ProductDto(Guid id, string name, string description, decimal price, Guid? categoryId, string? categoryName, ICollection<ProductImage>? images,
            string? details, DateTime createdDate, DateTime? modifiedDate)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            CategoryId = categoryId;
            CategoryName = categoryName;
            Images = images;
            Details = details;
            ModifiedDate = modifiedDate;
            CreatedDate = createdDate;
        }

        public ProductDto(Guid id, string name, string description, decimal price, Guid? categoryId, string? details)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            CategoryId = categoryId;
            Details = details;
        }
    }
}