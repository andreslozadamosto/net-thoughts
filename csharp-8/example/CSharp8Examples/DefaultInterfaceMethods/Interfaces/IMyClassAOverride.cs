using System;
using System.Collections.Generic;
using System.Text;

namespace DefaultInterfaceMethods.Interfaces
{
    public interface IMyClassAOverride : IMyClassA
    {
        public new void Works() => Console.WriteLine("Hello from IMyClassAOverride");
    }
}
