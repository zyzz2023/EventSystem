using EventGenerator.Models;

namespace EventGenerator.Services
{
    public class EventGeneratorService : BackgroundService
    {
        private readonly EventSenderService _eventSender;

        public EventGeneratorService(EventSenderService eventSender)
        {
            _eventSender = eventSender;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var random = new Random();
            var lastEventTime = DateTime.UtcNow;

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = random.Next(2000);
                await Task.Delay(delay, stoppingToken);

                var newEvent = new Event
                {
                    Id = Guid.NewGuid(),
                    Type = (EventTypeEnum)random.Next(1, 4),
                    Time = DateTime.UtcNow
                };

                //await _eventSender.SendToProcessor(newEvent);

                lastEventTime = DateTime.UtcNow;
            }
        }
    }
}
