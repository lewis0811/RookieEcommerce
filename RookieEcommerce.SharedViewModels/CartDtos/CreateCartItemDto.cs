namespace RookieEcommerce.SharedViewModels.CartDtos
{
    public class CreateCartItemDto
    {
        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}