using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVSystem.Communication
{
    public abstract class J1939Adapter
    {
        public abstract void Register();
        public abstract void Leave();
    }
}
