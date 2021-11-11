using BingLibrary.Tools;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows;
using VisionProject.GlobalVars;

namespace VisionProject.ViewModels
{
    public class AboutDialogViewModel : BindableBase, IDialogAware
    {
        public AboutDialogViewModel()
        {
            Name = Variables.Title;
            MachineID = Authorize.GetMachineCode();
        }

        private string _title = "关于软件";

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _name = "检测软件";

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string version = "版本 " + Application.ResourceAssembly.GetName().Version.ToString();

        public string Version
        {
            get { return version; }
            set { SetProperty(ref version, value); }
        }

        private string _machineID;

        public string MachineID
        {
            get { return _machineID; }
            set { SetProperty(ref _machineID, value); }
        }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}