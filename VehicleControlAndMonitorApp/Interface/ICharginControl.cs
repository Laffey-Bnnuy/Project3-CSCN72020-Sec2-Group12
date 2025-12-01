namespace EVSystem.Interfaces
{
    public interface IChargingControl
    {
        // Charging state
        bool IsCharging { get; }

        // Configuration
        float ChargeLimit { get; }
        string Schedule { get; }

        // Control commands
        void StartCharging();
        void StopCharging();
        void SetChargeLimit(float limit);
        void ScheduleCharging(string schedule);
    }
}
