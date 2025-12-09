using ElectricCarSystem.Core.Abstractions;

namespace ElectricCarSystem.Core.DriveModes.Modes
{
	public sealed class EcoMode : IDriveModeStrategy
	{
		public string Name => "Eco";
		public double PowerLimitKW => 80;
		public double RegenStrength => 0.9;
		public double ClimateEfficiencyBias => 1.0;
	}
}
