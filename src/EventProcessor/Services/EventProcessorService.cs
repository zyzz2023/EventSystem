using EventProcessor.Infrastructure.Repositories;
using EventProcessor.Models;
using Microsoft.EntityFrameworkCore;

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
                await CheckExpiredEvents();
                await Task.Delay(1000, stoppingToken);
            }
        }

        public void ProcessEvent(Event _event)
        {
            try
            {
                switch (_event.Type)
                {
                    case EventTypeEnum.Type1:
                        ProcessType1Event(_event);
                        break;

                    case EventTypeEnum.Type2:
                        ProcessType2Event(_event);
                        break;

                    case EventTypeEnum.Type3:
                        ProcessType3Event(_event);
                        break;
                }
            }
                 catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing event {_event.Id}. Event skipped.");
            }
        }
        private void ProcessType1Event(Event type1Event)
        {
            // Шаблон 2
            var pendingType2 = FindPendingEvent(EventTypeEnum.Type2);
            if (pendingType2 != null)
            {
                _ = CreateCompositeIncident(pendingType2.Event, type1Event, IncidentTypeEnum.Type2);
                RemovePendingEvent(pendingType2.Event.Id);
                _logger.LogInformation($"Pattern 2 matched: Event{pendingType2.Event.Id}(Type2) + Event{type1Event.Id}(Type1)");

                // Шаблон 3
                var pendingType3 = FindPendingEvent(EventTypeEnum.Type3);
                if (pendingType3 != null)
                {
                    _ = CreateSimpleIncident(pendingType3.Event, IncidentTypeEnum.Type3);
                    RemovePendingEvent(pendingType3.Event.Id);
                    _logger.LogInformation($"Pattern 3 matched: Event{pendingType3.Event.Id}(Type3) + Incident(Type2)");
                }
                return;
            }

            // Шаблон 1
            _ = CreateSimpleIncident(type1Event, IncidentTypeEnum.Type1);
        }
        private void ProcessType2Event(Event type2Event)
        {
            _pendingEvents[type2Event.Id] = new PendingEvent
            {
                Event = type2Event,
                ExpiryTime = DateTime.UtcNow.AddSeconds(20)
            };
            
            
            _logger.LogInformation($"Event {type2Event.Id} (Type2) waiting for Type1 within 20 seconds");
        }
        private void ProcessType3Event(Event type3Event)
        {
            _pendingEvents[type3Event.Id] = new PendingEvent
            {
                Event = type3Event,
                ExpiryTime = DateTime.UtcNow.AddSeconds(60)
            };
            
            
            _logger.LogInformation($"Event {type3Event.Id} (Type3) waiting for Incident within 60 seconds");
        }
        private async Task CreateSimpleIncident(Event _event, IncidentTypeEnum incidentType)
        {
            try
            {
                var incident = new Incident
                {
                    Id = Guid.NewGuid(),
                    Type = incidentType,
                    Time = DateTime.UtcNow,
                    Events = new List<Event> { _event }
                };

                await _incidentRepository.AddAsync(incident);

                _logger.LogInformation($"Created simple incident {incident.Id} with type {incidentType}");
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, $"Database error update for event {_event.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating incident for event {_event.Id}");
            }
        }
        private async Task CreateCompositeIncident(Event firstEvent, Event secondEvent, IncidentTypeEnum incidentType)
        {
            try
            {
                var incident = new Incident
                {
                    Id = Guid.NewGuid(),
                    Type = incidentType,
                    Time = DateTime.UtcNow,
                    Events = new List<Event> { firstEvent, secondEvent }
                };

                await _incidentRepository.AddAsync(incident);

                _logger.LogInformation($"Created composite incident {incident.Id} with type {incidentType}");
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, $"Database error update for events {firstEvent.Id} and {secondEvent}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating incident for events {firstEvent.Id} and {secondEvent}");
            }
        }
        private async Task CheckExpiredEvents()
        {
            List<Guid> expiredIds = new();
            List<PendingEvent> expiredEvents = new();

            var now = DateTime.UtcNow;
            foreach (var kvp in _pendingEvents)
            {
                if (kvp.Value.ExpiryTime <= now)
                {
                    expiredIds.Add(kvp.Key);
                    expiredEvents.Add(kvp.Value);
                }
            }

            foreach (var id in expiredIds)
            {
                _pendingEvents.Remove(id);
            }
            

            foreach (var _event in expiredEvents)
            {
                _logger.LogInformation($"Event {_event.Event.Id} expired. Creating simple incident.");
                await CreateSimpleIncident(_event.Event, IncidentTypeEnum.Type1);
            }
        }

        private PendingEvent? FindPendingEvent(EventTypeEnum eventType)
        {
            return _pendingEvents.Values
            .Where(pe => pe.Event.Type == eventType && pe.ExpiryTime > DateTime.UtcNow)
            .OrderBy(pe => pe.Event.Time)
            .FirstOrDefault();
        }

        private void RemovePendingEvent(Guid eventId)
        {   
            _pendingEvents.Remove(eventId);
            
        }
        private class PendingEvent
        {
            public Event Event { get; set; }
            public DateTime ExpiryTime { get; set; }
        }
    }
}
