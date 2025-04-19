using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CartDtos;

namespace RookieEcommerce.Application.Features.Carts.Commands
{
    public class CreateCartCommand : IRequest<CartCreateDto>
    {
        public Guid CustomerId { get; set; }
    }

    public class CreateCartCommandHandler(IUnitOfWork unitOfWork, ICartRepository cartRepository, ICustomerRepository customerRepository) : IRequestHandler<CreateCartCommand, CartCreateDto>
    {
        public async Task<CartCreateDto> Handle(CreateCartCommand request, CancellationToken cancellationToken)
        {
            // Check if customer exist
            var existCustomer = await customerRepository.AnyAsync(c => c.Id == request.CustomerId, cancellationToken);
            if (!existCustomer)
            {
                throw new InvalidOperationException($"Customer with id {request.CustomerId} does not exist.");
            }

            // Create new cart
            var cart = Cart.Create(request.CustomerId);

            // Add cart to repository
            await cartRepository.AddAsync(cart, cancellationToken);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return CartMapper.CartToCartCreateDto(cart);
        }
    }
}