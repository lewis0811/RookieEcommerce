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
        public virtual Cart? Cart { get; private set; } = null;

        public virtual Product? Product { get; private set; } = null;
        public virtual ProductVariant? ProductVariant { get; private set; } = null;

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

        public void UpdateExist(int quantity)
        {
            Quantity += quantity;
            ModifiedDate = DateTime.Now;
        }

        public void Update(int quantity)
        {
            if (Quantity != quantity) { Quantity = quantity; }
            ModifiedDate = DateTime.Now;
        }

    }
}