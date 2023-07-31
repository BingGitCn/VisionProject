using BingLibrary.Communication.Modbus;
using BingLibrary.Vision;
using BingLibrary.Vision.Engine;
using DLTools.Apply;
using HalconDotNet;
using HandyControl.Controls;
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
        //东尼mes
        public static string MesDetail = "";

        public static MESClass mes = new MESClass();

        #region 深度学习

        public static Inspect PinDeep = new DLTools.Apply.Inspect();

        #endregion 深度学习

        //软件启动登录
        public static bool IsSoftWareRun = false;

        #region 相机相关

        //添加引擎，这里使用数组，方便多相机或多线程使用
        public static List<WorkerEngine> WorkEngines = new List<WorkerEngine>()
        {
            new WorkerEngine(),
            new WorkerEngine(),
            new WorkerEngine(),
            new WorkerEngine(),
        };

        //添加相机名字，这里使用数组，方便多相机或多线程使用
        public static List<HCamera> Cameras = new List<HCamera>()
        {
            new HCamera("Cam1"),
            new HCamera("Cam2"),
            new HCamera("Cam3"),
            new HCamera("Cam4")
        };

        //图像窗口，需在mainwindow.cs入口指定对应的windowdata
        public static BingImageWindowData WindowData1 = new BingImageWindowData();

        //图像窗口，需在mainwindow.cs入口指定对应的windowdata
        public static BingImageWindowData WindowData2 = new BingImageWindowData();

        //图像窗口，需在mainwindow.cs入口指定对应的windowdata
        public static BingImageWindowData WindowData3 = new BingImageWindowData();

        //图像窗口，需在mainwindow.cs入口指定对应的windowdata
        public static BingImageWindowData WindowData4 = new BingImageWindowData();

        public static HImage CurrentImage1 = new HImage();

        //程序编辑弹出窗口对应的windowdata
        public static BingImageWindowData ImageWindowDataForFunction = new BingImageWindowData();

        public static HImage CurrentImageForFunction = new HImage();

        //位置参数  int代表程序的programsIndex List<ConfigSet>代表该程序下的所有位置，不关心顺序
        //脚本代码
        public static string ScriptCode = "";

        public static ScriptEdit scriptEdit = new ScriptEdit();

        public static List<string> Barcode1String = new List<string>();
        public static List<string> Barcode2String = new List<string>();

        #endregion 相机相关

        #region 克隆，深复制

        public static T DeepClone<T>(T data)
        {
            JsonSerializerSettings jsonSerializerSettings1 = new JsonSerializerSettings();
            jsonSerializerSettings1.PreserveReferencesHandling = PreserveReferencesHandling.All;
            jsonSerializerSettings1.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            jsonSerializerSettings1.TypeNameHandling = TypeNameHandling.All;
            jsonSerializerSettings1.Formatting = Formatting.Indented;

            JsonSerializerSettings jsonSerializerSettings2 = new JsonSerializerSettings();
            jsonSerializerSettings2.PreserveReferencesHandling = PreserveReferencesHandling.All;
            jsonSerializerSettings2.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            jsonSerializerSettings2.TypeNameHandling = TypeNameHandling.All;
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(data, jsonSerializerSettings1), jsonSerializerSettings2);
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
        public static ModbusNet HCPLC3 = new ModbusNet();
        public static ModbusNet HCPLC4 = new ModbusNet();

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
        public static ConfigSet CurrentConfigSet = new ConfigSet() { RowInput = 1, ColInput = 1 };

        //当前编辑的程序名字
        public static string ProgramName = "";

        //当前项目的名字
        public static string ProjectName = "";

        public static string ProjectImagesPath = AppDomain.CurrentDomain.BaseDirectory + "Projects\\Images\\";
        public static string ProjectObjectPath = AppDomain.CurrentDomain.BaseDirectory + "Projects\\Objects\\";

        //undone 图像长宽需设置
        public static int ImageWidth = 5472;

        public static int ImageHeight = 3648;

        //最终结果标志
        public static bool Result1OK = false;

        public static bool Result2OK = false;

        //图片地址
        public static string SaveImagePath;

        public static string SaveOriginalImagePath;

        //弹出窗口确认
        public static void ShowMessage(string msg)
        {
            HandyControl.Controls.MessageBox.Show(msg, "消息提示");
        }

        public static bool ShowConfirm(string msg)
        {
            return HandyControl.Controls.MessageBox.Ask(msg, "确认操作") == System.Windows.MessageBoxResult.OK;
        }

        public static void ShowGrowlInfo(string msg)
        {
            Growl.Info(msg);
        }

        public static void ShowGrowlWarning(string msg)
        {
            Growl.Warning(msg);
        }

        public static void ShowGrowlError(string msg)
        {
            Growl.Error(msg);
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
        public bool BoolResult { get; set; }//ok ng
        public HImage ResultImage { get; set; }//图片
        public string NGImagePath { get; set; }
        public string MessageResult { get; set; }

        public HRegion RunRegion { get; set; } = new HRegion();
        public HRegion RegionResult { get; set; } = new HRegion();
    }

    public class InspectFrame
    {
        public bool ContainsObject { get; set; }
        public double Row1 { get; set; } = 0;
        public double Col1 { get; set; } = 0;
        public double Row2 { get; set; } = 0;
        public double Col2 { get; set; } = 0;
        public double CrossRow { get; set; } = 0;
        public double CrossCol { get; set; } = 0;
    }

    public class ConfigSet
    {
        public string ConfigIndex { get; set; }

        public int RowInput { get; set; } = 1;
        public int ColInput { get; set; } = 1;

        public int Row1 { get; set; } = 0;
        public int Column1 { get; set; } = 0;
        public int Row2 { get; set; } = 3648;
        public int Column2 { get; set; } = 5472;

        public int RowChangeValue { get; set; } = 0;
        public int ColChangeValue { get; set; } = 0;

        public int RowMax { get; set; } = 0;
        public int RowMin { get; set; } = 0;

        public int ColMax { get; set; } = 0;
        public int ColMin { get; set; } = 0;
    }
}