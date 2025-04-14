using FluentValidation;

namespace RookieEcommerce.Application.Features.Categories.Commands
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.Update.Name)
                .NotEmpty().MaximumLength(100);

            RuleFor(x => x.Update.Description)
                .NotEmpty().MaximumLength(200);
        }
    }
}