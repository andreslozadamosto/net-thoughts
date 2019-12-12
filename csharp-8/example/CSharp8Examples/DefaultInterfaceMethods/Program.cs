using DefaultInterfaceMethods.Implementations;
using DefaultInterfaceMethods.Interfaces;
using DefaultInterfaceMethods.Mixins;
using System;
using static DefaultInterfaceMethods.Mixins.IColoredLight;

namespace DefaultInterfaceMethods
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Default Interface Methods");
            Console.WriteLine();

            IMyClassA myClass = new MyClassA();
            myClass.Works();

            IMyClassB myClassWithMultiInterfacesB = new MyClassWithMultiInterfaces(); // gets the more implicit Interface in multi-hieritance
            myClassWithMultiInterfacesB.Works(); 

            IMyClassA myClassWithMultiInterfacesA = new MyClassWithMultiInterfaces(); // gets the more implicit Interface in multi-hieritance
            myClassWithMultiInterfacesA.Works(); 

            MyClassWithMultiInterfaces myClassWithMultiInterfaces = new MyClassWithMultiInterfaces();
            // myClassWithMultiInterfaces.Works(); <-- compilation error

            // when the method is implemented in the class it has more prevalecence than the default method
            MyClassWithMutiInterfacesWithNoDefaults myClassWithMutiInterfacesWithNoDefaults = new MyClassWithMutiInterfacesWithNoDefaults();
            myClassWithMutiInterfacesWithNoDefaults.Works();

            IMyClassA myClassWithMutiInterfacesWithNoDefaultA = new MyClassWithMutiInterfacesWithNoDefaults();
            myClassWithMutiInterfacesWithNoDefaultA.Works();

            IMyClassC myClassWithMutiInterfacesWithNoDefaultC = new MyClassWithMutiInterfacesWithNoDefaults();
            myClassWithMutiInterfacesWithNoDefaultC.Works();

            MyClassWithMultiInterfacesAndOverride myClassWithMultiInterfacesAndOverride = new MyClassWithMultiInterfacesAndOverride();
            myClassWithMultiInterfacesAndOverride.Works();

            IMyClassD myClassD = new MyClassD();
            Console.WriteLine($"My name is {myClassD.Name}");

            MyClassD myClassD2 = new MyClassD();
            // Console.WriteLine($"My name is {myClassD2.Name}"); <-- compilation error

            IMyClassAOverride myClassAOverride = new MyClassAOverride();
            myClassAOverride.Works();

            IMyClassEOverride myClassEOverride = new MyClassEOverride();
            myClassEOverride.Works();

            // ===============================================================================================
            // MIXINS
            ILight light = new SimpleLight();
            light.SwitchOn();
            light.SwitchOff();

            ITimerLight timerLight = new TimerLight();
            timerLight.DelaySwithOn(1);
            timerLight.DelaySwithOff(2);

            CrazyLight crazyLight = new CrazyLight();
            ((IColoredLight)crazyLight).SetColor(COLOR.RED);
            ((ITimerLight)crazyLight).DelaySwithOn(1);

        }
    }
}
