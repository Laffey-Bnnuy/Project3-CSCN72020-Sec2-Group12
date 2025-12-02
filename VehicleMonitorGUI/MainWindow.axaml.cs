using Avalonia.Controls;
using Avalonia.Interactivity;

using VehicleMonitorGUI.MockComponents;

using System.Collections.ObjectModel;

namespace VehicleMonitorGUI;

public partial class MainWindow : Window
{
    private readonly MockBatteryMonitor _battery = new();
    private readonly MockChargingControl _charging = new();
    private readonly MockLightControl _lights = new();
    private readonly MockTirePressureMonitor _tire = new();
    private readonly MockReverseAlerts _reverseAlerts = new();
    private readonly MockRearViewCamera _camera = new();

    private readonly ObservableCollection<string> _pressureList = new();
    private readonly ObservableCollection<string> _snapshots = new();

    public MainWindow()
    {
        InitializeComponent();

        // BIND UI LISTS
        PressureList.ItemsSource = _pressureList;
        SnapshotsList.ItemsSource = _snapshots;

        // INITIAL BATTERY
        BatteryLevelBar.Value = _battery.GetBatteryLevel();
        BatteryStatusText.Text = $"{_battery.GetBatteryLevel()}%";
    }

    // 🔋 ========== CHARGING ==========
    private void StartCharging_Click(object? sender, RoutedEventArgs e)
    {
        _charging.StartCharging();
        ChargingStatusText.Text = "Charging Started";
    }

    private void StopCharging_Click(object? sender, RoutedEventArgs e)
    {
        _charging.StopCharging();
        ChargingStatusText.Text = "Charging Stopped";
    }

    // 💡 ========== LIGHTS ==========
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

    // 🛞 ========== TIRE PRESSURE ==========
    private void ReadPressure_Click(object? sender, RoutedEventArgs e)
    {
        _pressureList.Clear();

        var pressures = _tire.ReadPressure();

        foreach (var p in pressures)
            _pressureList.Add($"{p.Tire} → {p.Pressure} PSI");
    }

    // 🔄 ========== REVERSE ALERTS ==========
    private void CheckAlert_Click(object? sender, RoutedEventArgs e)
    {
        var distance = DistanceSlider.Value;
        AlertOutput.Text = _reverseAlerts.GetAlertMessage(distance);
    }

    // 📷 ========== CAMERA ==========
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
}
