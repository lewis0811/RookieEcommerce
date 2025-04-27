namespace RookieEcommerce.CustomerSite.Models
{
    public class CartSummaryViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal TotalPrice { get; set; }
    }
}
