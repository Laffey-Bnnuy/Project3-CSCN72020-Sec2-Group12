using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVSystem.Interfaces
{
    public interface IReverseAlerts
    {
        bool ObstacleDetected { get; }
        float DistanceToObstacle { get; }
        void UpdateSensorData();
        bool DetectObstacle();
        void WarnDriver();
        string GetStatus();
    }
}