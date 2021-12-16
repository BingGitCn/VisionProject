using Prism.Commands;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using VisionProject.GlobalVars;

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

            Variables.Logs.WriteInfo(Variables.HCPLC.IsConnected ? "PLC连接成功" : "PLC连接失败");
            PLCStatus.Value = Variables.HCPLC.IsConnected;

        }

        #endregion 初始化

        //运行
        private async void run()
        {
            while (true)
            {
                await Task.Delay(100);



            }
        }



        private DelegateCommand _test;
        public DelegateCommand Test =>
            _test ?? (_test = new DelegateCommand(()=> {

                //Do Something..
               


            }));

       



















    }
}