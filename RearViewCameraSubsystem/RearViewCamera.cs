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

        public Sensitivity AlertSensitivity { get; set; } = Sensitivity.Medium;
        public bool AlertsMuted { get; set; } = false;
        private bool _faultTriggered = false;

        public RearViewCamera()
        {
            _sensor = new ReverseAlert();
            _sensor.OnDistanceChanged += HandleDistance;
        }

        public void Activate()
        {
            if (_faultTriggered) return;

            _active = true;
            _frameTimer = new Timer(NextFrame, null, 0, 300);
            _sensor.Start();
        }

        public void Deactivate()
        {
            _active = false;
            _frameTimer?.Dispose();
            _sensor.Stop();
        }

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

        private void NextFrame(object? state)
        {
            if (!_active || _faultTriggered) return;

            if (_frameIndex >= _frames.Count)
                _frameIndex = 0;

            string frame = _frames[_frameIndex++];
            RecordedFrames.Add(frame);
            OnFrameUpdated?.Invoke(frame);
        }

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
        }

        public IReadOnlyList<string> Playback() => RecordedFrames.AsReadOnly();

        public void TriggerFault()
        {
            _faultTriggered = true;
            _frameTimer?.Dispose();
            _sensor.Stop();
        }

        // test helper for xUnit
        public void TestInjectFrame(string frame)
        {
            RecordedFrames.Add(frame);
            OnFrameUpdated?.Invoke(frame);
        }
    }
}
