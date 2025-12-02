using System;

namespace EVSystem.Mock;

public class MockRearViewCamera
{
    public void Activate() { }
    public void Deactivate() { }

    public string CaptureSnapshot()
    {
        return $"Snapshot_{DateTime.Now:HH_mm_ss}";
    }
}
