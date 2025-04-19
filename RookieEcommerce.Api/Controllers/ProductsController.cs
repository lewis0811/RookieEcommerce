using MediatR;
using Microsoft.AspNetCore.Mvc;
using RookieEcommerce.Application.Features.Products.Commands;
using RookieEcommerce.Application.Features.Products.Queries;

namespace RookieEcommerce.Api.Controllers
{
    [Route("api/v{version:apiVersion}/products")]
    [ApiController]
    public class ProductsController(IMediator mediator) : ControllerBase
    {
        // GET: products
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery query, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // GET: product/{productId}
        [HttpGet("{product-id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProduct([FromRoute(Name = "product-id")] Guid productId, bool isIncludeItems, CancellationToken cancellationToken)
        {
            var query = new GetProductByIdQuery { Id = productId, IsIncludeItems = isIncludeItems };
            var result = await mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        // POST: products
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
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