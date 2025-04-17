using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductImageDtos;

namespace RookieEcommerce.Application.Features.ProductImages.Commands
{
    public class CreateProductImageCommand : IRequest<ProductImageCreateDto>
    {
        public Guid ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
    }

    public class CreateProductImageCommandHandler(IUnitOfWork unitOfWork, IProductImageRepository productImageRepository, IProductRepository productRepository)
        : IRequestHandler<CreateProductImageCommand, ProductImageCreateDto>
    {
        public async Task<ProductImageCreateDto> Handle(CreateProductImageCommand request, CancellationToken cancellationToken)
        {
            // Check if the product is exist
            var productExist = await productRepository.AnyAsync(c => c.Id == request.ProductId, cancellationToken);
            if (!productExist) { throw new InvalidOperationException($"Product Id {request.ProductId} not found."); }

            // Create product image instance
            var productImage = ProductImage.Create(request.ProductId, request.ImageUrl, request.AltText);

            // Check for exist product's images
            var existImages = await productImageRepository.CountAsync(c => c.ProductId.Equals(request.ProductId), cancellationToken);
            productImage.SortOrder += existImages + 1;

            // Add product image via repo
            await productImageRepository.AddAsync(productImage, cancellationToken);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to dto and return
            var dto = ProductImageMapper.ProductImageToProductImageCreateDto(productImage);

            return dto;
        }
    }
}