using System.Threading.Tasks;
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
            Variables.Title = SystemConfig.Title;
            Title = Variables.Title;
            if (SystemConfig.Password != "")
                Variables.CurrentPassword = SystemConfig.Password;
            else
                Variables.CurrentPassword = "123456";

            //测试
            //IsLogin = true;

            initProjects();
            initPLC();
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
    }
}