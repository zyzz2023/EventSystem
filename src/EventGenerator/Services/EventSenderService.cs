using EventGenerator.Models;

namespace EventGenerator.Services
{
    public class EventSenderService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EventSenderService> _logger;

        public EventSenderService(HttpClient httpClient, ILogger<EventSenderService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task SendToProcessor(Event _event)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7216/api/events/process", _event);


                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to send event. Status: {response.StatusCode}");
                }
                else
                {
                    _logger.LogInformation($"Event {_event.Id} ({_event.Type}) sent successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending event {_event.Id}");
            }
        }
    }
}
