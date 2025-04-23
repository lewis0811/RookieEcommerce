using MediatR;
using Microsoft.EntityFrameworkCore;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductDtos;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.Products.Queries
{
    public class GetProductByIdQuery : IRequest<ProductDetailsDto>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public bool IsIncludeItems { get; set; }
    }

    public class GetProductByIdQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductByIdQuery, ProductDetailsDto>
    {
        public async Task<ProductDetailsDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            // Check if the product exist
            var product = await productRepository
                .GetByIdAsync(request.Id,
                filter => filter
                    .Include(c => c.Category)
                    .Include(c => c.Variants)
                    .Include(c => c.Images), 
                cancellationToken)
                ?? throw new InvalidOperationException($"Product Id {request.Id} not found.");

            // Map to dto and return
            return ProductMapper.ProductToProductDetailsDto(product);
        }
    }
}