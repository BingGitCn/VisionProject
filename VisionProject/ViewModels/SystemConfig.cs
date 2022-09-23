using BingLibrary.FileOpreate;
using BingLibrary.Logs;
using Prism.Commands;
using System.Collections.Generic;
using VisionProject.GlobalVars;
using Log = BingLibrary.Logs.LogOpreate;

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
                    Log.Info("系统设置保存成功。");
                }
                catch { Log.Error("系统设置保存失败。"); }
            }));

        private void getSystemConfig()
        {
            try
            {
                SystemConfig = Serialize.ReadJsonV2<SystemConfigData>(Variables.BaseDirectory + "system.config");
                if (SystemConfig == null)
                    SystemConfig = new SystemConfigData();
                Log.Info("系统设置加载成功。");
            }
            catch { Log.Error("系统设置保存失败。"); }
        }

        private DelegateCommand _updatePassword;

        public DelegateCommand UpdatePassword =>
            _updatePassword ?? (_updatePassword = new DelegateCommand(() =>
            {
                if (SystemConfig.UserIndex >= UserIndex)
                {
                    SystemConfig.UserIndex = UserIndex;
                    Variables.ShowMessage("当前权限无法更改此账号密码。");
                    return;
                }

                if (SystemConfig.UserIndex < 0)
                    SystemConfig.UserIndex = UserIndex;

                if (SystemConfig.UserIndex + 1 == UserIndex)
                {
                    if (SystemConfig.Passwords[SystemConfig.UserIndex + 1] == SystemConfig.CurrentPassword)
                    {
                        SystemConfig.Passwords[SystemConfig.UserIndex + 1] = SystemConfig.NewPassword;

                        Variables.ShowMessage("密码更新成功，请立即保存！\r\n此为唯一密码！");
                        Log.Info("更新密码操作。");
                    }
                }
                else if (SystemConfig.UserIndex + 1 < UserIndex)

                {
                    SystemConfig.Passwords[SystemConfig.UserIndex + 1] = SystemConfig.NewPassword;

                    Variables.ShowMessage("密码更新成功，请立即保存！\r\n此为唯一密码！");
                    Log.Info("更新密码操作。");
                }

                SystemConfig.NewPassword = "";
                SystemConfig.CurrentPassword = "";
            }));
    }

    public class SystemConfigData
    {
        public string Title { set; get; }

        public int UserIndex { set; get; }

        public int HardDrive { set; get; }
        public double FreeSpace { set; get; }

        public Dictionary<int, string> Passwords { set; get; } = new Dictionary<int, string>() {
            { 1,"123456"},{ 2,"123456"},{ 3,"123456"}
        };

        public string CurrentPassword { set; get; }
        public string NewPassword { set; get; }
        public bool IsRestoreDirectory { set; get; }
    }
}