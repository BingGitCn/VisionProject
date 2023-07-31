using BingLibrary.Extension;
using BingLibrary.Vision;
using HalconDotNet;

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
            hWindow.ClearWindow();
            hWindow.SetPart(new HTuple(0), new HTuple(0), h, w);
            hWindow.DispObj(image);
        }

        //添加消息显示
        public void AddText(string text, int row, int column, HalconColors halconColor, int fontSize = 16)
        {
            hWindow.SetFont("default-Normal-" + fontSize.ToString());
            hWindow.DispText(text, "image", row, column, halconColor.ToDescription(), "box_color", "#ffffff77");
        }

        //添加region显示
        public void AddRegion(HRegion region, HalconColors halconColor)
        {
            hWindow.SetColor(halconColor.ToDescription());
            hWindow.SetLineWidth(15);
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