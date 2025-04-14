using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.SharedViewModels;

namespace RookieEcommerce.Application.Features.ProductVariants.Queries
{
    public record GetProductVariantsByProductQuery(Guid ProductId) : IRequest<IEnumerable<ProductVariantDto>>
    {
    }

    public class GetProductVariantsQueryHandler(IProductVariantRepository productVariantRepository)
        : IRequestHandler<GetProductVariantsByProductQuery, IEnumerable<ProductVariantDto>>
    {
        public async Task<IEnumerable<ProductVariantDto>> Handle(GetProductVariantsByProductQuery request, CancellationToken cancellationToken)
        {
            var variants = await productVariantRepository.ListAllAsync(c => c.ProductId == request.ProductId, cancellationToken);

            var variantDtos = variants.Select(pv => new ProductVariantDto(
                    pv.Id,
                    pv.ProductId,
                    pv.VariantType,
                    pv.Name,
                    pv.Sku,
                    pv.StockQuantity,
                    pv.Price,
                    pv.CreatedDate,
                    pv.ModifiedDate
                ))
            .ToList();

            return variantDtos;
        }
    }
}