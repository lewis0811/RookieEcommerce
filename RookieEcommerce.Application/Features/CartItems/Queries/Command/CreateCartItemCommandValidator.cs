using FluentValidation;

namespace RookieEcommerce.Application.Features.CartItems.Queries.Command
{
    public class CreateCartItemCommandValidator : AbstractValidator<CreateCartItemCommand>
    {
        public CreateCartItemCommandValidator()
        {
            RuleFor(c => c.Quantity)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}