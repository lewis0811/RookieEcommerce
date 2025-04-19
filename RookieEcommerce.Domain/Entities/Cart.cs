namespace RookieEcommerce.Domain.Entities
{
    public class Cart : BaseEntity
    {
        // Foreign key
        public Guid CustomerId { get; set; }

        // Navigation Properties
        public virtual Customer? Customer { get; set; } = new();

        public virtual ICollection<CartItem> Items { get; set; } = [];

        public static Cart Create(Guid customerId)
        {
            return new Cart { CustomerId = customerId, Customer = null };
        }
    }
}