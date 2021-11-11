﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Windows;
using OfficeOpenXml;
using System.IO;
using VisionProject.GlobalVars;
using System;
using OfficeOpenXml.Style;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace VisionProject.ViewModels
{
    public partial class MainWindowViewModel : BindableBase
    {
        private ShowStatus _resultStatus = new ShowStatus();
        public ShowStatus ResultStatus
        {
            get { return _resultStatus; }
            set { SetProperty(ref _resultStatus, value); }
        }


        private string _title = "外观检测软件";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        IDialogService curDialogService;
        public MainWindowViewModel(IDialogService dialogService)
        {
            curDialogService = dialogService;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            initStatistic();
            getSystemConfig();
            Variables.Title = SystemConfig.Title;
            Title = Variables.Title;

            
        }



        #region 底部状态显示ConnectStatus
        private ShowStatus _plcStatus = new ShowStatus();
        public ShowStatus PLCStatus
        {
            get { return _plcStatus; }
            set { SetProperty(ref _plcStatus, value); }
        }


        #endregion



        #region 数据统计

       

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

        

        private ObservableCollection<StatisticData> _homeStatisticData=new ObservableCollection<StatisticData>();
        public ObservableCollection<StatisticData> HomeStatisticData
        {
            get { return _homeStatisticData; }
            set { SetProperty(ref _homeStatisticData, value); }
        }

        private ObservableCollection<StatisticData> _allStatisticData=new ObservableCollection<StatisticData>();
        public ObservableCollection<StatisticData> AllStatisticData
        {
            get { return _allStatisticData; }
            set { SetProperty(ref _allStatisticData, value); }
        }


        //初始化统计数据
      async  void initStatistic()
        {
            await Task.Delay(100);
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
                    Variables.Logs.WriteInfo("数据加载成功。");
    
                }
                catch { Variables.Logs.WriteError("数据加载失败。"); }

         
           
        }

        /// <summary>
        /// 检测成功时调用
        /// </summary>
        void setOK()
        {
            try {
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
        void setNG()
        {
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
            _statisticOperate ?? (_statisticOperate = new DelegateCommand<string>((string param)=> {
                switch (param) {
                    case "clear":
                       bool rst = Variables.ShowConfirm("确认清除当前计数？");
                        if (rst) {
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
                            string path = folderBrowserDialog.SelectedPath+"\\";
                            try
                            {
                                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "Statistics.xlsx", path + "产量统计.xlsx");
                                Variables.Logs.WriteInfo("导出产量统计成功。");
                                Variables.ShowMessage("导出成功。");
                            }
                            catch { Variables.Logs.WriteError("导出产量统计失败。"); }

                        }
                         
                        break;
                }
            
            }));

        

        #endregion

        #region 底部Status

        private string version = Application.ResourceAssembly.GetName().Version.ToString();

        public string Version
        {
            get { return version; }
            set { SetProperty(ref version, value); }
        }

        private string supplier = "Leader R&D";

        public string Supplier
        {
            get { return supplier; }
            set { SetProperty(ref supplier, value); }
        }

        private DelegateCommand _showAboutDialog;
        public DelegateCommand ShowAboutDialog =>
            _showAboutDialog ?? (_showAboutDialog = new DelegateCommand(() => {

                curDialogService.ShowDialog(GlobalVars.DialogNames.ShowAboutWindow);

            }));
        #endregion

        #region  登录密码
        private string _password = "";
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

       

        private int _userIndex = 0;
        public int UserIndex
        {
            get { return _userIndex; }
            set { SetProperty(ref _userIndex, value); }
        }

        private DelegateCommand _login;
        public DelegateCommand Login =>
            _login ?? (_login = new DelegateCommand(() => {
                if (UserIndex == 0)
                    IsLogin = false;
                else if (UserIndex == 1)
                {
                    if (Password == "123456")
                    {
                        IsLogin = true;
                    }
                }

                if (IsLogin == false)
                    UserIndex = 0;

                Password = "";

            }));


        private bool _isLogin = false;
        public bool IsLogin
        {
            get { return _isLogin; }
            set { SetProperty(ref _isLogin, value); }
        }
        #endregion
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
