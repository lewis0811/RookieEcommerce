using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RookieEcommerce.Application.Features.ProductRatings.Commands;
using RookieEcommerce.Application.Features.ProductRatings.Queries;

namespace RookieEcommerce.Api.Controllers
{
    [Route("api/v{version:apiVersion}/product-ratings")]
    [ApiController]
    [ApiVersion(1)]
    public class ProductRatingsController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetProductRatings([FromQuery] GetProductRatingQuery query, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result); 
        }

        [HttpGet("{rating-id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProductRatingById([FromRoute(Name = "rating-id")] Guid ratingId, bool isIncludedItems, CancellationToken cancellationToken)
        {
            var query = new GetProductRatingByIdQuery { Id = ratingId, IsIncludeItems = isIncludedItems };
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddProductRating([FromBody] CreateProductRatingCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(AddProductRating), result);
        }

        [HttpPut("{rating-id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateProductRating([FromRoute(Name = "rating-id")] Guid ratingId, [FromBody] UpdateProductRatingCommand command, CancellationToken cancellationToken)
        {
            command.Id = ratingId;
            await mediator.Send(command, cancellationToken);

            return Ok();
        }

        [HttpDelete("{rating-id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProductRating([FromRoute] Guid ratingId, CancellationToken cancellationToken)
        {
            var command = new DeleteProductRatingCommand { Id = ratingId };
            await mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}