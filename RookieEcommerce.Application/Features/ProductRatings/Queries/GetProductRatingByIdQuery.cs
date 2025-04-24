using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductRatingDtos;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.ProductRatings.Queries
{
    public class GetProductRatingByIdQuery : IRequest<ProductRatingDetailsDto>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public bool IsIncludeItems { get; set; }
    }

    public class GetProductRatingByIdQueryHandler(IProductRatingRepository productRatingRepository) : IRequestHandler<GetProductRatingByIdQuery , ProductRatingDetailsDto>
    {

        public async Task<ProductRatingDetailsDto> Handle(GetProductRatingByIdQuery request, CancellationToken cancellationToken)
        {
            Func<IQueryable<ProductRating>, IIncludableQueryable<ProductRating, object>>? filter = null;
            
            // Check if the request is include items
            if (request.IsIncludeItems)
            {
                filter = query => query
                    .Include(c => c.Product!)
                        .ThenInclude(c => c.Variants)
                    .Include(c => c.Customer)!;
            }
            
            // Check if the rating exist
            var instance = await productRatingRepository.GetByIdAsync(request.Id, filter, cancellationToken)
                ?? throw new InvalidOperationException($"Produce Rating Id {request.Id} not found.");

            // Map to dto and return
            return ProductRatingMapper.ProductRatingToProductRatingDetailsDto(instance);
        }
    }
}