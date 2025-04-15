using MediatR;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.SharedViewModels.ProductDtos;

namespace RookieEcommerce.Application.Features.Products.Queries
{
    public class GetProductsQuery : PaginatedQuery, IRequest<PagedResult<ProductDetailsDto>>
    {
        public Guid? CategoryId { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinPrice { get; set; }
    }

    public class GetProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsQuery, PagedResult<ProductDetailsDto>>
    {
        public async Task<PagedResult<ProductDetailsDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            // Get paginated of products
            var products = await productRepository.GetPaginated(request);

            // Map to dto
            var dtos = ProductMapper.ProductListToProductDetailsDto(products.Items);

            // Map dto to page result and return
            var pagedResult = new PagedResult<ProductDetailsDto>(
                dtos,
                products.TotalCount,
                products.PageNumber,
                products.PageSize
                );

            return pagedResult;
        }
    }
}