using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Features.Customers.Queries;
using RookieEcommerce.SharedViewModels.CustomerDtos;
using System.Threading.Tasks;

namespace RookieEcommerce.Api.Controllers
{
    [Route("api/v{version:apiVersion}/customers")]
    [ApiController]
    public class CustomersController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<PaginationList<CustomerDetailsDto>>> GetAllCustomersAsync([FromQuery]GetCustomersQuery query, CancellationToken cancellationToken)
        {
            var customers = await mediator.Send(query, cancellationToken);
            return Ok(customers);
        }

        [HttpGet("{id}")]
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
