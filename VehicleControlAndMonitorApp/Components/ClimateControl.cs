using System;
using EVSystem.Interfaces;

namespace EVSystem.Components
{
	/// <summary>
	/// Manages cabin temperature and pre-conditioning.
	/// </summary>
	public class ClimateControl : IClimateControl
	{
		private const float MinTemp = -30.0f;
		private const float MaxTemp = 60.0f;
		private const float StepC = 0.5f;   // degrees per UpdateClimate()

		public float CurrentTemperature { get; private set; }
		public float TargetTemperature { get; private set; }
		public bool PreConditioningEnabled { get; private set; }

		public ClimateControl(float initialTempC = 20.0f)
		{
			CurrentTemperature = Clamp(initialTempC);
			TargetTemperature = CurrentTemperature;
			PreConditioningEnabled = false;
		}

		public void SetTemperature(float targetCelsius)
		{
			TargetTemperature = Clamp(targetCelsius);
			Console.WriteLine($"[ClimateControl] Target temperature set to {TargetTemperature:F1}°C");
		}

		public void EnablePreConditioning()
		{
			PreConditioningEnabled = true;
			Console.WriteLine("[ClimateControl] Pre-conditioning ENABLED.");
		}

		public void DisablePreConditioning()
		{
			PreConditioningEnabled = false;
			Console.WriteLine("[ClimateControl] Pre-conditioning DISABLED.");
		}

		public void UpdateClimate()
		{
			float delta = TargetTemperature - CurrentTemperature;

			if (Math.Abs(delta) <= StepC)
			{
				CurrentTemperature = TargetTemperature;
			}
			else
			{
				CurrentTemperature += Math.Sign(delta) * StepC;
				CurrentTemperature = Clamp(CurrentTemperature);
			}

			Console.WriteLine($"[ClimateControl] Current: {CurrentTemperature:F1}°C → Target: {TargetTemperature:F1}°C");
		}

		public string GetStatus()
		{
			return $"Climate - Current: {CurrentTemperature:F1}°C, Target: {TargetTemperature:F1}°C, " +
				   $"Pre-conditioning: {(PreConditioningEnabled ? "ON" : "OFF")}";
		}

		private static float Clamp(float value)
		{
			if (value < MinTemp) return MinTemp;
			if (value > MaxTemp) return MaxTemp;
			return value;
		}
	}
}
