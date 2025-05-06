using MediatR;
using Microsoft.EntityFrameworkCore;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductVariantDtos;
using System.ComponentModel.DataAnnotations;

namespace RookieEcommerce.Application.Features.ProductVariants.Commands
{
    public class CreateVariantCommand : IRequest<ProductVariantCreateDto>
    {
        public Guid ProductId { get; set; }

        public string VariantType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        [Range(1, int.MaxValue)]
        public int StockQuantity { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Price { get; set; }
    }

    public class CreateVariantCommandHandler(IUnitOfWork unitOfWork, IProductRepository productRepository, IProductVariantRepository productVariantRepository)
        : IRequestHandler<CreateVariantCommand, ProductVariantCreateDto>
    {
        public async Task<ProductVariantCreateDto> Handle(CreateVariantCommand request, CancellationToken cancellationToken)
        {
            // Check if product exist
            var productExist = await productRepository
                .GetByIdAsync(request.ProductId,
                filter => filter.Include(c => c.Variants),
                cancellationToken)
                ?? throw new InvalidOperationException($"Product with ID {request.ProductId} not found.");

            // Check if variant exist
            var existVariant = await productVariantRepository.AnyAsync(c => c.Name.ToLower().Equals(request.Name), cancellationToken);
            if (existVariant) throw new InvalidOperationException($"Product variant name {request.Name} already exist.");

            // Create new variant entity
            var variant = ProductVariant.Create(request.ProductId, request.Name, request.Price, request.StockQuantity, request.VariantType);

            // Generate Sku number
            var existingSkus = productExist.Sku;
            var numberOfVariant = productExist.Variants.Count;
            numberOfVariant++;
            variant.Sku = existingSkus + "-" + numberOfVariant;

            // Add entity via Repository
            await productVariantRepository.AddAsync(variant, cancellationToken);

            // Save changes via Unit Of Work
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to Dto and return
            return ProductVariantMapper.ProductVariantToProductVariantCreateDto(variant);
        }
    }
}