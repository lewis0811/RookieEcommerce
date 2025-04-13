﻿using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.Products.Variants.Commands
{
    public class UpdateVariantCommand : IRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
    }

    public class UpdateVariantCommandHandler(IUnitOfWork unitOfWork, IProductVariantRepository productVariantRepository, IProductRepository productRepository)
        : IRequestHandler<UpdateVariantCommand>
    {
        public async Task Handle(UpdateVariantCommand request, CancellationToken cancellationToken)
        {
            // Check if product variant exist
            var productVariant = await productVariantRepository.GetByIdAsync(request.Id, null, cancellationToken)
                ?? throw new KeyNotFoundException($"Product variant with ID {request.Id} not found.");

            // Update properties
            productVariant.Name = request.Name;
            productVariant.Sku = request.Sku;
            productVariant.StockQuantity = request.StockQuantity;
            productVariant.Price = request.Price;

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}