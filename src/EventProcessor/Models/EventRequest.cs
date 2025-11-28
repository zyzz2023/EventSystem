namespace EventProcessor.Models
{
    public class EventRequest
    {
        public EventTypeEnum Type { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
    }
}
