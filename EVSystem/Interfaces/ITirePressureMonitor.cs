using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVSystem.Interfaces
{
    public interface ITirePressureMonitor
    {
        Dictionary<string, float> TirePressures { get; }
        void UpdateTirePressures();
        bool CheckTirePressure();
        void AlertReplacement(string tire);
        string GetStatus();
    }
}
