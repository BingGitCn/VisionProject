using BingLibrary.Extension;
using BingLibrary.Vision;
using HalconDotNet;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionProject.RunTools
{
    public class HalWindowImage
    {
        private HWindow hWindow = new HWindow();

        public HalWindowImage(HImage image)
        {
            HTuple w, h;
            image.GetImageSize(out w, out h);
            //打开一个不可见的窗口
            hWindow.OpenWindow(0, 0, w, h, 0, "buffer", "");
            hWindow.DispImage(image);
        }

        //添加消息显示
        public void AddText(string text, int row, int column, HalconColors halconColor)
        {
            hWindow.DispText(text, "image", row, column, halconColor.ToDescription(), new HTuple(), new HTuple());
        }

        //添加region显示
        public void AddRegion(HRegion region, HalconColors halconColor)
        {
            hWindow.SetColor(halconColor.ToDescription());
            hWindow.SetDraw("margin");
            hWindow.DispObj(region);
        }

        //获取窗口截图
        public HImage GetWindowImage()
        {
            return hWindow.DumpWindowImage();
        }

        //关闭窗口，释放资源
        public void Close()
        {
            hWindow?.CloseWindow();
            hWindow.Dispose();
        }
    }
}