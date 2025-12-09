using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
 
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using EV_SCADA.Modules;
using DoorControlType = EV_SCADA.Modules.DoorControl;

namespace System_UI.Modules.DoorControl
{
    public partial class DoorControlView : UserControl
    {
        private readonly DoorControlType _door = new();
  
        private readonly ObservableCollection<string> _logs = new();

        public DoorControlView()
        {
            InitializeComponent();
            DoorLogsList.ItemsSource = _logs;

            _door.OnStateChanged += s =>
                Dispatcher.UIThread.Post(() =>
                {
                    DoorStateText.Text = $"State: {s}"; // test
                });
        }

        private async void Unlock_Click(object? sender, RoutedEventArgs e)
        {
            var tokens = _door.GetActiveTokens();
            if (tokens.Count == 0)
            {
                await ShowMessage("No valid tokens. Generate one first.");
                return;
            }

            await _door.UnlockDoorAsync(tokens[0].Id);
            _logs.Add(_door.Logs[^1].ToString());
        }

        private async void Lock_Click(object? sender, RoutedEventArgs e)
        {
            await _door.LockDoorAsync();
            _logs.Add(_door.Logs[^1].ToString());
        }

        private void GenerateToken_Click(object? sender, RoutedEventArgs e)
        {
            var token = _door.GenerateToken(TimeSpan.FromMinutes(1));
            _logs.Add($"Token generated: {token.Id}");
        }

        private async Task ShowMessage(string msg)
        {
            var w = new Window
            {
                Width = 250,
                Height = 120,
                Content = new TextBlock
                {
                    Text = msg,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                }
            };

            await w.ShowDialog((Window)this.VisualRoot!);
        }
    }
}
