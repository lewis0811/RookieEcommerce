using FluentValidation;

namespace RookieEcommerce.Application.Features.Products.Commands
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
            RuleFor(c => c.Description).NotEmpty().MaximumLength(200);
            RuleFor(c => c.Price).GreaterThanOrEqualTo(5000); // Lowest ewallet transfer
        }
    }
}