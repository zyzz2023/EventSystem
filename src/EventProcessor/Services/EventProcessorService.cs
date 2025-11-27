using EventProcessor.Infrastructure.Repositories;
using EventProcessor.Models;

namespace EventProcessor.Services
{
    public class EventProcessorService : BackgroundService
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly ILogger<EventProcessorService> _logger;
        private readonly Dictionary<Guid, PendingEvent> _pendingEvents = new();
        
        public EventProcessorService(
            IIncidentRepository incidentRepository,
            ILogger<EventProcessorService> logger)
        {
            _incidentRepository = incidentRepository;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            while (!stoppingToken.IsCancellationRequested)
            {
            }

            var incident = new Incident
            {
                Id = Guid.NewGuid(),
                Type = IncidentTypeEnum.Type1,
                Time = DateTime.Now,
                Events = new List<Event>()
            };

            await _incidentRepository.AddAsync(incident);
            await _incidentRepository.SaveChangesAsync();
        }

        private class PendingEvent
        {
            public Event Event { get; set; }
            public DateTime ExpiryTime { get; set; }
        }
    }
}
