using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVSystem.Interfaces
{
    interface IBattery
    {
        float BatteryLevel { get; }
        float Temperature { get; }
        float RemainingDriveTime { get; }
        float RemainingKm { get; }
        void UpdateBatteryLevel(float level);
        void SetBatteryMode(string mode);
        string GetStatus();
    }

}
