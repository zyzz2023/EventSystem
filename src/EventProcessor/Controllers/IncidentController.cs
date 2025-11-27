using Microsoft.AspNetCore.Mvc;
using EventProcessor.Models;
using EventProcessor.Services;

namespace EventProcessor.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class IncidentsController : ControllerBase
    {
        private readonly EventProcessorService _eventProcessor;
        public IncidentsController(EventProcessorService sender)
        {
            _eventProcessor = sender;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessEvent([FromBody] Event _event)
        {
            if (_event == null)
                return BadRequest("Event is required");


            try
            {
                _eventProcessor.ProcessEvent(_event);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
