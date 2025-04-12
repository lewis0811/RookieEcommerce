using MediatR;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.SharedViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RookieEcommerce.Application.Features.Products.Queries
{
    public class GetProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
    {
        public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await productRepository.GetPaginatedProduct(request);

            var productDtos = products.Items
                .Select(p => new ProductDto
                (
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.Category!.Name,
                    p.Images,
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
