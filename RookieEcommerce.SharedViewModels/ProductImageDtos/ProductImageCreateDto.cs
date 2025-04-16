using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.SharedViewModels.ProductImageDtos
{
    public class ProductImageCreateDto : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsPrimary { get; set; } = false;
    }
}