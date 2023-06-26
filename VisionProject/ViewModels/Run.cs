using BingLibrary.Extension;
using BingLibrary.Tools;
using HalconDotNet;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisionProject.GlobalVars;
using VisionProject.RunTools;
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

            //显示每次拍照的拼图
            showImage();

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

            await Task.Run(() =>
            {
                Variables.HCPLC0.Init("127.0.0.1", 502, 0);
                Variables.HCPLC1.Init("127.0.0.1", 502, 0);
                Variables.HCPLC2.Init("127.0.0.1", 502, 0);
            });
            PLCStatus.Value = Variables.HCPLC0.IsConnected && Variables.HCPLC1.IsConnected && Variables.HCPLC2.IsConnected;

            Log.Info(PLCStatus.Value ? "PLC连接成功" : "PLC连接失败");
        }

        #endregion 初始化

        private HImage Cam1Image = new HImage();
        private HImage Cam2Image = new HImage();

        private List<RunResult> run1Results = new List<RunResult>();
        private List<RunResult> run2Results = new List<RunResult>();

        private bool grab1Done = false;
        private bool grab2Done = false;

        private bool run1Done = false;
        private bool run2Done = false;

        // 1 是左右拍同一产品，2 是左右拍不同产品
        private int CameraMode = 1;

        private async void run()
        {
            int step1 = 0;
            int step2 = 0;

            int position1 = 0;
            int position2 = 0;

            bool run1Last = false;
            bool run2Last = false;

            while (true)
            {
                await Task.Delay(10);
                if (!PLCStatus.Value) continue;

                // 握手信号
                var heartBeat = Variables.HCPLC0.ReadM(2500, 1)[0];
                if (heartBeat)
                {
                    Variables.HCPLC0.WriteM(2500, new bool[] { false });
                    Variables.HCPLC0.WriteM(2550, new bool[] { false });
                }

                switch (step1)
                {
                    case 0:
                        //触发拍照，左相机
                        bool grabTrigger1 = Variables.HCPLC0.ReadM(2501, 1)[0];
                        if (grabTrigger1)
                        {
                            Variables.HCPLC0.WriteM(2501, new bool[] { false });
                            grab1();
                            step1 = 2;
                        }
                        break;

                    case 2:
                        //拍照完成，左相机
                        if (grab1Done)
                        {
                            grab1Done = false;
                            position1 = Variables.HCPLC0.ReadD(356, 1)[0];

                            //undone 默认第一次拍照是扫码，同时生成大图
                            if (position1 == 0)
                            {
                                // 1 是左右拍同一产品，2 是左右拍不同产品
                                CameraMode = Variables.HCPLC0.ReadD(352, 1)[0];
                                initTotalImage(CameraMode);
                            }

                            await imageQueues.Enqueue(new ImageQueue() { CameraIndex = 1, PositionIndex = position1, Image = Cam1Image.CopyImage() }, imageQueues.tokenNone);

                            run1(position1, Cam1Image.CopyImage(), Variables.CurrentProject.Programs.ElementAt(0).Value[position1]);
                            Variables.HCPLC0.WriteM(2551, new bool[] { false });//拍照完成信号，同时去处理
                            step1 = 3;
                        }

                        break;

                    case 3:
                        if (run1Done)
                        {
                            run1Done = false;

                            //todo 根据结果做对应处理
                            if (CameraMode == 1 || CameraMode == 2)
                            {
                                // undone 这里需要注意，约定1，空穴检测，2. 扫码，ng方式不一样
                                if (position1 == 0)
                                {
                                    //空穴
                                    if (!run1Results[0].BoolResult)
                                    {
                                        Variables.HCPLC0.WriteM(2568, new bool[] { true });
                                    }
                                    if (run1Results[1].MessageResult.Length < 15)
                                    {
                                        //扫码失败
                                        Variables.HCPLC0.WriteM(2557, new bool[] { true });
                                    }
                                    else
                                    {
                                        //扫码成功
                                        Variables.HCPLC0.WriteM(2556, new bool[] { true });
                                    }

                                    // todo mse处理，如果mes启用的话
                                }
                                else
                                {
                                    foreach (var rst in run1Results)
                                    {
                                        //todo 处理，相关信息可能需要存储到表格
                                    }
                                }
                            }

                            //全部拍完成信号
                            bool grabDone1 = Variables.HCPLC0.ReadM(2511, 1)[0];
                            if (grabDone1)
                            {
                                Variables.HCPLC0.WriteM(2511, new bool[] { false });//最后一次拍照标志位
                                Variables.HCPLC0.WriteM(2553, new bool[] { true });//处理完成信号
                                run1Last = true;
                                step1 = 4;
                            }
                            else
                            {
                                step1 = 0;
                                Variables.HCPLC0.WriteM(2553, new bool[] { true });//处理完成信号
                                //todo 当前位置处理失败
                            }
                        }

                        break;

                    case 4:
                        if (run1Last && run2Last)
                        {
                            //全部处理完成
                        }

                        break;
                }
            }
        }

        //用于拼图，队列
        private class ImageQueue
        {
            public int CameraIndex { set; get; }
            public int PositionIndex { set; get; }
            public HImage Image { set; get; }
        }

        private AsyncQueue<ImageQueue> imageQueues = new AsyncQueue<ImageQueue>();

        private HImage TotalImageLeft = new HImage();
        private HImage TotalImageRight = new HImage();

        /// <summary>
        /// 生成大图
        /// </summary>
        private void initTotalImage(int mode)
        {
            int rows = 0, cols = 0;
            HImage image = new HImage();
            if (mode == 1)
            {
                HImage image1 = new HImage(); HImage image2 = new HImage(); HImage image3 = new HImage();

                for (int i = 0; i < 2; i++)
                {
                    var p = Variables.CurrentProject.Programs.ElementAt(i).Value;
                    for (int j = 0; j < p.Count; j++)
                    {
                        if (rows < p.ElementAt(j).ProductConfigSet.RowInput)
                            rows = p.ElementAt(j).ProductConfigSet.RowInput;
                        if (cols < p.ElementAt(j).ProductConfigSet.ColInput)
                            cols = p.ElementAt(j).ProductConfigSet.ColInput;
                    }
                }

                image.GenImageConst("byte", Variables.ImageWidth * cols, Variables.ImageHeight * rows);

                image1 = image.GenImageProto(new HTuple(128));
                image2 = image.GenImageProto(new HTuple(128));
                image3 = image.GenImageProto(new HTuple(128));
                TotalImageLeft = image1.Compose3(image2, image3);
            }
            else if (mode == 2)
            {
                HImage image1 = new HImage(); HImage image2 = new HImage(); HImage image3 = new HImage();
                //左
                var p = Variables.CurrentProject.Programs.ElementAt(0).Value;
                for (int j = 0; j < p.Count; j++)
                {
                    if (rows < p.ElementAt(j).ProductConfigSet.RowInput)
                        rows = p.ElementAt(j).ProductConfigSet.RowInput;
                    if (cols < p.ElementAt(j).ProductConfigSet.ColInput)
                        cols = p.ElementAt(j).ProductConfigSet.ColInput;
                }

                image.GenImageConst("byte", Variables.ImageWidth * cols, Variables.ImageHeight * rows);

                image1 = image.GenImageProto(new HTuple(128));
                image2 = image.GenImageProto(new HTuple(128));
                image3 = image.GenImageProto(new HTuple(128));
                TotalImageLeft = image1.Compose3(image2, image3);

                //右
                p = Variables.CurrentProject.Programs.ElementAt(1).Value;
                for (int j = 0; j < p.Count; j++)
                {
                    if (rows < p.ElementAt(j).ProductConfigSet.RowInput)
                        rows = p.ElementAt(j).ProductConfigSet.RowInput;
                    if (cols < p.ElementAt(j).ProductConfigSet.ColInput)
                        cols = p.ElementAt(j).ProductConfigSet.ColInput;
                }

                image.GenImageConst("byte", Variables.ImageWidth * cols, Variables.ImageHeight * rows);

                image1 = image.GenImageProto(new HTuple(128));
                image2 = image.GenImageProto(new HTuple(128));
                image3 = image.GenImageProto(new HTuple(128));
                TotalImageRight = image1.Compose3(image2, image3);
            }
        }

        //拼图和显示
        private async void showImage()
        {
            HRegion rect = new HRegion(new HTuple(0), 0, Variables.ImageWidth - 1, Variables.ImageHeight - 1);
            HTuple rows = new HTuple(), columns = new HTuple();
            rect.GetRegionPoints(out rows, out columns);

            while (true)
            {
                await Task.Delay(10);

                if (imageQueues.Count() > 0)
                {
                    var imageQueue = await imageQueues.Dequeue(imageQueues.tokenNone);
                    int rowCount = 0; int columnCount = 0;

                    if (CameraMode == 1)
                    {
                        var grayVals = imageQueue.Image.GetGrayval(rows, columns);
                        if (imageQueue.CameraIndex == 1)
                        {
                            rowCount = Variables.CurrentProject.Programs.ElementAt(0).Value[imageQueue.PositionIndex].ProductConfigSet.RowInput;
                            columnCount = Variables.CurrentProject.Programs.ElementAt(0).Value[imageQueue.PositionIndex].ProductConfigSet.ColInput;
                        }
                        else if (imageQueue.CameraIndex == 2)
                        {
                            rowCount = Variables.CurrentProject.Programs.ElementAt(1).Value[imageQueue.PositionIndex].ProductConfigSet.RowInput;
                            columnCount = Variables.CurrentProject.Programs.ElementAt(1).Value[imageQueue.PositionIndex].ProductConfigSet.ColInput;
                        }

                        TotalImageLeft.SetGrayval(new HTuple(Variables.ImageHeight * (rowCount - 1)) + rows, new HTuple(Variables.ImageWidth * (columnCount - 1)) + columns, grayVals);

                        Variables.WindowData1.WindowCtrl.ShowImageToWindow(TotalImageLeft.CopyImage());
                        Variables.WindowData2.WindowCtrl.ShowImageToWindow(TotalImageLeft.CopyImage());
                    }
                    else if (CameraMode == 2)
                    {
                        var grayVals = imageQueue.Image.GetGrayval(rows, columns);
                        if (imageQueue.CameraIndex == 1)
                        {
                            rowCount = Variables.CurrentProject.Programs.ElementAt(0).Value[imageQueue.PositionIndex].ProductConfigSet.RowInput;
                            columnCount = Variables.CurrentProject.Programs.ElementAt(0).Value[imageQueue.PositionIndex].ProductConfigSet.ColInput;
                            TotalImageLeft.SetGrayval(new HTuple(Variables.ImageHeight * (rowCount - 1)) + rows, new HTuple(Variables.ImageWidth * (columnCount - 1)) + columns, grayVals);
                            Variables.WindowData1.WindowCtrl.ShowImageToWindow(TotalImageLeft.CopyImage());
                        }
                        else if (imageQueue.CameraIndex == 2)
                        {
                            rowCount = Variables.CurrentProject.Programs.ElementAt(1).Value[imageQueue.PositionIndex].ProductConfigSet.RowInput;
                            columnCount = Variables.CurrentProject.Programs.ElementAt(1).Value[imageQueue.PositionIndex].ProductConfigSet.ColInput;
                            TotalImageRight.SetGrayval(new HTuple(Variables.ImageHeight * (rowCount - 1)) + rows, new HTuple(Variables.ImageWidth * (columnCount - 1)) + columns, grayVals);
                            Variables.WindowData2.WindowCtrl.ShowImageToWindow(TotalImageRight.CopyImage());
                        }
                    }
                    imageQueue.Image?.Dispose();
                }
            }
        }

        private async void grab1()
        {
            grab1Done = false;
            await Task.Run(() =>
            {
                Cam1Image = Variables.Cameras[0].GrabOne();
            });
            grab1Done = true;
        }

        private async void run1(int position, HImage image, SubProgram sp)
        {
            run1Done = false;
            run1Results.Clear();
            if (Variables.CurrentProject.Programs.Count < 2)
                return;
            if (position != sp.ProductIndex)
                Log.Warn(string.Format("【run1】检测位置不对应：{0} & {1}", position, sp.ProductIndex));

            await Task.Run(() =>
            {
                foreach (var pd in sp.ProgramDatas)
                {
                    if (pd.InspectFunction == Functions.图像比对.ToDescription())
                    {
                        run1Results.Add(
                                   Function_MatchTool.Run(image, pd)
                                   );
                    }
                    else if (pd.InspectFunction == Functions.Blob分析.ToDescription())
                    {
                        run1Results.Add(
                                 Function_BlobTool.Run(image, pd)
                                 );
                    }
                    else if (pd.InspectFunction == Functions.条码识别.ToDescription())
                    {
                        run1Results.Add(
                                Function_CodeTool.Run(image, pd)
                               );
                    }
                }
            });
            run1Done = true;
        }
    }
}