using HalconDotNet;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using VisionProject.GlobalVars;

namespace VisionProject.ViewModels
{
    public class Function_TestViewModel : BindableBase, IDialogAware
    {
        #region 窗口相关

        private string _title = "元件检测";

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
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

        #endregion 窗口相关

        public Function_TestViewModel()
        {
            init();
        }

        private HImage currentImage = new HImage();

        private async void init()
        {
            await Task.Delay(300);
            try
            {
                if (Variables.CurrentProgram[Variables.ProgramIndex].Parameters.ContainsKey("Param1"))
                    Param1 = (double)Variables.CurrentProgram[Variables.ProgramIndex]
                    .Parameters["Param1"];

                Variables.ImageWindowDataForFunction.CurrentImage = Variables.CurrentImageForFunction;

                //显示彩色图像
                Variables.ImageWindowDataForFunction.ClearHObjects();
                Variables.ImageWindowDataForFunction.Repaint();
            }
            catch { }
        }

        private double _param1;

        public double Param1
        {
            get { return _param1; }
            set { SetProperty(ref _param1, value); }
        }
    }
}