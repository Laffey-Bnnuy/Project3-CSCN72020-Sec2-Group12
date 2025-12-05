using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;
using EV_SCADA.Modules;

namespace EV_SCADA.GUI
{
    public partial class MainWindow : Window
    {
        // Door + Camera Subsystems
        private readonly DoorControl _doorControl = new();
        private readonly RearViewCamera _camera = new();

        // UI Collections
        private readonly ObservableCollection<string> DoorLogs = new();
        private readonly ObservableCollection<string> PlaybackLogs = new();

        public MainWindow()
        {
            InitializeComponent();

            // Bind lists
            DoorLogsList.ItemsSource = DoorLogs;
            PlaybackList.ItemsSource = PlaybackLogs;

            // Door state event
            _doorControl.OnStateChanged += state =>
                Dispatcher.UIThread.Post(() =>
                {
                    DoorStateText.Text = $"State: {state}";
                });

            // Camera alert event (NO CameraFeed used)
            _camera.OnAlert += alert =>
                Dispatcher.UIThread.Post(() =>
                {
                    CameraAlert.Text = alert;
                });

            // Frame event removed (no CameraFeed)
            _camera.OnFrameUpdated += frame => { };
        }

        // ---------------- Door Control ----------------

        private async void UnlockButton_Click(object? sender, RoutedEventArgs e)
        {
            var tokens = _doorControl.GetActiveTokens();

            if (tokens.Count == 0)
            {
                await ShowMessage("No active tokens. Generate one first.");
                return;
            }

            await _doorControl.UnlockDoorAsync(tokens[0].Id);
            DoorLogs.Add(_doorControl.Logs[^1].ToString());
        }

        private async void LockButton_Click(object? sender, RoutedEventArgs e)
        {
            await _doorControl.LockDoorAsync();
            DoorLogs.Add(_doorControl.Logs[^1].ToString());
        }

        private void GenerateTokenButton_Click(object? sender, RoutedEventArgs e)
        {
            var token = _doorControl.GenerateToken(TimeSpan.FromMinutes(1));
            _ = ShowMessage($"Token Created: {token.Id}");
        }

        // ---------------- Camera Control ----------------

        private void ActivateCameraButton_Click(object? sender, RoutedEventArgs e)
        {
            _camera.Activate();
        }

        private void DeactivateCameraButton_Click(object? sender, RoutedEventArgs e)
        {
            _camera.Deactivate();
        }

        private void SnapshotButton_Click(object? sender, RoutedEventArgs e)
        {
            string snap = _camera.CaptureSnapshot();
            if (!string.IsNullOrWhiteSpace(snap))
                PlaybackLogs.Add(snap);
        }

        // ---------------- Popup Message ----------------

        private async Task ShowMessage(string message)
        {
            var w = new Window
            {
                Width = 280,
                Height = 140,
                Content = new TextBlock
                {
                    Text = message,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                }
            };

            await w.ShowDialog(this);
        }
    }
}