using BingLibrary.Extension;
using System.Collections.Generic;
using System.ComponentModel;

namespace VisionProject.GlobalVars
{
    public static class DialogNames
    {
        //Step..
        //app.xaml.cs中相应添加
        public static string ShowAboutWindow = "CA994EAF-4637-4D66-8263-F0862A346662";

        public static string ShowLoginWindow = "E2CC965E-DB54-4565-B347-CDE24E945DD9";

        public static string ShowParamSetDialog = "5696E502-F7EC-43AE-A39D-F01F8CAD7222";

        public static string ShowInspectCreateDialog = "749B6611-E3F4-47B9-BAFD-44CDD8DA9E05";

        public static string ShowLocationDialog = "9255539F-2D8E-44F2-90CC-E697E969A3A8";

        //Step..
        //工具或检测方式的名称，在此添加
        public static Dictionary<string, string> ToolNams = new Dictionary<string, string>()
        {
            // { "无",""},
           //  { "相机操作","7E0B398D-F0E9-415B-8C76-16BF7781FF6D"},
            // { "脚本工具","96E74602-DDE9-4AF6-9D58-8B90B9BB93F0"},

              { Functions.图像比对.ToDescription(),"8EBAA376-988A-4CDF-AD40-3FC9DE3B369C"},
              { Functions.Blob分析.ToDescription(),"99462AEB-2C55-4442-B12E-66D0B4B5A737"},
              { Functions.条码识别.ToDescription(),"F3D93CEF-E997-4AB6-93A9-F33167414DFA"},
              { Functions.视觉脚本.ToDescription(),"67137939-F6C5-4B8E-9862-D1D9AD46E1FD"},
        };
    }

    public enum Functions
    {
        [Description("图像比对")] 图像比对,
        [Description("Blob分析")] Blob分析,
        [Description("条码识别")] 条码识别,
        [Description("视觉脚本")] 视觉脚本
    }
}