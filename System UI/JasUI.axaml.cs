using Avalonia.Controls;
using EVSystem.Communication;
using EVSystem.Components;
using EVSystem.Components.DriveModes;
using EVSystem.Interfaces;
using EVSystem.Mock;
using System;

namespace System_UI
{
	public partial class JasUI : Window
	{
		private readonly IBattery _battery;
		private readonly IClimateControl _climate;
		private readonly DriveModeSelector _driveModes;

		public JasUI()
		{
			InitializeComponent();

			// Create shared adapter + components from EVSystem
			var j1939 = new MockJ1939Adapter();
			_battery = new BatteryMonitor(j1939);
			_climate = new ClimateControl();
			_driveModes = new DriveModeSelector(_battery); 

			RefreshAllStatus();
		}

		private void RefreshAllStatus()
		{
			// Climate UI
			CurrentTempText.Text = $"{_climate.CurrentTemperature:F1} °C";
			TargetTempText.Text = $"{_climate.TargetTemperature:F1} °C";
			PreCondStatusText.Text = _climate.PreConditioningEnabled ? "ON" : "OFF";
			ClimateStatusText.Text = _climate.GetStatus();

			// Drive + Battery UI
			CurrentModeText.Text = _driveModes.CurrentMode;
			BatteryStatusText.Text = _battery.GetStatus();
			DriveModeStatusText.Text = _driveModes.GetStatus();
		}

		// ------------- Event Handlers -------------

		private void OnSetTempClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (float.TryParse(TargetTempInput.Text, out float t))
			{
				_climate.SetTemperature(t);
			}
			else
			{
				ClimateStatusText.Text = "Invalid temperature input.";
			}

			RefreshAllStatus();
		}

		private void OnTogglePreCondClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (_climate.PreConditioningEnabled)
				_climate.DisablePreConditioning();
			else
				_climate.EnablePreConditioning();

			RefreshAllStatus();
		}

		private void OnClimateStepClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			_climate.UpdateClimate();
			RefreshAllStatus();
		}

		private void OnEcoClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			_driveModes.TrySetMode("Eco");
			RefreshAllStatus();
		}

		private void OnNormalClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			_driveModes.TrySetMode("Normal");
			RefreshAllStatus();
		}

		private void OnSportClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			_driveModes.TrySetMode("Sport");
			RefreshAllStatus();
		}

		private void OnBatteryNextClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			_battery.LoadNextData();
			RefreshAllStatus();
		}
	}
}
