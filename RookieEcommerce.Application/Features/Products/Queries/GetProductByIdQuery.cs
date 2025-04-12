using MediatR;
using RookieEcommerce.SharedViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RookieEcommerce.Application.Features.Products.Queries
{
    public class GetProductByIdQuery() : IRequest<ProductDto?>
    {
        public string? IncludeProperties { get; set; }
    }
}
