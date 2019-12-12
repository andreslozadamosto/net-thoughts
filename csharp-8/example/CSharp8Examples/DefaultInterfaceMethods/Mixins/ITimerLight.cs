using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DefaultInterfaceMethods.Mixins
{
    interface ITimerLight : ILight
    {
        public async void DelaySwithOff(int duration)
        {
            await Task.Delay(duration);
            SwitchOff();
        }

        public async void DelaySwithOn(int duration)
        {
            await Task.Delay(duration);
            SwitchOn();
        }
    }
}
