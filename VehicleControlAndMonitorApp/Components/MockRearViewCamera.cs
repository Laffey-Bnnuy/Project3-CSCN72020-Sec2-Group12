using EVSystem.Interfaces;
using System;

namespace EVSystem.Mock
{
    public class MockRearViewCamera : IRearViewCamera
    {
        public bool IsActive { get; private set; }

        public MockRearViewCamera()
        {
            IsActive = false;
        }

        public void ActivateCamera()
        {
            IsActive = true;
            Console.WriteLine("[RearViewCamera] Camera activated - streaming video feed");
        }

        public void DeactivateCamera()
        {
            IsActive = false;
            Console.WriteLine("[RearViewCamera] Camera deactivated");
        }

        public string GetVideoFeed()
        {
            if (IsActive)
            {
                return "[VIDEO FEED ACTIVE] Rear view camera streaming...";
            }
            return "[NO FEED] Camera is inactive";
        }
    }
}