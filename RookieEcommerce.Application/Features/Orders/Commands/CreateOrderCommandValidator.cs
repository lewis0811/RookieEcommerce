using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RookieEcommerce.Application.Features.Orders.Commands
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerId)
            .NotEmpty();

            RuleFor(x => x.PaymentMethod)
                 .IsInEnum().WithMessage("A valid payment method must be specified.");

            RuleFor(x => x.ShippingAddress)
                .NotNull().WithMessage("Shipping address is required.");

            RuleFor(x => x.OrderItems)
                .NotEmpty().WithMessage("Order must contain at least one item."); // Check 
        }

    }
}
