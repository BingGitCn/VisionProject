using BingLibrary.FileOpreate;
using Prism.Commands;
using Prism.Mvvm;
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
            _updatePassword ?? (_updatePassword = new DelegateCommand(()=> {

                if (Variables.CurrentPassword == SystemConfig.CurrentPassword)
                {
                    Variables.CurrentPassword = SystemConfig.NewPassword;
                    SystemConfig.Password= SystemConfig.NewPassword;

                    Variables.ShowMessage("密码更新成功，请立即保存！\r\n此为唯一密码！");
                }
                SystemConfig.NewPassword = "";
                SystemConfig.CurrentPassword = "";

            }));

    

    }

    public class SystemConfigData : BindableBase
    {
        private string _title = "检测软件";

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _currentPassword = "";

        public string CurrentPassword
        {
            get { return _currentPassword; }
            set { SetProperty(ref _currentPassword, value); }
        }

        private string _newPassword = "";

        public string NewPassword
        {
            get { return _newPassword; }
            set { SetProperty(ref _newPassword, value); }
        }

        private string _Password = "";

        public string Password
        {
            get { return _Password; }
            set { SetProperty(ref _Password, value); }
        }
    } 
}