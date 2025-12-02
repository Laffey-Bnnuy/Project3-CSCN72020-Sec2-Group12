namespace VehicleMonitorGUI.MockComponents;

public class MockChargingControl
{
    public bool IsCharging { get; private set; }

    public void StartCharging()
    {
        IsCharging = true;
    }

    public void StopCharging()
    {
        IsCharging = false;
    }
}
