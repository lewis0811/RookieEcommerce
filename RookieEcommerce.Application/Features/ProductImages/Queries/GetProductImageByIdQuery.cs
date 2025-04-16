using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.SharedViewModels.ProductImageDtos;

namespace RookieEcommerce.Application.Features.ProductImages.Queries
{
    public class GetProductImageByIdQuery : IRequest<ProductImageDetailsDto>
    {
        public Guid Id { get; set; }
    }

    public class GetProductImageByIdQueryHandler(IProductImageRepository productImageRepository) : IRequestHandler<GetProductImageByIdQuery, ProductImageDetailsDto>
    {
        public async Task<ProductImageDetailsDto> Handle(GetProductImageByIdQuery request, CancellationToken cancellationToken)
        {
            // Check if the image exist
            var instance = await productImageRepository.GetByIdAsync(request.Id, null, cancellationToken)
                ?? throw new InvalidOperationException($"Produce Image Id {request.Id} not found.");

            // Map to dto and return
            return ProductImageMapper.ProductImageToProductImageDetailsDto(instance);
        }
    }
}