namespace ElectricCarSystem.Core.Abstractions
{
    public interface IDriveModeStrategy
    {
        string Name { get; }
        double PowerLimitKW { get; }       // illustrative cap
        double RegenStrength { get; }      // 0..1
        double ClimateEfficiencyBias { get; } // 0..1 (1 = max efficiency)
    }
}
