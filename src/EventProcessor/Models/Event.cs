namespace EventProcessor.Models
{
    public class Event
    {
        public Guid Id { get; set; }
        public EventTypeEnum Type { get; set; }
        public DateTime Time { get; set; }
        public Guid? IncidentId { get; set; }
        public Incident? Incident { get; set; }
    }
}
