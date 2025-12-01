using EVSystem.Interfaces;

namespace EVSystem.Components.DriveModes
{
	public class EcoMode : IDriveModeStrategy
	{
		public string Name => "Eco";
		public float PowerFactor => 0.7f;
		public float RegenStrength => 0.9f;
	}
}
