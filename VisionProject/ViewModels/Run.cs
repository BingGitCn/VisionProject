using BingLibrary.Extension;
using BingLibrary.Tools;
using Prism.Commands;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisionProject.GlobalVars;
using Log = BingLibrary.Logs.LogOpreate;

namespace VisionProject.ViewModels
{
    /*运行*/

    public partial class MainWindowViewModel
    {
        #region 初始化

        private void initAll()
        {
            //获取数据统计
            initStatistic();
            //获取系统配置
            getSystemConfig();
            //获取mes配置
            getMESConfig();
            //获取剩余内存
            initFreeSpace();
            //软件名字
            Variables.Title = SystemConfig.Title;
            Title = Variables.Title;

            //测试
            //IsLogin = true;

            //初始化项目
            initProjects();
            //初始化PLC
            initPLC();
            //初始化引擎，可选
            initEngine();

            run();

            Variables.AutoHomeEventArgs.Subscribe(autoHome);
        }

        private void autoHome()
        {
            UserIndex = 0;
            CurrentPermit = PermitLevel.Nobody;
        }

        private async void initCameras(Dictionary<string, object> ps)
        {
            await Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i < Variables.Cameras.Count; i++)
                    {
                        int CameraTypeIndex = ps.BingGetOrAdd(i + ".CameraTypeIndex", 0).ToString().BingToInt();
                        double ExpouseTime = ps.BingGetOrAdd(i + ".ExpouseTime", 200).ToString().BingToDouble();
                        Variables.Cameras[i].CameraType = CameraTypeIndex;
                        Variables.Cameras[i].ExpouseTime = ExpouseTime;
                        Variables.Cameras[i].OpenCamera();
                    }
                }
                catch { }
            });
        }

        private void initEngine()
        {
            try
            {
                //这里自动获取目录下的脚本，添加至引擎
                for (int i = 0; i < Variables.WorkEngines.Count; i++)
                {
                    var folder = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory + "Projects\\Scripts" + i);
                    var procedureFiles = folder.GetFiles("*.hdvp");
                    foreach (var procedureFile in procedureFiles)
                        Variables.WorkEngines[i].AddProcedure(procedureFile.Name.Replace(".hdvp", ""), System.AppDomain.CurrentDomain.BaseDirectory + "Projects\\Scripts" + i);
                }
            }
            catch { }
        }

        public class Point
        {
            public double X { set; get; } = 0;
            public double Y { set; get; } = 0;
        }

        public class Da
        {
            public int ID { set; get; } = 10;
            public string Value { set; get; } = "Leader";

            //public List<Point> Points { set; get; } = new List<Point>() {
            //new Point(){ X=10.0,Y=20.0},
            //};
        }

        private async void initFreeSpace()
        {
            while (true)
            {
                await 5000;

                try
                {
                    FreeSpace = (SystemConfig.HardDrive == 0 ? "C:\\"
                           : SystemConfig.HardDrive == 1 ? "D:\\"
                            : SystemConfig.HardDrive == 2 ? "E:\\"
                             : SystemConfig.HardDrive == 3 ? "F:\\"
                              : SystemConfig.HardDrive == 4 ? "G:\\"
                           : "C:\\") + "可用容量：" +
                           Variables.GetFreeSpace(SystemConfig.HardDrive == 0 ? "C:\\"
                           : SystemConfig.HardDrive == 1 ? "D:\\"
                            : SystemConfig.HardDrive == 2 ? "E:\\"
                             : SystemConfig.HardDrive == 3 ? "F:\\"
                              : SystemConfig.HardDrive == 4 ? "G:\\"
                           : "C:\\");

                    double freeSpace = Variables.GetFreeSpaceRateValue(SystemConfig.HardDrive == 0 ? "C:\\"
                        : SystemConfig.HardDrive == 1 ? "D:\\"
                         : SystemConfig.HardDrive == 2 ? "E:\\"
                          : SystemConfig.HardDrive == 3 ? "F:\\"
                           : SystemConfig.HardDrive == 4 ? "G:\\"
                        : "C:\\");

                    HardWareStatus.Value = freeSpace >= SystemConfig.FreeSpace ? true : false;
                }
                catch { }
            }
        }

        //PLC初始化
        private async void initPLC()
        {
            //初始化PLC

            await 1000;
            for (int i = 0; i < 10; i++)
            {
                if (Variables.HCPLC.IsConnected)
                    break;
                Variables.HCPLC.Close();
                await Task.Run(() =>
                {
                    Variables.HCPLC.Init("127.0.0.1", 502, 01);
                });

                await 1000;
            }

            Log.Info(Variables.HCPLC.IsConnected ? "PLC连接成功" : "PLC连接失败");
            PLCStatus.Value = Variables.HCPLC.IsConnected;
        }

        #endregion 初始化

        //运行
        //如果有多个产品，且一个产品需分多次检测，可以设置产品的索引号，同一索引号依次处理。
        private async void run()
        {
            //Step..
            //获取工具名称
            List<string> ToolNames = DialogNames.ToolNams.Keys.ToList();
            while (true)
            {
                await 100;
                if (false)
                {
                    //这里需要判断下程序数量
                    if (Variables.CurrentProject.Programs.Count > 0)
                        continue;

                    //选择子程序。这里选择第一个0，
                    var program = Variables.CurrentProject.Programs.ElementAt(0).Value;
                    //选择对应产品的所有检测子程序。这里可以在打开项目的时候获取一次，无需每次执行获取。
                    int index = 1;//产品位置索引，这里通过索引来选择，也可以通过其它的，如ToolNames名称等。
                    var currentProduct = (from step in program
                                          where step.ProductIndex == index
                                          select step)
                                          .ToList();

                    for (int i = 0; i < currentProduct.Count - 1; i++)
                    {
                        //这里通过备注来判断执行对应脚本
                        // if (currentProduct[i].Content == "扫码")

                        //if (currentProduct[i].InspectFunction == ToolNames[0])
                        //{
                        //    //这里可以再对应的工具里面写好对应的静态执行方法，直接调用
                        //    //Do Something
                        //    //Function_SaveImageViewModel.SaveImages(new HalconDotNet.HImage(),"123", currentProduct[i].Parameters);
                        //}
                        //else if (currentProduct[i].InspectFunction == ToolNames[1])
                        //{
                        //    //Do Something
                        //}
                    }

                    //结果OK时使用
                    setOK();
                    //结果NG时使用
                    setNG();
                }
            }
        }
    }
}