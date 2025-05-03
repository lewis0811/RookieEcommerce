namespace RookieEcommerce.Domain.Entities
{
    public class Cart : BaseEntity
    {
        // Foreign key
        public string CustomerId { get; set; } = "";

        // Navigation Properties
        public virtual Customer? Customer { get; set; } = new();

        public virtual ICollection<CartItem> Items { get; set; } = [];

        public static Cart Create(string customerId)
        {
            return new Cart { CustomerId = customerId, Customer = null };
        }
    }
}