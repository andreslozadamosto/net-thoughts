using System;
using System.Collections.Generic;
using System.Text;

namespace DefaultInterfaceMethods.Mixins
{
    public interface IColoredLight : ILight
    {
        public enum COLOR
        {
            RED,
            YELLOW,
            WHITE,
            BLUE
        }

        protected COLOR SelectedColor { get; set; }

        public void SetColor(COLOR value) => SelectedColor = value;
    }
}
