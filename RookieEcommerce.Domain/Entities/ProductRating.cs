﻿namespace RookieEcommerce.Domain.Entities
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

        public static ProductRating Create(Guid productId, Guid customerId, double ratingValue, string? comment)
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