using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductRatingDtos;

namespace RookieEcommerce.Application.Features.ProductRatings.Commands
{
    public class CreateProductRatingCommand : IRequest<ProductRatingCreateDto>
    {
        public Guid ProductId { get; set; }
        public Guid CustomerId { get; set; }
        public double RatingValue { get; set; }
        public string? Comment { get; set; }
    }

    public class CreateProductRatingCommandHandler(IUnitOfWork unitOfWork,
        IProductRatingRepository productRatingRepository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository
        )
        : IRequestHandler<CreateProductRatingCommand, ProductRatingCreateDto>
    {
        public async Task<ProductRatingCreateDto> Handle(CreateProductRatingCommand request, CancellationToken cancellationToken)
        {
            // Check if the product is exist
            var productExist = await productRepository.AnyAsync(c => c.Id == request.ProductId, cancellationToken);
            if (!productExist) { throw new InvalidOperationException($"Product Rating Id {request.ProductId} not found."); }

            // Check if the customer exists; if yes, check if the customer has bought the product.
            await CheckIfCustomerBoughtProduct(request, cancellationToken);

            // Create product rating instance
            var productRating = ProductRating.Create(request.ProductId, request.CustomerId, request.RatingValue, request.Comment);

            // Add product rating via repo
            await productRatingRepository.AddAsync(productRating, cancellationToken);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to dto and return
            var dto = ProductRatingMapper.ProductRatingToProductRatingCreateDto(productRating);

            return dto;
        }

        private async Task CheckIfCustomerBoughtProduct(CreateProductRatingCommand request, CancellationToken cancellationToken)
        {
            var customerExist = await customerRepository.GetByIdAsync(request.CustomerId, "Orders.OrderItems", cancellationToken);
            if (customerExist == null) { throw new InvalidOperationException($"Customer Id {request.ProductId} not found."); }
            else
            {
                var orderItems = customerExist.Orders
                    .SelectMany(c => c.OrderItems).Select(c => c.ProductVariant)
                    .Any(c => c.ProductId == request.ProductId);
                if (!orderItems)
                {
                    throw new InvalidOperationException($"Customer hadn't buy a product Id{request.ProductId}.");
                }
            }
        }
    }
}