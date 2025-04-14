namespace RookieEcommerce.SharedViewModels.ProductDtos
{
    public class ProductSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; }
        public string? Details { get; }
        public Guid? CategoryId { get; }
        public string? CategoryName { get; }
    }
}