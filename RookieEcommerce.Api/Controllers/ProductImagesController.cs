using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using RookieEcommerce.Api.Constants;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Features.ProductImages.Commands;
using RookieEcommerce.Application.Features.ProductImages.Queries;
using RookieEcommerce.SharedViewModels.ProductImageDtos;

namespace RookieEcommerce.Api.Controllers
{
    [Route("api/v{version:apiVersion}/product-images")]
    [ApiController]
    [ApiVersion(1)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, Roles = ApplicationRole.Admin)]
    public class ProductImagesController(IMediator mediator) : ControllerBase
    {
        // GET: product-images
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<PaginationList<ProductImageDetailsDto>>> GetProductImages([FromQuery] GetProductImagesQuery query, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        // GET: product-images/{image-id}
        [HttpGet("{image-id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProductImageDetailsDto>> GetProductImage([FromRoute(Name = "image-id")] Guid imageId, CancellationToken cancellationToken)
        {
            var query = new GetProductImageByIdQuery { Id = imageId };
            var result = await mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        // POST: product-images
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ProductImageCreateDto>> AddProductImage([FromBody] CreateProductImageCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(AddProductImage), result);
        }

        // PUT: product-images/{image-id}
        [HttpPut("{image-id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProductImage([FromRoute(Name = "image-id")] Guid imageId,
            [FromBody] UpdateProductImageCommand command, CancellationToken cancellationToken)
        {
            command.Id = imageId;
            await mediator.Send(command, cancellationToken);

            return Ok();
        }

        // DELETE: product-images/{image-id}
        [HttpDelete("{image-id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProductImage([FromRoute(Name = "image-id")] Guid imageId, CancellationToken cancellationToken)
        {
            var command = new DeleteProductImageCommand { Id = imageId };
            await mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}