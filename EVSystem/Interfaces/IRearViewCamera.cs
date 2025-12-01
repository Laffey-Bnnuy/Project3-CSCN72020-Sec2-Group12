using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVSystem.Interfaces
{
    public interface IRearViewCamera
    {
        bool IsActive { get; }
        string GetVideoFeed();
        void ActivateCamera();
        void DeactivateCamera();
    }
}
