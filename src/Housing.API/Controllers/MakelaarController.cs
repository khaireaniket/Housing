using Housing.Application.Sale.Queries.GetTopMakelaars;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Housing.API.Controllers
{
    /// <summary>
    /// Controller to delegate calls to the appropriate command and query handlers
    /// </summary>
    [Route("api/makelaar")]
    [ApiController]
    public class MakelaarController : ControllerBase
    {
        private IMediator _mediator;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="mediator"></param>
        public MakelaarController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Endpoint will be used to fetch top Makelaars in Amsterdam region having the most object listed for sale
        /// </summary>
        /// <returns>List Of top makelaars in Amsterdam region with most object listed for sale</returns>
        [HttpGet("top/amsterdam")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetTopMakelaarsInAmsterdam()
        {
            var listOfMakelaars = await _mediator.Send(new GetTopMakelaarsQuery(10, "Amsterdam", "StatusBeschikbaar", false));

            if (listOfMakelaars is null || !listOfMakelaars.Any())
                return NoContent();

            return Ok(listOfMakelaars);
        }

        /// <summary>
        /// Endpoint will be used to fetch top Makelaars in Amsterdam region having the most object with garden listed for sale
        /// </summary>
        /// <returns>List Of top makelaars in Amsterdam region with most object with garden listed for sale</returns>
        [HttpGet("top/amsterdam/tuin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetTopMakelaarsInAmsterdamWithGarden()
        {
            var listOfMakelaars = await _mediator.Send(new GetTopMakelaarsQuery(10, "Amsterdam", "StatusBeschikbaar", true));

            if (listOfMakelaars is null || !listOfMakelaars.Any())
                return NoContent();

            return Ok(listOfMakelaars);
        }
    }
}
