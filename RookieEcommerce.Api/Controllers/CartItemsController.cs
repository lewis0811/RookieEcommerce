﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using RookieEcommerce.Api.Constants;
using RookieEcommerce.Application.Features.CartItems.Commands;

namespace RookieEcommerce.Api.Controllers
{
    [Route("api/v{version:apiVersion}/carts/{cart-id}/items")]
    [ApiController]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, Roles = $"{ApplicationRole.User}, {ApplicationRole.Admin}")]
    public class CartItemsController(IMediator mediator) : ControllerBase
    {
        // POST: api/carts/{cart-id}/items
        [HttpPost]
        public async Task<IActionResult> AddItem([FromRoute(Name = "cart-id")] Guid cartId, [FromBody] CreateCartItemCommand command)
        {
            command.CartId = cartId;
            var result = await mediator.Send(command);
            return CreatedAtAction(nameof(AddItem), result);
        }

        // PUT: api/carts/{cart-id}/items/{item-id}
        [HttpPut("{item-id}")]
        public async Task<IActionResult> UpdateItemQuantity([FromRoute(Name = "cart-id")] Guid cartId, [FromRoute(Name = "item-id")] Guid itemId,
        [FromBody] UpdateCartItemCommand command)
        {
            command.CartId = cartId;
            command.ItemId = itemId;
            await mediator.Send(command);
            return Ok();
        }

        // DELETE: api/carts/{cartId}/items/{item-id}
        [HttpDelete("{item-id}")]
        public async Task<IActionResult> RemoveItem([FromRoute(Name = "cart-id")] Guid cartId, [FromRoute(Name = "item-id")] Guid itemId)
        {
            await mediator.Send(new DeleteCartItemCommand { CartId = cartId, ItemId = itemId });
            return NoContent();
        }
    }
}