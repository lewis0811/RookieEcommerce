using MediatR;
using Microsoft.EntityFrameworkCore;
using RookieEcommerce.Application.Contacts.Persistence;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.ProductVariants.Commands
{
    public class UpdateVariantCommand : IRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string? Name { get; set; }
        public int? StockQuantity { get; set; }
        public decimal? Price { get; set; }
    }

    public class UpdateVariantCommandHandler(IUnitOfWork unitOfWork, IProductVariantRepository productVariantRepository, IProductRepository productRepository)
        : IRequestHandler<UpdateVariantCommand>
    {
        public async Task Handle(UpdateVariantCommand request, CancellationToken cancellationToken)
        {
            // Check if product variant exist
            var productVariant = await productVariantRepository.GetByIdAsync(request.Id, null, cancellationToken)
                ?? throw new InvalidOperationException($"Product variant with ID {request.Id} not found.");

            // Get the product of the product variant
            var product = await productRepository.GetByIdAsync(productVariant.ProductId, filter => filter.Include(c => c.Variants), cancellationToken)
                ?? throw new InvalidOperationException($"Product with ID {productVariant.ProductId} not found.");

            // Count the number of product quantity
            if (request.StockQuantity != null)
            {
                var totalStockQuantity = request.StockQuantity < productVariant.StockQuantity
                    ? product.TotalQuantity - (productVariant.StockQuantity - request.StockQuantity)
                    : product.TotalQuantity + (request.StockQuantity - productVariant.StockQuantity);

                product.Update(null, null, null, totalStockQuantity);
            }

            // Update properties
            productVariant.Update(request.Name, request.Price, request.StockQuantity);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}