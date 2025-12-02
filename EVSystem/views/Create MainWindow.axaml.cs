using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using EVSystem.Interfaces;
using EVSystem.Components;
using EVSystem.Mock;
using EVSystem.Communication;
using EVSystem.ViewModels;

namespace EVSystem.Views
{
    public partial class MainWindow : Window
    {
        // Components
        private J1939Adapter? adapter;
        private IBattery? battery;
        private IChargingControl? charger;
        private IRearViewCamera? camera;
        private IReverseAlerts? reverseAlerts;
        private ILightControl? lightControl;
        private ITirePressureMonitor? tireMonitor;

        private DispatcherTimer? updateTimer;
        private MainWindowViewModel? viewModel;

        public MainWindow()
        {
            InitializeComponent();
            InitializeComponents();
            StartMonitoring();
        }

        private void InitializeComponents()
        {
            viewModel = this.DataContext as MainWindowViewModel;
            
            adapter = new MockJ1939Adapter();
            battery = new BatteryMonitor(adapter);
            charger = new ChargingControl(adapter);
            camera = new MockRearViewCamera();
            reverseAlerts = new ReverseAlerts(adapter, camera);
            lightControl = new LightControl(adapter);
            tireMonitor = new TirePressureMonitor(adapter);
        }

        private void StartMonitoring()
        {
            UpdateAllData();
            
            updateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            updateTimer.Tick += (s, e) => UpdateAllData();
            updateTimer.Start();
        }

        private void UpdateAllData()
        {
            if (battery == null || charger == null || tireMonitor == null || 
                lightControl == null || reverseAlerts == null || viewModel == null) 
                return;

            // Update simulated data
            tireMonitor.UpdateTirePressures();
            lightControl.UpdateAmbientLight();
            reverseAlerts.UpdateSensorData();

            // Update ViewModel
            viewModel.BatteryLevel = $"{battery.BatteryLevel:F1}%";
            viewModel.BatteryTemp = $"{battery.Temperature:F1}°C";
            viewModel.BatteryRange = $"{battery.RemainingKm:F1} km";
            viewModel.BatteryMode = battery.BatteryMode;

            viewModel.ChargingStatus = charger.IsCharging ? "⚡ Charging" : "Not Charging";
            viewModel.IsCharging = charger.IsCharging;

            var pressures = tireMonitor.TirePressures;
            viewModel.TireFLPressure = $"{pressures["FrontLeft"]:F1} PSI {GetPressureIcon(pressures["FrontLeft"])}";
            viewModel.TireFRPressure = $"{pressures["FrontRight"]:F1} PSI {GetPressureIcon(pressures["FrontRight"])}";
            viewModel.TireRLPressure = $"{pressures["RearLeft"]:F1} PSI {GetPressureIcon(pressures["RearLeft"])}";
            viewModel.TireRRPressure = $"{pressures["RearRight"]:F1} PSI {GetPressureIcon(pressures["RearRight"])}";

            viewModel.LightMode = lightControl.LightMode;
            viewModel.AmbientLight = $"{lightControl.AmbientLight:F0} lux";
            viewModel.AutoLightMode = lightControl.AutoMode;

            viewModel.ObstacleStatus = reverseAlerts.ObstacleDetected ? "⚠️ DETECTED" : "✓ Clear";
            viewModel.ObstacleDistance = $"{reverseAlerts.DistanceToObstacle:F2} m";
            viewModel.CameraStatus = camera.IsActive ? "📹 Active" : "Inactive";
            viewModel.IsObstacleDetected = reverseAlerts.ObstacleDetected;
        }

        private string GetPressureIcon(float pressure)
        {
            if (pressure < 25) return "🚨";
            if (pressure < 30) return "⚠️";
            if (pressure > 35) return "⚠️";
            return "✓";
        }

        // Navigation Methods
        private void ShowOverview(object? sender, RoutedEventArgs e)
        {
            HideAllContent();
            var overview = this.FindControl<StackPanel>("OverviewContent");
            if (overview != null) overview.IsVisible = true;
            UpdateMenuButtons(sender as Button);
        }

        private void ShowBattery(object? sender, RoutedEventArgs e)
        {
            HideAllContent();
            var batteryContent = this.FindControl<StackPanel>("BatteryContent");
            if (batteryContent != null) batteryContent.IsVisible = true;
            UpdateMenuButtons(sender as Button);
        }

        private void ShowCharging(object? sender, RoutedEventArgs e)
        {
            HideAllContent();
            var chargingContent = this.FindControl<StackPanel>("ChargingContent");
            if (chargingContent != null) chargingContent.IsVisible = true;
            UpdateMenuButtons(sender as Button);
        }

        private void ShowTires(object? sender, RoutedEventArgs e)
        {
            HideAllContent();
            var tiresContent = this.FindControl<StackPanel>("TiresContent");
            if (tiresContent != null) tiresContent.IsVisible = true;
            UpdateMenuButtons(sender as Button);
        }

        private void ShowLights(object? sender, RoutedEventArgs e)
        {
            HideAllContent();
            var lightsContent = this.FindControl<StackPanel>("LightsContent");
            if (lightsContent != null) lightsContent.IsVisible = true;
            UpdateMenuButtons(sender as Button);
        }

        private void ShowSafety(object? sender, RoutedEventArgs e)
        {
            HideAllContent();
            var safetyContent = this.FindControl<StackPanel>("SafetyContent");
            if (safetyContent != null) safetyContent.IsVisible = true;
            UpdateMenuButtons(sender as Button);
        }

        private void HideAllContent()
        {
            var overview = this.FindControl<StackPanel>("OverviewContent");
            var batteryContent = this.FindControl<StackPanel>("BatteryContent");
            var chargingContent = this.FindControl<StackPanel>("ChargingContent");
            var tiresContent = this.FindControl<StackPanel>("TiresContent");
            var lightsContent = this.FindControl<StackPanel>("LightsContent");
            var safetyContent = this.FindControl<StackPanel>("SafetyContent");

            if (overview != null) overview.IsVisible = false;
            if (batteryContent != null) batteryContent.IsVisible = false;
            if (chargingContent != null) chargingContent.IsVisible = false;
            if (tiresContent != null) tiresContent.IsVisible = false;
            if (lightsContent != null) lightsContent.IsVisible = false;
            if (safetyContent != null) safetyContent.IsVisible = false;
        }

        private void UpdateMenuButtons(Button? activeButton)
        {
            // Remove active class from all menu buttons
            var sidebar = this.FindControl<Border>("sidebar");
            // Add logic to update button styles if needed
        }

        // Action Methods
        private async void SetBatteryMode(object? sender, RoutedEventArgs e)
        {
            if (battery == null) return;
            
            var combo = this.FindControl<ComboBox>("BatteryModeCombo");
            if (combo?.SelectedItem is ComboBoxItem item)
            {
                string mode = item.Content?.ToString() ?? "Normal";
                battery.SetBatteryMode(mode);
                UpdateAllData();
                
                await ShowMessageBox("Success", $"Battery mode set to {mode}");
            }
        }

        private async void StartCharging(object? sender, RoutedEventArgs e)
        {
            if (charger == null) return;
            
            charger.StartCharging();
            UpdateAllData();
            await ShowMessageBox("Success", "Charging started!");
        }

        private async void StopCharging(object? sender, RoutedEventArgs e)
        {
            if (charger == null) return;
            
            charger.StopCharging();
            UpdateAllData();
            await ShowMessageBox("Success", "Charging stopped!");
        }

        private async void SetChargingSchedule(object? sender, RoutedEventArgs e)
        {
            if (charger == null) return;
            
            var textBox = this.FindControl<TextBox>("ScheduleTextBox");
            if (textBox != null && !string.IsNullOrEmpty(textBox.Text))
            {
                charger.ScheduleCharging(textBox.Text);
                await ShowMessageBox("Success", $"Charging scheduled for: {textBox.Text}");
            }
        }

        private async void CheckTires(object? sender, RoutedEventArgs e)
        {
            if (tireMonitor == null) return;
            
            bool allOk = tireMonitor.CheckTirePressure();
            UpdateAllData();
            
            string message = allOk ? "All tires are normal! ✓" : "Some tires need attention! ⚠️";
            await ShowMessageBox("Tire Check", message);
        }

        private async void SetLightMode(object? sender, RoutedEventArgs e)
        {
            if (lightControl == null) return;
            
            var combo = this.FindControl<ComboBox>("LightModeCombo");
            if (combo?.SelectedItem is ComboBoxItem item)
            {
                string mode = item.Content?.ToString() ?? "Off";
                lightControl.SetLightMode(mode);
                UpdateAllData();
                
                await ShowMessageBox("Success", $"Light mode set to {mode}");
            }
        }

        private void AutoLightEnabled(object? sender, RoutedEventArgs e)
        {
            if (lightControl == null) return;
            
            lightControl.EnableAutoMode();
            
            var combo = this.FindControl<ComboBox>("LightModeCombo");
            var button = this.FindControl<Button>("SetLightModeBtn");
            
            if (combo != null) combo.IsEnabled = false;
            if (button != null) button.IsEnabled = false;
            
            UpdateAllData();
        }

        private void AutoLightDisabled(object? sender, RoutedEventArgs e)
        {
            if (lightControl == null) return;
            
            lightControl.DisableAutoMode();
            
            var combo = this.FindControl<ComboBox>("LightModeCombo");
            var button = this.FindControl<Button>("SetLightModeBtn");
            
            if (combo != null) combo.IsEnabled = true;
            if (button != null) button.IsEnabled = true;
            
            UpdateAllData();
        }

        private async void CheckObstacles(object? sender, RoutedEventArgs e)
        {
            if (reverseAlerts == null) return;
            
            bool detected = reverseAlerts.DetectObstacle();
            UpdateAllData();
            
            string message = detected 
                ? $"⚠️ OBSTACLE DETECTED!\nDistance: {reverseAlerts.DistanceToObstacle:F2}m\nCamera activated!"
                : "✓ No obstacles detected. Safe to reverse.";
                
            await ShowMessageBox("Obstacle Check", message);
        }

        private async System.Threading.Tasks.Task ShowMessageBox(string title, string message)
        {
            var msgBox = new Window
            {
                Title = title,
                Width = 400,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new StackPanel
                {
                    Margin = new Avalonia.Thickness(20),
                    Spacing = 20,
                    Children =
                    {
                        new TextBlock 
                        { 
                            Text = message, 
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                            FontSize = 14
                        },
                        new Button 
                        { 
                            Content = "OK", 
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            Padding = new Avalonia.Thickness(30, 10),
                            Command = new RelayCommand(() => msgBox.Close())
                        }
                    }
                }
            };
            
            await msgBox.ShowDialog(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            updateTimer?.Stop();
            adapter?.Leave();
            base.OnClosed(e);
        }
    }

    // Simple RelayCommand for buttons
    public class RelayCommand : System.Windows.Input.ICommand
    {
        private readonly Action _execute;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute();
    }
}