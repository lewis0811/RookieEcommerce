using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.ProductVariants.Commands
{
    public class DeleteVariantCommand : IRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }
    }

    public class DeleteVariantCommandHandler(IUnitOfWork unitOfWork, IProductVariantRepository productVariantRepository, IProductRepository productRepository) : IRequestHandler<DeleteVariantCommand>
    {
        public async Task Handle(DeleteVariantCommand request, CancellationToken cancellationToken)
        {
            // Check if product variant exist
            var productVariant = await productVariantRepository.GetByIdAsync(request.Id, null, cancellationToken)
                ?? throw new InvalidOperationException($"Product variant with ID {request.Id} not found.");

            // Get the product of the product variant
            var product = await productRepository.GetByIdAsync(productVariant.ProductId, null, cancellationToken)
                ?? throw new InvalidOperationException($"Product with ID {productVariant.ProductId} not found.");

            // Count the number of product quantity
            var totalStockQuantity = product.TotalQuantity - productVariant.StockQuantity;

            // Delete the product via Repository
            await productVariantRepository.DeleteAsync(productVariant, cancellationToken);

            // Update the product quantity
            product.Update(null, null, null, totalStockQuantity);

            // Update the product via Repository
            await productRepository.UpdateAsync(product, cancellationToken);

            // Save changes via Unit of work
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}