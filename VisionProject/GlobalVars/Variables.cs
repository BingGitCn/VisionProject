using BingLibrary.Communication.Modbus;
using BingLibrary.Vision;
using BingLibrary.Vision.Engine;
using HalconDotNet;
using Newtonsoft.Json;
using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using VisionProject.ViewModels;

namespace VisionProject.GlobalVars
{
    public static class Variables
    {
        #region 相机相关

        //添加引擎，这里使用数组，方便多相机或多线程使用
        public static List<WorkerEngine> WorkEngines = new List<WorkerEngine>()
        {
            new WorkerEngine(),
            new WorkerEngine()
        };

        //添加相机名字，这里使用数组，方便多相机或多线程使用
        public static List<HCamera> Cameras = new List<HCamera>()
        {
            new HCamera("[0] Integrated Webcam"),
            new HCamera("cam2")
        };

        //图像窗口，需在mainwindow.cs入口指定对应的windowdata
        public static BingImageWindowData WindowData1 = new BingImageWindowData();

        //图像窗口，需在mainwindow.cs入口指定对应的windowdata
        public static BingImageWindowData WindowData2 = new BingImageWindowData();

        public static HImage CurrentImage1 = new HImage();

        //程序编辑弹出窗口对应的windowdata
        public static BingImageWindowData ImageWindowDataForFunction = new BingImageWindowData();

        public static HImage CurrentImageForFunction = new HImage();

        //位置参数  int代表程序的programsIndex List<ConfigSet>代表该程序下的所有位置，不关心顺序
        //脚本代码
        public static string ScriptCode = "";

        public static ScriptEdit scriptEdit = new ScriptEdit();

        #endregion 相机相关

        #region 克隆，深复制

        public static T DeepClone<T>(T data)
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.All;
            jsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            jsonSerializerSettings.TypeNameHandling = TypeNameHandling.All;
            jsonSerializerSettings.Formatting = Formatting.Indented;
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(data, jsonSerializerSettings), jsonSerializerSettings);
        }

        #endregion 克隆，深复制

        #region 事件订阅

        //事件订阅
        //无操造自动回主界面
        public static PubSubEvent AutoHomeEventArgs = new PubSubEvent();

        public static bool IngoreAutoHome = false;

        #endregion 事件订阅

        #region PLC

        //不要在多个异步方法中同时调用一个！可以再申明一个。
        public static ModbusNet HCPLC0 = new ModbusNet();

        public static ModbusNet HCPLC1 = new ModbusNet();
        public static ModbusNet HCPLC2 = new ModbusNet();

        #endregion PLC

        //public static Dictionary<string, ParamSetVar> ParamSetVars = new Dictionary<string, ParamSetVar>();

        //public static ObservableCollection<ParamSetVar> GlobalVariableList = new ObservableCollection<ParamSetVar>();

        //Step..

        //全局Dialog服务
        public static IDialogService CurDialogService;

        //标题
        public static string Title = "";

        //密码
        public static string CurrentPassword = "";

        //路径
        public static string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public static string StatisticDataFilePath = AppDomain.CurrentDomain.BaseDirectory + "Statistics.xlsx";

        //项目
        public static Project CurrentProject = new Project();

        //当前编辑的检测程序
        public static ProgramData CurrentProgramData = new ProgramData();

        //当前编辑的位置参数
        public static ConfigSet CurrentConfigSet = new ConfigSet();

        //当前编辑的程序名字
        public static string ProgramName = "";

        //当前项目的名字
        public static string ProjectName = "";

        public static string ProjectImagesPath = AppDomain.CurrentDomain.BaseDirectory + "Projects\\Images\\";

        //undone 图像长宽需设置
        public static int ImageWidth = 5000;

        public static int ImageHeight = 4000;

        public static string SavePath = "C:\\Users\\PC\\Desktop\\";

        //弹出窗口确认
        public static void ShowMessage(string msg)
        {
            HandyControl.Controls.MessageBox.Show(msg, "消息提示");
        }

        public static bool ShowConfirm(string msg)
        {
            return HandyControl.Controls.MessageBox.Ask(msg, "确认操作") == System.Windows.MessageBoxResult.OK;
        }

        //剩余硬盘容量
        public static string GetFreeSpace(string path)
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                DriveInfo drive = new DriveInfo(directory.Root.Name);
                double freeSpaceMB = drive.AvailableFreeSpace / (1024.0 * 1024);

                string result;
                if (freeSpaceMB < 1024)
                {
                    result = $"{freeSpaceMB:F2}M";
                }
                else
                {
                    double freeSpaceGB = freeSpaceMB / 1024.0;
                    if (freeSpaceGB < 1024)
                    {
                        result = $"{freeSpaceGB:F2}G";
                    }
                    else
                    {
                        double freeSpaceTB = freeSpaceGB / 1024.0;
                        result = $"{freeSpaceTB:F2}T";
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return "N/A"; // 或者返回一个特殊值来表示错误情况
            }
        }

        //容量
        public static double GetFreeSpaceRateValue(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    return 0;

                DriveInfo drive = new DriveInfo(Path.GetPathRoot(path));
                if (!drive.IsReady)
                    return 0;

                double freeSpaceGB = drive.AvailableFreeSpace / (1024.0 * 1024 * 1024);
                return freeSpaceGB;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }

    public class RunResult
    {
        public bool BoolResult { get; set; }
        public HImage ResultImage { get; set; }
        public string NGImagePath { get; set; }
        public string IdentifyResult { get; set; }
    }

    public class ConfigSet
    {
        public string ConfigIndex { get; set; }

        public int RowInput { get; set; }
        public int ColInput { get; set; }
    }
}