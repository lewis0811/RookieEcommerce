using FluentValidation;

namespace RookieEcommerce.Application.Features.ProductImages.Commands
{
    public class UpdateProductImageCommandValidator : AbstractValidator<UpdateProductImageCommand>
    {
        public UpdateProductImageCommandValidator()
        {
            RuleFor(c => c.AltText)
                .MaximumLength(100);
            RuleFor(c => c.SortOrder)
                .GreaterThanOrEqualTo(0)
                .When(c => c.SortOrder.HasValue);
        }
    }
}