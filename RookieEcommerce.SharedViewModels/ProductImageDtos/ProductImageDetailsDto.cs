using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.SharedViewModels.ProductImageDtos
{
    public class ProductImageDetailsDto : BaseEntity
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsPrimary { get; set; } = false;

        // Foreign Key
        public Guid ProductId { get; set; }
    }
}