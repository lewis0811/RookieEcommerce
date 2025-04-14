using FluentValidation;

namespace RookieEcommerce.Application.Features.Categories.Commands
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Create.Name)
                .NotEmpty().MaximumLength(100);

            RuleFor(x => x.Create.Description)
                .NotEmpty().MaximumLength(200);
        }
    }
}