using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.ProductRatings.Commands
{
    public class DeleteProductRatingCommand : IRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }
    }

    public class DeleteProductRatingCommandHandler(IUnitOfWork unitOfWork, IProductRatingRepository productRatingRepository) : IRequestHandler<DeleteProductRatingCommand>
    {
        public async Task Handle(DeleteProductRatingCommand request, CancellationToken cancellationToken)
        {
            // Check if product rating is exist
            var productRating = await productRatingRepository.GetByIdAsync(request.Id, null, cancellationToken)
                ?? throw new InvalidOperationException($"Product rating Id {request.Id} not found.");

            // Delete product rating via Repository
            await productRatingRepository.DeleteAsync(productRating, cancellationToken);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}