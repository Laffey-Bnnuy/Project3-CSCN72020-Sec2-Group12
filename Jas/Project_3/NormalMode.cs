using ElectricCarSystem.Core.Abstractions;

namespace ElectricCarSystem.Core.DriveModes.Modes
{
	public sealed class NormalMode : IDriveModeStrategy
	{
		public string Name => "Normal";
		public double PowerLimitKW => 150;
		public double RegenStrength => 0.6;
		public double ClimateEfficiencyBias => 0.7;
	}
}
