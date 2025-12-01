using System;
using System.Threading;

namespace EV_SCADA.Modules
{
    public class ReverseAlert
    {
        private Timer? _sensorTimer;
        private int _distance = 200; // starting simulated distance in cm
        private readonly Random _rand = new();

        public event Action<int>? OnDistanceChanged;

        // Start simulating sensor readings
        public void Start()
        {
            Console.WriteLine("[ReverseAlert] Sensor activated.");
            _sensorTimer = new Timer(SimulateDistance, null, 0, 600); // update every 600 ms
        }

        // Stop the sensor simulation
        public void Stop()
        {
            _sensorTimer?.Dispose();
            Console.WriteLine("[ReverseAlert] Sensor stopped.");
        }

        // Simulate changing distances as car reverses
        private void SimulateDistance(object? _)
        {
            // Randomly reduce distance, then reset when close
            _distance -= _rand.Next(10, 40);

            if (_distance <= 0)
                _distance = 200; // reset distance when "clear"

            OnDistanceChanged?.Invoke(_distance);
        }

        // --------------------------------------------------------------------
        // Sprint 2 Unit Test Support
        // Helper method to calculate alert message WITHOUT requiring timer.
        // --------------------------------------------------------------------
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
