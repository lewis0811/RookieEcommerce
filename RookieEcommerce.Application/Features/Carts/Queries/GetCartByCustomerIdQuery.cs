using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CartDtos;

namespace RookieEcommerce.Application.Features.Carts.Queries
{
    public class GetCartByCustomerIdQuery : IRequest<CartDetailsDto>
    {
        public Guid CustomerId { get; set; }
        public bool IsIncludeItems { get; set; }
    }

    public class GetCartByCustomerIdQueryHandler(ICartRepository cartRepository) : IRequestHandler<GetCartByCustomerIdQuery, CartDetailsDto>
    {
        public async Task<CartDetailsDto> Handle(GetCartByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            // Check the cart
            Func<IQueryable<Cart>, IIncludableQueryable<Cart, object>>? includeExpression = null;
            if (request.IsIncludeItems)
            {
                includeExpression = query => query
                    .Include(c => c.Items)
                        .ThenInclude(c => c.Product)
                            .ThenInclude(c => c!.Images)
                    .Include(c => c.Items)
                        .ThenInclude(c => c.ProductVariant!);
            }

            // Get cart via repository
            var cart = await cartRepository.GetByAttributeAsync(
                    c => c.CustomerId == request.CustomerId.ToString(),
                    includeExpression,
                    cancellationToken)
                ?? throw new InvalidOperationException($"Cart for Customer {request.CustomerId} not found.");

            // Mapping to dto and return
            return CartMapper.CartToCartDetailsDto(cart);
        }
    }
}