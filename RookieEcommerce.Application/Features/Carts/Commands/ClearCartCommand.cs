using MediatR;

namespace RookieEcommerce.Application.Features.Carts.Commands
{
    public class ClearCartCommand : IRequest
    {
        public Guid CartId { get; set; }
    }
}