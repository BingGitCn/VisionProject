using HalconDotNet;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using VisionProject.GlobalVars;

namespace VisionProject.ViewModels
{
   

    public class Function_SpliteViewModel : BindableBase, IDialogAware,IFunction_ViewModel_Interface
    {
        #region 窗口相关

        private string _title = "分离产品。";

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

        public Function_SpliteViewModel()
        {
            _=Init();
        
        }

        private HImage currentImage = new HImage();

        public async Task<bool> Init()
        {
            await Task.Delay(300);
            try
            {
                if (Variables.CurrentProject.Parameters[Variables.CurrentProgram[Variables.ProgramIndex].ID].ContainsKey("Param1"))
                    Param1 = (double)Variables.CurrentProject.Parameters[Variables.CurrentProgram[Variables.ProgramIndex].ID]["Param1"];

                Variables.ImageWindowDataForFunction.CurrentImage = Variables.CurrentImageForFunction;

                //显示彩色图像
                Variables.ImageWindowDataForFunction.ClearHObjects();
                Variables.ImageWindowDataForFunction.Repaint();

                Variables.ClearCurrentParamsKeys();
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
    }
}
