using System;

namespace EV_SCADA.Modules
{
    public class AccessEvent
    {
        public string TokenId { get; set; } = "N/A";
        public string Action { get; set; } = "Unknown";
        public string Result { get; set; } = "Pending";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public override string ToString()
        {
            return $"{Timestamp:HH:mm:ss} | Token: {TokenId} | Action: {Action} | Result: {Result}";
        }
    }
}
