namespace RookieEcommerce.Domain.Entities
{
    public class ProductRating : BaseEntity
    {
        public double RatingValue { get; set; }
        public string? Comment { get; set; }

        // Foreign Keys
        public Guid ProductId { get; set; }

        public string CustomerId { get; set; } = "";

        // Navigation Properties
        public virtual Product? Product { get; set; }

        public virtual Customer? Customer { get; set; }

        public static ProductRating Create(Guid productId, string customerId, double ratingValue, string? comment)
        {
            return new ProductRating
            {
                ProductId = productId,
                CustomerId = customerId,
                RatingValue = ratingValue,
                Comment = comment
            };
        }

        public void Update(double? ratingValue, string? comment)
        {
            if (ratingValue != null && !ratingValue.Equals(RatingValue)) { RatingValue = (double)ratingValue; }
            if (comment != null && comment != Comment) { Comment = comment; }
        }
    }
}