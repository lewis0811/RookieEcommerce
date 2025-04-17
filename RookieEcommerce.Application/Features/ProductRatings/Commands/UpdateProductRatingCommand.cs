using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.ProductRatings.Commands
{
    public class UpdateProductRatingCommand : IRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public double? RatingValue { get; set; }
        public string? Comment { get; set; }
    }

    public class UpdateProductRatingCommandHandler(IUnitOfWork unitOfWork, IProductRatingRepository productRatingRepository) 
        : IRequestHandler<UpdateProductRatingCommand>
    {
        public async Task Handle(UpdateProductRatingCommand request, CancellationToken cancellationToken)
        {
            // Check if the product rating exist
            var productRating = await productRatingRepository.GetByIdAsync(request.Id, null, cancellationToken)
                ?? throw new InvalidOperationException($"Product rating Id {request.Id} not found.");

            // Map request to product rating
            productRating.Update(request.RatingValue, request.Comment);

            // Update via Repository
            await productRatingRepository.UpdateAsync(productRating, cancellationToken);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}