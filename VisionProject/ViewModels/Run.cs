﻿using BingLibrary.Logs;
using Prism.Commands;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisionProject.GlobalVars;
using Log = BingLibrary.Logs.LogOpreate;

namespace VisionProject.ViewModels
{
    public partial class MainWindowViewModel
    {
        #region 初始化

        private void initAll()
        {
            //获取数据统计
            initStatistic();
            //获取系统配置
            getSystemConfig();
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
        }

        private void initEngine()
        {
            try
            {
                //这里自动获取目录下的脚本，添加至引擎
                for (int i = 0; i < Variables.V2Engines.Count; i++)
                {
                    var folder = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory + "Projects\\Scripts" + i);
                    var procedureFiles = folder.GetFiles("*.hdvp");
                    foreach (var procedureFile in procedureFiles)
                        Variables.V2Engines[i].AddProcedure(procedureFile.Name.Replace(".hdvp", ""));
                    //也可自己添加
                    //Variables.V2Engine.AddProcedure("lvba");
                    //Variables.V2Engine.AddProcedure("lvba2");
                    //先添加脚本，再初始化，
                    Variables.V2Engines[i].Init(System.AppDomain.CurrentDomain.BaseDirectory + "Projects\\Scripts" + i);
                }
            }
            catch { }
        }

        private async void initFreeSpace()
        {
            while (true)
            {
                await Task.Delay(5000);
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

            await Task.Delay(1000);
            for (int i = 0; i < 10; i++)
            {
                if (Variables.HCPLC.IsConnected)
                    break;
                Variables.HCPLC.Close();
                await Task.Run(() =>
                {
                    Variables.HCPLC.Init("127.0.0.1", 502, 01);
                });

                await Task.Delay(1000);
            }

            Log.Info(Variables.HCPLC.IsConnected ? "PLC连接成功" : "PLC连接失败");
            PLCStatus.Value = Variables.HCPLC.IsConnected;
        }

        #endregion 初始化

        //运行
        //这里设想，如果一个产品多个位置，默认产品索引位置为0不容更改，依次调用程序列表对应的位置进行处理即可。
        //如果有多个产品，且一个产品需分多次检测，可以设置产品的索引号，同一索引号依次处理。
        private async void run()
        {
            //Step..
            List<string> ToolNames = DialogNames.ToolNams.Keys.ToList();
            while (true)
            {
                await Task.Delay(100);

                if (false)
                {
                    //这里需要判断下程序数量
                    if (Variables.CurrentProject.Programs.Count == 0)
                        continue;

                    //选择程序。这里默认0，第一个
                    var program = Variables.CurrentProject.Programs.ElementAt(0).Value;
                    //选择对应产品的所有检测子程序。这里可以在打开项目的时候获取一次，无需每次执行获取。
                    int index = 1;//产品位置索引，这里通过索引来选择，也可以通过其它的，如ToolNames名称等。
                    var currentProduct = (from step in program
                                          where step.ProductIndex == index
                                          select step)
                                          .ToList();

                    for (int i = 0; i < currentProduct.Count - 1; i++)
                    {
                        if (currentProduct[i].InspectFunction == ToolNames[0])
                        {
                            //这里可以再对应的工具里面写好对应的静态执行方法，直接调用
                            //Do Something
                            //Function_SaveImageViewModel.SaveImages(new HalconDotNet.HImage(),"123", currentProduct[i].Parameters);
                        }
                        else if (currentProduct[i].InspectFunction == ToolNames[1])
                        {
                            //Do Something
                        }
                    }

                    //结果OK时使用
                    setOK();
                    //结果NG时使用
                    setNG();
                }
            }
        }

        private DelegateCommand _test;

        public DelegateCommand Test =>
            _test ?? (_test = new DelegateCommand(() =>
            {
                //Do Something..
            }));
    }
}