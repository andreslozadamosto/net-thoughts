using System;
using System.Collections.Generic;
using System.Text;
using static DefaultInterfaceMethods.Mixins.IColoredLight;

namespace DefaultInterfaceMethods.Mixins
{
    public class CrazyLight : BaseLight, ITimerLight, IColoredLight
    {
        COLOR IColoredLight.SelectedColor { get; set; } = COLOR.WHITE;
    }
}
