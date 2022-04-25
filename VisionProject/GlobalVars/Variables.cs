using BingLibrary.Communication.PLC;
using BingLibrary.Logs;
using BingLibrary.Vision;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using VisionProject.ViewModels;
using BingLibrary.Vision.Engine;
using Prism.Services.Dialogs;

namespace VisionProject.GlobalVars
{
    public static class Variables
    {
        //全局Dialog服务
        public static IDialogService CurDialogService;

        //脚本代码
        public static string ScriptCode = "";
       public static ScriptEdit scriptEdit = new ScriptEdit();
      


        //V2 引擎
        public static VisionEngine2 V2Engine=new VisionEngine2();

        //PLC
        public static HuiChuanPLC HCPLC = new HuiChuanPLC();

        //标题
        public static string Title = "";

        //密码
        public static string CurrentPassword = "";

        //路径
        public static string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public static string StatisticDataFilePath = AppDomain.CurrentDomain.BaseDirectory + "Statistics.xlsx";

        
        //项目
        public static Project CurrentProject = new Project();

        //当前编辑的程序
        public static SubProgram CurrentSubProgram = new SubProgram();  

        //当前编辑的程序名字
        public static string ProgramName = "";
        /// <summary>
        /// 获取输入参数
        /// </summary>
        /// <param name="programName">程序名字</param>
        /// <param name="index">获取到第几步</param>
        /// <returns></returns>
        public static Dictionary<string, object> GetInputParams(string programName)
        {
            Dictionary<string, object> inputParams = new Dictionary<string, object>();
            try
            {
                var p = Variables.CurrentProject.Programs[programName];
            int   index = Variables.CurrentSubProgram.Index;

                if (index > p.Count)
                    index = p.Count;
                for (int i = 0; i < index; i++)
                {
                    var d = Variables.CurrentProject.Programs[programName][i].Parameters;
                    for (int j = 0; j < d.Keys.Count; j++)
                    {
                        inputParams.Add(programName + ":" + i + "." + p[i].InspectFunction + ":" + d.ElementAt(j).Key, d.ElementAt(j).Value); ;
                    }
                }

            }
            catch { }

            return inputParams;

        }


      





        //图像窗口，需在mainwindow.cs入口指定对应的windowdata
        public static BingImageWindowData WindowData1 = new BingImageWindowData();

        public static HImage CurrentImage1 = new HImage();

        //程序编辑弹出窗口对应的windowdata
        public static BingImageWindowData ImageWindowDataForFunction = new BingImageWindowData();

        public static HImage CurrentImageForFunction = new HImage();

        //弹出窗口确认
        public static void ShowMessage(string msg)
        {
            HandyControl.Controls.MessageBox.Show(msg, "消息提示");
        }

        public static bool ShowConfirm(string msg)
        {
            if (HandyControl.Controls.MessageBox.Ask(msg, "确认操作") == System.Windows.MessageBoxResult.OK)
                return true;
            else return false;
        }

        //剩余硬盘容量
        public static string GetFreeSpace(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            DriveInfo savedFolderDrive = new DriveInfo(directory.Root.Name);
            double rst = savedFolderDrive.AvailableFreeSpace / 1024 / 1024;
            if (rst < 1024)
                return rst + "M";
            else
            {
                rst = rst / 1024.0;
                if (rst < 1024)
                    return rst.ToString("f1") + "G";
                else
                {
                    rst = rst / 1024.0;
                    return rst.ToString("f1") + "T";
                }
            }


        }

     
        //容量
        public static double GetFreeSpaceRateValue(string path) 
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            DriveInfo savedFolderDrive = new DriveInfo(directory.Root.Name);
            double rst = savedFolderDrive.AvailableFreeSpace / 1024 / 1024/1024;
            return rst;
               
         

        }
    }
}