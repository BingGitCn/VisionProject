using BingLibrary.FileOpreate;
using Prism.Commands;
using System.Collections.Generic;
using VisionProject.GlobalVars;

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
            _saveSystemConfig ?? (_saveSystemConfig = new DelegateCommand(() =>
            {
                try
                {
                    Serialize.WriteJsonV2(SystemConfig, Variables.BaseDirectory + "system.config");
                    Variables.Logs.WriteInfo("系统设置保存成功。");
                }
                catch { Variables.Logs.WriteError("系统设置保存失败。"); }
            }));

        private void getSystemConfig()
        {
            try
            {
                SystemConfig = Serialize.ReadJsonV2<SystemConfigData>(Variables.BaseDirectory + "system.config");
                if (SystemConfig == null)
                    SystemConfig = new SystemConfigData();
                Variables.Logs.WriteInfo("系统设置加载成功。");
            }
            catch { Variables.Logs.WriteError("系统设置保存失败。"); }
        }

        private DelegateCommand _updatePassword;

        public DelegateCommand UpdatePassword =>
            _updatePassword ?? (_updatePassword = new DelegateCommand(() =>
            {
                if (Variables.CurrentPassword == SystemConfig.CurrentPassword)
                {
                    Variables.CurrentPassword = SystemConfig.NewPassword;
                    SystemConfig.Passwords[UserIndex] = SystemConfig.NewPassword;

                    Variables.ShowMessage("密码更新成功，请立即保存！\r\n此为唯一密码！");
                    Variables.Logs.WriteInfo("更新密码操作。");
                }
                SystemConfig.NewPassword = "";
                SystemConfig.CurrentPassword = "";
            }));
    }

    public class SystemConfigData
    {
        public string Title { set; get; }

        public Dictionary<int, string> Passwords { set; get; } = new Dictionary<int, string>() {
            { 1,"123456"},{ 2,"123456"},{ 3,"123456"}
        };
        public string CurrentPassword { set; get; }
        public string NewPassword { set; get; }
        public bool IsRestoreDirectory { set; get; }
    }
}