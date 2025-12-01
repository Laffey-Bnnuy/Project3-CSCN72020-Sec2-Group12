using System;
using System.Collections.Generic;
using EVSystem.Interfaces;
using EVSystem.Components.DriveModes;

namespace EVSystem.Components
{
	/// <summary>
	/// Selects between Eco / Normal / Sport and coordinates with the battery.
	/// High-risk because it depends on multiple subsystems.
	/// </summary>
	public class DriveModeSelector
	{
		private readonly Dictionary<string, IDriveModeStrategy> _modes;
		private IDriveModeStrategy _currentMode;
		private readonly IBattery _battery;

		public string CurrentMode => _currentMode.Name;

		public DriveModeSelector(IBattery battery)
		{
			_battery = battery ?? throw new ArgumentNullException(nameof(battery));

			// Register available modes
			_modes = new Dictionary<string, IDriveModeStrategy>(StringComparer.OrdinalIgnoreCase)
			{
				{ "Eco",    new EcoMode() },
				{ "Normal", new NormalMode() },
				{ "Sport",  new SportMode() }
			};

			// Default mode
			_currentMode = _modes["Normal"];
			ApplyCurrentMode();
		}

		/// <summary>
		/// Attempts to switch to the requested mode (Eco/Normal/Sport).
		/// Returns true if successful; false if the mode is invalid.
		/// </summary>
		public bool TrySetMode(string modeName)
		{
			if (string.IsNullOrWhiteSpace(modeName))
				return false;

			if (!_modes.TryGetValue(modeName, out var strategy))
			{
				Console.WriteLine($"[DriveModeSelector] Invalid mode: {modeName}. Use Eco, Normal, or Sport.");
				return false;
			}

			_currentMode = strategy;
			Console.WriteLine($"[DriveModeSelector] Mode changed to: {CurrentMode}");
			ApplyCurrentMode();
			return true;
		}

		private void ApplyCurrentMode()
		{
			// maps drive mode directly into battery mode
			_battery.SetBatteryMode(_currentMode.Name);

			Console.WriteLine(
				$"[DriveModeSelector] Applied {CurrentMode} → " +
				$"PowerFactor={_currentMode.PowerFactor:F1}, Regen={_currentMode.RegenStrength:F1}, " +
				$"BatteryMode={_battery.BatteryMode}"
			);
		}

		public string GetStatus()
		{
			return $"Drive Mode - Current: {CurrentMode}, BatteryMode: {_battery.BatteryMode}";
		}
	}
}
