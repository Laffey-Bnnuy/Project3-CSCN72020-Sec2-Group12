using System;
using ElectricCarSystem.Core.Abstractions;

namespace ElectricCarSystem.Core.Climate
{
	/// Single responsibility: HVAC state & transitions (no UI/IO).
	public sealed class ClimateControl : IClimateControl
	{
		private const double MinTemp = -30.0;
		private const double MaxTemp = 60.0;
		private const double StepC = 0.5; // change per UpdateClimate()

		public double CurrentTemperature { get; private set; }
		public double TargetTemperature { get; private set; }
		public bool PreConditioningEnabled { get; private set; }

		public ClimateControl(double initialTempC = 20.0)
		{
			CurrentTemperature = Clamp(initialTempC);
			TargetTemperature = CurrentTemperature;
		}

		public void SetTemperature(double targetCelsius)
		{
			TargetTemperature = Clamp(targetCelsius);
		}

		public void EnablePreConditioning() => PreConditioningEnabled = true;
		public void DisablePreConditioning() => PreConditioningEnabled = false;

		public void UpdateClimate()
		{
			var delta = TargetTemperature - CurrentTemperature;
			if (Math.Abs(delta) <= StepC)
			{
				CurrentTemperature = TargetTemperature;
				return;
			}
			CurrentTemperature += Math.Sign(delta) * StepC;
			CurrentTemperature = Clamp(CurrentTemperature);
		}

		private static double Clamp(double v) => Math.Max(MinTemp, Math.Min(MaxTemp, v));
	}
}
