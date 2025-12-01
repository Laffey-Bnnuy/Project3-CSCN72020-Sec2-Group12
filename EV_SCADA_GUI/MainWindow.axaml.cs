using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Collections.ObjectModel;
using EV_SCADA.Modules;

namespace EV_SCADA.GUI
{
    public partial class MainWindow : Window
    {
        private readonly DoorControl _doorControl = new();
        private readonly RearViewCamera _camera = new();

        private readonly ObservableCollection<string> DoorLogs = new();
        private readonly ObservableCollection<string> PlaybackLogs = new();

        public MainWindow()
        {
            InitializeComponent();

            // Bind ObservableCollections
            DoorLogsList.Items = DoorLogs;
            PlaybackList.Items = PlaybackLogs;

            // Subscribe to door events
            _doorControl.OnStateChanged += state =>
                Dispatcher.UIThread.Post(() => DoorStateText.Text = $"State: {state}");

            // Subscribe to camera frame updates
            _camera.OnFrameUpdated += frame =>
                Dispatcher.UIThread.Post(() => CameraFeed.Source = null); // replace with bitmap if available

            // Subscribe to camera alerts
            _camera.OnAlert += alert =>
                Dispatcher.UIThread.Post(() => CameraAlert.Text = alert);
        }

        // ---------- Door Controls ----------
        private async void UnlockButton_Click(object sender, RoutedEventArgs e)
        {
            var tokens = _doorControl.GetActiveTokens();
            if (tokens.Count > 0)
            {
                await _doorControl.UnlockDoorAsync(tokens[0].Id);
                DoorLogs.Add(_doorControl.Logs[^1].ToString());
            }
            else
            {
                await ShowMessage("No active tokens available!");
            }
        }

        private async void LockButton_Click(object sender, RoutedEventArgs e)
        {
            await _doorControl.LockDoorAsync();
            DoorLogs.Add(_doorControl.Logs[^1].ToString());
        }

        private void GenerateTokenButton_Click(object sender, RoutedEventArgs e)
        {
            var token = _doorControl.GenerateToken(TimeSpan.FromMinutes(1));
            _ = ShowMessage($"Token Generated: {token.Id}");
        }

        // ---------- Camera Controls ----------
        private void ActivateCameraButton_Click(object sender, RoutedEventArgs e) => _camera.Activate();
        private void DeactivateCameraButton_Click(object sender, RoutedEventArgs e) => _camera.Deactivate();

        private void SnapshotButton_Click(object sender, RoutedEventArgs e)
        {
            _camera.CaptureSnapshot();
            if (_camera.RecordedFrames.Count > 0)
                PlaybackLogs.Add(_camera.RecordedFrames[^1]);
        }

        // Simple message box for Avalonia
        private async System.Threading.Tasks.Task ShowMessage(string message)
        {
            var win = new Window
            {
                Width = 300,
                Height = 150,
                Content = new TextBlock
                {
                    Text = message,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                }
            };
            await win.ShowDialog(this);
        }
    }
}



