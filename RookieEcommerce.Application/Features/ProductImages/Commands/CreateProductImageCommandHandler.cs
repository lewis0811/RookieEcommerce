using FluentValidation;

namespace RookieEcommerce.Application.Features.ProductImages.Commands
{
    public class CreateProductImageCommandValidator : AbstractValidator<CreateProductImageCommand>
    {
        public CreateProductImageCommandValidator()
        {
            RuleFor(c => c.AltText)
                .MaximumLength(100);
            RuleFor(c => c.ImageUrl)
                .NotEmpty()
                .MaximumLength(2048)
                .Matches(@"^(http|https)://.*$").WithMessage("Please provide a valid URL starting with http or https.");
        }
    }
}