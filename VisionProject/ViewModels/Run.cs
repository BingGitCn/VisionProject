using Prism.Commands;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using VisionProject.GlobalVars;
using System.Linq;
using System.Collections.ObjectModel;
using BingLibrary.Logs;
using System.Collections.Generic;

namespace VisionProject.ViewModels
{
    public partial class MainWindowViewModel
    {
        #region 初始化

       

        private void initAll()
        {
            initStatistic();
            getSystemConfig();
            initFreeSpace();
            Variables.Title = SystemConfig.Title;
            Title = Variables.Title;
            

            //测试
            //IsLogin = true;

            initProjects();
            initPLC();


            run();
        }


        private async void initFreeSpace()
        {
            while (true)
            {
                await Task.Delay(5000);
                try {
                 FreeSpace = (SystemConfig.HardDrive == 0 ? "C:\\"
                        : SystemConfig.HardDrive == 1 ? "D:\\"
                         : SystemConfig.HardDrive == 2 ? "E:\\"
                          : SystemConfig.HardDrive == 3 ? "F:\\"
                           : SystemConfig.HardDrive == 4 ? "G:\\"
                        : "C:\\")+"可用容量："+
                        Variables.GetFreeSpace(SystemConfig.HardDrive == 0 ? "C:\\"
                        :SystemConfig.HardDrive == 1 ? "D:\\"
                         : SystemConfig.HardDrive == 2 ? "E:\\"
                          : SystemConfig.HardDrive == 3 ? "F:\\"
                           : SystemConfig.HardDrive == 4 ? "G:\\"
                        : "C:\\") ;

                    double freeSpace = Variables.GetFreeSpaceRateValue(SystemConfig.HardDrive == 0 ? "C:\\"
                        : SystemConfig.HardDrive == 1 ? "D:\\"
                         : SystemConfig.HardDrive == 2 ? "E:\\"
                          : SystemConfig.HardDrive == 3 ? "F:\\"
                           : SystemConfig.HardDrive == 4 ? "G:\\"
                        : "C:\\"); 


                    HardWareStatus.Value = freeSpace >= SystemConfig.FreeSpace ? true : false;


                } catch { }
            }
        
        }


        //PLC初始化
        private async void initPLC()
        {
            //初始化PLC
            Variables.HCPLC.Init("192.168.1.10", 502);
            await Task.Delay(1000);
            for (int i = 0; i < 10; i++)
            {
                if (Variables.HCPLC.IsConnected)
                    break;
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

            List<string> ToolNames = DialogNames.ToolNams.Keys.ToList();
            while (true)
            {
                await Task.Delay(100);

                if (false)
                {
                    int index = 1;//产品位置索引
                    var currentProduct = (from step in Programs1
                                          where step.ProductIndex == index
                                          select step)
                                          .ToList();

                    for (int i = 0; i < currentProduct.Count-1; i++)
                    {
                        if (currentProduct[i].InspectFunction == ToolNames[0])
                        {
                            //Do Something
                            //Function_SaveImageViewModel.SaveImages(new HalconDotNet.HImage(),"123", currentProduct[i].Parameters);
                        }
                  
                       else if (currentProduct[i].InspectFunction == ToolNames[1])
                        {
                            //Do Something
                        
                        }


                    }

                    

                }
               
                                   


            }
        }



        private DelegateCommand _test;
        public DelegateCommand Test =>
            _test ?? (_test = new DelegateCommand(()=> {

                //Do Something..
               


            }));

       



















    }
}