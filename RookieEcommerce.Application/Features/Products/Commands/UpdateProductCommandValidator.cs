using FluentValidation;

namespace RookieEcommerce.Application.Features.Products.Commands
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
            RuleFor(c => c.Description).NotEmpty().MaximumLength(200);
            RuleFor(c => c.Price).GreaterThanOrEqualTo(1);
        }
    }
}