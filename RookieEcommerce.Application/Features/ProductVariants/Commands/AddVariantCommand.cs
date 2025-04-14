using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.Domain.Enums;
using RookieEcommerce.SharedViewModels;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.ProductVariants.Commands
{
    public class AddVariantCommand : IRequest<ProductVariantDto>
    {
        [JsonIgnore]
        public Guid ProductId { get; set; }
        public PVariantType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
    }

    public class AddVariantCommandHandler(IUnitOfWork unitOfWork, IProductRepository productRepository, IProductVariantRepository productVariantRepository)
        : IRequestHandler<AddVariantCommand, ProductVariantDto>
    {
        public async Task<ProductVariantDto> Handle(AddVariantCommand request, CancellationToken cancellationToken)
        {
            // Check if product exist
            var productExist = await productRepository.AnyAsync(c => c.Id == request.ProductId, cancellationToken);
            if (!productExist) { throw new InvalidOperationException
                    ($"Product with ID { request.ProductId } not found."); }

            // Create new variant entity
            var variant = new ProductVariant
            {
                ProductId = request.ProductId,
                Name = request.Name,
                Sku = request.Sku,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                VariantType = request.Type
            };

            // Add entity via Repository
            await productVariantRepository.AddAsync(variant, cancellationToken);

            // Save changes via Unit Of Work
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to Dto and return
            return new ProductVariantDto(
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
        }
    }
}