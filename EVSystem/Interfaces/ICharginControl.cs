using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVSystem.Interfaces
{
    public interface IChargingControl
    {
        bool IsCharging { get; }
        void StartCharging();
        void StopCharging();
        void SetChargeLimit(float limit);
        void ScheduleCharging(string schedule);
        string GetSchedule();
    }
}
