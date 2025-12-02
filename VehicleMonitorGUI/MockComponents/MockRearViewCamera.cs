namespace VehicleMonitorGUI.MockComponents;

public class MockRearViewCamera
{
    private bool _active;
    private int _count;

    public void Activate()
    {
        _active = true;
    }

    public void Deactivate()
    {
        _active = false;
    }

    public string CaptureSnapshot()
    {
        if (!_active)
            return "Camera is Off";

        _count++;
        return $"Snapshot {_count} saved";
    }
}
