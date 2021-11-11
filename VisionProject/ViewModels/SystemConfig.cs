using Prism.Commands;
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
using BingLibrary.FileOpreate;
using System.Windows.Controls;

namespace VisionProject.ViewModels
{
    public partial class MainWindowViewModel
    {
        private SystemConfigData _systemConfig = new SystemConfigData();
        public SystemConfigData SystemConfig
        {
            get { return _systemConfig; }
            set { SetProperty(ref _systemConfig, value); }
        }


        private DelegateCommand _saveSystemConfig;
        public DelegateCommand SaveSystemConfig =>
            _saveSystemConfig ?? (_saveSystemConfig = new DelegateCommand(() =>{
                try {
                    Serialize.WriteJsonV2(SystemConfig, Variables.BaseDirectory + "system.config");
                    Variables.Logs.WriteInfo("系统设置保存成功。");

                } catch { Variables.Logs.WriteError("系统设置保存失败。"); }


                }));


        void getSystemConfig()
        {
            try {
                SystemConfig = Serialize.ReadJsonV2<SystemConfigData>(Variables.BaseDirectory + "system.config");
                if (SystemConfig == null)
                    SystemConfig = new SystemConfigData();
                Variables.Logs.WriteInfo("系统设置加载成功。");

            } catch { Variables.Logs.WriteError("系统设置保存失败。"); }
        
        }




      

    }
    public class SystemConfigData : BindableBase
    {

        private string _title = "检测软件";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }


    }
}
