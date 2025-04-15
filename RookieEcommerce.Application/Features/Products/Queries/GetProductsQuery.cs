using MediatR;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.SharedViewModels.ProductDtos;

namespace RookieEcommerce.Application.Features.Products.Queries
{
    public class GetProductsQuery : PaginatedQuery, IRequest<PagedResult<ProductDetailsDto>>
    {
        public decimal? MaxPrice { get; set; }
        public decimal? MinPrice { get; set; }
    }

    public class GetProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsQuery, PagedResult<ProductDetailsDto>>
    {
        public async Task<PagedResult<ProductDetailsDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await productRepository.GetPaginated(request);

            //var productDtos = products.Items
            //    .Select(p => new ProductDto
            //    (
            //        p.Id,
            //        p.Name,
            //        p.Description,
            //        p.Price,
            //        p.CategoryId,
            //        p.Category?.Name,
            //        p.Images,
            //        p.Details,
            //        p.CreatedDate,
            //        p.ModifiedDate
            //    ))
            //    .ToList();

            var dtos = ProductMapper.ProductListToProductDetailsDto(products.Items);

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