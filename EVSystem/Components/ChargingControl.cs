using EVSystem.Interfaces;
using EVSystem.Communication;

namespace EVSystem.Components
{
    public class ChargingControl : IChargingControl
    {
        private readonly J1939Adapter _j1939Adapter;
        public bool IsCharging { get; private set; }
        private float chargeLimit;
        private string schedule;

        public ChargingControl(J1939Adapter j1939Adapter)
        {
            _j1939Adapter = j1939Adapter;
            _j1939Adapter.Register();
        }

        public void StartCharging()
        {
            IsCharging = true;
        }

        public void StopCharging()
        {
            IsCharging = false;
        }

        public void SetChargeLimit(float limit)
        {
            chargeLimit = limit;
        }

        public void ScheduleCharging(string schedule)
        {
            this.schedule = schedule;
        }

        public string GetSchedule()
        {
            return schedule;
        }
    }
}