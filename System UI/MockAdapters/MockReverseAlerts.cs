namespace EVSystem.Components;

public class ReverseAlerts
{
    public string GetAlertMessage(double distance)
    {
        if (distance < 10) return "STOP - Obstacle Very Close!";
        if (distance < 30) return "Warning — Getting Close!";
        return "Safe Distance";
    }
}