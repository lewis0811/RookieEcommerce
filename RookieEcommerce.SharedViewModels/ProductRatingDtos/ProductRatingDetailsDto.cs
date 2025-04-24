using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CustomerDtos;
using RookieEcommerce.SharedViewModels.ProductDtos;

namespace RookieEcommerce.SharedViewModels.ProductRatingDtos
{
    public class ProductRatingDetailsDto : BaseEntity
    {
        public double RatingValue { get; set; }
        public string? Comment { get; set; }

        public CustomerDetailsDto? Customer { get; set; }
        public ProductDetailsDto? Product { get; set; }
        public Guid ProductId { get; set; }
        public Guid CustomerId { get; set; }
    }
}