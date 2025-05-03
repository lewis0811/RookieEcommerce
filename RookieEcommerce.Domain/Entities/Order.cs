using RookieEcommerce.Domain.Enums;

namespace RookieEcommerce.Domain.Entities
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }

        // Store Shipping Address components
        public Address ShippingAddress { get; set; } = new();

        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? TransactionId { get; set; } // ID from payment gateway
        public DateTime? PaymentDate { get; set; }

        // Foreign Key
        public string CustomerId { get; set; } = "";

        // Navigation Properties
        public virtual Customer? Customer { get; set; } = null;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];

        public static Order Create(string customerId, decimal calculatedTotalAmount, PaymentMethod paymentMethod, Address shippingAddress, List<OrderItem> orderItemsEntities)
        {
            return new Order
            {
                CustomerId = customerId,
                TotalAmount = calculatedTotalAmount,
                PaymentMethod = paymentMethod,
                ShippingAddress = shippingAddress,
                OrderItems = orderItemsEntities,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending
            };
        }

        public void Update(string? transactionId, OrderStatus? orderStatus, PaymentStatus? paymentStatus)
        {
            if (transactionId != null && transactionId != TransactionId) { TransactionId = transactionId; }
            if (orderStatus != null && orderStatus != Status) { Status = orderStatus.Value; }
            if (paymentStatus != null && paymentStatus != PaymentStatus) { PaymentStatus = paymentStatus.Value; }
            if (paymentStatus == PaymentStatus.Succeed)
            {
                PaymentDate = DateTime.Now;
            }

            UpdateModifiedDate();
        }
    }
}