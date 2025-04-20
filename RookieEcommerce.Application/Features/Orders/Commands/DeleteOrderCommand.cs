using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.Orders.Commands
{
    public class DeleteOrderCommand : IRequest
    {
        [JsonIgnore]
        public Guid OrderId { get; set; }
    }

    public class DeleteOrderCommandHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository) : IRequestHandler<DeleteOrderCommand>
    {
        public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            // Check if order exist
            var order = await orderRepository.GetByIdAsync(request.OrderId, null, cancellationToken)
                ?? throw new InvalidOperationException($"Order with Id {request.OrderId} not found.");

            // Delete order via Repository
            await orderRepository.DeleteAsync(order, cancellationToken);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}