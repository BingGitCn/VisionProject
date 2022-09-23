using BingLibrary.Tools;
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
    public partial class MainWindow : HandyControl.Controls.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //非商业授权
            GlobalTools.EnableNonCommercial();
            //日志
            GlobalTools.EnableLog();
            //全局捕获异常
            GlobalTools.EnableGlobalCatchException();
            //单例模式，不允许多开
            GlobalTools.EnableSingleton();
            //线程更新ui
            GlobalTools.EnableOnUIThread();

            //excel授权
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            //给图像控件windowdata全局
            Variables.WindowData1 = ImageWindow1.windowData;

            //regionManager.RegisterViewWithRegion(GlobalPrism.RegionNames.StatusRegionName, typeof(Status));
            //GlobalVars.Variables.ImageWindowData = imageWin.windowData;

            //获取统计图表
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