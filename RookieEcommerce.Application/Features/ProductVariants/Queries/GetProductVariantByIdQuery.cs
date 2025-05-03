using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.SharedViewModels.ProductVariantDtos;

namespace RookieEcommerce.Application.Features.ProductVariants.Queries
{
    public class GetProductVariantByIdQuery : IRequest<ProductVariantDetailsDto>
    {
        public Guid Id { get; set; }
    }

    public class GetProductVariantByIdQueryHandler(IProductVariantRepository productVariantRepository) : IRequestHandler<GetProductVariantByIdQuery, ProductVariantDetailsDto>
    {
        public async Task<ProductVariantDetailsDto> Handle(GetProductVariantByIdQuery request, CancellationToken cancellationToken)
        {
            // Check if the product variant exist
            var variant = await productVariantRepository.GetByIdAsync(request.Id, null, cancellationToken)
                ?? throw new InvalidOperationException($"Product variant Id {request.Id} not found.");

            // Map product to dto and return
            var dto = ProductVariantMapper.ProductVariantToProductVariantDetailsDto(variant);

            return dto;
        }
    }
}