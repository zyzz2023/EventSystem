using Microsoft.AspNetCore.Mvc;
using EventProcessor.Models;
using EventProcessor.Services;
using EventProcessor.Infrastructure.Repositories;

namespace EventProcessor.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class IncidentsController : ControllerBase
    {
        private readonly EventProcessorService _eventProcessor;
        private readonly IIncidentRepository _incidentRepository;
        public IncidentsController(EventProcessorService sender, IIncidentRepository incidentRepository)
        {
            _eventProcessor = sender;
            _incidentRepository = incidentRepository;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessEvent([FromBody] EventRequest request)
        {
            if (request == null)
                return BadRequest("Event is required");

            try
            {
                var _event = new Event
                {
                    Id = Guid.NewGuid(),
                    Type = request.Type,
                    Time = request.Time
                };
                _eventProcessor.ProcessEvent(_event);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Incident>>> GetIncidents(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var incidents = await _incidentRepository.GetIncidentsAsync(page, pageSize);
                return Ok(incidents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
