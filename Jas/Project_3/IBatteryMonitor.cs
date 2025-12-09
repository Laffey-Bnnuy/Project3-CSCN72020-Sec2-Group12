namespace ElectricCarSystem.Core.Abstractions
{
    public interface IBatteryMonitor
    {
        double MaxDischargeKW { get; set; }
        double RegenerationFactor { get; set; }
    }
}
