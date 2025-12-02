using EVSystem.Components;
using EVSystem.Interfaces;
using System;

namespace EVSystem.Controllers
{
    public class EVSystemController
    {
        private readonly BatteryMonitor _batteryMonitor;
        private readonly ChargingControl _chargingControl;

        public event Action<string>? OnStatusChanged;
        public event Action<float>? OnBatteryLevelChanged;
        public event Action<bool>? OnChargingStateChanged;

        public EVSystemController(BatteryMonitor batteryMonitor, ChargingControl chargingControl)
        {
            _batteryMonitor = batteryMonitor;
            _chargingControl = chargingControl;
        }

        // Battery handling

        public float GetBatteryLevel() => _batteryMonitor.BatteryLevel;
        public string GetBatteryStatus() => _batteryMonitor.GetStatus();

        public void RefreshSystemData()
        {
            _batteryMonitor.LoadNextData();
            _chargingControl.LoadNextData();

            OnBatteryLevelChanged?.Invoke(_batteryMonitor.BatteryLevel);
            OnStatusChanged?.Invoke(GetBatteryStatus());
            OnChargingStateChanged?.Invoke(_chargingControl.IsCharging);
        }

        // Charging control

        public void StartCharging()
        {
            _chargingControl.StartCharging();
            OnChargingStateChanged?.Invoke(true);
            OnStatusChanged?.Invoke("Charging started.");
        }

        public void StopCharging()
        {
            _chargingControl.StopCharging();
            OnChargingStateChanged?.Invoke(false);
            OnStatusChanged?.Invoke("Charging stopped.");
        }

       

        public void ScheduleCharging(string schedule)
        {
            if (string.IsNullOrWhiteSpace(schedule))
            {
                OnStatusChanged?.Invoke("Invalid schedule format.");
                return;
            }

            _chargingControl.ScheduleCharging(schedule);
            OnStatusChanged?.Invoke($"Charging scheduled: {schedule}");
        }

        public void SimulateBatteryUpdate(float newLevel)
        {
           
            var batteryProp = typeof(BatteryMonitor).GetProperty("BatteryLevel");
            batteryProp?.SetValue(_batteryMonitor, newLevel);

            
            OnBatteryLevelChanged?.Invoke(newLevel);
            OnStatusChanged?.Invoke($"Battery updated to {newLevel}%");
        }


        public float CurrentChargeLimit { get; private set; } = 80;

        public void SetBattryChargeLimit(float limit)
        {
            if (limit < 10 || limit > 100)
            {
                OnStatusChanged?.Invoke("Invalid limit. Must be between 10% and 100%.");
                return;
            }

            _chargingControl.SetChargeLimit(limit);
            CurrentChargeLimit = limit;

            OnStatusChanged?.Invoke($"Charge limit set to {limit}%.");
        }


    }
}
