using MediatR;
using Microsoft.AspNetCore.Mvc;
using RookieEcommerce.Api.Constants;
using RookieEcommerce.Application.Features.Products.Commands;
using RookieEcommerce.Application.Features.Products.Queries;

namespace RookieEcommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IMediator mediator) : ControllerBase
    {
        // GET: api/Products
        [HttpGet(ApiEndPointConstant.Products.ProductsEndpoint)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery query, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // GET: api/Product/{productId}
        [HttpGet(ApiEndPointConstant.Products.ProductEndpoint)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProduct(Guid productId, string? includeProperties, CancellationToken cancellationToken)
        {
            var query = new GetProductByIdQuery { Id = productId, IncludeProperties = includeProperties };
            var result = await mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        // POST: api/Products
        [HttpPost(ApiEndPointConstant.Products.ProductsEndpoint)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(AddProduct), result);
        }

        // PUT: api/Product/{productId}
        [HttpPut(ApiEndPointConstant.Products.ProductEndpoint)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateProduct(Guid productId, [FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
        {
            command.Id = productId;
            await mediator.Send(command, cancellationToken);

            return Ok();
        }

        // DELETE: api/Product/{productId}
        [HttpDelete(ApiEndPointConstant.Products.ProductEndpoint)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteProduct(Guid productId, CancellationToken cancellationToken)
        {
            var command = new DeleteProductCommand { Id = productId };
            await mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}