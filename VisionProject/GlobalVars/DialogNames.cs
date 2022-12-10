﻿using System.Collections.Generic;

namespace VisionProject.GlobalVars
{
    public static class DialogNames
    {
        //Step..
        //app.xaml.cs中相应添加
        public static string ShowAboutWindow = "CA994EAF-4637-4D66-8263-F0862A346662";

        //Step..
        //工具或检测方式的名称，在此添加
        public static Dictionary<string, string> ToolNams = new Dictionary<string, string>()
        {
            { "无",""},
             { "相机操作","7E0B398D-F0E9-415B-8C76-16BF7781FF6D"},
            { "测试","E1B3C54E-93A6-421D-9D8A-F592B13E8B1A"},
            { "脚本测试","96E74602-DDE9-4AF6-9D58-8B90B9BB93F0"},
            { "保存图像","A5E81A1F-EDDA-447C-B196-8ABF2F04B58E"},
        };
    }
}