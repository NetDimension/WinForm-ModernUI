using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDimension.WinForm
{
    public static class ColorExtensions
    {
        public static Color FromHSB(this Color _, float hue, float saturation, float brightness)
        {
            float r = 0;
            float g = 0;
            float b = 0;

            if (saturation == 0)
            {
                r = g = b = brightness;
            }
            else
            {
                // the color wheel consists of 6 sectors. Figure out which sector you're in.
                float sectorPos = hue / 60.0f;
                int sectorNumber = (int)(Math.Floor(sectorPos));
                // get the fractional part of the sector
                float fractionalSector = sectorPos - sectorNumber;

                // calculate values for the three axes of the color. 
                float p = brightness * (1.0f - saturation);
                float q = brightness * (1.0f - (saturation * fractionalSector));
                float t = brightness * (1.0f - (saturation * (1 - fractionalSector)));

                // assign the fractional colors to r, g, and b based on the sector the angle is in.
                switch (sectorNumber)
                {
                    case 0:
                        r = brightness;
                        g = t;
                        b = p;
                        break;
                    case 1:
                        r = q;
                        g = brightness;
                        b = p;
                        break;
                    case 2:
                        r = p;
                        g = brightness;
                        b = t;
                        break;
                    case 3:
                        r = p;
                        g = q;
                        b = brightness;
                        break;
                    case 4:
                        r = t;
                        g = p;
                        b = brightness;
                        break;
                    case 5:
                        r = brightness;
                        g = p;
                        b = q;
                        break;
                }
            }
            return Color.FromArgb(Convert.ToByte(255), Convert.ToByte(r * 255), Convert.ToByte(g * 255), Convert.ToByte(b * 255));
        }
    }
}
