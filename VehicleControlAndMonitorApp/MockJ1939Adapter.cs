using EVSystem.Communication;
using System;

namespace EVSystem.Mock
{
    public class MockJ1939Adapter : J1939Adapter
    {
        private bool _isRegistered = false;

        public override void Register()
        {
            if (!_isRegistered)
            {
                Console.WriteLine("[J1939] Adapter registered to CAN bus");
                _isRegistered = true;
            }
        }

        public override void Leave()
        {
            if (_isRegistered)
            {
                Console.WriteLine("[J1939] Adapter disconnected from CAN bus");
                _isRegistered = false;
            }
        }
    }
}
