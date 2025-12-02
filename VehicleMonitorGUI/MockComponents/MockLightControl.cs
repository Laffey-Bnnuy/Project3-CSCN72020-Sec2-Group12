namespace VehicleMonitorGUI.MockComponents;

public class MockLightControl
{
    public bool AreLightsOn { get; private set; }

    public void TurnLightsOn()
    {
        AreLightsOn = true;
    }

    public void TurnLightsOff()
    {
        AreLightsOn = false;
    }
}
