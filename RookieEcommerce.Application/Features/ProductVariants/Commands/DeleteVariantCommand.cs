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

    public class DeleteVariantCommandHandler(IUnitOfWork unitOfWork, IProductVariantRepository productVariantRepository) : IRequestHandler<DeleteVariantCommand>
    {
        public async Task Handle(DeleteVariantCommand request, CancellationToken cancellationToken)
        {
            // Check if product variant exist
            var productVariant = await productVariantRepository.GetByIdAsync(request.Id, null, cancellationToken)
                ?? throw new InvalidOperationException($"Product variant with ID {request.Id} not found.");

            // Delete the product via Repository
            await productVariantRepository.DeleteAsync(productVariant, cancellationToken);

            // Save changes via Unit of work
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}