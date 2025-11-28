using System;
using System.Collections.Generic;
using ElectricCarSystem.Core.Abstractions;

namespace ElectricCarSystem.Core.DriveModes
{
    /// Strategy-based mode orchestrator. Applies selected mode to collaborators.
    public sealed class DriveModeSelector
    {
        private readonly Dictionary<string, IDriveModeStrategy> _strategies;
        private IDriveModeStrategy _current;
        private readonly IBatteryMonitor? _battery; // optional for Sprint 1

        public string CurrentMode => _current.Name;

        public DriveModeSelector(
            IEnumerable<IDriveModeStrategy> strategies,
            string defaultMode = "Normal",
            IBatteryMonitor? battery = null)
        {
            _strategies = new(StringComparer.OrdinalIgnoreCase);
            foreach (var s in strategies) _strategies[s.Name] = s;

            _battery = battery;
            if (!_strategies.TryGetValue(defaultMode, out _current))
            {
                foreach (var s in _strategies.Values) { _current = s; break; }
            }
        }

        public bool TrySetMode(string modeName)
        {
            if (string.IsNullOrWhiteSpace(modeName)) return false;
            if (!_strategies.TryGetValue(modeName, out var chosen)) return false;

            _current = chosen;
            ApplyModeSettings();
            return true;
        }

        public void ApplyModeSettings()
        {
            if (_battery != null)
            {
                _battery.MaxDischargeKW     = _current.PowerLimitKW;
                _battery.RegenerationFactor = _current.RegenStrength;
            }
            // In Sprint 2: raise an event to inform ClimateControl about _current.ClimateEfficiencyBias
        }

        public IDriveModeStrategy CurrentStrategy() => _current;
    }
}
