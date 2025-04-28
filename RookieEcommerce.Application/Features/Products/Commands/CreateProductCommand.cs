using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductDtos;

namespace RookieEcommerce.Application.Features.Products.Commands
{
    public class CreateProductCommand : IRequest<ProductCreateDto>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } = 1; 
        public Guid CategoryId { get; set; }
        public string? Details { get; set; }
    }

    public class CreateProductCommandHandler(IUnitOfWork unitOfWork, IProductRepository productRepository) : IRequestHandler<CreateProductCommand, ProductCreateDto>
    {
        public async Task<ProductCreateDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Check if product already exist
            var productExist = await productRepository.AnyAsync(c => c.Name == request.Name, cancellationToken);
            if (productExist) { throw new ArgumentException($"Product name {request.Name} already exist."); }
            
            // Create product instance
            var product = Product.Create(request.Name, request.Description, request.Price, request.CategoryId, request.Details);

            // Generate Sku number
            var existingSkus = await productRepository.ListAllAsync(c => c.Sku.Contains(product.Sku), null, cancellationToken);
            product.Sku += $"-{existingSkus.Count + 1}";

            // Add product via Repository
            await productRepository.AddAsync(product, cancellationToken);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to dto and return the result
            return ProductMapper.ProductToProductCreateDto(product);
        }
    }
}