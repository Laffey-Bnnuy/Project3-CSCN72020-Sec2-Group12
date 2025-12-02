using System.Collections.Generic;
using System.Linq;

namespace EVSystem.Components;

public class TirePressureMonitor
{
    public IEnumerable<(string Tire, int Pressure)> ReadPressure()
    {
        return new List<(string, int)>
        {
            ("Front Left", 33),
            ("Front Right", 34),
            ("Rear Left", 32),
            ("Rear Right", 33)
        };
    }
}