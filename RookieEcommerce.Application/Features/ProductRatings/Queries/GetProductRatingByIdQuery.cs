using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.SharedViewModels.ProductRatingDtos;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.ProductRatings.Queries
{
    public class GetProductRatingByIdQuery : IRequest<ProductRatingDetailsDto>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
    }

    public class GetProductRatingByIdQueryHandler(IProductRatingRepository productRatingRepository) : IRequestHandler<GetProductRatingByIdQuery , ProductRatingDetailsDto>
    {

        public async Task<ProductRatingDetailsDto> Handle(GetProductRatingByIdQuery request, CancellationToken cancellationToken)
        {
            // Check if the rating exist
            var instance = await productRatingRepository.GetByIdAsync(request.Id, null, cancellationToken)
                ?? throw new InvalidOperationException($"Produce Rating Id {request.Id} not found.");

            // Map to dto and return
            return ProductRatingMapper.ProductRatingToProductRatingDetailsDto(instance);
        }
    }
}