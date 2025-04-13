using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.SharedViewModels;

namespace RookieEcommerce.Application.Features.Products.Variants.Queries
{
    public class GetProductVariantByIdQuery : IRequest<ProductVariantDto>
    {
        public Guid Id { get; set; }
    }

    public class GetProductVariantByIdQueryHandler(IProductVariantRepository productVariantRepository) : IRequestHandler<GetProductVariantByIdQuery, ProductVariantDto>
    {
        public async Task<ProductVariantDto> Handle(GetProductVariantByIdQuery request, CancellationToken cancellationToken)
        {
            // Check if the product variant exist
            var variant = await productVariantRepository.GetByIdAsync(request.Id, null, cancellationToken)
                ?? throw new KeyNotFoundException($"Product variant Id {request.Id} not found.");

            var variantDto = new ProductVariantDto(
                variant.Id,
                variant.ProductId,
                variant.VariantType,
                variant.Name,
                variant.Sku,
                variant.StockQuantity,
                variant.Price,
                variant.CreatedDate,
                variant.ModifiedDate
                );

            return variantDto;
        }
    }
}