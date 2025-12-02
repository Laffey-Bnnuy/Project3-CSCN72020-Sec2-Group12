namespace VehicleMonitorGUI.MockComponents;

public class MockReverseAlerts
{
    public string GetAlertMessage(double distanceCm)
    {
        if (distanceCm <= 0)
            return "No sensor data";

        if (distanceCm < 30)
            return "⚠️ CRITICAL: Stop immediately!";
        if (distanceCm < 60)
            return "🚨 Very close object detected";
        if (distanceCm < 100)
            return "⚠️ Object nearby";
        if (distanceCm < 150)
            return "ℹ️ Object detected at safe distance";

        return "✅ Area clear";
    }
}
