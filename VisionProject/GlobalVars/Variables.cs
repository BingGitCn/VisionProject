using BingLibrary.Controls.Log;
using BingLibrary.Vision;
using System;
using VisionProject.ViewModels;
using BingLibrary.Communication.PLC;
namespace VisionProject.GlobalVars
{
    public static class Variables
    {
        //PLC
        public static HuiChuanPLC HCPLC = new HuiChuanPLC();

        //标题
        public static string Title = "";
        //密码
        public static string CurrentPassword="";
        //路径
        public static string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public static string StatisticDataFilePath = AppDomain.CurrentDomain.BaseDirectory + "Statistics.xlsx";
        //日志
        public static LogDateTime Logs = new LogDateTime(AppDomain.CurrentDomain.BaseDirectory + "Logs");

        //项目
        public static Project CurrentProject = new Project();

        

        //图像窗口，需在mainwindow.cs入口指定对应的windowdata
        public static BingImageWindowData WindowData1 = new BingImageWindowData();

        //弹出窗口确认
        public static void ShowMessage(string msg)
        {
            HandyControl.Controls.MessageBox.Show(msg,"消息提示");
        }

        public static bool ShowConfirm(string msg)
        {
            if (HandyControl.Controls.MessageBox.Ask(msg,"确认操作") == System.Windows.MessageBoxResult.OK)
                return true;
            else return false;
        }
    }
}