using FluentValidation;

namespace RookieEcommerce.Application.Features.Categories.Commands
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().MaximumLength(100);

            RuleFor(x => x.Description)
                .NotEmpty().MaximumLength(200);
        }
    }
}