using EVSystem.Communication;
using EVSystem.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace EVSystem.Components
{
    public class LightControl : ILightControl
    {
        private readonly J1939Adapter _j1939Adapter;
        private string _dataFilePath;
        private string[] _ambientData;
        private int _currentIndex;

        public string LightMode { get; private set; }
        public bool AutoMode { get; private set; }
        public float AmbientLight { get; private set; }

        public LightControl(J1939Adapter j1939Adapter, string dataFilePath = "Data/light_control_data.csv")
        {
            _j1939Adapter = j1939Adapter;
            _dataFilePath = dataFilePath;
            _currentIndex = 0;

            _j1939Adapter.Register();
            LightMode = "Off";
            AutoMode = false;
            
            LoadAmbientData();
            UpdateAmbientLight();
        }

        private void LoadAmbientData()
        {
            try
            {
                if (File.Exists(_dataFilePath))
                {
                    _ambientData = File.ReadAllLines(_dataFilePath)
                                      .Skip(1) // Skip header
                                      .ToArray();
                }
                else
                {
                    Console.WriteLine($"[LightControl] Warning: Data file not found at {_dataFilePath}. Using default values.");
                    _ambientData = new string[] { "800", "600", "300", "150", "50", "750" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LightControl] Error loading data: {ex.Message}");
                _ambientData = new string[] { "800", "600", "300", "150", "50", "750" };
            }
        }

        public void UpdateAmbientLight()
        {
            if (_ambientData == null || _ambientData.Length == 0)
            {
                return;
            }

            AmbientLight = float.Parse(_ambientData[_currentIndex]);
            _currentIndex = (_currentIndex + 1) % _ambientData.Length;

            if (AutoMode)
            {
                AutoAdjustLights();
            }
        }

        private void AutoAdjustLights()
        {
            if (AmbientLight < 200)
            {
                LightMode = "Full";
                Console.WriteLine("[LightControl] 💡 Auto: Lights set to FULL (dark environment)");
            }
            else if (AmbientLight < 500)
            {
                LightMode = "Parking";
                Console.WriteLine("[LightControl] 💡 Auto: Lights set to PARKING (dim environment)");
            }
            else
            {
                LightMode = "Off";
                Console.WriteLine("[LightControl] 💡 Auto: Lights turned OFF (bright environment)");
            }
        }

        public void SetLightMode(string mode)
        {
            if (mode == "Full" || mode == "Parking" || mode == "Off")
            {
                LightMode = mode;
                AutoMode = false;
                Console.WriteLine($"[LightControl] Manual mode: Lights set to {mode}");
            }
            else
            {
                Console.WriteLine($"[LightControl] Invalid mode: {mode}. Use 'Full', 'Parking', or 'Off'");
            }
        }

        public void EnableAutoMode()
        {
            AutoMode = true;
            Console.WriteLine("[LightControl] Auto mode ENABLED");
            UpdateAmbientLight();
        }

        public void DisableAutoMode()
        {
            AutoMode = false;
            Console.WriteLine("[LightControl] Auto mode DISABLED");
        }

        public string GetStatus()
        {
            return $"Light Control - Mode: {LightMode}, Auto: {(AutoMode ? "ON" : "OFF")}, Ambient Light: {AmbientLight:F0} lux";
        }
    }
}