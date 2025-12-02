using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using EVSystem.Components;
using EVSystem.Controllers;
using EVSystem.Mock;
using System;

namespace System_UI
{
    public partial class MainWindow : Window
    {
        private readonly EVSystemController _controller;
        private readonly DispatcherTimer _chargeTimer;
        private float _stepRate = 2f; // Battery % per tick


        
        public MainWindow()
        {
            InitializeComponent();
          
            // Create system components
            var j1939 = new MockJ1939Adapter();
            var battery = new BatteryMonitor(j1939);
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


    }

}
