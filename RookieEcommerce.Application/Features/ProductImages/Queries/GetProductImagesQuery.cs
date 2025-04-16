using MediatR;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.SharedViewModels.ProductImageDtos;

namespace RookieEcommerce.Application.Features.ProductImages.Queries
{
    public class GetProductImagesQuery : PaginatedQuery, IRequest<PaginationList<ProductImageDetailsDto>>
    {
        public Guid? ProductId { get; set; }
        public bool? IsPrimary { get; set; }
    }

    public class GetProductImagesQueryHandler(IProductImageRepository productImageRepository) : IRequestHandler<GetProductImagesQuery, PaginationList<ProductImageDetailsDto>>
    {
        public async Task<PaginationList<ProductImageDetailsDto>> Handle(GetProductImagesQuery request, CancellationToken cancellationToken)
        {
            // Get paginated of product images
            var pgList = await productImageRepository.GetPaginated(request);

            // Map to dto
            var dtos = ProductImageMapper.ProductImageListToProductImageDeatilsDto(pgList.Items);

            // Map dto to page result and return
            var pagedResult = new PaginationList<ProductImageDetailsDto>(
                dtos,
                pgList.TotalCount,
                pgList.PageNumber,
                pgList.PageSize
                );

            return pagedResult;
        }
    }
}