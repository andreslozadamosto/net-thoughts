using System;
using System.Collections.Generic;
using System.Text;

namespace DefaultInterfaceMethods.Interfaces
{
    public interface IMyClassEOverride : IMyClassE
    {
        void IMyClassE.Works()
        {
            Console.WriteLine("Hello from IMyClassEOverride");
        }
    }
}
