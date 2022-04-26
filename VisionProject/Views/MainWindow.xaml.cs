﻿using BingLibrary.Tools;
using OfficeOpenXml;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.IO;
using System.Windows;
using VisionProject.GlobalVars;

namespace VisionProject.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string MName = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
            string PName = System.IO.Path.GetFileNameWithoutExtension(MName);
            System.Diagnostics.Process[] myProcess = System.Diagnostics.Process.GetProcessesByName(PName);
            if (myProcess.Length > 1)
            {
                MessageBox.Show("本程序一次只能运行一个实例！", "提示");
                Application.Current.Shutdown();
                return;
            }

            //Step..
            if (false)//是否显示启动动画
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

            getPlot(7);
        }

        private void getPlot(int days)
        {
            try
            {
                string plotFile = AppDomain.CurrentDomain.BaseDirectory + "plot.xlsx";
                File.Copy(Variables.StatisticDataFilePath, AppDomain.CurrentDomain.BaseDirectory + "plot.xlsx", true);
                FileInfo fileInfo = new FileInfo(plotFile);
                ExcelPackage package = new ExcelPackage(fileInfo);
                var w = package.Workbook.Worksheets[0];
                w.Protection.IsProtected = false;

                PlotModel plotModel = new PlotModel { Title = "最近 " + days + " 天" };

                var l = new Legend
                {
                    LegendPosition = LegendPosition.RightTop,
                    LegendPlacement = LegendPlacement.Outside
                };

                plotModel.Legends.Add(l);

                int rowCount = 0;
                try
                {
                    rowCount = w.Dimension.Rows;
                }
                catch { }

                int realDays = 0;
                if (rowCount - 5 > days)
                    realDays = 5 + days;
                else
                    realDays = rowCount;

                var line1 = new LineSeries() { Title = "OK 数量", Color = OxyColors.Green, MarkerType = MarkerType.Circle };
                var line2 = new LineSeries() { Title = "NG 数量", Color = OxyColors.Red, MarkerType = MarkerType.Circle };

                for (int i = 5; i <= realDays; i++)
                {
                    var ok = double.Parse(w.Cells[i, 2].Value.ToString());
                    var ng = double.Parse(w.Cells[i, 3].Value.ToString());
                    line1.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(4 - i)), ok));
                    line2.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(4 - i)), ng));
                }
                var startDate = DateTime.Now.AddDays(-days);
                var endDate = DateTime.Now;

                var minValue = DateTimeAxis.ToDouble(startDate);
                var maxValue = DateTimeAxis.ToDouble(endDate);

                plotModel.Series.Add(line1);
                plotModel.Series.Add(line2);
                plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minValue, Maximum = maxValue, StringFormat = "M/d" });

                plot.Model = plotModel;
            }
            catch { }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            if (Variables.ShowConfirm("确认退出软件？") == false)
                return;
            else
                Environment.Exit(0);
        }

        private void plotDays_DropDownClosed(object sender, EventArgs e)
        {
            if (plotDays.SelectedIndex == 0)
                getPlot(7);
            else if (plotDays.SelectedIndex == 1)
                getPlot(15);
            else if (plotDays.SelectedIndex == 2)
                getPlot(30);
        }
    }
}