using BingLibrary.Vision.Engine;
using HalconDotNet;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace BingLibrary.Vision.Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private BingImageWindowData windowData1 = new BingImageWindowData();
        private BingImageWindowData windowData2 = new BingImageWindowData();

        public MainWindow()
        {
            InitializeComponent();
            windowData1 = win1.windowData;
            windowData2 = win2.windowData;

            workerEngine1.AddProcedure("TestEngine1", AppDomain.CurrentDomain.BaseDirectory + "Engine1");
            workerEngine2.AddProcedure("TestEngine2", AppDomain.CurrentDomain.BaseDirectory + "Engine2");
        }

        private WorkerEngine workerEngine1 = new WorkerEngine();
        private WorkerEngine workerEngine2 = new WorkerEngine();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            run1();
            run2();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            windowData1.MessageCtrl.AddMessageVar("hello", 100, 100);

            HRegion hRegion = new HRegion(200.0, 200, 300, 300);
            windowData1.DispObjectCtrl.AddDispObjectVar(hRegion);

            windowData1.WindowCtrl.Repaint();
        }

        private async void run1()
        {
            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(1000);
                workerEngine1.SetParam<HTuple>("TestEngine1", "A", 10);
                workerEngine1.SetParam<HTuple>("TestEngine1", "B", 20);
                workerEngine1.InspectProcedure("TestEngine1");
                var c = workerEngine1.GetParam<HTuple>("TestEngine1", "C");
            }
        }

        private async void run2()
        {
            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(1000);
                workerEngine2.SetParam<HTuple>("TestEngine2", "A", 10);
                workerEngine2.SetParam<HTuple>("TestEngine2", "B", 20);
                workerEngine2.InspectProcedure("TestEngine2");
                var c = workerEngine2.GetParam<HTuple>("TestEngine2", "C");
            }
        }
    }
}