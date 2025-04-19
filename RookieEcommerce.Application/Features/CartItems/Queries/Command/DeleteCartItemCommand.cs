using MediatR;
using Microsoft.EntityFrameworkCore;
using RookieEcommerce.Application.Contacts.Persistence;

namespace RookieEcommerce.Application.Features.CartItems.Queries.Command
{
    public class DeleteCartItemCommand : IRequest
    {
        public Guid CartId { get; set; }
        public Guid ItemId { get; set; }
    }

    public class DeleteCardItemCommandHandler(IUnitOfWork unitOfWork, ICartRepository cartRepository) : IRequestHandler<DeleteCartItemCommand>
    {
        public async Task Handle(DeleteCartItemCommand request, CancellationToken cancellationToken)
        {
            // Check if cart item exits
            // Get the specific Cart aggregate root (including items)
            var cart = await cartRepository.GetByIdAsync(request.CartId, query => query.Include(c => c.Items), cancellationToken) 
                ?? throw new InvalidOperationException($"Cart ID {request.CartId} not found.");

            // Find the specific CartItem within the aggregate
            var itemToRemove = cart.Items.FirstOrDefault(item => item.Id == request.ItemId) 
                ?? throw new InvalidOperationException($"CartItem {request.ItemId} not found in Cart {request.CartId}.");

            // Remove the item from the aggregate's collection
            cart.Items.Remove(itemToRemove);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}