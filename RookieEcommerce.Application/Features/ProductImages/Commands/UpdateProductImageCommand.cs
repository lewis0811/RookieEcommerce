using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.ProductImages.Commands
{
    public class UpdateProductImageCommand : IRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string? AltText { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsPrimary { get; set; }
    }

    public class UpdateProductImageCommandHandler(IUnitOfWork unitOfWork, IProductImageRepository productImageRepository) : IRequestHandler<UpdateProductImageCommand>
    {
        public async Task Handle(UpdateProductImageCommand request, CancellationToken cancellationToken)
        {
            // Check if the product image exist
            var productImage = await productImageRepository.GetByIdAsync(request.Id, null, cancellationToken)
                ?? throw new InvalidOperationException($"Product image Id {request.Id} not found.");

            // Check if request.SortOder != null, if then managed to swap the exist image that contain request's order number
            await AdjustSortOrderAsync(request, productImage, cancellationToken);

            // Map request to product image
            productImage.Update(request.AltText, request.SortOrder, request.IsPrimary);

            // Update via Repository
            await productImageRepository.UpdateAsync(productImage, cancellationToken);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task AdjustSortOrderAsync(UpdateProductImageCommand request, Domain.Entities.ProductImage productImage, CancellationToken cancellationToken)
        {
            if (request.SortOrder != null)
            {
                var totalImage = await productImageRepository.CountAsync(c => c.ProductId.Equals(productImage.ProductId), cancellationToken);
                // Set max, min sort order
                switch (request.SortOrder)
                {
                    case int s when s < 1:
                        request.SortOrder = 1;
                        break;

                    case int s when s > totalImage:
                        request.SortOrder = totalImage;
                        break;
                }

                // Get existing image that contain request's order number
                var existingImageAtTargetOrder = await productImageRepository
                    .GetByAttributeAsync(
                    img => img.ProductId == productImage.ProductId
                    && img.SortOrder == request.SortOrder && img.Id != productImage.Id, 
                    null, // Exclude self
                    cancellationToken);

                // Swap previous image sort order to exist image sort order if it not null
                if (existingImageAtTargetOrder != null)
                {
                    existingImageAtTargetOrder.Update(null, productImage.SortOrder, null);

                    // Update via Repository
                    await productImageRepository.UpdateAsync(existingImageAtTargetOrder, cancellationToken);
                }
            }
        }
    }
}