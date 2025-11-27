using Microsoft.AspNetCore.Mvc;
using EventGenerator.Models;
using EventGenerator.Services;

namespace EventGenerator.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly EventSenderService _eventSender;
        public EventsController(EventSenderService sender) 
        {
            _eventSender = sender;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateManualEvent()
        {
            var random = new Random();
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Type = (EventTypeEnum)random.Next(1, 4),
                Time = DateTime.UtcNow
            };
            
            await _eventSender.SendToProcessor(newEvent);
            return Ok();
        }
    }
}
