using EVSystem.Interfaces;

namespace EVSystem.Components.DriveModes
{
	public class SportMode : IDriveModeStrategy
	{
		public string Name => "Sport";
		public float PowerFactor => 1.3f;
		public float RegenStrength => 0.4f;
	}
}
