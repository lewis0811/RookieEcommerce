using MediatR;
using Microsoft.EntityFrameworkCore;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Domain.Enums;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.Orders.Commands
{
    public class UpdateOrderCommand : IRequest
    {
        [JsonIgnore]
        public Guid OrderId { get; set; }
        public string? TransactionId { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
    }

    public class UpdateOrderCommandHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, ICartRepository cartRepository) : IRequestHandler<UpdateOrderCommand>
    {
        public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAsync(request.OrderId,
                filter => filter.Include(c => c.OrderItems), 
                cancellationToken) 
                ?? throw new InvalidOperationException($"Order Id {request.OrderId} not found.");
            
            order.Update(request.TransactionId, null, request.PaymentStatus);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Check if order payment is successful then delete cart items that have in order items
            if (order.PaymentStatus == PaymentStatus.Succeed)
            {
                // Get the cart of order's customer via Repository
                var cart = await cartRepository
                    .GetByAttributeAsync(c => c.CustomerId == order.CustomerId,
                    filter => filter.Include(c => c.Items)
                    , cancellationToken)
                    ?? throw new InvalidOperationException($"Cart Id {order.CustomerId} not found.");

                // Remove ordered items in the cart
                foreach (var orderItem in order.OrderItems)
                {
                    var cartItem = cart.Items.FirstOrDefault(c => c.ProductId == orderItem.ProductId);
                    if (cartItem != null)
                    {
                        cart.Items.Remove(cartItem);
                    }
                }

                // Update order's status
                order.Update(null, OrderStatus.Ordered, null);

                // Save changes 
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}