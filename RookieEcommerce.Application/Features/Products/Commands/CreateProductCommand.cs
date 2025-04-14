using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductDtos;

namespace RookieEcommerce.Application.Features.Products.Commands
{
    public record CreateProductCommand(string Name, string Description, decimal Price, Guid? CategoryId, string? Details) : IRequest<ProductDto>;

    public class CreateProductCommandHandler(IUnitOfWork unitOfWork, IProductRepository productRepository) : IRequestHandler<CreateProductCommand, ProductDto>
    {
        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Check if product already exist
            var productExist = await productRepository.AnyAsync(c => c.Name == request.Name, cancellationToken);
            if (productExist) { throw new ArgumentException($"Product name {request.Name} already exist."); }
            // Create product instance
            var product = Product.Create(request.Name, request.Description, request.Price, request.CategoryId, request.Details);

            // Add product via Repository
            await productRepository.AddAsync(product, cancellationToken);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to dto and return the result
            return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.CategoryId, product.Details);
        }
    }
}