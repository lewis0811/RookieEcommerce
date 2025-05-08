using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using RookieEcommerce.Api.Constants;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Features.Categories.Commands;
using RookieEcommerce.Application.Features.Categories.Queries;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CategoryDtos;

namespace RookieEcommerce.Api.Controllers
{
    [Route("api/v{version:apiVersion}/categories")]
    [ApiController]
    [ApiVersion(1)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, Roles = $"{ApplicationRole.User}, {ApplicationRole.Admin}")]
    public class CategoriesController(IMediator mediator) : ControllerBase
    {
        // GET: pcategories
        [HttpGet("pagination")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<ActionResult<PaginationList<CategoryDetailsDto>>> GetPCategories([FromQuery] GetPCategoriesQuery query, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // GET: categories
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<Category>>> GetCategories([FromQuery] GetCategoriesQuery query, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // GET: categories/{category-id}
        [HttpGet("{category-id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CategoryDetailsDto>> GetCategory([FromRoute(Name = "category-id")] Guid categoryId, bool isIncludeItems, CancellationToken cancellationToken)
        {
            var query = new GetCategoryByIdQuery { Id = categoryId, IsIncludeItems = isIncludeItems };
            var result = await mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        // POST: categories
        [HttpPost]
        [ProducesResponseType(201)]
        [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, Roles = $"{ApplicationRole.Admin}")]
        public async Task<ActionResult<CategoryCreateDto>> AddCategory([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(AddCategory), result);
        }

        // PUT: categories/{category-id}
        [HttpPut("{category-id}")]
        [ProducesResponseType(200)]
        [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, Roles = $"{ApplicationRole.Admin}")]
        public async Task<ActionResult> UpdateCategory([FromRoute(Name = "category-id")] Guid categoryId, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            command.Id = categoryId;
            await mediator.Send(command, cancellationToken);

            return Ok();
        }

        // DELETE: categories/{category-id}
        [HttpDelete("{category-id}")]
        [ProducesResponseType(204)]
        [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, Roles = $"{ApplicationRole.Admin}")]
        public async Task<IActionResult> DeleteCategory([FromRoute(Name = "category-id")] Guid categoryId, CancellationToken cancellationToken)
        {
            var command = new DeleteCategoryCommand { Id = categoryId };
            await mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}