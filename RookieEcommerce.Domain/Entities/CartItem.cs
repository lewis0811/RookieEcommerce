namespace RookieEcommerce.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public int Quantity { get; private set; }

        // Foreign Keys
        public Guid CartId { get; private set; }

        public Guid ProductId { get; private set; }
        public Guid? ProductVariantId { get; private set; }

        // Navigation Properties
        public virtual Cart Cart { get; private set; } = new();

        public virtual Product Product { get; private set; } = new();
        public virtual ProductVariant ProductVariant { get; private set; } = new();

        // Methods
        public static CartItem Create(Guid cartId, Guid productId, Guid? productVariantId, int quantity)
        {
            return new CartItem
            {
                CartId = cartId,
                ProductId = productId,
                ProductVariantId = productVariantId,
                Quantity = quantity
            };
        }

        public void Update(int quantity)
        {
            if (quantity != Quantity) { Quantity = quantity; }
            ModifiedDate = DateTime.Now;
        }
    }
}