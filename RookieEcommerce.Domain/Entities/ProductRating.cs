namespace RookieEcommerce.Domain.Entities
{
    public class ProductRating : BaseEntity
    {
        public double RatingValue { get; set; }
        public string? Comment { get; set; }

        // Foreign Keys
        public Guid ProductId { get; set; }
        public Guid CustomerId { get; set; }

        // Navigation Properties
        public virtual Product Product { get; set; } = new();

        public virtual Customer Customer { get; set; } = new();
    }
}