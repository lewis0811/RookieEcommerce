using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.Products.Queries
{
    public class GetProductByIdQuery : IRequest<ProductDto>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string? IncludeProperties { get; set; }
    }

    public class GetProductByIdQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            // Check if the product exist
            var product = await productRepository.GetByIdAsync(request.Id, request.IncludeProperties, cancellationToken)
                ?? throw new InvalidOperationException($"Product Id {request.Id} not found.");

            // Map to dto and return
            return new ProductDto(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.CategoryId,
                product.Category?.Name,
                product.Images,
                product.Details,
                product.CreatedDate,
                product.ModifiedDate
                );
        }
    }
}