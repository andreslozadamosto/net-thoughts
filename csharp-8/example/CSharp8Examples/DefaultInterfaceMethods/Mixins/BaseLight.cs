using System;
using System.Collections.Generic;
using System.Text;
using static DefaultInterfaceMethods.Mixins.ILight;

namespace DefaultInterfaceMethods.Mixins
{
    public class BaseLight : ILight
    {
        STATUS ILight.Status { get; set; } = STATUS.OFF;
    }
}
