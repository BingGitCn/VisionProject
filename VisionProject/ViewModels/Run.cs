using BingLibrary.Communication;
using BingLibrary.Extension;
using BingLibrary.Tools;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using VisionProject.GlobalVars;
using VisionProject.RunTools;
using Log = BingLibrary.Logs.LogOpreate;
using BingLibrary.FileOpreate;
using System.Collections.ObjectModel;

namespace VisionProject.ViewModels
{
    /*运行*/

    // 230704之前的run方法我从项目中排除了，但没有删除
    public partial class MainWindowViewModel
    {
        #region 初始化

        private async void initAll()
        {
            while (!Variables.IsSoftWareRun)
                await Task.Delay(1000);
            try
            {
                if (!Directory.Exists(Variables.ProjectImagesPath))
                    Directory.CreateDirectory(Variables.ProjectImagesPath);
                if (!Directory.Exists(Variables.ProjectObjectPath))
                    Directory.CreateDirectory(Variables.ProjectObjectPath);
            }
            catch { }

            HOperatorSet.SetSystem(new HTuple("clip_region"), new HTuple("false"));
            //获取数据统计
            initStatistic();
            runStatistic();
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
            //初始cam
            initCameras();
            //初始化项目
            initProjects();
            //初始化PLC
            initPLC();
            //初始化引擎，可选
            initEngine();

            //显示每次拍照的拼图
            showImage();
            runA(); //相机1，左相机
            runB(); //相机2，右相机
            runC(); //相机3
            runD(); //相机4

            initDeep();
            Variables.AutoHomeEventArgs.Subscribe(autoHome);

            initMes();
        }

        private async void initDeep()
        {
            await Task.Delay(100);
            await Task.Run(() =>
            {
                var rst = Variables.PinDeep.LoadModel(AppDomain.CurrentDomain.BaseDirectory + "PublicModels\\model_pin.hdl", 2, false);
                Log.Info("Pin模型初始化：" + (rst.IsActionOK ? "成功." : "失败."));
            });
        }

        //汇总mes需要上传的数据
        private string mesResultStr1 = "";

        private string mesResultStr2 = "";

        /// <summary>
        /// 启动时判断mes是否连接成功
        /// </summary>
        public async void initMes()
        {
            while (true)
            {
                await Task.Delay(3000);
                if (SystemConfig.UseMES)
                {
                    if (!MesStatus.Value)
                    {
                        Variables.mes.setMESConfig(MesDatas[0].Value, MesDatas[1].Value, MesDatas[2].Value, MesDatas[3].Value);
                        bool isConnect = Variables.mes.isConnect();
                        Log.Info("Mes初始化：" + Variables.mes.GetHttpUrl() + ",服务器链接" + (isConnect ? "成功." : "失败."));
                        MesStatus.Value = isConnect;
                    }
                }
                else
                {
                    MesStatus.Value = false;
                }
            }
        }

        /// <summary>
        /// 一定时间后自动返回主页面
        /// </summary>
        private void autoHome()
        {
            UserIndex = 0;
            CurrentPermit = PermitLevel.Nobody;
        }

        /// <summary>
        /// 打开相机，注意曝光时间
        /// </summary>
        private async void initCameras()
        {
            await Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i < Variables.Cameras.Count; i++)
                    {
                        Variables.Cameras[i].CameraType = 0;

                        if (i == 0 || i == 1)
                            Variables.Cameras[i].ExpouseTime = 8000;
                        if (i == 2 || i == 3)
                            Variables.Cameras[i].ExpouseTime = 10000;
                        Variables.Cameras[i].OpenCamera();
                        Log.Info(Variables.Cameras[i].IsOpened ? "相机" + (i + 1) + "初始化成功" : "相机" + (i + 1) + "初始化失败");
                        if (!Variables.Cameras[i].IsOpened)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                System.Threading.Thread.Sleep(3000);
                                Log.Warn("相机" + (i + 1) + "正在重新初始化");
                                Variables.Cameras[i].CloseCamera();
                                Variables.Cameras[i].OpenCamera();
                                Log.Info(Variables.Cameras[i].IsOpened ? "相机" + (i + 1) + "初始化成功" : "相机" + (i + 1) + "初始化失败");
                                if (Variables.Cameras[i].IsOpened)
                                    break;
                            }
                        }
                    }

                    CamStatus.Value = Variables.Cameras[0].IsOpened && Variables.Cameras[1].IsOpened && Variables.Cameras[2].IsOpened && Variables.Cameras[3].IsOpened;
                }
                catch
                {
                    CamStatus.Value = false;
                }
            });
        }

        /// <summary>
        /// 初始化脚本引擎
        /// </summary>
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

        /// <summary>
        /// 硬盘容量充足，允许工作
        /// </summary>
        private bool sapceCanWork = true;

        /// <summary>
        /// 判断硬盘剩余容量
        /// </summary>
        private async void initFreeSpace()
        {
            while (true)
            {
                await 3000;

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
                    sapceCanWork = true;
                    if (freeSpace < SystemConfig.FreeSpace)
                        Variables.ShowGrowlWarning("硬盘容量不足！请立即停止工作！");
                    else if (freeSpace < SystemConfig.FreeSpace / 2.0)
                        Variables.ShowGrowlError("硬盘容量严重不足！即将停止工作！");
                    else if (freeSpace < SystemConfig.FreeSpace / 4.0)
                    {
                        sapceCanWork = false;
                        Log.Error("硬盘剩余容量严重不足，软件停止工作！");
                    }
                    Variables.SaveImagePath = (SystemConfig.HardDrive == 0 ? "C:\\"
                       : SystemConfig.HardDrive == 1 ? "D:\\"
                        : SystemConfig.HardDrive == 2 ? "E:\\"
                         : SystemConfig.HardDrive == 3 ? "F:\\"
                          : SystemConfig.HardDrive == 4 ? "G:\\" : "E:\\") + "VPImages\\RunImages\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\";
                    Variables.SaveOriginalImagePath = (SystemConfig.HardDrive == 0 ? "C:\\"
                           : SystemConfig.HardDrive == 1 ? "D:\\"
                            : SystemConfig.HardDrive == 2 ? "E:\\"
                             : SystemConfig.HardDrive == 3 ? "F:\\"
                              : SystemConfig.HardDrive == 4 ? "G:\\" : "E:\\") + "VPImages\\OriginalImages\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\";
                }
                catch { }
            }
        }

        //PLC初始化
        private async void initPLC()
        {
            //初始化PLC

            await 3000;

            await Task.Run(() =>
            {
                //Variables.HCPLC0.Init("192.168.1.50", 502, 0);
                Variables.HCPLC1.Init("192.168.1.50", 502, 0);
                Variables.HCPLC2.Init("192.168.1.50", 502, 0);
                Variables.HCPLC3.Init("192.168.1.50", 502, 0);
                Variables.HCPLC4.Init("192.168.1.50", 502, 0);

                if (Variables.HCPLC1.IsConnected)
                {
                    //犯颈椎病的这台机刚开软件要给个处理完成
                    Variables.HCPLC1.WriteM(2553, new bool[] { true });//左相机单次处理处理完成
                    Variables.HCPLC1.WriteM(2554, new bool[] { true });//右相机单次处理处理完成
                }
            });

            //PLCStatus.Value = Variables.HCPLC0.IsConnected && Variables.HCPLC1.IsConnected && Variables.HCPLC2.IsConnected;
            PLCStatus.Value = Variables.HCPLC1.IsConnected && Variables.HCPLC2.IsConnected && Variables.HCPLC3.IsConnected && Variables.HCPLC4.IsConnected;

            Log.Info(PLCStatus.Value ? "PLC连接成功" : "PLC连接失败");
        }

        #endregion 初始化

        private ObservableCollection<RunShowData> _runShowData = new ObservableCollection<RunShowData>();

        public ObservableCollection<RunShowData> RunShowDatas
        {
            get { return _runShowData; }
            set { SetProperty(ref _runShowData, value); }
        }

        // 1 是左右拍同一产品，2 是左右拍不同产品
        private int CameraMode = 1;

        //是否显示结果到拼图
        private bool IsShowResultToPinTu = false;

        private HImage Cam1Image = new HImage();
        private HImage Cam2Image = new HImage();
        private HImage Cam3Image = new HImage();
        private HImage Cam4Image = new HImage();

        private List<RunResult> run1Results = new List<RunResult>();
        private List<RunResult> run2Results = new List<RunResult>();
        private List<RunResult> run3Results = new List<RunResult>();
        private List<RunResult> run4Results = new List<RunResult>();

        //正面检测全部完成
        private bool isRunADone = false;

        private bool isRunBDone = false;

        //拼图完成
        private bool isRun1PinTuDone = false;

        private bool isRun2PinTuDone = false;

        //Pin检测完成完成
        private bool isRun1PinDone = false;

        private bool isRun2PinDone = false;

        //mes上传完成
        private bool isRunMes = false;

        //扫码计数
        private int count1 = 0;

        private int count2 = 0;

        //左右工位是否屏蔽
        private bool isIgnore1 = false;

        private bool isIgnore2 = false;

        private static async Task<string> getBarCodeByHttp(string ip, int line)
        {
            try
            {
                var client = new HttpClient();
                string ipstr = $"http://{ip}:8080/api/barcode/find?position={line}";
                var request = new HttpRequestMessage(HttpMethod.Get, ipstr);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var barCode = await response.Content.ReadAsStringAsync();
                client.Dispose();
                return barCode;
            }
            catch { return "error"; }
        }

        private async void runA()
        {
            //当前拍照位置
            int position = 0;

            while (true)
            {
                await Task.Delay(10);
                if (!PLCStatus.Value) continue;
                //界面设置，1为双相机拍同一个产品
                CameraMode = Variables.CurrentProject.CameraMode;
                IsShowResultToPinTu = Variables.CurrentProject.IsShowResultToPinTu;

                // 握手信号
                Variables.HCPLC1.WriteM(2550, new bool[] { true });

                //左屏蔽
                isIgnore1 = Variables.HCPLC1.ReadM(4003, 1)[0];
                //右屏蔽
                isIgnore2 = Variables.HCPLC1.ReadM(4004, 1)[0];

                //todo 这里要从前面工站读取条码判断是哪种模式，这里强制硬盘剩余空间不足不允许进行
                if (Variables.HCPLC1.ReadM(2540, 1)[0] == true && sapceCanWork)
                {
                    // 扫码计数清零
                    count1 = 0;
                    count2 = 0;
                    //重新生成显示的结果
                    initShowDatas(CameraMode);

                    Variables.Barcode1String.Clear();
                    Variables.Barcode2String.Clear();

                    Variables.HCPLC1.WriteM(2540, new bool[] { false });

                    //这里需和孙鑫建确认读取条码

                    bool isBarcodeOK = true;

                    try
                    {
                        string barcodeStr = await getBarCodeByHttp("192.168.1.25", 4);
                        if (barcodeStr != "error")
                        {
                            if (barcodeStr == "|||")
                                isBarcodeOK = false;
                        }
                        else
                            isBarcodeOK = false;
                    }
                    catch { isBarcodeOK = false; }

                    //读到条码为true
                    if (isBarcodeOK)
                        Variables.HCPLC1.WriteM(2581, new bool[] { true });
                    //给PLC，同上
                    Variables.HCPLC1.WriteM(2580, new bool[] { true });
                }

                //todo查询料号 确认发送  需要现场更改
                if (Variables.HCPLC1.ReadM(2505, 1)[0] == true)
                {
                    Variables.HCPLC1.WriteW(354, new short[] { Variables.CurrentProject.PLCProjectName1 });

                    Variables.HCPLC1.WriteM(2555, new bool[] { true });
                }

                //相机1拍照
                if (Variables.HCPLC1.ReadM(2501, 1)[0])
                {
                    position = Variables.HCPLC1.ReadW(356, 1)[0];
                    Variables.HCPLC1.WriteM(2501, new bool[] { false });//左相机拍照信号关闭

                    if (position == 0 && count1 == 0)
                    {
                        initTotalImage(CameraMode, 1);
                    }

                    await Task.Run(() =>
                    {
                        Cam1Image?.Dispose();
                        Cam1Image = Variables.Cameras[0].GrabOne();
                    });
                    if (position != 0)
                        Variables.HCPLC1.WriteM(2551, new bool[] { true });//拍照完成
                    if (position != 0)
                        if (!IsShowResultToPinTu)
                            await imageQueues.Enqueue(new ImageQueue() { CameraIndex = 1, PositionIndex = position, Image = Cam1Image.CopyImage() }, imageQueues.tokenNone);

                    //处理，如果PLC给的位置大于程序数量，则强行位置减1
                    if (position >= Variables.CurrentProject.Programs.ElementAt(0).Value.Count)
                        position = position - 1;
                    await process1(position, Cam1Image.CopyImage(), Variables.CurrentProject.Programs.ElementAt(0).Value[position], count1);
                    count1++;
                    if (position == 0)
                    {
                        Variables.WindowData1.WindowCtrl.ShowImageToWindow(Cam1Image.CopyImage());
                        Variables.WindowData1.WindowCtrl.Repaint();

                        //对于第一个位置，规定好，主扫码，
                        HalWindowImage hwi = new HalWindowImage(Cam1Image);

                        //扫码失败
                        if (!run1Results[0].BoolResult)
                        {
                            // 置位扫码失败信号，再复位扫码成功信号
                            // Variables.HCPLC1.WriteM(2557, new bool[] { true });
                            Variables.Barcode1String.Add("default1");
                            Variables.HCPLC1.WriteM(2556, new bool[] { true });
                            Variables.Result1OK = false;
                        }
                        else//扫码成功
                        {
                            Variables.Barcode1String.Add(run1Results[0].MessageResult);
                            Variables.HCPLC1.WriteM(2556, new bool[] { true });
                            Log.Info("位置" + count1 + run1Results[0].MessageResult);
                        }
                        if (SystemConfig.IsSaveOriginalImage)
                        {
                            SaveImageRun(Cam1Image.CopyImage(), Variables.SaveOriginalImagePath + Variables.Barcode1String[0], "cam1_" + position + "_" + count1, "bmp");
                        }

                        hwi.Close();
                        // 扫码成功上传mes
                        if (run1Results[0].BoolResult && Variables.Barcode1String.Count == 1)
                        {
                            if (SystemConfig.UseMES)
                            {
                            }
                        }
                    }
                    else
                    {
                        HalWindowImage hwi = new HalWindowImage(Cam1Image);

                        int ngCount = 0;
                        bool totalBool = true;
                        foreach (var rr in run1Results)
                        {
                            try
                            {
                                if (!rr.BoolResult)
                                {
                                    totalBool = false;
                                    hwi.AddRegion(rr.RegionResult, BingLibrary.Vision.HalconColors.红色);

                                    if (!IsShowResultToPinTu)
                                        hwi.AddText(rr.MessageResult, 20 + ngCount * 60, 20, BingLibrary.Vision.HalconColors.红色, 32);
                                    else
                                    {
                                        ConfigSet cs = Variables.CurrentProject.Programs.ElementAt(0).Value[position].ProductConfigSet;
                                        hwi.AddText(rr.MessageResult, cs.Row1 + 20 + ngCount * 60, cs.Column1 + 20, BingLibrary.Vision.HalconColors.红色, 32);
                                    }

                                    ngCount++;

                                    if (rr.MessageResult.Contains("焊点"))
                                        mesResultStr1 += "LOCATION:" + "1_" + position + "," + ",TEST_RESULT:Repair,ITEM_ERRCODE:焊点异常;";
                                    else if (rr.MessageResult.Contains("铆点"))
                                        mesResultStr1 += "LOCATION:" + "1_" + position + "," + ",TEST_RESULT:Repair,ITEM_ERRCODE:铆点异常;";
                                    else if (rr.MessageResult.Contains("铝巴"))
                                        mesResultStr1 += "LOCATION:" + "1_" + position + "," + ",TEST_RESULT:Repair,ITEM_ERRCODE:铝巴异常;";
                                }
                            }
                            catch { }
                        }
                        if (!totalBool)
                        {
                            Variables.Result1OK = false;
                            SaveImageRun(hwi.GetWindowImage(), Variables.SaveImagePath + Variables.Barcode1String[0] + "\\" + "NG\\", "cam1_" + position, "jpg");
                        }

                        if (IsShowResultToPinTu)
                        {
                            if (!totalBool)
                                await imageQueues.Enqueue(new ImageQueue() { CameraIndex = 1, PositionIndex = position, Image = hwi.GetWindowImage() }, imageQueues.tokenNone);
                            else
                                await imageQueues.Enqueue(new ImageQueue() { CameraIndex = 1, PositionIndex = position, Image = Cam1Image.CopyImage() }, imageQueues.tokenNone);
                        }

                        hwi.Close();

                        //展示页面
                        for (int i = 0; i < run1Results.Count; i++)
                        {
                            try
                            {
                                RunShowDatas[Variables.CurrentProject.Programs.ElementAt(0).Value.Count - 1 + position - 1].RunStatus = totalBool ? 1 : 0;
                                if (run1Results[i].BoolResult)
                                {
                                    RunShowDatas[Variables.CurrentProject.Programs.ElementAt(0).Value.Count - 1 + position - 1].RunShowDataResult[i] += "OK";
                                }
                                else
                                {
                                    RunShowDatas[Variables.CurrentProject.Programs.ElementAt(0).Value.Count - 1 + position - 1].RunShowDataResult[i] += "NG";
                                }
                            }
                            catch { }
                        }
                    }

                    // 相机1全部处理完成
                    if (position == Variables.CurrentProject.Programs.ElementAt(0).Value.Count - 1)
                        isRunADone = true;

                    Variables.HCPLC1.WriteM(2553, new bool[] { true });//处理完成
                }

                //如果是两个相机拍同一个产品
                if (CameraMode == 1)
                {
                    if (isRunADone && isRunBDone)
                    {
                        //如果相机3PIN程序为空，则相机3不检测pin，直接为true
                        if (Variables.CurrentProject.Programs.ElementAt(2).Value.Count == 0)
                        {
                            isRun1PinDone = true;
                        }
                        //如果相机4PIN程序为空，则相机3不检测pin，直接为true
                        if (Variables.CurrentProject.Programs.ElementAt(3).Value.Count == 0)
                        {
                            isRun2PinDone = true;
                        }
                        isRunADone = false;
                        isRunBDone = false;
                    }
                }
                else
                {
                    if (isRunADone)
                    {
                        isRunADone = false;
                        //如果相机3PIN程序为空，则相机3不检测pin，直接为true
                        if (Variables.CurrentProject.Programs.ElementAt(2).Value.Count == 0)
                        {
                            isRun1PinDone = true;
                        }
                    }
                }
            }
        }

        private async void runB()
        {
            int position = 0;

            while (true)
            {
                await Task.Delay(10);
                if (!PLCStatus.Value) continue;

                //todo查询料号 确认发送  需要现场更改
                if (Variables.HCPLC1.ReadM(2510, 1)[0] == true)
                {
                    Variables.HCPLC1.WriteW(354, new short[] { Variables.CurrentProject.PLCProjectName2 });

                    Variables.HCPLC1.WriteM(2560, new bool[] { true });
                }

                //相机2拍照
                if (Variables.HCPLC2.ReadM(2502, 1)[0])
                {
                    position = Variables.HCPLC2.ReadW(357, 1)[0];
                    Variables.HCPLC2.WriteM(2502, new bool[] { false });//左相机拍照信号关闭

                    if (position == 0 && count2 == 0 && CameraMode == 1 && Variables.Barcode1String.Count == 0)
                    {
                        initTotalImage(CameraMode, 2);
                    }
                    else if (position == 0 && count2 == 0 && CameraMode == 2)
                    {
                        initTotalImage(CameraMode, 2);
                    }

                    await Task.Run(() =>
                    {
                        Cam2Image?.Dispose();
                        Cam2Image = Variables.Cameras[1].GrabOne();
                    });
                    if (position != 0)
                        Variables.HCPLC2.WriteM(2552, new bool[] { true });//拍照完成
                    if (position != 0)
                        if (!IsShowResultToPinTu)
                            await imageQueues.Enqueue(new ImageQueue() { CameraIndex = 2, PositionIndex = position, Image = Cam2Image.CopyImage() }, imageQueues.tokenNone);

                    //处理，如果PLC给的位置大于程序数量，则强行位置减1
                    if (position >= Variables.CurrentProject.Programs.ElementAt(1).Value.Count)
                        position = position - 1;
                    await process2(position, Cam2Image.CopyImage(), Variables.CurrentProject.Programs.ElementAt(1).Value[position], count2);
                    count2++;
                    if (position == 0 && CameraMode == 2)
                    {
                        Variables.WindowData2.WindowCtrl.ShowImageToWindow(Cam2Image.CopyImage());
                        Variables.WindowData2.WindowCtrl.Repaint();
                        //对于第一个位置，规定好，主扫码，
                        HalWindowImage hwi = new HalWindowImage(Cam2Image);

                        //扫码失败
                        if (!run2Results[0].BoolResult)
                        {
                            //置位扫码失败信号，再复位扫码成功信号
                            // Variables.HCPLC2.WriteM(2559, new bool[] { true });
                            Variables.Barcode2String.Add("default2");
                            Variables.HCPLC2.WriteM(2558, new bool[] { true });
                            Variables.Result2OK = false;
                        }
                        else//扫码成功
                        {
                            Variables.Barcode2String.Add(run2Results[0].MessageResult);
                            Variables.HCPLC2.WriteM(2558, new bool[] { true });
                            Log.Info("位置" + count2 + run2Results[0].MessageResult);
                        }
                        if (SystemConfig.IsSaveOriginalImage)
                        {
                            SaveImageRun(Cam2Image.CopyImage(), Variables.SaveOriginalImagePath + Variables.Barcode2String[0], "cam2_" + position + "_" + count2, "bmp");
                        }
                        //todo：保存图像
                        hwi.Close();

                        // 扫码成功上传mes
                        if (run2Results[0].BoolResult && Variables.Barcode2String.Count == 1)
                        {
                            if (SystemConfig.UseMES)
                            {
                            }
                        }
                    }
                    else if (position == 0 && CameraMode == 1)
                    {
                        //对于第一个位置，规定好，主扫码，
                        HalWindowImage hwi = new HalWindowImage(Cam2Image);

                        //扫码失败
                        if (!run2Results[0].BoolResult)
                        {
                            //置位扫码失败信号，再复位扫码成功信号
                            // Variables.HCPLC2.WriteM(2559, new bool[] { true });
                            Variables.Barcode1String.Add("default1");
                            Variables.HCPLC2.WriteM(2558, new bool[] { true });
                            Variables.Result1OK = false;
                        }
                        else//扫码成功
                        {
                            Variables.Barcode1String.Add(run2Results[0].MessageResult);
                            Variables.HCPLC2.WriteM(2558, new bool[] { true });
                            Log.Info("条码" + count2 + "：" + run2Results[0].MessageResult);
                        }

                        //todo：保存图像
                        if (SystemConfig.IsSaveOriginalImage)
                        {
                            SaveImageRun(Cam2Image.CopyImage(), Variables.SaveOriginalImagePath + Variables.Barcode1String[0], "cam2_" + position + "_" + count2, "bmp");
                        }
                        hwi.Close();
                        // 扫码成功上传mes
                        if (run2Results[0].BoolResult && Variables.Barcode1String.Count == 1)
                        {
                            if (SystemConfig.UseMES)
                            {
                            }
                        }
                    }
                    else
                    {
                        HalWindowImage hwi = new HalWindowImage(Cam2Image);

                        int ngCount = 0;
                        bool totalBool = true;
                        foreach (var rr in run2Results)
                        {
                            try
                            {
                                if (!rr.BoolResult)
                                {
                                    totalBool = false;
                                    hwi.AddRegion(rr.RegionResult, BingLibrary.Vision.HalconColors.红色);

                                    if (!IsShowResultToPinTu)
                                        hwi.AddText(rr.MessageResult, 20 + ngCount * 60, 20, BingLibrary.Vision.HalconColors.红色, 32);
                                    else
                                    {
                                        ConfigSet cs = Variables.CurrentProject.Programs.ElementAt(1).Value[position].ProductConfigSet;
                                        hwi.AddText(rr.MessageResult, cs.Row1 + 20 + ngCount * 60, cs.Column1 + 20, BingLibrary.Vision.HalconColors.红色, 32);
                                    }

                                    ngCount++;

                                    if (rr.MessageResult.Contains("焊点"))
                                        mesResultStr2 += "LOCATION:" + "2_" + position + "," + ",TEST_RESULT:Repair,ITEM_ERRCODE:焊点异常;";
                                    else if (rr.MessageResult.Contains("铆点"))
                                        mesResultStr2 += "LOCATION:" + "2_" + position + "," + ",TEST_RESULT:Repair,ITEM_ERRCODE:铆点异常;";
                                    else if (rr.MessageResult.Contains("铝巴"))
                                        mesResultStr2 += "LOCATION:" + "2_" + position + "," + ",TEST_RESULT:Repair,ITEM_ERRCODE:铝巴异常;";
                                }
                                //else
                                //{
                                //   // hwi.AddRegion(rr.RegionResult, BingLibrary.Vision.HalconColors.绿色);
                                //}
                            }
                            catch { }
                        }
                        if (!totalBool)
                        {
                            if (CameraMode == 1)
                            {
                                Variables.Result1OK = false;
                                SaveImageRun(hwi.GetWindowImage(), Variables.SaveImagePath + Variables.Barcode1String[0] + "\\" + "NG\\", "cam2" + "_" + position, "jpg");
                            }
                            else if (CameraMode == 2)
                            {
                                Variables.Result2OK = false;
                                SaveImageRun(hwi.GetWindowImage(), Variables.SaveImagePath + Variables.Barcode2String[0] + "\\" + "NG\\", "cam2" + "_" + position, "jpg");
                            }
                        }

                        if (IsShowResultToPinTu)
                        {
                            if (!totalBool)
                            {
                                await imageQueues.Enqueue(new ImageQueue() { CameraIndex = 2, PositionIndex = position, Image = hwi.GetWindowImage() }, imageQueues.tokenNone);
                            }
                            else
                                await imageQueues.Enqueue(new ImageQueue() { CameraIndex = 2, PositionIndex = position, Image = Cam2Image.CopyImage() }, imageQueues.tokenNone);
                        }

                        //展示页面
                        for (int i = 0; i < run2Results.Count; i++)
                        {
                            try
                            {
                                RunShowDatas[position - 1].RunStatus = totalBool ? 1 : 0;
                                if (run2Results[i].BoolResult)
                                {
                                    RunShowDatas[position - 1].RunShowDataResult[i] += "OK";
                                }
                                else
                                {
                                    RunShowDatas[position - 1].RunShowDataResult[i] += "NG";
                                }
                            }
                            catch { }
                        }

                        hwi.Close();
                    }

                    // 相机2全部处理完成
                    if (position == Variables.CurrentProject.Programs.ElementAt(1).Value.Count - 1)
                        isRunBDone = true;

                    Variables.HCPLC2.WriteM(2554, new bool[] { true });//处理完成
                }

                if (CameraMode == 2)
                {
                    //相机2全部完成
                    if (isRunBDone)
                    {
                        isRunBDone = false;
                        //如果相机3PIN程序为空，则相机3不检测pin，直接为true
                        if (Variables.CurrentProject.Programs.ElementAt(3).Value.Count == 0)
                        {
                            isRun2PinDone = true;
                        }
                    }
                }
            }
        }

        private async void runC()
        {
            int position = 0;

            while (true)
            {
                await Task.Delay(10);
                if (!PLCStatus.Value) continue;

                //相机4拍照
                if (Variables.HCPLC3.ReadM(2503, 1)[0])
                {
                    //plc从1开始给
                    position = Variables.HCPLC3.ReadW(358, 1)[0] - 1;
                    Variables.HCPLC3.WriteM(2503, new bool[] { false });//右pin相机拍照信号关闭

                    await Task.Run(() =>
                    {
                        Cam3Image?.Dispose();
                        Cam3Image = Variables.Cameras[2].GrabOne();
                    });

                    if (!TotalImagePinLeft.IsInitialized())
                        TotalImagePinLeft = Cam3Image.CopyImage();
                    else
                        TotalImagePinLeft = TotalImagePinLeft.ConcatObj(Cam3Image.CopyImage());

                    Variables.WindowData3.WindowCtrl.ShowImageToWindow(TotalImagePinLeft.TileImages(1, "vertical"));
                    Variables.WindowData3.WindowCtrl.FitImageToWindow();
                    Variables.WindowData3.WindowCtrl.Repaint();

                    //处理，如果PLC给的位置大于程序数量，则强行位置减1
                    if (position >= Variables.CurrentProject.Programs.ElementAt(2).Value.Count)
                        position = position - 1;
                    await process3(position, Cam3Image.CopyImage(), Variables.CurrentProject.Programs.ElementAt(2).Value[position]);

                    if (SystemConfig.IsSaveOriginalImage)
                    {
                        SaveImageRun(Cam3Image.CopyImage(), Variables.SaveOriginalImagePath + Variables.Barcode1String[0], "3_" + position, "bmp");
                    }

                    int ngCount = 0;
                    bool totalBool = true;

                    HalWindowImage hwi = new HalWindowImage(Cam3Image);

                    for (int m = 0; m < run3Results.Count; m++)
                    {
                        try
                        {
                            if (run3Results[m].MessageResult.Contains("PIN针检测"))
                            {
                                hwi = new HalWindowImage(run3Results[m].ResultImage);
                                hwi.AddRegion(run3Results[m].RegionResult, BingLibrary.Vision.HalconColors.红色);

                                string[] results = run3Results[m].MessageResult.Split('\n');
                                var AllowOffset = double.Parse((Variables.CurrentProject.Programs.ElementAt(2).Value[position].ProgramDatas[m].Parameters.BingGetOrAdd("AllowOffset", 0)).ToString());
                                var Frames = (List<InspectFrame>)Variables.CurrentProject.Programs.ElementAt(2).Value[position].ProgramDatas[m].Parameters.BingGetOrAdd("InspectFrames", new List<InspectFrame>());
                                for (int i = 0; i < Frames.Count; i++)
                                {
                                    double row = Frames[i].Row1;
                                    double col = Frames[i].Col1;
                                    double distance = 0;
                                    bool b = double.TryParse(results[i + 1], out distance);
                                    //results[i].Split("");
                                    if (!Frames[i].ContainsObject)
                                    {
                                        hwi.AddText("", (int)(row), (int)(col), BingLibrary.Vision.HalconColors.红色, 24);
                                    }
                                    else if (distance < AllowOffset)
                                    {
                                        if (results[i + 1] == "缺失")
                                            hwi.AddText(results[i + 1], (int)(row) - 12, (int)(col), BingLibrary.Vision.HalconColors.红色, 24);
                                        else
                                            hwi.AddText(results[i + 1], (int)(row) - 12, (int)(col), BingLibrary.Vision.HalconColors.蓝色, 24);
                                    }
                                    else { hwi.AddText(results[i + 1], (int)(row) - 12, (int)(col), BingLibrary.Vision.HalconColors.红色, 24); }
                                    // hwi.AddText(results[i], (int)InspectFrames[i].Row1, (int)InspectFrames[i].Col1, BingLibrary.Vision.HalconColors.红色, 32);
                                }

                                if (!run3Results[m].BoolResult)
                                {
                                    totalBool = false;
                                    hwi.AddText(run3Results[m].MessageResult, 20 + ngCount * 60, 20, BingLibrary.Vision.HalconColors.红色, 32);
                                    ngCount++;
                                }
                                if (!totalBool)
                                {
                                    mesResultStr1 += "LOCATION:" + "3_" + position + "," + ",TEST_RESULT:Repair,ITEM_ERRCODE:pin异常;";

                                    Variables.Result1OK = false;
                                    SaveImageRun(hwi.GetWindowImage(), Variables.SaveImagePath + Variables.Barcode1String[0] + "\\" + "Pin\\", "cam3" + "_" + position + "_" + m, "jpg");
                                }

                                hwi.Close();
                            }
                        }
                        catch { }
                    }

                    for (int i = 0; i < run3Results.Count; i++)
                    {
                        try
                        {
                            RunShowDatas[Variables.CurrentProject.Programs.ElementAt(0).Value.Count - 1 + Variables.CurrentProject.Programs.ElementAt(1).Value.Count - 1 + Variables.CurrentProject.Programs.ElementAt(2).Value.Count + position].RunStatus = totalBool ? 1 : 0;
                            if (run3Results[i].BoolResult)
                            {
                                RunShowDatas[Variables.CurrentProject.Programs.ElementAt(0).Value.Count - 1 + Variables.CurrentProject.Programs.ElementAt(1).Value.Count - 1 + Variables.CurrentProject.Programs.ElementAt(2).Value.Count + position].RunShowDataResult[i] += "OK";
                            }
                            else
                            {
                                RunShowDatas[Variables.CurrentProject.Programs.ElementAt(0).Value.Count - 1 + Variables.CurrentProject.Programs.ElementAt(1).Value.Count - 1 + Variables.CurrentProject.Programs.ElementAt(2).Value.Count + position].RunShowDataResult[i] += "NG";
                            }
                        }
                        catch { }
                    }

                    Variables.HCPLC3.WriteM(2570, new bool[] { true });//拍照完成
                    Variables.HCPLC3.WriteM(2571, new bool[] { true });//处理完成

                    if (position == Variables.CurrentProject.Programs.ElementAt(2).Value.Count - 1)
                        isRun1PinDone = true;
                }

                if (isIgnore1)
                    isRun1PinDone = true;
                if (isIgnore2)
                    isRun2PinDone = true;

                if (isIgnore1 && isIgnore2)
                {
                    isRun1PinDone = false;
                    isRun2PinDone = false;
                }

                //如果pin检测完成
                if (isRun1PinDone && isRun2PinDone)
                {
                    isRun1PinDone = false;
                    isRun2PinDone = false;

                    // mes 上传
                    if (CameraMode == 1)
                    {
                        if (SystemConfig.UseMES)
                        {
                            string meslog1 = Variables.mes.UploadAOI(Variables.Barcode1String[0], MesDatas[6].Value, MesDatas[7].Value, Variables.SaveImagePath, MesDatas[8].Value, Variables.Result1OK ? "Y" : "N", MesDatas[4].Value, MesDatas[5].Value, mesResultStr1);
                            Log.Info("MES上传成功");
                            Log.Info("MES返回：" + meslog1);
                        }
                        try
                        {
                            using (var client = new HttpClient())
                            {
                                string fileName = Variables.SaveImagePath + Variables.Barcode1String[0] + "\\" + "ALL\\TotalImage.jpg";
                                var form = new MultipartFormDataContent();
                                var fileContent = new ByteArrayContent(File.ReadAllBytes(fileName));
                                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                                string[] strs = fileName.Split(new char[] { '\\' });
                                form.Add(fileContent, "File", strs[strs.Length - 1]);
                                form.Add(new StringContent("1"), "LineCode");//线体号
                                form.Add(new StringContent("4"), "MachineCode");//设备号
                                form.Add(new StringContent(Variables.Result1OK ? "PASS" : "NG"), "Result");//结果 NG PASS
                                form.Add(new StringContent(mesResultStr1), "Exception");//异常说明
                                form.Add(new StringContent(Variables.Barcode1String[0].Replace("条码：", "")), "BarCode");//异常说明
                                form.Add(new StringContent("0"), "Position");//异常说明
                                _ = await client.PostAsync($"http://192.168.1.200:5000/Machine/BarCodeLogInit", form);
                            }
                        }
                        catch
                        {
                            try
                            {
                                string filePath = Variables.SaveImagePath + Variables.Barcode1String[0];
                                saveResultToCSV(Variables.Result1OK ? "PASS" : "NG", Variables.Barcode1String[0].Replace("条码：", ""),
                               MesDatas[8].Value, MesDatas[7].Value, filePath, mesResultStr1);
                            }
                            catch { }
                        }
                    }
                    else if (CameraMode == 2)
                    {
                        if (!isIgnore1)//左未屏蔽
                        {
                            if (SystemConfig.UseMES)
                            {
                                string meslog1 = Variables.mes.UploadAOI(Variables.Barcode1String[0], MesDatas[6].Value, MesDatas[7].Value, Variables.SaveImagePath, MesDatas[8].Value, Variables.Result1OK ? "Y" : "N", MesDatas[4].Value, MesDatas[5].Value, mesResultStr1);
                                Log.Info("左侧MES上传成功");
                                Log.Info("左侧MES返回：" + meslog1);
                            }
                            try
                            {
                                using (var client = new HttpClient())
                                {
                                    string fileName = Variables.SaveImagePath + Variables.Barcode1String[0] + "\\" + "ALL\\TotalImageLeft.jpg";
                                    var form = new MultipartFormDataContent();
                                    var fileContent = new ByteArrayContent(File.ReadAllBytes(fileName));
                                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                                    string[] strs = fileName.Split(new char[] { '\\' });
                                    form.Add(fileContent, "File", strs[strs.Length - 1]);
                                    form.Add(new StringContent("1"), "LineCode");//线体号
                                    form.Add(new StringContent("4"), "MachineCode");//设备号
                                    form.Add(new StringContent(Variables.Result1OK ? "PASS" : "NG"), "Result");//结果 NG PASS
                                    form.Add(new StringContent(mesResultStr1), "Exception");//异常说明
                                    form.Add(new StringContent(Variables.Barcode1String[0].Replace("条码：", "")), "BarCode");//异常说明
                                    form.Add(new StringContent("0"), "Position");//异常说明
                                    _ = await client.PostAsync($"http://192.168.1.200:5000/Machine/BarCodeLogInit", form);
                                }
                            }
                            catch (Exception ex) { Log.Error("本机mes失败：" + ex.Message); }

                            try
                            {
                                string filePath = Variables.SaveImagePath + Variables.Barcode1String[0];
                                saveResultToCSV(Variables.Result1OK ? "PASS" : "NG", Variables.Barcode1String[0].Replace("条码：", ""),
                               MesDatas[8].Value, MesDatas[7].Value, filePath, mesResultStr1);
                            }
                            catch { }
                        }
                        if (!isIgnore2)//右未屏蔽
                        {
                            if (SystemConfig.UseMES)
                            {
                                string meslog2 = Variables.mes.UploadAOI(Variables.Barcode2String[0], MesDatas[6].Value, MesDatas[7].Value, Variables.SaveImagePath, MesDatas[8].Value, Variables.Result2OK ? "Y" : "N", MesDatas[4].Value, MesDatas[5].Value, mesResultStr2);
                                Log.Info("右侧MES上传成功");
                                Log.Info("右侧MES返回：" + meslog2);
                            }
                            try
                            {
                                using (var client = new HttpClient())
                                {
                                    string fileName = Variables.SaveImagePath + Variables.Barcode2String[0] + "\\" + "ALL\\TotalImageRight.jpg";
                                    var form = new MultipartFormDataContent();
                                    var fileContent = new ByteArrayContent(File.ReadAllBytes(fileName));
                                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                                    string[] strs = fileName.Split(new char[] { '\\' });
                                    form.Add(fileContent, "File", strs[strs.Length - 1]);
                                    form.Add(new StringContent("1"), "LineCode");//线体号
                                    form.Add(new StringContent("4"), "MachineCode");//设备号
                                    form.Add(new StringContent(Variables.Result2OK ? "PASS" : "NG"), "Result");//结果 NG PASS
                                    form.Add(new StringContent(mesResultStr2), "Exception");//异常说明
                                    form.Add(new StringContent(Variables.Barcode2String[0].Replace("条码：", "")), "BarCode");//异常说明
                                    form.Add(new StringContent("0"), "Position");//异常说明
                                    _ = await client.PostAsync($"http://192.168.1.200:5000/Machine/BarCodeLogInit", form);
                                }
                            }
                            catch (Exception ex) { Log.Error("本机mes失败：" + ex.Message); }

                            try
                            {
                                string filePath = Variables.SaveImagePath + Variables.Barcode2String[0];
                                saveResultToCSV(Variables.Result2OK ? "PASS" : "NG", Variables.Barcode2String[0].Replace("条码：", ""),
                               MesDatas[8].Value, MesDatas[7].Value, filePath, mesResultStr2);
                            }
                            catch { }
                        }
                    }

                    isRunMes = true;
                }

                if (isIgnore1)
                    isRun1PinTuDone = true;
                if (isIgnore2)
                    isRun2PinTuDone = true;

                if (isIgnore1 && isIgnore2)
                {
                    isRun1PinTuDone = false;
                    isRun2PinTuDone = false;
                }

                // 全部结束
                if (isRunMes && isRun1PinTuDone && isRun2PinTuDone)
                {
                    if (CameraMode == 1)
                    {
                        if (Variables.Result1OK == true)
                            setOK();
                        else
                            setNG();

                        if (Variables.Result1OK == true)
                        { ResultStatus1.Value = 1; ResultStatus2.Value = 1; }
                        else
                        { ResultStatus1.Value = 0; ResultStatus2.Value = 0; }
                    }
                    else if (CameraMode == 2)
                    {
                        if (!isIgnore1)
                        {
                            if (Variables.Result1OK == true)
                                setOK();
                            else
                                setNG();
                            if (Variables.Result1OK == true)
                                ResultStatus1.Value = 1;
                            else
                                ResultStatus1.Value = 0;
                        }

                        //延时
                        await Task.Delay(200);
                        if (!isIgnore2)
                        {
                            if (Variables.Result2OK == true)
                                setOK();
                            else
                                setNG();
                            if (Variables.Result2OK == true)
                                ResultStatus2.Value = 1;
                            else
                                ResultStatus2.Value = 0;
                        }
                    }

                    isRunMes = false;
                    isRun1PinTuDone = false;
                    isRun2PinTuDone = false;
                    Variables.HCPLC3.WriteM(2561, new bool[] { true });
                }
            }
        }

        private async void runD()
        {
            int position = 0;

            while (true)
            {
                await Task.Delay(10);
                if (!PLCStatus.Value) continue;

                //相机4拍照
                if (Variables.HCPLC4.ReadM(2504, 1)[0])
                {
                    position = Variables.HCPLC4.ReadW(359, 1)[0] - 1;
                    Variables.HCPLC4.WriteM(2504, new bool[] { false });//右pin相机拍照信号关闭

                    await Task.Run(() =>
                    {
                        Cam4Image?.Dispose();
                        Cam4Image = Variables.Cameras[3].GrabOne();
                    });

                    if (!TotalImagePinRight.IsInitialized())
                        TotalImagePinRight = Cam4Image.CopyImage();
                    else
                        TotalImagePinRight = TotalImagePinRight.ConcatObj(Cam4Image.CopyImage());

                    Variables.WindowData4.WindowCtrl.ShowImageToWindow(TotalImagePinRight.TileImages(1, "vertical"));
                    Variables.WindowData4.WindowCtrl.FitImageToWindow();
                    Variables.WindowData4.WindowCtrl.Repaint();

                    //处理，如果PLC给的位置大于程序数量，则强行位置减1
                    if (position >= Variables.CurrentProject.Programs.ElementAt(3).Value.Count)
                        position = position - 1;

                    await process4(position, Cam4Image.CopyImage(), Variables.CurrentProject.Programs.ElementAt(3).Value[position]);

                    if (SystemConfig.IsSaveOriginalImage)
                    {
                        if (CameraMode == 1)
                            SaveImageRun(Cam4Image.CopyImage(), Variables.SaveOriginalImagePath + Variables.Barcode1String[0], "4_" + position, "bmp");
                        else if (CameraMode == 2)
                            SaveImageRun(Cam4Image.CopyImage(), Variables.SaveOriginalImagePath + Variables.Barcode2String[0], "4_" + position, "bmp");
                    }

                    int ngCount = 0;
                    bool totalBool = true;

                    HalWindowImage hwi = new HalWindowImage(Cam4Image);

                    for (int m = 0; m < run4Results.Count; m++)
                    {
                        try
                        {
                            if (run4Results[m].MessageResult.Contains("PIN针检测"))
                            {
                                hwi = new HalWindowImage(run4Results[m].ResultImage);
                                hwi.AddRegion(run4Results[m].RegionResult, BingLibrary.Vision.HalconColors.红色);

                                string[] results = run4Results[m].MessageResult.Split('\n');
                                var AllowOffset = double.Parse((Variables.CurrentProject.Programs.ElementAt(2).Value[position].ProgramDatas[m].Parameters.BingGetOrAdd("AllowOffset", 0)).ToString());
                                var Frames = (List<InspectFrame>)Variables.CurrentProject.Programs.ElementAt(2).Value[position].ProgramDatas[m].Parameters.BingGetOrAdd("InspectFrames", new List<InspectFrame>());
                                for (int i = 0; i < Frames.Count; i++)
                                {
                                    double row = Frames[i].Row1;
                                    double col = Frames[i].Col1;
                                    double distance = 0;
                                    bool b = double.TryParse(results[i + 1], out distance);
                                    //results[i].Split("");
                                    if (!Frames[i].ContainsObject)
                                    {
                                        hwi.AddText("", (int)(row), (int)(col), BingLibrary.Vision.HalconColors.红色, 24);
                                    }
                                    else if (distance < AllowOffset)
                                    {
                                        if (results[i + 1] == "缺失")
                                            hwi.AddText(results[i + 1], (int)(row) - 12, (int)(col), BingLibrary.Vision.HalconColors.红色, 24);
                                        else
                                            hwi.AddText(results[i + 1], (int)(row) - 12, (int)(col), BingLibrary.Vision.HalconColors.蓝色, 24);
                                    }
                                    else { hwi.AddText(results[i + 1], (int)(row) - 12, (int)(col), BingLibrary.Vision.HalconColors.红色, 24); }
                                    // hwi.AddText(results[i], (int)InspectFrames[i].Row1, (int)InspectFrames[i].Col1, BingLibrary.Vision.HalconColors.红色, 32);
                                }

                                if (!run4Results[m].BoolResult)
                                {
                                    totalBool = false;
                                    hwi.AddText(run4Results[m].MessageResult, 20 + ngCount * 60, 20, BingLibrary.Vision.HalconColors.红色, 32);
                                    ngCount++;
                                }
                                if (!totalBool)
                                {
                                    if (CameraMode == 1)
                                    {
                                        mesResultStr1 += "LOCATION:" + "4_" + position + "," + ",TEST_RESULT:Repair,ITEM_ERRCODE:pin异常;";

                                        Variables.Result1OK = false;
                                        SaveImageRun(hwi.GetWindowImage(), Variables.SaveImagePath + Variables.Barcode1String[0] + "\\" + "Pin\\", "cam4" + "_" + position + "_" + m, "jpg");
                                    }
                                    else if (CameraMode == 2)
                                    {
                                        mesResultStr2 += "LOCATION:" + "4_" + position + "," + ",TEST_RESULT:Repair,ITEM_ERRCODE:pin异常;";

                                        Variables.Result2OK = false;
                                        SaveImageRun(hwi.GetWindowImage(), Variables.SaveImagePath + Variables.Barcode2String[0] + "\\" + "Pin\\", "cam4" + "_" + position + "_" + m, "jpg");
                                    }
                                }

                                hwi.Close();
                            }
                        }
                        catch { }
                    }

                    for (int i = 0; i < run4Results.Count; i++)
                    {
                        try
                        {
                            RunShowDatas[Variables.CurrentProject.Programs.ElementAt(0).Value.Count - 1 + Variables.CurrentProject.Programs.ElementAt(1).Value.Count - 1 + position].RunStatus = totalBool ? 1 : 0;
                            if (run4Results[i].BoolResult)
                            {
                                RunShowDatas[Variables.CurrentProject.Programs.ElementAt(0).Value.Count - 1 + Variables.CurrentProject.Programs.ElementAt(1).Value.Count - 1 + position].RunShowDataResult[i] += "OK";
                            }
                            else
                            {
                                RunShowDatas[Variables.CurrentProject.Programs.ElementAt(0).Value.Count - 1 + Variables.CurrentProject.Programs.ElementAt(1).Value.Count - 1 + position].RunShowDataResult[i] += "NG";
                            }
                        }
                        catch { }
                    }

                    Variables.HCPLC4.WriteM(2575, new bool[] { true });//拍照完成
                    Variables.HCPLC4.WriteM(2576, new bool[] { true });//处理完成

                    if (position == Variables.CurrentProject.Programs.ElementAt(3).Value.Count - 1)
                        isRun2PinDone = true;
                }
            }
        }

        #region 保存图像

        public static void SaveImageRun(HImage image, string path, string name, string format = "jpg")
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                saveImage(image, path + "\\" + name, format == "jpg" ? "1" : "0");
            }
            catch (Exception ex) { BingLibrary.Logs.LogOpreate.Error("保存图像的失败" + ex.Message); }
        }

        private static void saveImage(HImage image, string name, string f)
        {
            if (f == "0")
                image.WriteImage("bmp", new HTuple(0), new HTuple(name + ".bmp"));
            else
                image.WriteImage("jpeg", new HTuple(0), new HTuple(name + ".jpg"));
        }

        #endregion 保存图像

        #region 0718修改

        //用于拼图，队列
        private class ImageQueue
        {
            public int CameraIndex { set; get; }
            public int PositionIndex { set; get; }
            public HImage Image { set; get; }
        }

        private AsyncQueue<ImageQueue> imageQueues = new AsyncQueue<ImageQueue>();

        //左右拼图
        private HImage TotalImageLeft = new HImage();

        private HImage TotalImageRight = new HImage();
        private double multValue = 0.5;//缩放显示的倍率

        //pin 拼图
        private HImage TotalImagePinLeft = new HImage();

        private HImage TotalImagePinRight = new HImage();

        /// <summary>
        /// 生成大图
        /// </summary>
        private void initTotalImage(int mode, int cameraIndex = 1)
        {
            Variables.Result1OK = true;
            Variables.Result2OK = true;

            if (mode == 1)
            {
                ResultStatus1.Value = 2;
                ResultStatus2.Value = 2;
                TotalImagePinLeft?.Dispose();
                TotalImagePinLeft.GenEmptyObj();
                TotalImagePinRight?.Dispose();
                TotalImagePinRight.GenEmptyObj();
            }
            else if (mode == 2)
            {
                if (cameraIndex == 1)
                {
                    ResultStatus1.Value = 2;
                    TotalImagePinLeft?.Dispose();
                    TotalImagePinLeft.GenEmptyObj();
                }
                else if (cameraIndex == 2)
                {
                    ResultStatus2.Value = 2;
                    TotalImagePinRight?.Dispose();
                    TotalImagePinRight.GenEmptyObj();
                }
            }

            mesResultStr1 = "";
            mesResultStr2 = "";

            int rows = 0, cols = 0;
            HImage image = new HImage();
            if (mode == 1)
            {
                HImage image1 = new HImage(); HImage image2 = new HImage(); HImage image3 = new HImage();

                int[,] rowArray = new int[64, 64];
                int[,] colArray = new int[64, 64];

                for (int i = 0; i < 2; i++)
                {
                    var p = Variables.CurrentProject.Programs.ElementAt(i).Value;
                    for (int j = 0; j < p.Count; j++)
                    {
                        rowArray[p.ElementAt(j).ProductConfigSet.RowInput, p.ElementAt(j).ProductConfigSet.ColInput] = p.ElementAt(j).ProductConfigSet.Row2 - p.ElementAt(j).ProductConfigSet.Row1;
                        colArray[p.ElementAt(j).ProductConfigSet.RowInput, p.ElementAt(j).ProductConfigSet.ColInput] = p.ElementAt(j).ProductConfigSet.Column2 - p.ElementAt(j).ProductConfigSet.Column1;
                    }
                }

                for (int i = 0; i < 64; i++)
                {
                    int tempRows = 0;
                    for (int j = 0; j < 64; j++)
                    {
                        tempRows += rowArray[j, i];
                    }

                    if (rows < tempRows)
                        rows = tempRows;
                }

                for (int i = 0; i < 64; i++)
                {
                    int tempColumns = 0;

                    for (int j = 0; j < 64; j++)
                    {
                        tempColumns += colArray[i, j];
                    }

                    if (cols < tempColumns)
                        cols = tempColumns;
                }

                image.GenImageConst("byte", (int)(multValue * cols),
                    (int)(multValue * rows));

                image1 = image.GenImageProto(new HTuple(128));
                image2 = image.GenImageProto(new HTuple(128));
                image3 = image.GenImageProto(new HTuple(128));
                TotalImageLeft = image1.Compose3(image2, image3);
            }
            else if (mode == 2)
            {
                if (cameraIndex == 1)
                {
                    HImage image1 = new HImage(); HImage image2 = new HImage(); HImage image3 = new HImage();
                    //左
                    var p = Variables.CurrentProject.Programs.ElementAt(0).Value;
                    int[,] rowArray = new int[64, 64];
                    int[,] colArray = new int[64, 64];
                    for (int j = 0; j < p.Count; j++)
                    {
                        rowArray[p.ElementAt(j).ProductConfigSet.RowInput, p.ElementAt(j).ProductConfigSet.ColInput] = p.ElementAt(j).ProductConfigSet.Row2 - p.ElementAt(j).ProductConfigSet.Row1;
                        colArray[p.ElementAt(j).ProductConfigSet.RowInput, p.ElementAt(j).ProductConfigSet.ColInput] = p.ElementAt(j).ProductConfigSet.Column2 - p.ElementAt(j).ProductConfigSet.Column1;
                    }

                    for (int i = 0; i < 64; i++)
                    {
                        int tempRows = 0;
                        for (int j = 0; j < 64; j++)
                        {
                            tempRows += rowArray[j, i];
                        }

                        if (rows < tempRows)
                            rows = tempRows;
                    }

                    for (int i = 0; i < 64; i++)
                    {
                        int tempColumns = 0;
                        for (int j = 0; j < 64; j++)
                        {
                            tempColumns += colArray[i, j];
                        }

                        if (cols < tempColumns)
                            cols = tempColumns;
                    }

                    image.GenImageConst("byte", (int)(multValue * cols),
                        (int)(multValue * rows));

                    image1 = image.GenImageProto(new HTuple(128));
                    image2 = image.GenImageProto(new HTuple(128));
                    image3 = image.GenImageProto(new HTuple(128));
                    TotalImageLeft = image1.Compose3(image2, image3);
                }
                else if (cameraIndex == 2)
                {
                    //右
                    HImage image1 = new HImage(); HImage image2 = new HImage(); HImage image3 = new HImage();
                    var p = Variables.CurrentProject.Programs.ElementAt(1).Value;
                    int[,] rowArray = new int[64, 64];
                    int[,] colArray = new int[64, 64];
                    for (int j = 0; j < p.Count; j++)
                    {
                        rowArray[p.ElementAt(j).ProductConfigSet.RowInput, p.ElementAt(j).ProductConfigSet.ColInput] = p.ElementAt(j).ProductConfigSet.Row2 - p.ElementAt(j).ProductConfigSet.Row1;
                        colArray[p.ElementAt(j).ProductConfigSet.RowInput, p.ElementAt(j).ProductConfigSet.ColInput] = p.ElementAt(j).ProductConfigSet.Column2 - p.ElementAt(j).ProductConfigSet.Column1;
                    }

                    for (int i = 0; i < 64; i++)
                    {
                        int tempRows = 0;
                        for (int j = 0; j < 64; j++)
                        {
                            tempRows += rowArray[j, i];
                        }

                        if (rows < tempRows)
                            rows = tempRows;
                    }

                    for (int i = 0; i < 64; i++)
                    {
                        int tempColumns = 0;
                        for (int j = 0; j < 64; j++)
                        {
                            tempColumns += colArray[i, j];
                        }

                        if (cols < tempColumns)
                            cols = tempColumns;
                    }

                    image.GenImageConst("byte", (int)(multValue * cols),
                        (int)(multValue * rows));

                    image1 = image.GenImageProto(new HTuple(128));
                    image2 = image.GenImageProto(new HTuple(128));
                    image3 = image.GenImageProto(new HTuple(128));
                    TotalImageRight = image1.Compose3(image2, image3);
                }
            }
        }

        //拼图和显示
        private async void showImage()
        {
            //HRegion rect = new HRegion(new HTuple(0), 0, Variables.ImageWidth - 1, Variables.ImageHeight - 1);
            //HTuple rows = new HTuple(), columns = new HTuple();
            //rect.GetRegionPoints(out rows, out columns);

            HTuple rows = new HTuple(), columns = new HTuple();
            while (true)
            {
                await Task.Delay(10);

                if (imageQueues.Count() > 0)
                {
                    var imageQueue = await imageQueues.Dequeue(imageQueues.tokenNone);
                    int rowCount = 0; int columnCount = 0;

                    if (imageQueue.PositionIndex <= 1)
                    {
                        isRun1PinTuDone = false;
                        isRun2PinTuDone = false;
                    }

                    await Task.Run(() =>
                    {
                        if (SystemConfig.IsSaveOriginalImage)
                        {
                            if (CameraMode == 1)
                                SaveImageRun(imageQueue.Image, Variables.SaveOriginalImagePath + Variables.Barcode1String[0] + "\\", imageQueue.CameraIndex + "_" + imageQueue.PositionIndex, "bmp");
                            else
                            {
                                if (imageQueue.CameraIndex == 1)
                                {
                                    SaveImageRun(imageQueue.Image, Variables.SaveOriginalImagePath + Variables.Barcode1String[0] + "\\", imageQueue.CameraIndex + "_" + imageQueue.PositionIndex, "bmp");
                                }
                                else if (imageQueue.CameraIndex == 2)
                                {
                                    SaveImageRun(imageQueue.Image, Variables.SaveOriginalImagePath + Variables.Barcode2String[0] + "\\", imageQueue.CameraIndex + "_" + imageQueue.PositionIndex, "bmp");
                                }
                            }
                        }
                        ConfigSet cs = new ConfigSet();
                        if (imageQueue.CameraIndex == 1)
                            cs = Variables.CurrentProject.Programs.ElementAt(0).Value[imageQueue.PositionIndex].ProductConfigSet;
                        else if (imageQueue.CameraIndex == 2)
                            cs = Variables.CurrentProject.Programs.ElementAt(1).Value[imageQueue.PositionIndex].ProductConfigSet;

                        HImage hImageTemp = imageQueue.Image.CropRectangle1((double)cs.Row1, cs.Column1,
                           cs.Row2, cs.Column2);
                        hImageTemp = hImageTemp.ZoomImageSize((int)((cs.Column2 - cs.Column1) * multValue), (int)((cs.Row2 - cs.Row1) * multValue), "constant");
                        var rect = hImageTemp.GetDomain();
                        rect.GetRegionPoints(out rows, out columns);
                        rect.Dispose();
                        var grayVals = hImageTemp.GetGrayval(rows, columns);

                        if (imageQueue.CameraIndex == 1)
                        {
                            rowCount = Variables.CurrentProject.Programs.ElementAt(0).Value[imageQueue.PositionIndex].ProductConfigSet.RowInput;
                            columnCount = Variables.CurrentProject.Programs.ElementAt(0).Value[imageQueue.PositionIndex].ProductConfigSet.ColInput;
                            if (imageQueue.PositionIndex == Variables.CurrentProject.Programs.ElementAt(0).Value.Count - 1)
                                isRun1PinTuDone = true;
                        }
                        else if (imageQueue.CameraIndex == 2)
                        {
                            rowCount = Variables.CurrentProject.Programs.ElementAt(1).Value[imageQueue.PositionIndex].ProductConfigSet.RowInput;
                            columnCount = Variables.CurrentProject.Programs.ElementAt(1).Value[imageQueue.PositionIndex].ProductConfigSet.ColInput;
                            if (imageQueue.PositionIndex == Variables.CurrentProject.Programs.ElementAt(1).Value.Count - 1)
                                isRun2PinTuDone = true;
                        }

                        if (CameraMode == 1)
                        {
                            int[,] rowArray = new int[64, 64];
                            int[,] colArray = new int[64, 64];

                            for (int i = 0; i < 2; i++)
                            {
                                var p = Variables.CurrentProject.Programs.ElementAt(i).Value;
                                for (int j = 0; j < p.Count; j++)
                                {
                                    rowArray[p.ElementAt(j).ProductConfigSet.RowInput, p.ElementAt(j).ProductConfigSet.ColInput] = p.ElementAt(j).ProductConfigSet.Row2 - p.ElementAt(j).ProductConfigSet.Row1;
                                    colArray[p.ElementAt(j).ProductConfigSet.RowInput, p.ElementAt(j).ProductConfigSet.ColInput] = p.ElementAt(j).ProductConfigSet.Column2 - p.ElementAt(j).ProductConfigSet.Column1;
                                }
                            }

                            int row = 0, column = 0;

                            for (int i = 0; i < rowCount; i++)
                            {
                                row += rowArray[i, columnCount];
                            }

                            for (int i = 0; i < columnCount; i++)
                            {
                                column += colArray[rowCount, i];
                            }

                            TotalImageLeft.SetGrayval(new HTuple((int)(row * multValue)) + rows,
                               new HTuple((int)(column * multValue)) + columns, grayVals);

                            if (imageQueues.Count() == 0 || imageQueues.Count() == 2 || imageQueues.Count() == 4 || imageQueues.Count() == 8)
                            {
                                Variables.WindowData1.WindowCtrl.ShowImageToWindow(TotalImageLeft.ZoomImageFactor(0.25, 0.25, "constant"));
                                Variables.WindowData2.WindowCtrl.ShowImageToWindow(TotalImageLeft.ZoomImageFactor(0.25, 0.25, "constant"));
                                Variables.WindowData1.WindowCtrl.FitImageToWindow();
                                Variables.WindowData2.WindowCtrl.FitImageToWindow();
                                Variables.WindowData1.WindowCtrl.Repaint();
                                Variables.WindowData2.WindowCtrl.Repaint();
                            }
                        }
                        else if (CameraMode == 2)
                        {
                            int[,] rowArray = new int[64, 64];
                            int[,] colArray = new int[64, 64];

                            if (imageQueue.CameraIndex == 1)
                            {
                                var p = Variables.CurrentProject.Programs.ElementAt(0).Value;
                                for (int j = 0; j < p.Count; j++)
                                {
                                    rowArray[p.ElementAt(j).ProductConfigSet.RowInput, p.ElementAt(j).ProductConfigSet.ColInput] = p.ElementAt(j).ProductConfigSet.Row2 - p.ElementAt(j).ProductConfigSet.Row1;
                                    colArray[p.ElementAt(j).ProductConfigSet.RowInput, p.ElementAt(j).ProductConfigSet.ColInput] = p.ElementAt(j).ProductConfigSet.Column2 - p.ElementAt(j).ProductConfigSet.Column1;
                                }

                                int row = 0, column = 0;

                                for (int i = 0; i < rowCount; i++)
                                {
                                    row += rowArray[i, columnCount];
                                }

                                for (int i = 0; i < columnCount; i++)
                                {
                                    column += colArray[rowCount, i];
                                }

                                TotalImageLeft.SetGrayval(new HTuple((int)(row * multValue)) + rows,
                                    new HTuple((int)(column * multValue)) + columns, grayVals);
                            }
                            else if (imageQueue.CameraIndex == 2)
                            {
                                var p = Variables.CurrentProject.Programs.ElementAt(1).Value;
                                for (int j = 0; j < p.Count; j++)
                                {
                                    rowArray[p.ElementAt(j).ProductConfigSet.RowInput, p.ElementAt(j).ProductConfigSet.ColInput] = p.ElementAt(j).ProductConfigSet.Row2 - p.ElementAt(j).ProductConfigSet.Row1;
                                    colArray[p.ElementAt(j).ProductConfigSet.RowInput, p.ElementAt(j).ProductConfigSet.ColInput] = p.ElementAt(j).ProductConfigSet.Column2 - p.ElementAt(j).ProductConfigSet.Column1;
                                }

                                int row = 0, column = 0;

                                for (int i = 0; i < rowCount; i++)
                                {
                                    row += rowArray[i, columnCount];
                                }

                                for (int i = 0; i < columnCount; i++)
                                {
                                    column += colArray[rowCount, i];
                                }

                                TotalImageRight.SetGrayval(new HTuple((int)(row * multValue)) + rows,
                                    new HTuple((int)(column * multValue)) + columns, grayVals);
                            }

                            if (imageQueues.Count() == 0 || imageQueues.Count() == 2 || imageQueues.Count() == 4 || imageQueues.Count() == 8)
                            {
                                if (!isIgnore1)
                                {
                                    Variables.WindowData1.WindowCtrl.ShowImageToWindow(TotalImageLeft.ZoomImageFactor(0.25, 0.25, "constant"));
                                    Variables.WindowData1.WindowCtrl.FitImageToWindow();
                                    Variables.WindowData1.WindowCtrl.Repaint();
                                }
                                if (!isIgnore2)
                                {
                                    Variables.WindowData2.WindowCtrl.ShowImageToWindow(TotalImageRight.ZoomImageFactor(0.25, 0.25, "constant"));
                                    Variables.WindowData2.WindowCtrl.FitImageToWindow();
                                    Variables.WindowData2.WindowCtrl.Repaint();
                                }
                            }
                        }

                        BingLibrary.Logs.LogOpreate.Info("待拼图数量" + imageQueues.Count());

                        if (isRun1PinTuDone && isRun2PinTuDone)
                        {
                            if (CameraMode == 1)
                            {
                                SaveImageRun(TotalImageLeft, Variables.SaveImagePath + Variables.Barcode1String[0] + "\\" + "ALL\\", "TotalImage", "jpg");
                            }
                            else if (CameraMode == 2)
                            {
                                if (!isIgnore1)
                                    SaveImageRun(TotalImageLeft, Variables.SaveImagePath + Variables.Barcode1String[0] + "\\" + "ALL\\", "TotalImageLeft", "jpg");
                                if (!isIgnore2)
                                    SaveImageRun(TotalImageRight, Variables.SaveImagePath + Variables.Barcode2String[0] + "\\" + "ALL\\", "TotalImageRight", "jpg");
                            }
                            TotalImageLeft?.Dispose();
                            TotalImageRight?.Dispose();

                            //isRun1PinTuDone = false;
                            //isRun2PinTuDone = false;
                            BingLibrary.Logs.LogOpreate.Info("保存拼图成功");
                        }
                    });

                    imageQueue.Image?.Dispose();
                }
            }
        }

        #endregion 0718修改

        private async Task<bool> process1(int position, HImage image, SubProgram sp, int count)
        {
            run1Results.Clear();
            if (Variables.CurrentProject.Programs.Count < 2)
                return false;
            if (position != sp.ProductIndex)
                Log.Warn(string.Format("【process1】检测位置不对应：{0} & {1}", position, sp.ProductIndex));

            if (position == 0)
            {
                await Task.Run(() =>
                {
                    var pd = sp.ProgramDatas[count];
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
                    else if (pd.InspectFunction == Functions.PIN针检测.ToDescription())
                    {
                        run1Results.Add(
                                Function_PINOneTool.Run(image, pd)
                               );
                    }
                    else if (pd.InspectFunction == Functions.视觉脚本.ToDescription())
                    {
                        run1Results.Add(
                                Function_ScriptTool.Run(image, pd)
                               );
                    }
                });
            }
            else
            {
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
                        else if (pd.InspectFunction == Functions.PIN针检测.ToDescription())
                        {
                            run1Results.Add(
                                    Function_PINOneTool.Run(image, pd)
                                   );
                        }
                        else if (pd.InspectFunction == Functions.视觉脚本.ToDescription())
                        {
                            run1Results.Add(
                                    Function_ScriptTool.Run(image, pd)
                                   );
                        }
                    }
                });
            }

            return true;
        }

        private async Task<bool> process2(int position, HImage image, SubProgram sp, int count)
        {
            run2Results.Clear();
            if (Variables.CurrentProject.Programs.Count < 2)
                return false;
            if (position != sp.ProductIndex)
                Log.Warn(string.Format("【process2】检测位置不对应：{0} & {1}", position, sp.ProductIndex));

            if (position == 0)
            {
                await Task.Run(() =>
                {
                    var pd = sp.ProgramDatas[count];
                    if (pd.InspectFunction == Functions.图像比对.ToDescription())
                    {
                        run2Results.Add(
                                   Function_MatchTool.Run(image, pd)
                                   );
                    }
                    else if (pd.InspectFunction == Functions.Blob分析.ToDescription())
                    {
                        run2Results.Add(
                                 Function_BlobTool.Run(image, pd)
                                 );
                    }
                    else if (pd.InspectFunction == Functions.条码识别.ToDescription())
                    {
                        run2Results.Add(
                                Function_CodeTool.Run(image, pd)
                               );
                    }
                    else if (pd.InspectFunction == Functions.PIN针检测.ToDescription())
                    {
                        run1Results.Add(
                                Function_PINOneTool.Run(image, pd)
                               );
                    }
                    else if (pd.InspectFunction == Functions.视觉脚本.ToDescription())
                    {
                        run1Results.Add(
                                Function_ScriptTool.Run(image, pd)
                               );
                    }
                });
            }
            else
            {
                await Task.Run(() =>
                {
                    foreach (var pd in sp.ProgramDatas)
                    {
                        if (pd.InspectFunction == Functions.图像比对.ToDescription())
                        {
                            run2Results.Add(
                                       Function_MatchTool.Run(image, pd)
                                       );
                        }
                        else if (pd.InspectFunction == Functions.Blob分析.ToDescription())
                        {
                            run2Results.Add(
                                     Function_BlobTool.Run(image, pd)
                                     );
                        }
                        else if (pd.InspectFunction == Functions.条码识别.ToDescription())
                        {
                            run2Results.Add(
                                    Function_CodeTool.Run(image, pd)
                                   );
                        }
                        else if (pd.InspectFunction == Functions.PIN针检测.ToDescription())
                        {
                            run1Results.Add(
                                    Function_PINOneTool.Run(image, pd)
                                   );
                        }
                        else if (pd.InspectFunction == Functions.视觉脚本.ToDescription())
                        {
                            run1Results.Add(
                                    Function_ScriptTool.Run(image, pd)
                                   );
                        }
                    }
                });
            }

            return true;
        }

        private async Task<bool> process3(int position, HImage image, SubProgram sp)
        {
            run3Results.Clear();
            if (Variables.CurrentProject.Programs.Count < 4)
                return false;
            if (position != sp.ProductIndex)
                Log.Warn(string.Format("【process3】检测位置不对应：{0} & {1}", position, sp.ProductIndex));

            await Task.Run(() =>
            {
                for (int i = 0; i < sp.ProgramDatas.Count; i++)
                {
                    if (sp.ProgramDatas[i].InspectFunction == Functions.图像比对.ToDescription())
                    {
                        run3Results.Add(
                                   Function_MatchTool.Run(image, sp.ProgramDatas[i])
                                   );
                    }
                    else if (sp.ProgramDatas[i].InspectFunction == Functions.Blob分析.ToDescription())
                    {
                        run3Results.Add(
                                 Function_BlobTool.Run(image, sp.ProgramDatas[i])
                                 );
                    }
                    else if (sp.ProgramDatas[i].InspectFunction == Functions.条码识别.ToDescription())
                    {
                        run3Results.Add(
                                Function_CodeTool.Run(image, sp.ProgramDatas[i])
                               );
                    }
                    else if (sp.ProgramDatas[i].InspectFunction == Functions.PIN针检测.ToDescription())
                    {
                        run3Results.Add(
                                Function_PINOneTool.Run(image, sp.ProgramDatas[i])
                               );
                    }
                    else if (sp.ProgramDatas[i].InspectFunction == Functions.视觉脚本.ToDescription())
                    {
                        run3Results.Add(
                                Function_ScriptTool.Run(image, sp.ProgramDatas[i])
                               );
                    }
                }
            });
            return true;
        }

        private async Task<bool> process4(int position, HImage image, SubProgram sp)
        {
            run4Results.Clear();
            if (Variables.CurrentProject.Programs.Count < 4)
                return false;
            if (position != sp.ProductIndex)
                Log.Warn(string.Format("【process4】检测位置不对应：{0} & {1}", position, sp.ProductIndex));

            await Task.Run(() =>
            {
                for (int i = 0; i < sp.ProgramDatas.Count; i++)
                {
                    if (sp.ProgramDatas[i].InspectFunction == Functions.图像比对.ToDescription())
                    {
                        run4Results.Add(
                                   Function_MatchTool.Run(image, sp.ProgramDatas[i])
                                   );
                    }
                    else if (sp.ProgramDatas[i].InspectFunction == Functions.Blob分析.ToDescription())
                    {
                        run4Results.Add(
                                 Function_BlobTool.Run(image, sp.ProgramDatas[i])
                                 );
                    }
                    else if (sp.ProgramDatas[i].InspectFunction == Functions.条码识别.ToDescription())
                    {
                        run4Results.Add(
                                Function_CodeTool.Run(image, sp.ProgramDatas[i])
                               );
                    }
                    else if (sp.ProgramDatas[i].InspectFunction == Functions.PIN针检测.ToDescription())
                    {
                        run4Results.Add(
                                Function_PINOneTool.Run(image, sp.ProgramDatas[i])
                               );
                    }
                    else if (sp.ProgramDatas[i].InspectFunction == Functions.视觉脚本.ToDescription())
                    {
                        run4Results.Add(
                                Function_ScriptTool.Run(image, sp.ProgramDatas[i])
                               );
                    }
                }
            });
            return true;
        }

        //csv保存在D盘
        private void saveResultToCSV(string _result, string _Barcode, string _yuangong, string _machine, string _path, string _detial)
        {
            string filepathDate = System.DateTime.Now.ToString("yyyy/MM/dd");
            string nowtime = System.DateTime.Now.ToString("HH:mm:ss");
            string filePath = "D:\\AVI报表\\";
            string fileName = getBanci() + ".csv";

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            try
            {
                if (!File.Exists(filePath + fileName))
                {
                    string[] heads = { "检测日期", "检测时间", "状态", "员工号", "机台编号", "大板码", "不良存图", "详细信息" };
                    Csvfile.AddNewLine(filePath + fileName, heads);
                }
                string[] conte = { filepathDate, nowtime, _result, _yuangong, _machine, _Barcode, _path, _detial };
                Csvfile.AddNewLine(filePath + fileName, conte);
            }
            catch (Exception ex)
            {
            }
        }

        private string getBanci()
        {
            string rs = "";
            if (DateTime.Now.Hour >= 8 && DateTime.Now.Hour < 20)
            {
                rs += DateTime.Now.ToString("yyyyMMdd") + "Day";
            }
            else
            {
                if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 8)
                {
                    rs += DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "Night";
                }
                else
                {
                    rs += DateTime.Now.ToString("yyyyMMdd") + "Night";
                }
            }
            return rs;
        }
    }
}