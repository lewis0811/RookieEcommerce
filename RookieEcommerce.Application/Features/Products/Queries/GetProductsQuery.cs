using MediatR;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductDtos;
using System.ComponentModel.DataAnnotations;

namespace RookieEcommerce.Application.Features.Products.Queries
{
    public class GetProductsQuery : PaginatedQuery, IRequest<PaginationList<ProductDetailsDto>>
    {
        public Guid? CategoryId { get; set; }

        [Range(typeof(decimal), "1", "79228162514264337593543950335")]
        public decimal? MaxPrice { get; set; }

        [Range(typeof(decimal), "1", "79228162514264337593543950335")]
        public decimal? MinPrice { get; set; }

        public bool IsIncludeItems { get; set; }
    }

    public class GetProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsQuery, PaginationList<ProductDetailsDto>>
    {
        public async Task<PaginationList<ProductDetailsDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? query = null;

            // Check if IsIncludeItems is true, then include products and images
            if (request.IsIncludeItems)
            {
                query = filter => filter.Include(c => c.Images);
            }

            // Get paginated of products
            var products = await productRepository.GetPaginated(request, query);

            // Map to dto
            var dtos = ProductMapper.ProductListToProductDetailsDto(products.Items);

            // Map dto to page result and return
            var pagedResult = new PaginationList<ProductDetailsDto>(
                dtos,
                products.TotalCount,
                products.PageNumber,
                products.PageSize
                );

            return pagedResult;
        }
    }
}