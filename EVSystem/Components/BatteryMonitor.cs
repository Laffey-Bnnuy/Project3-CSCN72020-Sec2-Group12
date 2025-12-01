using EVSystem.Communication;
using EVSystem.Interfaces;

namespace EVSystem.Components
{
    public class BatteryMonitor : IBattery
    {
        private readonly J1939Adapter _j1939Adapter;

        public float BatteryLevel { get; private set; }
        public float Temperature { get; private set; }
        public float RemainingDriveTime { get; private set; }
        public float RemainingKm { get; private set; }
        public string BatteryMode { get; private set; }

        public BatteryMonitor(J1939Adapter j1939Adapter)
        {
            _j1939Adapter = j1939Adapter;
            _j1939Adapter.Register();

            BatteryLevel = 100f;
            Temperature = 25f;
            BatteryMode = "Normal";
            UpdateDriveEstimates();
        }

        public void SetBatteryMode(string mode)
        {
            BatteryMode = mode;
            UpdateDriveEstimates();
        }

        public void UpdateBatteryLevel(float newLevel)
        {
            BatteryLevel = newLevel;
            UpdateDriveEstimates();
        }

        private void UpdateDriveEstimates()
        {
            float baseKm = 400f;
            float baseTime = 8f;

            float modeMultiplier = BatteryMode switch
            {
                "Eco" => 1.2f,
                "Normal" => 1.0f,
                "Sport" => 0.8f,
                _ => 1.0f
            };

            RemainingKm = baseKm * (BatteryLevel / 100f) * modeMultiplier;
            RemainingDriveTime = baseTime * (BatteryLevel / 100f) * modeMultiplier;
        }

        public string GetStatus()
        {
            return $"Battery: {BatteryLevel}%, Mode: {BatteryMode}, Temp: {Temperature}°C, " +
                   $"Range: {RemainingKm:F1} km, Drive Time: {RemainingDriveTime:F1} hr";
        }
    }
}