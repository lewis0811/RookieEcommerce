using FluentValidation;

namespace RookieEcommerce.Application.Features.Categories.Commands
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().MaximumLength(100);

            RuleFor(x => x.Description)
                .NotEmpty().MaximumLength(200);
        }
    }
}