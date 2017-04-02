using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YeelightControl.Models
{
    public class Color
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public int ToInt()
        {
            int value = 0;
            value |= R << 16;
            value |= G << 8;
            value |= B;
            return value;
        }

        public static Color FromInt(int value)
        {
            var color = new Color();
            color.R = (value & 0xFF0000) >> 16;
            color.G = (value & 0xFF00) >> 8;
            color.B = value & 0xFF;
            return color;
        }
    }
}
