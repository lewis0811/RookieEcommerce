using MediatR;
using Microsoft.AspNetCore.Mvc;
using RookieEcommerce.Api.Constants;
using RookieEcommerce.Application.Features.Categories.Commands;
using RookieEcommerce.Application.Features.Categories.Queries;

namespace RookieEcommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(IMediator mediator) : ControllerBase
    {
        [HttpGet(ApiEndPointConstant.Categories.CategoriesEndpoint)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetCategories([FromQuery] GetCategoriesQuery query, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet(ApiEndPointConstant.Categories.CategoryEndpoint)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCategory(Guid categoryId, string? includeProperties, CancellationToken cancellationToken)
        {
            var query = new GetCategoryByIdQuery { Id = categoryId, IncludeProperties = includeProperties };
            var result = await mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        // POST: api/Categories
        [HttpPost(ApiEndPointConstant.Categories.CategoriesEndpoint)]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(AddCategory), result);
        }

        // PUT: api/Categories/{categoryId}
        [HttpPut(ApiEndPointConstant.Categories.CategoryEndpoint)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            command.Id = categoryId;
            await mediator.Send(command, cancellationToken);

            return Ok();
        }

        // DELETE: api/Categories/{categoryId}
        [HttpDelete(ApiEndPointConstant.Categories.CategoryEndpoint)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteCategory(Guid categoryId, CancellationToken cancellationToken)
        {
            var command = new DeleteCategoryCommand { Id = categoryId };
            await mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}