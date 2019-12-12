using DefaultInterfaceMethods.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DefaultInterfaceMethods.Implementations
{
    public class MyClassWithMultiInterfacesAndOverride: IMyClassA, IMyClassB
    {
        public void Works() => Console.WriteLine("Hello from MyClassWithMultiInterfacesAndOverride");
    }
}
