using System;
using System.Threading;

namespace EV_SCADA.Modules
{
    public class ReverseAlert
    {
        private Timer? _sensorTimer;
        private int _distance = 200;
        private readonly Random _rand = new();

        public event Action<int>? OnDistanceChanged;

        public void Start()
        {
            _sensorTimer = new Timer(SimulateDistance, null, 0, 600);
        }

        public void Stop()
        {
            _sensorTimer?.Dispose();
        }

        private void SimulateDistance(object? _)
        {
            _distance -= _rand.Next(10, 40);
            if (_distance <= 0)
                _distance = 200;

            OnDistanceChanged?.Invoke(_distance);
        }

        public string GetAlertMessage(int distance)
        {
            return distance switch
            {
                < 30  => "STOP! Obstacle too close!",
                < 100 => "Warning: Obstacle nearby",
                _     => "Clear"
            };
        }
    }
}
