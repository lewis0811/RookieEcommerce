using MediatR;
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

    public class UpdateVariantCommandHandler(IUnitOfWork unitOfWork, IProductVariantRepository productVariantRepository)
        : IRequestHandler<UpdateVariantCommand>
    {
        public async Task Handle(UpdateVariantCommand request, CancellationToken cancellationToken)
        {
            // Check if product variant exist
            var productVariant = await productVariantRepository.GetByIdAsync(request.Id, null, cancellationToken)
                ?? throw new InvalidOperationException($"Product variant with ID {request.Id} not found.");

            // Update properties
            productVariant.Update(request.Id, request.Name, request.Price, request.StockQuantity);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}