using System;

namespace EV_SCADA.Modules
{
    public class AccessToken
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }

        public bool IsValid()
        {
            return !IsUsed && DateTime.UtcNow <= ExpiresAt;
        }
    }
}
