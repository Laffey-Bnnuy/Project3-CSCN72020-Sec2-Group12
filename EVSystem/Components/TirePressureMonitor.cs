using EVSystem.Communication;
using EVSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EVSystem.Components
{
    public class TirePressureMonitor : ITirePressureMonitor
    {
        private readonly J1939Adapter _j1939Adapter;
        private string _dataFilePath;
        private string[] _pressureData;
        private int _currentIndex;

        public Dictionary<string, float> TirePressures { get; private set; }

        private const float MIN_PRESSURE = 30.0f; // PSI
        private const float MAX_PRESSURE = 35.0f; // PSI
        private const float CRITICAL_PRESSURE = 25.0f; // PSI

        public TirePressureMonitor(J1939Adapter j1939Adapter, string dataFilePath = "Data/tire_pressure_data.csv")
        {
            _j1939Adapter = j1939Adapter;
            _dataFilePath = dataFilePath;
            _currentIndex = 0;

            _j1939Adapter.Register();

            TirePressures = new Dictionary<string, float>
            {
                { "FrontLeft", 32.0f },
                { "FrontRight", 32.0f },
                { "RearLeft", 32.0f },
                { "RearRight", 32.0f }
            };

            LoadPressureData();
            UpdateTirePressures();
        }

        private void LoadPressureData()
        {
            try
            {
                if (File.Exists(_dataFilePath))
                {
                    _pressureData = File.ReadAllLines(_dataFilePath)
                                       .Skip(1) // Skip header
                                       .ToArray();
                }
                else
                {
                    Console.WriteLine($"[TirePressureMonitor] Warning: Data file not found at {_dataFilePath}. Using default values.");
                    _pressureData = new string[] { "32.0,32.5,31.8,32.2", "31.5,32.0,31.0,31.8", "30.0,31.5,29.5,30.8" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TirePressureMonitor] Error loading data: {ex.Message}");
                _pressureData = new string[] { "32.0,32.5,31.8,32.2", "31.5,32.0,31.0,31.8", "30.0,31.5,29.5,30.8" };
            }
        }

        public void UpdateTirePressures()
        {
            if (_pressureData == null || _pressureData.Length == 0)
            {
                return;
            }

            string[] values = _pressureData[_currentIndex].Split(',');

            TirePressures["FrontLeft"] = float.Parse(values[0]);
            TirePressures["FrontRight"] = float.Parse(values[1]);
            TirePressures["RearLeft"] = float.Parse(values[2]);
            TirePressures["RearRight"] = float.Parse(values[3]);

            _currentIndex = (_currentIndex + 1) % _pressureData.Length;
        }

        public bool CheckTirePressure()
        {
            bool allOk = true;

            foreach (var tire in TirePressures)
            {
                if (tire.Value < CRITICAL_PRESSURE)
                {
                    AlertReplacement(tire.Key);
                    allOk = false;
                }
                else if (tire.Value < MIN_PRESSURE)
                {
                    Console.WriteLine($"[TirePressureMonitor] ⚠️  LOW PRESSURE: {tire.Key} = {tire.Value:F1} PSI (Min: {MIN_PRESSURE} PSI)");
                    allOk = false;
                }
                else if (tire.Value > MAX_PRESSURE)
                {
                    Console.WriteLine($"[TirePressureMonitor] ⚠️  HIGH PRESSURE: {tire.Key} = {tire.Value:F1} PSI (Max: {MAX_PRESSURE} PSI)");
                    allOk = false;
                }
            }

            if (allOk)
            {
                Console.WriteLine("[TirePressureMonitor] ✓ All tire pressures are normal");
            }

            return allOk;
        }

        public void AlertReplacement(string tire)
        {
            Console.WriteLine($"[TirePressureMonitor] 🚨 CRITICAL: {tire} pressure is {TirePressures[tire]:F1} PSI - IMMEDIATE REPLACEMENT REQUIRED!");
        }

        public string GetStatus()
        {
            return $"Tire Pressure - FL: {TirePressures["FrontLeft"]:F1}, FR: {TirePressures["FrontRight"]:F1}, " +
                   $"RL: {TirePressures["RearLeft"]:F1}, RR: {TirePressures["RearRight"]:F1} PSI";
        }
    }
}
