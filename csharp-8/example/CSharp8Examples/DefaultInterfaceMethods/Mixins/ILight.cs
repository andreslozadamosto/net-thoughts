using System;
using System.Collections.Generic;
using System.Text;

namespace DefaultInterfaceMethods.Mixins
{
    public interface ILight
    {
        protected enum STATUS
        {
            ON,
            OFF
        }

        protected STATUS Status { get; set; }

        public bool IsOn() => Status == STATUS.ON;

        public bool isOff() => Status == STATUS.OFF;

        public void SwitchOff() => Status = STATUS.OFF;

        public void SwitchOn() => Status = STATUS.ON;
    }
}
