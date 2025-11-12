using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVSystem.Interfaces
{
    public interface ILightControl
    {
        string LightMode { get; }
        bool AutoMode { get; }
        float AmbientLight { get; }
        void SetLightMode(string mode);
        void EnableAutoMode();
        void DisableAutoMode();
        void UpdateAmbientLight();
        string GetStatus();
    }
}