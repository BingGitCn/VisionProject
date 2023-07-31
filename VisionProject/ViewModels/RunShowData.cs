using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using VisionProject.GlobalVars;
using System.IO;

namespace VisionProject.ViewModels
{
    public class RunShowData : BindableBase
    {
        private ObservableCollection<string> _runShowDataResult = new ObservableCollection<string>();

        public ObservableCollection<string> RunShowDataResult
        {
            get { return _runShowDataResult; }
            set { SetProperty(ref _runShowDataResult, value); }
        }

        private DelegateCommand<string> _doSomething;

        public DelegateCommand<string> DoSomething =>
            _doSomething ?? (_doSomething = new DelegateCommand<string>(ExecuteDoSomething));

        private void ExecuteDoSomething(string parameter)
        {
            try
            {
                if (parameter.Contains("cam1"))
                {
                    Process.Start("explorer", Variables.SaveImagePath + Variables.Barcode1String[0] + "\\" + "NG\\" + parameter.Replace("-", "_") + ".jpg");
                }
                if (parameter.Contains("cam2"))
                {
                    if (File.Exists(Variables.SaveImagePath + Variables.Barcode1String[0] + "\\" + "NG\\" + parameter.Replace("-", "_") + ".jpg"))
                        Process.Start("explorer", Variables.SaveImagePath + Variables.Barcode1String[0] + "\\" + "NG\\" + parameter.Replace("-", "_") + ".jpg");
                    if (File.Exists(Variables.SaveImagePath + Variables.Barcode2String[0] + "\\" + "NG\\" + parameter.Replace("-", "_") + ".jpg"))
                        Process.Start("explorer", Variables.SaveImagePath + Variables.Barcode2String[0] + "\\" + "NG\\" + parameter.Replace("-", "_") + ".jpg");
                }

                if (parameter.Contains("cam3"))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (File.Exists(Variables.SaveImagePath + Variables.Barcode1String[0] + "\\" + "Pin\\" + parameter.Replace("-", "_") + "_" + i + ".jpg"))
                            Process.Start("explorer", Variables.SaveImagePath + Variables.Barcode1String[0] + "\\" + "Pin\\" + parameter.Replace("-", "_") + "_" + i + ".jpg");
                    }
                }

                if (parameter.Contains("cam4"))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (File.Exists(Variables.SaveImagePath + Variables.Barcode1String[0] + "\\" + "Pin\\" + parameter.Replace("-", "_") + "_" + i + ".jpg"))
                            Process.Start("explorer", Variables.SaveImagePath + Variables.Barcode1String[0] + "\\" + "Pin\\" + parameter.Replace("-", "_") + "_" + i + ".jpg");
                    }

                    for (int i = 0; i < 8; i++)
                    {
                        if (File.Exists(Variables.SaveImagePath + Variables.Barcode2String[0] + "\\" + "Pin\\" + parameter.Replace("-", "_") + "_" + i + ".jpg"))
                            Process.Start("explorer", Variables.SaveImagePath + Variables.Barcode2String[0] + "\\" + "Pin\\" + parameter.Replace("-", "_") + "_" + i + ".jpg");
                    }
                }
            }
            catch (Exception ex) { Variables.ShowGrowlError(ex.Message); }
        }

        private int _runStatus = 2;

        public int RunStatus
        {
            get { return _runStatus; }
            set { SetProperty(ref _runStatus, value); }
        }

        private string _header;

        public string Header
        {
            get { return _header; }
            set { SetProperty(ref _header, value); }
        }

        private double _top;

        public double Top
        {
            get { return _top; }
            set { SetProperty(ref _top, value); }
        }

        private double _left;

        public double Left
        {
            get { return _left; }
            set { SetProperty(ref _left, value); }
        }
    }
}