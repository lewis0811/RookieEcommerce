using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.Products.Commands
{
    public record DeleteProductCommand : IRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }
    }

    public class DeleteProductCommandHandler(IUnitOfWork unitOfWork, IProductRepository productRepository) : IRequestHandler<DeleteProductCommand>
    {
        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            // Check if the product exist
            var product = await productRepository.GetByIdAsync(request.Id, null, cancellationToken)
                ?? throw new InvalidOperationException ($"Product Id {request.Id} not found.");

            // Delete product via Repository
            await productRepository.DeleteAsync(product, cancellationToken);

            // Save changes via UnitOfWork
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}