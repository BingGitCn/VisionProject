using BingLibrary.Logs;
using BingLibrary.Tools;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using VisionProject.GlobalVars;
using BingLibrary.Extension;
using OxyPlot;
using OxyPlot.Series;
using Log = BingLibrary.Logs.LogOpreate;

namespace VisionProject.ViewModels
{
    public partial class MainWindowViewModel : BindableBase
    {
        //设备运行状态，可用于展示PLC的运行，待机，故障等。
        private string _machineStatus = "待机中";

        public string MachineStatus
        {
            get { return _machineStatus; }
            set { SetProperty(ref _machineStatus, value); }
        }

        private string _title = "";

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel(IDialogService dialogService)
        {
            Variables.CurDialogService = dialogService;
            initAll();
        }

        #region 底部状态显示ConnectStatus

        private ShowStatus _plcStatus = new ShowStatus();

        public ShowStatus PLCStatus
        {
            get { return _plcStatus; }
            set { SetProperty(ref _plcStatus, value); }
        }

        private ShowStatus _hardWareStatus = new ShowStatus();

        public ShowStatus HardWareStatus
        {
            get { return _hardWareStatus; }
            set { SetProperty(ref _hardWareStatus, value); }
        }

        #endregion 底部状态显示ConnectStatus

        #region 数据统计

        private ShowStatus _resultStatus = new ShowStatus() { Value = true };

        public ShowStatus ResultStatus
        {
            get { return _resultStatus; }
            set { SetProperty(ref _resultStatus, value); }
        }

        //数据类
        public class StatisticData : BindableBase
        {
            private string _currentDate;

            public string CurrentDate
            {
                get { return _currentDate; }
                set { SetProperty(ref _currentDate, value); }
            }

            private double _ok;

            public double OK
            {
                get { return _ok; }
                set { SetProperty(ref _ok, value); }
            }

            private double _ng;

            public double NG
            {
                get { return _ng; }
                set { SetProperty(ref _ng, value); }
            }

            private double _all;

            public double All
            {
                get { return _all; }
                set { SetProperty(ref _all, value); }
            }

            private string _rate;

            public string Rate
            {
                get { return _rate; }
                set { SetProperty(ref _rate, value); }
            }
        }

        private ObservableCollection<StatisticData> _homeStatisticData = new ObservableCollection<StatisticData>();

        public ObservableCollection<StatisticData> HomeStatisticData
        {
            get { return _homeStatisticData; }
            set { SetProperty(ref _homeStatisticData, value); }
        }

        private ObservableCollection<StatisticData> _allStatisticData = new ObservableCollection<StatisticData>();

        public ObservableCollection<StatisticData> AllStatisticData
        {
            get { return _allStatisticData; }
            set { SetProperty(ref _allStatisticData, value); }
        }

        //初始化统计数据
        private async void initStatistic()
        {
            await 100;

            try
            {
                if (!File.Exists(Variables.StatisticDataFilePath))
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "n.xlsx", Variables.StatisticDataFilePath);
                FileInfo fileInfo = new FileInfo(Variables.StatisticDataFilePath);
                ExcelPackage package = new ExcelPackage(fileInfo);
                var w = package.Workbook.Worksheets[0];
                w.Protection.IsProtected = false;
                w.DefaultColWidth = 20;

                int rowCount = 1;
                try
                {
                    rowCount = w.Dimension.Rows + 1;
                }
                catch { }
                if (rowCount == 1)
                {
                    w.Cells[1, 1, 1, 5].Merge = true;
                    w.Cells[1, 1].Style.Font.Size = 18;
                    w.Cells[1, 1].Style.Font.Bold = true;
                    w.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    w.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    w.Cells[1, 1].Value = "数据统计";

                    rowCount = 2;

                    w.Cells[rowCount, 1].Value = "时间"; w.Cells[rowCount, 1].Style.Font.Bold = true;
                    w.Cells[rowCount, 2].Value = "良品"; w.Cells[rowCount, 2].Style.Font.Bold = true;
                    w.Cells[rowCount, 3].Value = "不良"; w.Cells[rowCount, 3].Style.Font.Bold = true;
                    w.Cells[rowCount, 4].Value = "总计"; w.Cells[rowCount, 4].Style.Font.Bold = true;
                    w.Cells[rowCount, 5].Value = "良率"; w.Cells[rowCount, 5].Style.Font.Bold = true;
                    rowCount = 3;

                    w.Cells[rowCount, 1].Value = "当前";
                    w.Cells[rowCount, 2].Value = 0;
                    w.Cells[rowCount, 3].Value = 0;
                    w.Cells[rowCount, 4].Value = 0;
                    w.Cells[rowCount, 5].Value = "0%";

                    rowCount = 4;

                    w.Cells[rowCount, 1].Value = DateTime.Now.ToString("yyyy/MM/dd");
                    w.Cells[rowCount, 2].Value = 0;
                    w.Cells[rowCount, 3].Value = 0;
                    w.Cells[rowCount, 4].Value = 0;
                    w.Cells[rowCount, 5].Value = "0%";
                }
                else
                {
                    if (w.Cells[4, 1].Value.ToString() != DateTime.Now.ToString("yyyy/MM/dd"))
                    {
                        w.InsertRow(4, 1);
                        rowCount = 4;

                        w.Cells[rowCount, 1].Value = DateTime.Now.ToString("yyyy/MM/dd");
                        w.Cells[rowCount, 2].Value = 0;
                        w.Cells[rowCount, 3].Value = 0;
                        w.Cells[rowCount, 4].Value = 0;
                        w.Cells[rowCount, 5].Value = "0%";
                    }
                }

                HomeStatisticData.Clear();
                for (int i = 3; i <= 4; i++)
                {
                    var sd = new StatisticData();
                    sd.CurrentDate = w.Cells[i, 1].Value.ToString();
                    sd.OK = double.Parse(w.Cells[i, 2].Value.ToString());
                    sd.NG = double.Parse(w.Cells[i, 3].Value.ToString());
                    sd.All = double.Parse(w.Cells[i, 4].Value.ToString());
                    sd.Rate = w.Cells[i, 5].Value.ToString();
                    HomeStatisticData.Add(sd);
                }

                AllStatisticData.Clear();
                try
                {
                    rowCount = w.Dimension.Rows;
                }
                catch { }

                for (int i = 3; i <= rowCount; i++)
                {
                    var sd = new StatisticData();
                    sd.CurrentDate = w.Cells[i, 1].Value.ToString();
                    sd.OK = double.Parse(w.Cells[i, 2].Value.ToString());
                    sd.NG = double.Parse(w.Cells[i, 3].Value.ToString());
                    sd.All = double.Parse(w.Cells[i, 4].Value.ToString());
                    sd.Rate = w.Cells[i, 5].Value.ToString();
                    AllStatisticData.Add(sd);
                }

                //锁住
                w.Protection.IsProtected = true;
                package.Save();
                package.Dispose();

                Log.Info("数据加载成功。");
            }
            catch { Log.Error("数据加载失败。"); }
        }

        /// <summary>
        /// 检测成功时调用
        /// </summary>
        private void setOK()
        {
            ResultStatus.Value = true;
            try
            {
                FileInfo fileInfo = new FileInfo(Variables.StatisticDataFilePath);
                ExcelPackage package = new ExcelPackage(fileInfo);
                var w = package.Workbook.Worksheets[0];
                w.Protection.IsProtected = false;
                w.DefaultColWidth = 20;

                int rowCount = 4;

                if (w.Cells[4, 1].Value.ToString() != DateTime.Now.ToString("yyyy/MM/dd"))
                {
                    w.InsertRow(4, 1);
                    rowCount = 4;

                    w.Cells[rowCount, 1].Value = DateTime.Now.ToString("yyyy/MM/dd");
                    w.Cells[rowCount, 2].Value = 0;
                    w.Cells[rowCount, 3].Value = 0;
                    w.Cells[rowCount, 4].Value = 0;
                    w.Cells[rowCount, 5].Value = "0%";
                }

                w.Cells[3, 2].Value = double.Parse(w.Cells[3, 2].Value.ToString()) + 1;
                w.Cells[3, 4].Value = double.Parse(w.Cells[3, 2].Value.ToString()) + double.Parse(w.Cells[3, 3].Value.ToString());
                w.Cells[3, 5].Value = (double.Parse(w.Cells[3, 2].Value.ToString()) / double.Parse(w.Cells[3, 4].Value.ToString()) * 100.0).ToString("f2") + "%";

                w.Cells[4, 2].Value = double.Parse(w.Cells[4, 2].Value.ToString()) + 1;
                w.Cells[4, 4].Value = double.Parse(w.Cells[4, 2].Value.ToString()) + double.Parse(w.Cells[4, 3].Value.ToString());
                w.Cells[4, 5].Value = (double.Parse(w.Cells[4, 2].Value.ToString()) / double.Parse(w.Cells[4, 4].Value.ToString()) * 100.0).ToString("f2") + "%";

                HomeStatisticData.Clear();
                for (int i = 3; i <= 4; i++)
                {
                    var sd = new StatisticData();
                    sd.CurrentDate = w.Cells[i, 1].Value.ToString();
                    sd.OK = double.Parse(w.Cells[i, 2].Value.ToString());
                    sd.NG = double.Parse(w.Cells[i, 3].Value.ToString());
                    sd.All = double.Parse(w.Cells[i, 4].Value.ToString());
                    sd.Rate = w.Cells[i, 5].Value.ToString();
                    HomeStatisticData.Add(sd);
                }

                //锁住
                w.Protection.IsProtected = true;
                package.Save();
                package.Dispose();
            }
            catch { }
        }

        /// <summary>
        /// 检测失败是调用
        /// </summary>
        private void setNG()
        {
            ResultStatus.Value = false;
            try
            {
                FileInfo fileInfo = new FileInfo(Variables.StatisticDataFilePath);
                ExcelPackage package = new ExcelPackage(fileInfo);
                var w = package.Workbook.Worksheets[0];
                w.Protection.IsProtected = false;
                w.DefaultColWidth = 20;

                int rowCount = 4;

                if (w.Cells[4, 1].Value.ToString() != DateTime.Now.ToString("yyyy/MM/dd"))
                {
                    w.InsertRow(4, 1);
                    rowCount = 4;

                    w.Cells[rowCount, 1].Value = DateTime.Now.ToString("yyyy/MM/dd");
                    w.Cells[rowCount, 2].Value = 0;
                    w.Cells[rowCount, 3].Value = 0;
                    w.Cells[rowCount, 4].Value = 0;
                    w.Cells[rowCount, 5].Value = "0%";
                }

                w.Cells[3, 3].Value = double.Parse(w.Cells[3, 3].Value.ToString()) + 1;
                w.Cells[3, 4].Value = double.Parse(w.Cells[3, 2].Value.ToString()) + double.Parse(w.Cells[3, 3].Value.ToString());
                w.Cells[3, 5].Value = (double.Parse(w.Cells[3, 2].Value.ToString()) / double.Parse(w.Cells[3, 4].Value.ToString()) * 100.0).ToString("f2") + "%";

                w.Cells[4, 3].Value = double.Parse(w.Cells[4, 3].Value.ToString()) + 1;
                w.Cells[4, 4].Value = double.Parse(w.Cells[4, 2].Value.ToString()) + double.Parse(w.Cells[4, 3].Value.ToString());
                w.Cells[4, 5].Value = (double.Parse(w.Cells[4, 2].Value.ToString()) / double.Parse(w.Cells[4, 4].Value.ToString()) * 100.0).ToString("f2") + "%";

                HomeStatisticData.Clear();
                for (int i = 3; i <= 4; i++)
                {
                    var sd = new StatisticData();
                    sd.CurrentDate = w.Cells[i, 1].Value.ToString();
                    sd.OK = double.Parse(w.Cells[i, 2].Value.ToString());
                    sd.NG = double.Parse(w.Cells[i, 3].Value.ToString());
                    sd.All = double.Parse(w.Cells[i, 4].Value.ToString());
                    sd.Rate = w.Cells[i, 5].Value.ToString();
                    HomeStatisticData.Add(sd);
                }

                //锁住
                w.Protection.IsProtected = true;
                package.Save();
                package.Dispose();
            }
            catch { }
        }

        //数据操作，清除，刷新，导出
        private DelegateCommand<string> _statisticOperate;

        public DelegateCommand<string> StatisticOperate =>
            _statisticOperate ?? (_statisticOperate = new DelegateCommand<string>((string param) =>
            {
                switch (param)
                {
                    case "clear":

                        bool rst = Variables.ShowConfirm("确认清除当前计数？");
                        if (rst)
                        {
                            try
                            {
                                FileInfo fileInfo = new FileInfo(Variables.StatisticDataFilePath);
                                ExcelPackage package = new ExcelPackage(fileInfo);
                                var w = package.Workbook.Worksheets[0];
                                w.Protection.IsProtected = false;
                                w.DefaultColWidth = 20;

                                w.Cells[3, 2].Value = 0;
                                w.Cells[3, 3].Value = 0;
                                w.Cells[3, 4].Value = 0;
                                w.Cells[3, 5].Value = "0%";

                                //锁住
                                w.Protection.IsProtected = true;
                                package.Save();
                                package.Dispose();
                            }
                            catch { }
                            initStatistic();
                        }

                        break;

                    case "refresh":
                        initStatistic();
                        break;

                    case "export":
                        System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                        folderBrowserDialog.Description = "请选择导出的文件路径";
                        var dlg = folderBrowserDialog.ShowDialog();
                        if (dlg == System.Windows.Forms.DialogResult.OK)
                        {
                            string path = folderBrowserDialog.SelectedPath + "\\";
                            try
                            {
                                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "Statistics.xlsx", path + "产量统计.xlsx");
                                Log.Info("导出产量统计成功。");
                                Variables.ShowMessage("导出成功。");
                            }
                            catch { Log.Error("导出产量统计失败。"); }
                        }
                        break;
                }
            }));

        #endregion 数据统计

        #region 底部Status

        private string _freeSpace;

        public string FreeSpace
        {
            get { return _freeSpace; }
            set { SetProperty(ref _freeSpace, value); }
        }

        private string version = Application.ResourceAssembly.GetName().Version.ToString();

        public string Version
        {
            get { return version; }
            set { SetProperty(ref version, value); }
        }

        private string supplier = "Leader";

        public string Supplier
        {
            get { return supplier; }
            set { SetProperty(ref supplier, value); }
        }

        private DelegateCommand _showAboutDialog;

        public DelegateCommand ShowAboutDialog =>
            _showAboutDialog ?? (_showAboutDialog = new DelegateCommand(() =>
            {
                Variables.CurDialogService.ShowDialog(GlobalVars.DialogNames.ShowAboutWindow);
            }));

        #endregion 底部Status

        #region 登录密码

        private PermitLevel _currentPermit;

        public PermitLevel CurrentPermit
        {
            get { return _currentPermit; }
            set { SetProperty(ref _currentPermit, value); }
        }

        private int _userIndex = 0;

        public int UserIndex
        {
            get { return _userIndex; }
            set { SetProperty(ref _userIndex, value); }
        }

        private DelegateCommand _login;

        public DelegateCommand Login =>
            _login ?? (_login = new DelegateCommand(() =>
            {
                if (UserIndex == 0)
                    CurrentPermit = PermitLevel.Nobody;
                else
                {
                    LoginPad.Open = true;
                    LoginPad.CallBack = finishLogin;
                }
            }));

        private void finishLogin(string result)
        {
            if (result == SystemConfig.Passwords[UserIndex])
            {
                Variables.CurrentPassword = result;
                if (UserIndex == 1)
                    CurrentPermit = PermitLevel.Operater;
                if (UserIndex == 2)
                    CurrentPermit = PermitLevel.Engineer;
                if (UserIndex == 3)
                    CurrentPermit = PermitLevel.Administrator;
            }
            else
            {
                if (result == DateTime.Now.AddDays(0).ToString("yyMMdd") && UserIndex == 3)
                {
                    CurrentPermit = PermitLevel.Administrator;
                }
                else
                {
                    CurrentPermit = PermitLevel.Nobody;
                    UserIndex = 0;
                }
            }
        }

        public NumberPadViewModel LoginPad { get; set; } = new NumberPadViewModel();

        #endregion 登录密码
    }

    //底部状态显示基类
    public class ShowStatus : BindableBase
    {
        private bool _value = false;

        public bool Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }
    }
}