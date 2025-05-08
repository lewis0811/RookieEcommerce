using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using RookieEcommerce.Api.Constants;
using RookieEcommerce.Application.Features.Carts.Commands;
using RookieEcommerce.Application.Features.Carts.Queries;
using System.Security.Claims;

namespace RookieEcommerce.Api.Controllers
{
    [Route("api/v{version:apiVersion}/carts")]
    [ApiController]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, Roles = $"{ApplicationRole.User}, {ApplicationRole.Admin}")]
    public class CartsController(IMediator mediator) : ControllerBase
    {
        // GET: api/carts/my-cart
        [HttpGet("my-cart")]
        public async Task<IActionResult> GetMyCart(bool isIncludeItems)
        {
            var customerId = GetUserIdFromClaims();
            if (customerId == Guid.Empty) return Unauthorized();

            var query = new GetCartByCustomerIdQuery { CustomerId = customerId, IsIncludeItems = isIncludeItems };
            var result = await mediator.Send(query);

            return Ok(result);
        }

        // GET: api/carts
        [HttpGet]
        public async Task<IActionResult> GetCartByCustomer([FromQuery(Name = "customer-id")] Guid customerId, bool isIncludeItems, CancellationToken cancellationToken)
        {
            var query = new GetCartByCustomerIdQuery { CustomerId = customerId, IsIncludeItems = isIncludeItems };
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // POST: api/carts
        [HttpPost]
        public async Task<IActionResult> CreateCart([FromBody] CreateCartCommand command, CancellationToken cancellationToken)
        {
            var cart = await mediator.Send(command, cancellationToken);
            return Ok(new { Id = cart.Id });
        }

        // DELETE: api/carts/{cart-id}/
        [HttpDelete("{cart-id}/items")]
        [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, Roles = $"{ApplicationRole.Admin}")]
        public async Task<IActionResult> ClearCart([FromRoute(Name = "cart-id")] Guid cartId)
        {
            await mediator.Send(new ClearCartCommand { CartId = cartId });
            return NoContent();
        }

        private Guid GetUserIdFromClaims()
        {
            // Example implementation
            var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }
}