using BingLibrary.Tools;
using OfficeOpenXml;
using Prism.Regions;
using System;
using System.Windows;
using VisionProject.GlobalVars;

namespace VisionProject.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            //Step..
            if (true)//是否显示启动动画
            {
                StartLogo.MainWindow startLogo = new StartLogo.MainWindow("视觉检测软件");
                startLogo.Show();
            }



            //excel授权
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //给图像控件windowdata全局
            Variables.WindowData1 = ImageWindow1.windowData;

            //regionManager.RegisterViewWithRegion(GlobalPrism.RegionNames.StatusRegionName, typeof(Status));
            //GlobalVars.Variables.ImageWindowData = imageWin.windowData;

            //主线程上更新
            UIThread.InitializeWithDispatcher();
            //UIThread.OnUIThread(new Action(() => {
            //    //do something
            //}));

            string MName = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
            string PName = System.IO.Path.GetFileNameWithoutExtension(MName);
            System.Diagnostics.Process[] myProcess = System.Diagnostics.Process.GetProcessesByName(PName);

            if (myProcess.Length > 1)
            {
                MessageBox.Show("本程序一次只能运行一个实例！", "提示");
                Application.Current.Shutdown();
                return;
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            if (Variables.ShowConfirm("确认退出软件？") == false)
                return;
            else
                Environment.Exit(0);
        }
    }
}