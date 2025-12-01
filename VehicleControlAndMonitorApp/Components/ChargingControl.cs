using EVSystem.Interfaces;
using EVSystem.Communication;
using System;
using System.IO;
using System.Linq;

namespace EVSystem.Components
{
    public class ChargingControl : IChargingControl
    {
        private readonly J1939Adapter _j1939Adapter;
        private readonly IBattery _battery;

        private string _dataFilePath;
        private string[] _chargingData;
        private int _currentIndex;

        public bool IsCharging { get; private set; }
        public float ChargeLimit { get; private set; }
        public string Schedule { get; private set; }

        private const float ChargeRatePerCycle = 2.5f; // % increase per LoadNextData

        public ChargingControl(J1939Adapter j1939Adapter, IBattery battery, string dataFilePath = "Data/charging_data.csv")
        {
            _j1939Adapter = j1939Adapter;
            _battery = battery;

            _j1939Adapter.Register();
            _dataFilePath = dataFilePath;

            LoadChargingData();
            LoadNextData();
        }

        private void LoadChargingData()
        {
            try
            {
                if (File.Exists(_dataFilePath))
                {
                    _chargingData = File.ReadAllLines(_dataFilePath)
                                        .Skip(1)
                                        .ToArray();
                }
                else
                {
                    Console.WriteLine($"[ChargingControl] Warning: Data file not found at {_dataFilePath}. Using fallback demo data.");

                    _chargingData = new[]
                    {
                        "false,80,22:00-06:00",
                        "true,100,Immediate",
                        "false,50,01:00-04:00"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ChargingControl] Error reading file: {ex.Message}");
                _chargingData = new[] { "false,80,22:00-06:00" };
            }
        }

        public void LoadNextData()
        {
            if (_chargingData == null || !_chargingData.Any())
                return;

            var parts = _chargingData[_currentIndex].Split(',').Select(x => x.Trim()).ToArray();
            _currentIndex = (_currentIndex + 1) % _chargingData.Length;

            if (parts.Length >= 3)
            {
                IsCharging = bool.Parse(parts[0]);
                ChargeLimit = float.Parse(parts[1]);
                Schedule = parts[2];
            }

            if (IsCharging)
                ApplyChargingEffect();

            Console.WriteLine($"[ChargingControl] Charging: {IsCharging}, Limit: {ChargeLimit}%, Schedule: {Schedule}");
        }

        private void ApplyChargingEffect()
        {
            if (_battery == null) return;

            float newLevel = _battery.BatteryLevel + ChargeRatePerCycle;

            if (newLevel >= ChargeLimit)
            {
                newLevel = ChargeLimit;
                IsCharging = false;
                Console.WriteLine("[ChargingControl] Charge limit reached — charging stopped.");
            }

            _battery.UpdateBatteryLevel(newLevel);

           
            if (_battery.Temperature < 45)
                _battery.Temperature += 0.3f;
        }

        public void StartCharging() => IsCharging = true;
        public void StopCharging() => IsCharging = false;
        public void SetChargeLimit(float limit) => ChargeLimit = limit;
        public void ScheduleCharging(string schedule) => Schedule = schedule;
        public string GetSchedule() => Schedule;
    }
}
