using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using EVSystem.Components;
using EVSystem.Controllers;
using EVSystem.Mock;
using System;
using System.Collections.ObjectModel;
using VehicleMonitorGUI.MockComponents;

namespace System_UI
{
    public partial class MainWindow : Window
    {

        private readonly EVSystemController _controller;
        private readonly DispatcherTimer _chargeTimer;


       

        private readonly ObservableCollection<string> _pressureList = new();
        private readonly ObservableCollection<string> _snapshots = new();
        private float _stepRate = 2f; // Battery % per tick

        
      
        private readonly MockLightControl _lights = new();
        private readonly MockTirePressureMonitor _tire = new();
        private readonly MockReverseAlerts _reverseAlerts = new();
        private readonly MockRearViewCameraUI _camera = new();



        public MainWindow()
        {
            InitializeComponent();
          
            // Create system components
            var j1939 = new MockJ1939Adapter();
            var battery = new BatteryMonitor(j1939, "Data/battery_data.csv");
            var charger = new ChargingControl(j1939,battery);

            _controller = new EVSystemController(battery, charger);
            
            // Timer setup
            _chargeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _chargeTimer.Tick += ChargeTick;
            
            // Controller event handlers
            _controller.OnBatteryLevelChanged += level =>
            {
                BatteryLabel.Text = $"Battery Level: {level}%";
            };

            _controller.OnChargingStateChanged += state =>
            {
                ChargingLabel.Text = state ? "Charging: YES" : "Charging: NO";

                if (state)
                    _chargeTimer.Start();
                else
                    _chargeTimer.Stop();
            };

            _controller.OnStatusChanged += message =>
            {
                StatusLabel.Text = $"Status: {message}";
            };

            // UI buttons
            RefreshButton.Click += (_, _) => _controller.RefreshSystemData();
            StartChargeButton.Click += (_, _) => _controller.StartCharging();
            StopChargeButton.Click += (_, _) => _controller.StopCharging();
            RefreshBatteryButton.Click += RefreshBatteryClick;
            RefreshChargingButton.Click += RefreshChargingClick;

			// UI for Jas
			OpenJasUIButton.Click += OpenJasUIButton_Click;


		// Set limit charge
		_controller.OnChargeLimitChanged += UpdateChargeLimitDisplay;
            LimitStatus.Text = $"Current limit: {_controller.GetChargeLimit()}%";
            LimitInput.Text = _controller.GetChargeLimit().ToString();

            ApplyLimitBtn.Click += (_, _) =>
            {
                if (!float.TryParse(LimitInput.Text, out float newLimit))
                {
                    LimitStatus.Text = "Invalid number.";
                    return;
                }

                if (newLimit < 10 || newLimit > 100)
                {
                    LimitStatus.Text = "Limit must be between 10 and 100.";
                    return;
                }

                _controller.SetBattryChargeLimit(newLimit);

                LimitStatus.Text = $"Current limit: {newLimit}%";
            };


            //Nikhil bind UI
          
            PressureList.ItemsSource = _pressureList;
            SnapshotsList.ItemsSource = _snapshots;

        }


        //Simulate charging
        private void ChargeTick(object? sender, EventArgs e)
        {
            float current = _controller.GetBatteryLevel();

            if (!_controller.GetChargeLimit().Equals(null) && current >= _controller.GetChargeLimit())
            {
                _controller.StopCharging();
                _chargeTimer.Stop();
                return;
            }

            float newLevel = current + _stepRate;

            if (newLevel >= _controller.GetChargeLimit())
            {
                newLevel = _controller.GetChargeLimit();
                _controller.StopCharging();
                _chargeTimer.Stop();
            }

            _controller.SimulateBatteryUpdate(newLevel);
            
        }
        private void UpdateChargeLimitDisplay(float limit)
        {
            LimitStatus.Text = $"Current limit: {limit}%";
            LimitInput.Text = limit.ToString();
        }


        private void RefreshBatteryClick(object? sender, RoutedEventArgs e)
        {
            _controller.RefreshBatteryData();
        }

        private void RefreshChargingClick(object? sender, RoutedEventArgs e)
        {
            _controller.RefreshChargingData();
        }



        // ========== LIGHTS ==========
        private void LightsOn_Click(object? sender, RoutedEventArgs e)
        {
            _lights.TurnLightsOn();
            LightStatus.Text = "Lights ON";
        }

        private void LightsOff_Click(object? sender, RoutedEventArgs e)
        {
            _lights.TurnLightsOff();
            LightStatus.Text = "Lights OFF";
        }

        // ========== TIRE PRESSURE ==========
        private void ReadPressure_Click(object? sender, RoutedEventArgs e)
        {
            _pressureList.Clear();

            var pressures = _tire.ReadPressure();

            foreach (var p in pressures)
                _pressureList.Add($"{p.Tire} -> {p.Pressure} PSI");
        }

        // ========== REVERSE ALERTS ==========
        private void CheckAlert_Click(object? sender, RoutedEventArgs e)
        {
            var distance = DistanceSlider.Value;
            AlertOutput.Text = _reverseAlerts.GetAlertMessage(distance);
        }

        // ========== CAMERA ==========
        private void ActivateCamera_Click(object? sender, RoutedEventArgs e)
        {
            _camera.Activate();
            CameraStatusText.Text = "Camera Activated";
        }

        private void DeactivateCamera_Click(object? sender, RoutedEventArgs e)
        {
            _camera.Deactivate();
            CameraStatusText.Text = "Camera Deactivated";
        }

        private void Snapshot_Click(object? sender, RoutedEventArgs e)
        {
            var snap = _camera.CaptureSnapshot();
            _snapshots.Add(snap);
        }
        // Jas UI
		private void OpenJasUIButton_Click(object? sender, RoutedEventArgs e)
		{
			var jasWindow = new JasUI();  
			jasWindow.Show();             
		}

	}

}

