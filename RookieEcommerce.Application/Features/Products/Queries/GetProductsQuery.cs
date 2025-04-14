using MediatR;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.SharedViewModels.ProductDtos;

namespace RookieEcommerce.Application.Features.Products.Queries
{
    public class GetProductsQuery : PaginatedQuery, IRequest<PagedResult<ProductDto>>
    {
        public decimal? MaxPrice { get; set; }
        public decimal? MinPrice { get; set; }
    }

    public class GetProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
    {
        public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await productRepository.GetPaginated(request);

            var productDtos = products.Items
                .Select(p => new ProductDto
                (
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.CategoryId,
                    p.Category?.Name,
                    p.Images,
                    p.Details,
                    p.CreatedDate,
                    p.ModifiedDate
                ))
                .ToList();

            var pagedResult = new PagedResult<ProductDto>(
                productDtos,
                products.TotalCount,
                products.PageNumber,
                products.PageSize
                );

            return pagedResult;
        }
    }
}