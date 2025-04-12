using MediatR;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RookieEcommerce.Application.Features.Products.Queries
{
    public class GetProductsQuery : PaginatedQuery ,IRequest<PagedResult<ProductDto>>
    {
        public decimal? MaxPrice { get; set; }
        public decimal? MinPrice { get; set; }
    }
}
