using EVSystem.Interfaces;

namespace EVSystem.Components.DriveModes
{
	public class NormalMode : IDriveModeStrategy
	{
		public string Name => "Normal";
		public float PowerFactor => 1.0f;
		public float RegenStrength => 0.6f;
	}
}
