//using FluentValidation;

//namespace RookieEcommerce.Application.Features.ProductRatings.Commands
//{
//    public class CreateProductRatingCommandValidator : AbstractValidator<CreateProductRatingCommand>
//    {
//        public CreateProductRatingCommandValidator()
//        {
//            RuleFor(c => c.RatingValue)
//                .NotEmpty()
//                .GreaterThanOrEqualTo(1)
//                .LessThanOrEqualTo(5);

//            RuleFor(c => c.Comment)
//                .MinimumLength(10)
//                .MaximumLength(2000);
//        }
//    }
//}