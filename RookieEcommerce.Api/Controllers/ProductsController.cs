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
        [HttpGet(ApiEndPointConstant.Product.ProductsEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery query, CancellationToken cancellationToken)
        {
            var products = await mediator.Send(query, cancellationToken);
            return Ok(products);
        }

        // GET: api/Products/{productId}
        [HttpGet(ApiEndPointConstant.Product.ProductEndpoint)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct(Guid productId, string? includeProperties, CancellationToken cancellationToken)
        {
            var query = new GetProductByIdQuery { Id = productId, IncludeProperties = includeProperties };
            var productDto = await mediator.Send(query, cancellationToken);

            return Ok(productDto);
        }

        // POST: api/Products
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
        {
            var product = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(AddProduct), product);
        }

        // PUT: api/Product/{productId}
        [HttpPut("{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProduct(Guid productId, [FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
        {
            command.Id = productId;
            await mediator.Send(command, cancellationToken);

            return Ok();
        }

        [HttpDelete("{productId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteProduct(Guid productId, CancellationToken cancellationToken)
        {
            var command = new DeleteProductCommand { Id = productId };
            await mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}