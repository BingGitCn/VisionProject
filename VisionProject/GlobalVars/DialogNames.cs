using System.Collections.Generic;

namespace VisionProject.GlobalVars
{
    public static class DialogNames
    {
        //Step..
        //app.xaml.cs中相应添加
        public static string ShowAboutWindow = "CA994EAF-4637-4D66-8263-F0862A346662";

        public static string ShowLoginWindow = "E2CC965E-DB54-4565-B347-CDE24E945DD9";

        //Step..
        //工具或检测方式的名称，在此添加
        public static Dictionary<string, string> ToolNams = new Dictionary<string, string>()
        {
             { "无",""},
             { "相机操作","7E0B398D-F0E9-415B-8C76-16BF7781FF6D"},
             { "脚本工具","96E74602-DDE9-4AF6-9D58-8B90B9BB93F0"},
             { "视觉脚本","67137939-F6C5-4B8E-9862-D1D9AD46E1FD"},
             { "参数设置","5696E502-F7EC-43AE-A39D-F01F8CAD7222"}
        };
    }
}