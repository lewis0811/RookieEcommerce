using FluentValidation;

namespace RookieEcommerce.Application.Features.CartItems.Commands
{
    public class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>
    {
        public UpdateCartItemCommandValidator()
        {
            RuleFor(c => c.Quantity)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}