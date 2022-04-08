using HalconDotNet;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisionProject.GlobalVars;
using System.Linq;

namespace VisionProject.ViewModels
{
    public class Function_TestViewModel : BindableBase, IDialogAware,IFunction_ViewModel_Interface
    {
        #region 窗口相关

        private string _title = "测试窗口，无其它用途。";

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
            _=Init();
          
        }

        private HImage currentImage = new HImage();

        public async Task<bool> Init()
        {
            await Task.Delay(300);
            try
            {
                if (Variables.CurrentSubProgram.Parameters.ContainsKey("Param1"))
                    Param1 = (double)Variables.CurrentSubProgram.Parameters["Param1"];

                Variables.ImageWindowDataForFunction.CurrentImage = Variables.CurrentImageForFunction;

                //显示彩色图像
                Variables.ImageWindowDataForFunction.ClearHObjects();
                Variables.ImageWindowDataForFunction.Repaint();



                //清除多余的key，仅保留当前设置。
                Variables.CurrentSubProgram.Parameters.Clear();
                Update();

                return true;
            }
            catch { return false; }
        }

        public bool Update()
        {
            return true;
        }

        private double _param1;

        public double Param1
        {
            get { return _param1; }
            set { SetProperty(ref _param1, value); }
        }

        private List<string> _inputParams; 
        public List<string> InputParams
        {
            get { return _inputParams; }
            set { SetProperty(ref _inputParams, value); }
        }
    }
}