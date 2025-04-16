using MediatR;
using Microsoft.AspNetCore.Mvc;
using RookieEcommerce.Api.Constants;
using RookieEcommerce.Application.Features.Products.Queries;
using RookieEcommerce.Application.Features.ProductVariants.Commands;
using RookieEcommerce.Application.Features.ProductVariants.Queries;

namespace RookieEcommerce.Api.Controllers
{
    [Route("api/v{version:apiVersion}/product-variants")]
    [ApiController]
    public class ProductVariantsController(IMediator mediator) : ControllerBase
    {
        // GET: product-variants
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetProductVariants([FromQuery] GetProductVariantsQuery query, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // GET: product-variants/{variant-id}
        [HttpGet("{variant-id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProductVariant([FromRoute(Name = "variant-id")] Guid productVariantId, string? includeProperties, CancellationToken cancellationToken)
        {
            var query = new GetProductByIdQuery { Id = productVariantId, IncludeProperties = includeProperties };
            var result = await mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        // POST product-variants
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddProductVariant(CreateVariantCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        // PUT: product-variants/{variant-id}
        [HttpPut("{variant-id}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateProductVariant([FromRoute(Name = "variant-id")] Guid productVariantId, [FromBody] UpdateVariantCommand command, CancellationToken cancellationToken)
        {
            command.Id = productVariantId;
            await mediator.Send(command, cancellationToken);

            return Ok();
        }

        // DELETE: product-variants/{variant-id}
        [HttpDelete("{variant-id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteProductVariant([FromRoute(Name = "variant-id")] Guid productVariantId, CancellationToken cancellationToken)
        {
            var command = new DeleteVariantCommand { Id = productVariantId };
            await mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}