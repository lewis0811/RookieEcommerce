using MediatR;
using Microsoft.AspNetCore.Mvc;
using RookieEcommerce.Application.Features.Products.Queries;
using RookieEcommerce.Application.Features.Products.Variants.Commands;
using RookieEcommerce.Application.Features.Products.Variants.Queries;
using RookieEcommerce.SharedViewModels;

namespace RookieEcommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IMediator mediator) : ControllerBase
    {
        // GET: api/products
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery query, CancellationToken cancellationToken)
        {
            var products = await mediator.Send(query, cancellationToken);
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct(Guid id, GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var productDto = await mediator.Send(query, cancellationToken);
            return Ok(productDto);
        }

        // GET: api/products/{productId}/variants
        [HttpGet("{productId}/variants")]
        [ProducesResponseType(typeof(IEnumerable<ProductVariantDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVariants(Guid productId, CancellationToken cancellationToken)
        {
            var query = new GetProductVariantsByProductQuery(productId);
            var variants = await mediator.Send(query, cancellationToken);
            return Ok(variants);
        }
    }
}