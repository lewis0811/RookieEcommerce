﻿using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.Products.Commands
{
    public class UpdateProductCommand : IRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Details { get; set; }
        public int? TotalQuantity { get; set; }
        public int? TotalSell { get; set; }
    }

    public class UpdateProductCommandHandler(IUnitOfWork unitOfWork, IProductRepository productRepository) : IRequestHandler<UpdateProductCommand>
    {
        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            // Check if the product exist
            var product = await productRepository.GetByIdAsync(request.Id, null, cancellationToken)
                ?? throw new InvalidOperationException($"Product Id {request.Id} not found.");

            // Map request to product
            product.Update(request.Name, request.Description, request.Price, request.Details, request.TotalQuantity, request.TotalSell);

            // Update via Repository
            await productRepository.UpdateAsync(product, cancellationToken);

            // Save changes via UnitOfWork
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}