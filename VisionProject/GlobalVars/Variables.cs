using BingLibrary.Controls.Log;
using BingLibrary.Vision;
using System;
using VisionProject.ViewModels;

namespace VisionProject.GlobalVars
{
    public static class Variables
    {
        //标题
        public static string Title = "";
        public static string CurrentPassword="";

        //路径
        public static string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public static string StatisticDataFilePath = AppDomain.CurrentDomain.BaseDirectory + "Statistics.xlsx";

        //项目
        public static Project CurrentProject = new Project();

        //日志
        public static LogDateTime Logs = new LogDateTime(AppDomain.CurrentDomain.BaseDirectory + "Logs");

        //图像窗口，需在mainwindow.cs入口指定对应的windowdata
        public static BingImageWindowData WindowData1 = new BingImageWindowData();

        //弹出窗口确认
        public static void ShowMessage(string msg)
        {
            HandyControl.Controls.MessageBox.Show(msg);
        }

        public static bool ShowConfirm(string msg)
        {
            if (HandyControl.Controls.MessageBox.Ask(msg) == System.Windows.MessageBoxResult.OK)
                return true;
            else return false;
        }
    }
}