using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.SharedViewModels.ProductRatingDtos
{
    public class ProductRatingDetailsDto : BaseEntity
    {
        public double RatingValue { get; set; }
        public string? Comment { get; set; }

        // Foreign Keys
        public Guid ProductId { get; set; }
        public Guid CustomerId { get; set; }
    }
}