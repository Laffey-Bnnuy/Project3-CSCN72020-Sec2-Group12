using EVSystem.Interfaces;
using EVSystem.Communication;
using System;
using System.IO;
using System.Linq;

namespace EVSystem.Components
{
    public class BatteryMonitor : IBattery
    {
        private readonly J1939Adapter _j1939Adapter;
        private readonly string _dataFilePath;
        private string[] _batteryData;
        private int _currentIndex;

        public float BatteryLevel { get; private set; }
        public float Temperature { get; set; }
        public float RemainingDriveTime { get; private set; }
        public float RemainingKm { get; private set; }
        public string BatteryMode { get; private set; }

        public bool AutoMode { get; private set; }     // Matches style of LightControl

        public BatteryMonitor(J1939Adapter adapter, string dataFilePath = "Data/battery_data.csv")
        {
            _j1939Adapter = adapter;
            _dataFilePath = dataFilePath;
            _currentIndex = 0;

            _j1939Adapter.Register();
            BatteryMode = "Normal";
            AutoMode = false;

            LoadBatteryData();
            LoadNextData();
        }

        private void LoadBatteryData()
        {
            try
            {
                if (File.Exists(_dataFilePath))
                {
                    _batteryData = File.ReadAllLines(_dataFilePath)
                                       .Skip(1) // Skip header
                                       .ToArray();
                }
                else
                {
                    Console.WriteLine($"[BatteryMonitor] Warning: Data file not found at {_dataFilePath}. Using fallback demo data.");
                    _batteryData = new string[]
                    {
                        "75, 28, Eco",
                        "60, 32, Normal",
                        "45, 36, Sport",
                        "20, 40, Eco",
                        "10, 42, Sport"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BatteryMonitor] Error loading data: {ex.Message}");
                _batteryData = new string[] { "50, 25, Normal" };
            }
        }

        public void LoadNextData()
        {
            if (_batteryData == null || !_batteryData.Any())
                return;

            var parts = _batteryData[_currentIndex].Split(',').Select(x => x.Trim()).ToArray();
            _currentIndex = (_currentIndex + 1) % _batteryData.Length;

            if (parts.Length >= 3)
            {
                BatteryLevel = float.Parse(parts[0]);
                Temperature = float.Parse(parts[1]);
                SetBatteryMode(parts[2]);
            }

            UpdateDriveEstimates();

            if (AutoMode)
                AutoOptimizeMode();

            Console.WriteLine($"[BatteryMonitor] Updated data loaded: {GetStatus()}");
        }

        private void AutoOptimizeMode()
        {
            if (BatteryLevel < 25)
            {
                BatteryMode = "Eco";
                Console.WriteLine("[BatteryMonitor] Auto: Low battery detected → Switching to ECO mode.");
            }
            else if (Temperature > 40)
            {
                BatteryMode = "Normal";
                Console.WriteLine("[BatteryMonitor] Auto: High temperature → Reducing load (Normal mode).");
            }
        }

        public void SetBatteryMode(string mode)
        {
            BatteryMode = mode;
            AutoMode = false;

            Console.WriteLine($"[BatteryMonitor] Manual mode set to {mode}");
            UpdateDriveEstimates();
        }

        public void EnableAutoMode()
        {
            AutoMode = true;
            Console.WriteLine("[BatteryMonitor] Auto mode ENABLED.");
        }

        public void DisableAutoMode()
        {
            AutoMode = false;
            Console.WriteLine("[BatteryMonitor] Auto mode DISABLED.");
        }

        private void UpdateDriveEstimates()
        {
            float baseKm = 400f;
            float baseHours = 8f;

            float modeMultiplier = BatteryMode switch
            {
                "Eco" => 1.2f,
                "Normal" => 1.0f,
                "Sport" => 0.8f,
                _ => 1.0f
            };

            RemainingKm = baseKm * (BatteryLevel / 100f) * modeMultiplier;
            RemainingDriveTime = baseHours * (BatteryLevel / 100f) * modeMultiplier;
        }

        public void UpdateBatteryLevel(float delta)
        {
            BatteryLevel = delta;

            
            if (BatteryLevel > 100)
                BatteryLevel = 100;
            if (BatteryLevel < 0)
                BatteryLevel = 0;

            
           
            Console.WriteLine($"[BatteryMonitor] Battery updated: {BatteryLevel}%");
        }

        public string GetStatus()
        {
            return $"Battery: {BatteryLevel}% | Temp: {Temperature}°C | Mode: {BatteryMode} | Auto: {(AutoMode ? "ON" : "OFF")} | Range: {RemainingKm:F1} km | Drive Time: {RemainingDriveTime:F1} hr";
        }
    }
}
