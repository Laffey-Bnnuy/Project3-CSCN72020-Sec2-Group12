using System;

namespace EVSystem.Interfaces
{
    public interface IBattery
    {
        // Current values
        float BatteryLevel { get; }
        float Temperature { get; set; }
        float RemainingDriveTime { get; }
        float RemainingKm { get; }
        string BatteryMode { get; }
        bool AutoMode { get; }

        // Data loading/processing
        void LoadNextData();

        // Mode control
        void SetBatteryMode(string mode);
        void EnableAutoMode();
        void DisableAutoMode();

        // Reporting
        string GetStatus();

        // Battery level control
        void UpdateBatteryLevel(float delta);

    }
}
