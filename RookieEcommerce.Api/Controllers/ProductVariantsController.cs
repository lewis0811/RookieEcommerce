using MediatR;
using Microsoft.AspNetCore.Mvc;
using RookieEcommerce.Api.Constants;
using RookieEcommerce.Application.Features.Products.Commands;
using RookieEcommerce.Application.Features.Products.Queries;
using RookieEcommerce.Application.Features.ProductVariants.Commands;
using RookieEcommerce.Application.Features.ProductVariants.Queries;

namespace RookieEcommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariantsController(IMediator mediator) : ControllerBase
    {
        [HttpGet(ApiEndPointConstant.ProductVariants.ProductVariantsEndpoint)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetProductVariants([FromQuery] GetProductVariantsQuery query, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // GET: api/ProductVariant/{productVariantId}
        [HttpGet(ApiEndPointConstant.ProductVariants.ProductVariantEndpoint)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProductVariant(Guid productVariantId, string? includeProperties, CancellationToken cancellationToken)
        {
            var query = new GetProductByIdQuery { Id = productVariantId, IncludeProperties = includeProperties };
            var result = await mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        // POST api/ProductVariant
        [HttpPost(ApiEndPointConstant.ProductVariants.ProductVariantsEndpoint)]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddProductVariant(AddVariantCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        // PUT: api/ProductVariant/{productVariantId}
        [HttpPut(ApiEndPointConstant.ProductVariants.ProductVariantEndpoint)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateProductVariant([FromRoute(Name = "product-variantId")] Guid productVariantId, [FromBody] UpdateVariantCommand command, CancellationToken cancellationToken)
        {
            command.Id = productVariantId;
            await mediator.Send(command, cancellationToken);

            return Ok();
        }

        // DELETE: api/ProductVariant/{productVariantId}
        [HttpDelete(ApiEndPointConstant.ProductVariants.ProductVariantEndpoint)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteProductVariant([FromRoute(Name = "product-variantId")] Guid productVariantId, CancellationToken cancellationToken)
        {
            var command = new DeleteVariantCommand { Id = productVariantId };
            await mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}