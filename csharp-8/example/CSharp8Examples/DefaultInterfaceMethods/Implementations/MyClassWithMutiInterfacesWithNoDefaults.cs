using DefaultInterfaceMethods.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DefaultInterfaceMethods.Implementations
{
    public class MyClassWithMutiInterfacesWithNoDefaults : IMyClassA, IMyClassC
    {
        public void Works() => Console.WriteLine("Hello from MyClassWithMutiInterfacesWithNoDefaults");
    }
}
