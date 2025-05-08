using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using RookieEcommerce.Api.Constants;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Features.Products.Commands;
using RookieEcommerce.Application.Features.Products.Queries;
using RookieEcommerce.SharedViewModels.ProductDtos;

namespace RookieEcommerce.Api.Controllers
{
    [Route("api/v{version:apiVersion}/products")]
    [ApiController]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, Roles = $"{ApplicationRole.User}, {ApplicationRole.Admin}")]

    public class ProductsController(IMediator mediator) : ControllerBase
    {
        // GET: products
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<ActionResult<PaginationList<ProductDetailsDto>>> GetProducts([FromQuery] GetProductsQuery query, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // GET: product/{productId}
        [HttpGet("{product-id}")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProductDetailsDto>> GetProduct([FromRoute(Name = "product-id")] Guid productId, bool isIncludeItems, CancellationToken cancellationToken)
        {
            var query = new GetProductByIdQuery { Id = productId, IsIncludeItems = isIncludeItems };
            var result = await mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        // POST: products
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ProductCreateDto>> AddProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(AddProduct), result);
        }

        // PUT: products/{productId}
        [HttpPut("{product-id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProduct([FromRoute(Name = "product-id")] Guid productId, [FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
        {
            command.Id = productId;
            await mediator.Send(command, cancellationToken);

            return Ok();
        }

        // DELETE: products/{productId}
        [HttpDelete("{product-id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProduct([FromRoute(Name = "product-id")] Guid productId, CancellationToken cancellationToken)
        {
            var command = new DeleteProductCommand { Id = productId };
            await mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}