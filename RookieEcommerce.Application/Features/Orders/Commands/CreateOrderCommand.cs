using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.Domain.Enums;
using RookieEcommerce.SharedViewModels.OrderDtos;

namespace RookieEcommerce.Application.Features.Orders.Commands
{
    public class CreateOrderCommand : IRequest<OrderCreateDto>
    {
        public string Email { get; set; }
        public Guid CustomerId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public Address ShippingAddress { get; set; } = new();
        public List<CreateOrderItemDto> OrderItems { get; set; } = [];
    }

    public class CreateOrderCommandHandler(IUnitOfWork unitOfWork,
        IOrderRepository orderRepository, 
        IProductVariantRepository productVariantRepository,
        IProductRepository productRepository,
        IEmailService emailService)
        : IRequestHandler<CreateOrderCommand, OrderCreateDto>
    {
        public async Task<OrderCreateDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            decimal calculatedTotalAmount = 0;
            var orderItemsEntities = new List<OrderItem>();

            // --- Step 1: Calculate Total and Prepare OrderItem Entities (with Variant Logic) ---
            foreach (var itemDto in request.OrderItems)
            {
                decimal unitPrice;
                Guid productId = itemDto.ProductId;
                Guid? productVariantId = itemDto.ProductVariantId;
                int quantity = itemDto.Quantity;

                // Determine Price based on Variant ID
                if (productVariantId.HasValue && productVariantId.Value != Guid.Empty)
                {
                    // Use Product Variant
                    var variant = await productVariantRepository.GetByIdAsync(productVariantId.Value);

                    // Check if variant exists AND belongs to the base product
                    if (variant == null || variant.ProductId != productId)
                    {
                        throw new InvalidOperationException($"Invalid Product Variant ID {productVariantId.Value} for Product ID {productId}.");
                    }
                    unitPrice = variant.Price; // Get price from variant
                }
                else
                {
                    // Use Base Product
                    var product = await productRepository.GetByIdAsync(productId) 
                        ?? throw new InvalidOperationException($"Product with ID {productId} not found.");
                    
                    unitPrice = product.Price; // Get price from product
                }

                // Calculate line total
                decimal lineTotal = unitPrice * quantity;
                calculatedTotalAmount += lineTotal;

                // Create the domain entity for the order item
                orderItemsEntities.Add(new OrderItem
                {
                    ProductId = productId,
                    ProductVariantId = productVariantId,
                    Quantity = quantity,
                    Price = unitPrice
                });
            }

            // Create the Order 
            var order = Order.Create(request.CustomerId, calculatedTotalAmount, request.PaymentMethod, request.ShippingAddress, orderItemsEntities);

            // Add to repository
            await orderRepository.AddAsync(order, cancellationToken);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Send Order Confirmation Email
            await emailService.SendEmailAsync(request.Email, "Order Confirmation", "Your order has been placed successfully!");

            // Map to DTO and return
            var orderDto = OrderMapper.OrderToOrderCreateDto(order);
            return orderDto;
        }
    }
}