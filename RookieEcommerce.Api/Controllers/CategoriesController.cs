using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieEcommerce.Application.Features.Categories.Commands;
using RookieEcommerce.Application.Features.Categories.Queries;

namespace RookieEcommerce.Api.Controllers
{
    [Route("api/v{version:apiVersion}/categories")]
    [ApiController]
    [ApiVersion(1)]
    public class CategoriesController(IMediator mediator) : ControllerBase
    {
        // GET: pcategories
        [HttpGet("pagination")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetPCategories([FromQuery] GetPCategoriesQuery query, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // GET: categories
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetCategories([FromQuery] GetCategoriesQuery query, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // GET: categories/{category-id}
        [HttpGet("{category-id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCategory([FromRoute(Name = "category-id")] Guid categoryId, bool isIncludeItems, CancellationToken cancellationToken)
        {
            var query = new GetCategoryByIdQuery { Id = categoryId, IsIncludeItems = isIncludeItems };
            var result = await mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        // POST: categories
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(AddCategory), result);
        }

        // PUT: categories/{category-id}
        [HttpPut("{category-id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateCategory([FromRoute(Name = "category-id")] Guid categoryId, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            command.Id = categoryId;
            await mediator.Send(command, cancellationToken);

            return Ok();
        }

        // DELETE: categories/{category-id}
        [HttpDelete("{category-id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteCategory([FromRoute(Name = "category-id")] Guid categoryId, CancellationToken cancellationToken)
        {
            var command = new DeleteCategoryCommand { Id = categoryId };
            await mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}