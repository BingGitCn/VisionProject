using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingLibrary.Vision
{
    public class MessageBase
    {
        public double PositionX { get; set; } = 0;

        public double PositionY { get; set; } = 0;

        public HalconColors ShowColor { get; set; } = HalconColors.绿色;

        public string ShowContent { get; set; } = string.Empty;

        public int ShowFontSize { set; get; }

        public HalconCoordinateSystem ShowMode { set; get; }

        public MessageBase(double posX, double posY, string text, int fontsize = 12, HalconColors color = HalconColors.绿色, HalconCoordinateSystem mode = HalconCoordinateSystem.window)
        {
            PositionX = posX;
            PositionY = posY;
            ShowColor = color;
            ShowContent = text;
            ShowFontSize = fontsize;
            ShowMode = mode;
        }
    }
}