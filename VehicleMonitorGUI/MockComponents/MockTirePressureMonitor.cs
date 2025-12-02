using System.Collections.Generic;

namespace VehicleMonitorGUI.MockComponents;

public record TirePressureRow(string Tire, double Pressure);

public class MockTirePressureMonitor
{
    public IEnumerable<TirePressureRow> ReadPressure()
    {
        return new[]
        {
            new TirePressureRow("Front Left", 34.5),
            new TirePressureRow("Front Right", 35.0),
            new TirePressureRow("Rear Left", 33.8),
            new TirePressureRow("Rear Right", 34.2),
        };
    }
}
