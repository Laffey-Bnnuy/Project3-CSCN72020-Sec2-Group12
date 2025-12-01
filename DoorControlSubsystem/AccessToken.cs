using System;

namespace EV_SCADA.Modules
{
    public class AccessToken
    {
        public string Id { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;

        public bool IsValid()
        {
            return !IsUsed && DateTime.UtcNow < ExpiresAt;
        }

        public override string ToString()
        {
            return $"Token[{Id}] Expires: {ExpiresAt}, Used: {IsUsed}";
        }
    }
}