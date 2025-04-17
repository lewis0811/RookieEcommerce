using FluentValidation;

namespace RookieEcommerce.Application.Features.ProductRatings.Commands
{
    public class UpdateProductRatingCommandValidator : AbstractValidator<UpdateProductRatingCommand>
    {
        public UpdateProductRatingCommandValidator()
        {
            RuleFor(c => c.RatingValue)
                .NotEmpty()
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(5);

            RuleFor(c => c.Comment)
                .MinimumLength(10)
                .MaximumLength(2000);
        }
    }
}