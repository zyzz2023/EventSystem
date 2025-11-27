using System.Text.Json.Serialization;

namespace EventProcessor.Models
{
    public class Event
    {
        public Guid Id { get; set; }
        public EventTypeEnum Type { get; set; }
        public DateTime Time { get; set; }
        public Guid? IncidentId { get; set; }
        [JsonIgnore] // Для избежания ошибки бесконечной цепочки зависимости при выводе в json
        public Incident? Incident { get; set; }
    }
}
