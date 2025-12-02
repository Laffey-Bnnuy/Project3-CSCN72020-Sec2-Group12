namespace VehicleMonitorGUI.MockComponents;

public class MockBatteryMonitor
{
    private int _batteryLevel;

    public MockBatteryMonitor(int initialLevel = 72)
    {
        _batteryLevel = initialLevel;
    }

    public int GetBatteryLevel() => _batteryLevel;
}
