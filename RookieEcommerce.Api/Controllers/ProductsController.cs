using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RookieEcommerce.Application.Features.Products.Queries;
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
        public async Task<IActionResult> GetProducts([FromQuery]GetProductsQuery query,CancellationToken cancellationToken)
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
    }
}
