namespace RookieEcommerce.Domain.Entities
{
    public class ProductImage : BaseEntity
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsPrimary { get; set; } = false;

        // Foreign Key
        public Guid ProductId { get; set; }

        // Navigation Property
        public virtual Product Product { get; set; } = null!;

        public static ProductImage Create(Guid productId, string imageUrl, string? altText)
        {
            return new ProductImage
            {
                ProductId = productId,
                ImageUrl = imageUrl,
                AltText = altText
            };
        }

        public void Update(string? altText, int? sortOrder, bool? isPrimary)
        {
            if (altText != null && altText != AltText) { AltText = altText; }
            if (sortOrder != null && sortOrder != SortOrder) { SortOrder = (int)sortOrder; }
            if (isPrimary != null && isPrimary != IsPrimary) { IsPrimary = (bool)isPrimary; }
        }
    }
}