using MediatR;
using RookieEcommerce.SharedViewModels;

namespace RookieEcommerce.Application.Features.Products.Commands
{
    public class CreateProductCommand(string Name, string Description, Guid? CategoryId) : IRequest<ProductDto>;
}