using System;

namespace EVSystem.Interfaces
{
	/// <summary>
	/// Interface for managing cabin climate (heating, cooling, pre-conditioning).
	/// </summary>
	public interface IClimateControl
	{
		float CurrentTemperature { get; }
		float TargetTemperature { get; }
		bool PreConditioningEnabled { get; }

		void SetTemperature(float targetCelsius);
		void EnablePreConditioning();
		void DisablePreConditioning();

		/// <summary>
		/// Moves the current temperature one step toward the target.
		/// Intended to be called on a timer or update cycle.
		/// </summary>
		void UpdateClimate();

		/// <summary>
		/// Returns a human-readable summary of current climate state.
		/// </summary>
		string GetStatus();
	}
}
