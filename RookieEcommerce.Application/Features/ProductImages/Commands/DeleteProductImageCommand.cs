using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;

namespace RookieEcommerce.Application.Features.ProductImages.Commands
{
    public class DeleteProductImageCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class DeleteProductImageCommandHandler(IUnitOfWork unitOfWork, IProductImageRepository productImageRepository) : IRequestHandler<DeleteProductImageCommand>
    {
        public async Task Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
        {
            // Check if product image is exist
            var productImage = await productImageRepository.GetByIdAsync(request.Id, null, cancellationToken)
                ?? throw new InvalidOperationException($"Product image Id {request.Id} not found.");

            // Delete product image via Repository
            await productImageRepository.DeleteAsync(productImage, cancellationToken);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}