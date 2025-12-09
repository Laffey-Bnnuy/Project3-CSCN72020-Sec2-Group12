namespace ElectricCarSystem.Core.Abstractions
{
    public interface IClimateControl
    {
        double CurrentTemperature { get; }
        double TargetTemperature  { get; }
        bool   PreConditioningEnabled { get; }

        void SetTemperature(double targetCelsius);
        void EnablePreConditioning();
        void DisablePreConditioning();
        /// Moves CurrentTemperature one step toward TargetTemperature.
        void UpdateClimate();
    }
}
