using MediatR;
using Microsoft.EntityFrameworkCore;
using RookieEcommerce.Application.Contacts.Persistence;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.CartItems.Commands
{
    public class UpdateCartItemCommand : IRequest
    {
        [JsonIgnore]
        public Guid CartId { get; set; }

        [JsonIgnore]
        public Guid ItemId { get; set; }

        public int Quantity { get; set; }
    }

    public class UpdateCartItemCommandHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateCartItemCommand>
    {
        private readonly ICartRepository _cartRepository = cartRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            // Check if the cart exist
            var cart = await _cartRepository.GetByIdAsync(
                request.CartId,
                c => c
                    .Include(c => c.Items!)
                        .ThenInclude(i => i.ProductVariant!),
                cancellationToken
             )
                ?? throw new InvalidOperationException($"Cart Id {request.CartId} not found.");

            // Get the item to update
            var itemToUpdate = cart.Items.FirstOrDefault(item => item.Id == request.ItemId)
                ?? throw new InvalidOperationException($"Item with Id {request.ItemId} not found in Cart {request.CartId}.");

            // Update the quantity on the found item
            itemToUpdate.Update(request.Quantity);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}