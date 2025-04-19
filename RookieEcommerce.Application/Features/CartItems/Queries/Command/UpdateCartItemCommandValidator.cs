using FluentValidation;

namespace RookieEcommerce.Application.Features.CartItems.Queries.Command
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