namespace RookieEcommerce.SharedViewModels.ProductRatingDtos
{
    public class CreateProductRatingDto
    {
        public Guid ProductId { get; set; }
        public Guid CustomerId { get; set; }
        public double RatingValue { get; set; }
        public string? Comment { get; set; }
    }
}
