using MediatR;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.SharedViewModels.ProductRatingDtos;

namespace RookieEcommerce.Application.Features.ProductRatings.Queries
{
    public class GetProductRatingQuery : PaginatedQuery, IRequest<PaginationList<ProductRatingDetailsDto>>
    {
        public Guid? ProductId { get; set; }
        public Guid? CustomerId { get; set; }
        public double? MinRatingValue { get; set; }
        public double? MaxRatingValue { get; set; }
    }

    public class GetProductRatingQueryHandler(IProductRatingRepository productRatingRepository) : IRequestHandler<GetProductRatingQuery, PaginationList<ProductRatingDetailsDto>>
    {
        public async Task<PaginationList<ProductRatingDetailsDto>> Handle(GetProductRatingQuery request, CancellationToken cancellationToken)
        {
            // Get paginated of product images
            var pgList = await productRatingRepository.GetPaginated(request);

            // Map to dto
            var dtos = ProductRatingMapper.ProductRatingListToProductRatingDetailsDto(pgList.Items);

            // Map dto to page result and return
            var pagedResult = new PaginationList<ProductRatingDetailsDto>(
                dtos,
                pgList.TotalCount,
                pgList.PageNumber,
                pgList.PageSize
                );

            return pagedResult;
        }
    }
}