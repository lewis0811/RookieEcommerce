namespace RookieEcommerce.CustomerSite.Models
{
    public class CartItemViewModel
    {
        public string? ProductName { get; set; }
        public string? ProductVariantName { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public string? ImageUrl { get; set; }
    }
}