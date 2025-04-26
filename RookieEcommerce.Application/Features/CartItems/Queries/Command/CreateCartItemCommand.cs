using MediatR;
using Microsoft.EntityFrameworkCore;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CartDtos;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.CartItems.Queries.Command
{
    public class CreateCartItemCommand : IRequest<CartItemCreateDto>
    {
        [JsonIgnore]
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateCartItemCommandHandler(
        IUnitOfWork unitOfWork,
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IProductVariantRepository productVariantRepository
        ) : IRequestHandler<CreateCartItemCommand, CartItemCreateDto>
    {
        public async Task<CartItemCreateDto> Handle(CreateCartItemCommand request, CancellationToken cancellationToken)
        {
            // Validate cart exists
            var cart = await cartRepository.GetByIdAsync(request.CartId, query => query.Include(c => c.Items), cancellationToken)
                ?? throw new InvalidOperationException($"Cart with id {request.CartId} does not exist.");
            // Validate product variant exist if provided
            var productVariant = await productVariantRepository.AnyAsync(c => c.Id == request.ProductVariantId, cancellationToken);
            if (request.ProductVariantId != null && !productVariant)
            {
                throw new InvalidOperationException($"Product variant with id {request.ProductVariantId} does not exist.");
            }

            // Validate product exists
            var product = await productRepository.AnyAsync(c => c.Id == request.ProductId, cancellationToken);
            if (!product)
            {
                throw new InvalidOperationException($"Product with id {request.ProductId} does not exist.");
            }

            // Check if item already exists in cart
            var cartItem = productVariant
                ? cart.Items.FirstOrDefault(c => c.ProductVariantId == request.ProductVariantId)
                : cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (cartItem != null)
            {
                // Update existing item quantity
                cartItem.UpdateExist(request.Quantity);
            }
            else
            {
                cartItem = CartItem.Create
                (
                    request.CartId,
                    request.ProductId,
                    request.ProductVariantId,
                    request.Quantity
                );
                // Add new item to cart
                cart.Items.Add(cartItem);
            }

            // Update via Repository
            await cartRepository.UpdateAsync(cart, cancellationToken);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO and return
            return CartMapper.CartItemToCartItemCreateDto(cartItem);
        }
    }
}