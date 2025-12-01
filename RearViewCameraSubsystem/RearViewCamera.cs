using System;
using System.Collections.Generic;
using System.Threading;

namespace EV_SCADA.Modules
{
    public enum Sensitivity { Low, Medium, High }

    public class RearViewCamera
    {
        private Timer? _frameTimer;
        private int _frameIndex = 0;

        private readonly List<string> _frames = new() { "Frame1", "Frame2", "Frame3", "Frame4" };
        private bool _active = false;

        public event Action<string>? OnFrameUpdated;
        public event Action<string>? OnAlert;

        private readonly ReverseAlert _sensor;
        public List<string> RecordedFrames { get; } = new();

        // Sprint 2 Properties
        public Sensitivity AlertSensitivity { get; set; } = Sensitivity.Medium;
        public bool AlertsMuted { get; set; } = false;
        private bool _faultTriggered = false;

        public RearViewCamera()
        {
            _sensor = new ReverseAlert();
            _sensor.OnDistanceChanged += HandleDistance;
        }

        /// <summary>
        /// Activates the camera and starts frame updates and sensor simulation
        /// </summary>
        public void Activate()
        {
            if (_faultTriggered) return;

            _active = true;
            Console.WriteLine("[Camera] Activated.");
            _frameTimer = new Timer(NextFrame, null, 0, 300);
            _sensor.Start();
        }

        /// <summary>
        /// Deactivates the camera and stops updates
        /// </summary>
        public void Deactivate()
        {
            _active = false;
            Console.WriteLine("[Camera] Deactivated.");
            _frameTimer?.Dispose();
            _sensor.Stop();
        }

        /// <summary>
        /// Captures the current frame as a snapshot
        /// </summary>
       public string CaptureSnapshot()
       {
       if (_active && _frameIndex < _frames.Count)
       {
        string frame = _frames[_frameIndex];
        RecordedFrames.Add(frame);
        OnFrameUpdated?.Invoke(frame);
        return frame;
       }
       return string.Empty;
       }
  


        /// <summary>
        /// Handles the next frame update in the live feed
        /// </summary>
        private void NextFrame(object? state)
        {
            if (!_active || _faultTriggered) return;

            if (_frameIndex >= _frames.Count)
                _frameIndex = 0;

            var frame = _frames[_frameIndex++];
            RecordedFrames.Add(frame);
            OnFrameUpdated?.Invoke(frame);
            Console.WriteLine($"Displaying: {frame}");
        }

        /// <summary>
        /// Handles reverse sensor distance updates and triggers alerts
        /// </summary>
        private void HandleDistance(int distance)
        {
            if (_faultTriggered || AlertsMuted) return;

            string alert = distance switch
            {
                < 30 => AlertSensitivity == Sensitivity.High ? "STOP! Obstacle too close!" : "Warning: Obstacle nearby",
                < 100 => "Warning: Obstacle nearby",
                _ => "Clear"
            };

            OnAlert?.Invoke(alert);
            Console.WriteLine($"[Alert] {alert} (Distance: {distance}cm)");
        }

        /// <summary>
        /// Returns a read-only copy of all recorded frames
        /// </summary>
        public IReadOnlyList<string> Playback() => RecordedFrames.AsReadOnly();

        /// <summary>
        /// Triggers a fault, stopping all camera operations
        /// </summary>
        public void TriggerFault()
        {
            _faultTriggered = true;
            _frameTimer?.Dispose();
            _sensor.Stop();
            Console.WriteLine("[Camera] Fault triggered! Camera disabled.");
        }

        /// <summary>
        /// Test hook to inject a frame manually for unit testing
        /// </summary>
        internal void TestInjectFrame(string frame)
        {
            RecordedFrames.Add(frame);
            OnFrameUpdated?.Invoke(frame);
        }
    }
}

