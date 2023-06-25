using BingLibrary.Extension;
using BingLibrary.Vision;
using HalconDotNet;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using VisionProject.GlobalVars;

namespace VisionProject.ViewModels
{
    public class Function_ClsViewModel : BindableBase, IDialogAware, IFunction_ViewModel_Interface
    {
        public Function_ClsViewModel()
        {
        }

        private int tabIndex;

        public int TabIndex
        {
            get { return tabIndex; }
            set { SetProperty(ref tabIndex, value); }
        }

        private int oldTabIndex = -1;
        private DelegateCommand _doSwitchTab;

        public DelegateCommand DoSwitchTab =>
            _doSwitchTab ?? (_doSwitchTab = new DelegateCommand(ExecuteDoSwitchTab));

        private void ExecuteDoSwitchTab()
        {
            if (oldTabIndex != TabIndex)
            {
                oldTabIndex = TabIndex;
                if (TabIndex == 0)
                { }
                else if (TabIndex == 1)
                {
                    //runCode();
                }
            }
        }

        #region 图像选择

        private HImage image = new HImage();

        private bool _isSaveNG;

        public bool IsSaveNG
        {
            get { return _isSaveNG; }
            set { SetProperty(ref _isSaveNG, value); }
        }

        private DelegateCommand<string> _imageOperate;

        public DelegateCommand<string> ImageOperate =>
            _imageOperate ?? (_imageOperate = new DelegateCommand<string>(ExecuteImageOperate));

        private void ExecuteImageOperate(string parameter)
        {
            Variables.ImageWindowDataForFunction.WindowCtrl.DrawMode = HalconDrawing.margin;
            switch (parameter)
            {
                case "select":
                    try
                    {
                        var rst = openImageDialog();

                        if (rst != "")
                        {
                            image = new HImage(rst);
                            Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                            Variables.ImageWindowDataForFunction.WindowCtrl.ShowImageToWindow(image.CopyImage());
                            Variables.ImageWindowDataForFunction.WindowCtrl.FitImageToWindow();

                            Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = false;
                        }
                    }
                    catch { }
                    break;

                case "":
                    break;
            }
        }

        private string openImageDialog()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "所有文件|*.*|Tiff文件|*.tif|BMP文件|*.bmp|Jpeg文件|*.jpg";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DereferenceLinks = false;
            openFileDialog.AutoUpgradeEnabled = true;
            System.Windows.Forms.DialogResult result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return "";
            }
            string fileName = openFileDialog.FileName;
            return fileName;
        }

        private DelegateCommand _isSaveNGChanged;

        public DelegateCommand IsSaveNGChanged =>
            _isSaveNGChanged ?? (_isSaveNGChanged = new DelegateCommand(ExecuteIsSaveNGChanged));

        private void ExecuteIsSaveNGChanged()
        {
            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsSaveNG", IsSaveNG);
        }

        #endregion 图像选择

        public string Title => "条码识别";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        private bool isInitlized = false;

        public async Task<bool> Init()
        {
            isInitlized = false;
            await Task.Delay(300);

            IsSaveNG = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsSaveNG", false);

            await Task.Delay(200);
            isInitlized = true;
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _ = Init();
        }

        public bool Update()
        {
            return true;
        }
    }
}