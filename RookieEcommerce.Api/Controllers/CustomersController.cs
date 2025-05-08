using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using RookieEcommerce.Api.Constants;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Features.Customers.Queries;
using RookieEcommerce.SharedViewModels.CustomerDtos;

namespace RookieEcommerce.Api.Controllers
{
    [Route("api/v{version:apiVersion}/customers")]
    [ApiController]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, Roles = $"{ApplicationRole.Admin}")]
    public class CustomersController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<PaginationList<CustomerDetailsDto>>> GetAllCustomersAsync([FromQuery] GetCustomersQuery query, CancellationToken cancellationToken)
        {
            var customers = await mediator.Send(query, cancellationToken);
            return Ok(customers);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CustomerDetailsDto>> GetCustomerById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetCustomerByIdQuery { Id = id };
            var customer = await mediator.Send(query, cancellationToken);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }
    }
}