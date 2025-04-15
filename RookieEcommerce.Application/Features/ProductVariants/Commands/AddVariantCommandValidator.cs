using FluentValidation;

namespace RookieEcommerce.Application.Features.ProductVariants.Commands
{
    public class AddVariantCommandValidator : AbstractValidator<AddVariantCommand>
    {
        public AddVariantCommandValidator()
        {
            RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
            RuleFor(c => c.Price).GreaterThanOrEqualTo(0);
            RuleFor(c => c.StockQuantity).GreaterThanOrEqualTo(0);
            RuleFor(c => c.VariantType).NotEmpty().MaximumLength(100);
        }
    }
}