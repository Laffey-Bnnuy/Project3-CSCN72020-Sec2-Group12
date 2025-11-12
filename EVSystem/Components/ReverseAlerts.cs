using EVSystem.Communication;
using EVSystem.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace EVSystem.Components
{
    public class ReverseAlerts : IReverseAlerts
    {
        private readonly J1939Adapter _j1939Adapter;
        private readonly IRearViewCamera _rearViewCamera;
        private string _dataFilePath;
        private string[] _sensorData;
        private int _currentIndex;

        public bool ObstacleDetected { get; private set; }
        public float DistanceToObstacle { get; private set; }

        public ReverseAlerts(J1939Adapter j1939Adapter, IRearViewCamera rearViewCamera, string dataFilePath = "Data/reverse_alerts_data.csv")
        {
            _j1939Adapter = j1939Adapter;
            _rearViewCamera = rearViewCamera;
            _dataFilePath = dataFilePath;
            _currentIndex = 0;

            _j1939Adapter.Register();
            LoadSensorData();
            UpdateSensorData();
        }

        private void LoadSensorData()
        {
            try
            {
                if (File.Exists(_dataFilePath))
                {
                    _sensorData = File.ReadAllLines(_dataFilePath)
                                     .Skip(1) // Skip header
                                     .ToArray();
                }
                else
                {
                    Console.WriteLine($"[ReverseAlerts] Warning: Data file not found at {_dataFilePath}. Using default values.");
                    _sensorData = new string[] { "2.5,true", "1.8,true", "3.0,false" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReverseAlerts] Error loading data: {ex.Message}");
                _sensorData = new string[] { "2.5,true", "1.8,true", "3.0,false" };
            }
        }

        public void UpdateSensorData()
        {
            if (_sensorData == null || _sensorData.Length == 0)
            {
                return;
            }

            string[] values = _sensorData[_currentIndex].Split(',');
            DistanceToObstacle = float.Parse(values[0]);
            ObstacleDetected = bool.Parse(values[1]);

            _currentIndex = (_currentIndex + 1) % _sensorData.Length;
        }

        public bool DetectObstacle()
        {
            UpdateSensorData();

            if (ObstacleDetected && DistanceToObstacle < 2.0f)
            {
                _rearViewCamera.ActivateCamera();
                WarnDriver();
                return true;
            }

            return false;
        }

        public void WarnDriver()
        {
            if (ObstacleDetected)
            {
                string warningLevel = DistanceToObstacle switch
                {
                    < 0.5f => "CRITICAL",
                    < 1.0f => "HIGH",
                    < 2.0f => "MEDIUM",
                    _ => "LOW"
                };

                Console.WriteLine($"[ReverseAlerts] ⚠️  WARNING ({warningLevel}): Obstacle detected at {DistanceToObstacle:F2}m!");
                Console.WriteLine($"[ReverseAlerts] {_rearViewCamera.GetVideoFeed()}");
            }
        }

        public string GetStatus()
        {
            return $"Reverse Alerts - Obstacle: {(ObstacleDetected ? "YES" : "NO")}, Distance: {DistanceToObstacle:F2}m, Camera: {(_rearViewCamera.IsActive ? "ACTIVE" : "INACTIVE")}";
        }
    }
}