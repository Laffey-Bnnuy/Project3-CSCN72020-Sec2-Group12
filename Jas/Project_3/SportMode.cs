using ElectricCarSystem.Core.Abstractions;

namespace ElectricCarSystem.Core.DriveModes.Modes
{
	public sealed class SportMode : IDriveModeStrategy
	{
		public string Name => "Sport";
		public double PowerLimitKW => 250;
		public double RegenStrength => 0.4;
		public double ClimateEfficiencyBias => 0.5;
	}
}
