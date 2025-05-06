using MediatR;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.SharedViewModels.ProductVariantDtos;
using System.ComponentModel.DataAnnotations;

namespace RookieEcommerce.Application.Features.ProductVariants.Queries
{
    public class GetProductVariantsQuery : PaginatedQuery, IRequest<PaginationList<ProductVariantDetailsDto>>
    {
        public Guid? ProductId { get; set; }
        public string? VariantType { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? MinPrice { get; set; } = 0;

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? MaxPrice { get; set; }
    }

    public class GetProductVariantsQueryHandler(IProductVariantRepository productVariantRepository)
        : IRequestHandler<GetProductVariantsQuery, PaginationList<ProductVariantDetailsDto>>
    {
        public async Task<PaginationList<ProductVariantDetailsDto>> Handle(GetProductVariantsQuery request, CancellationToken cancellationToken)
        {
            var variants = await productVariantRepository.GetPaginated(request);

            var dtos = ProductVariantMapper.ProductVariantListToProductVariantDetailsDto(variants.Items);

            return new PaginationList<ProductVariantDetailsDto>(
                dtos,
                variants.TotalCount,
                variants.PageNumber,
                variants.PageSize
                );
        }
    }
}