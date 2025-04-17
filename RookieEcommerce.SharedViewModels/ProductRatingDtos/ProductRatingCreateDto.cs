using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.SharedViewModels.ProductRatingDtos
{
    public class ProductRatingCreateDto : BaseEntity
    {
        public Guid ProductId { get; set; }

        public Guid CustomerId { get; set; }
        public double RatingValue { get; set; }
        public string? Comment { get; set; }
    }
}