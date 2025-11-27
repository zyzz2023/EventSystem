using Microsoft.AspNetCore.Mvc;
using EventProcessor.Models;
using EventProcessor.Services;

namespace EventProcessor.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly EventProcessorService _eventProcessor;
        public EventsController(EventProcessorService sender)
        {
            _eventProcessor = sender;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessEvent([FromBody] Event _event)
        {
            _eventProcessor.ProcessEvent(_event);
            return Ok();
        }
    }
}
