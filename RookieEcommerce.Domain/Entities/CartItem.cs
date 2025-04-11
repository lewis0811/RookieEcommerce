namespace RookieEcommerce.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public int Quantity { get; set; }

        // Foreign Keys
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        
        // Navigation Properties
        public virtual Cart Cart { get; set; } = new();
        public virtual Product Product { get; set; } = new();
    }
}