using BingLibrary.Communication;
using BingLibrary.Extension;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VisionProject.GlobalVars;

namespace VisionProject.ViewModels
{
    public class Function_CameraViewModel : BindableBase, IDialogAware, IFunction_ViewModel_Interface
    {
        #region 窗口相关

        private string _title = "保存图像";

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
            if (beginGrab == true)
            {
                MoreContent = "连续拍摄";
                beginGrab = false;
            }
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }

        #endregion 窗口相关

        private ObservableCollection<string> _cameraNames;

        public ObservableCollection<string> CameraNames
        {
            get { return _cameraNames; }
            set { SetProperty(ref _cameraNames, value); }
        }

        private int _cameraIndex;

        public int CameraIndex
        {
            get { return _cameraIndex; }
            set { SetProperty(ref _cameraIndex, value); }
        }

        private int _cameraTypeIndex;

        /// <summary>
        /// 引擎索引
        /// </summary>
        public int CameraTypeIndex
        {
            get { return _cameraTypeIndex; }
            set { SetProperty(ref _cameraTypeIndex, value); }
        }

        private double _expouseTime;

        public double ExpouseTime
        {
            get { return _expouseTime; }
            set { SetProperty(ref _expouseTime, value); }
        }

        private DelegateCommand _selectedCamera;

        public DelegateCommand SelectedCamera =>
            _selectedCamera ?? (_selectedCamera = new DelegateCommand(ExecuteSelectedCamera));

        private void ExecuteSelectedCamera()
        {
            // CameraIndex = int.Parse(Variables.CurrentSubProgram.Parameters.BingGetOrAdd("CameraIndex", 0).ToString());
            CameraTypeIndex = Variables.CurrentSubProgram.Parameters.BingGetOrAdd(CameraIndex + ".CameraTypeIndex", 0).ToString().BingToInt();
            ExpouseTime = Variables.CurrentSubProgram.Parameters.BingGetOrAdd(CameraIndex + ".ExpouseTime", 200).ToString().BingToDouble();

            if (Variables.Cameras[CameraIndex].IsOpened)
                NotOpened = false;
            else
                NotOpened = true;
        }

        public Function_CameraViewModel()
        {
            _ = Init();
        }

        private bool _notOpened = true;

        public bool NotOpened
        {
            get { return _notOpened; }
            set { SetProperty(ref _notOpened, value); }
        }

        private DelegateCommand<string> _cameraOperate;

        public DelegateCommand<string> CameraOperate =>
            _cameraOperate ?? (_cameraOperate = new DelegateCommand<string>(ExecuteCameraOperate));

        private async void ExecuteCameraOperate(string parameter)
        {
            if (parameter == "open")
            {
                if (!Variables.Cameras[CameraIndex].IsOpened)
                {
                    Variables.Cameras[CameraIndex].CameraType = CameraTypeIndex;
                    Variables.Cameras[CameraIndex].ExpouseTime = ExpouseTime;
                    Variables.Cameras[CameraIndex].OpenCamera();
                    Variables.ImageWindowDataForFunction.WindowCtrl.ShowImageToWindow(Variables.Cameras[CameraIndex].GrabOne());
                    Variables.ImageWindowDataForFunction.WindowCtrl.FitImageToWindow();
                    Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
                }

                if (Variables.Cameras[CameraIndex].IsOpened)
                    NotOpened = false;
                else
                    NotOpened = true;
            }
            else if (parameter == "close")
            {
                if (Variables.Cameras[CameraIndex].IsOpened)
                {
                    if (beginGrab == true)
                    {
                        MoreContent = "连续拍摄";
                        beginGrab = false;
                    }
                    await 1000;
                    Variables.Cameras[CameraIndex].CloseCamera();
                }
                NotOpened = true;
            }
        }

        private string _moreContent = "连续拍摄";

        public string MoreContent
        {
            get { return _moreContent; }
            set { SetProperty(ref _moreContent, value); }
        }

        private DelegateCommand<string> _grabImage;

        public DelegateCommand<string> GrabImage =>
            _grabImage ?? (_grabImage = new DelegateCommand<string>(ExecuteGrabImage));

        private void ExecuteGrabImage(string parameter)
        {
            if (parameter == "one")
            {
                try
                {
                    if (beginGrab == true)
                        return;
                    Variables.Cameras[CameraIndex].SetExpouseTime(ExpouseTime);
                    Variables.ImageWindowDataForFunction.WindowCtrl.ShowImageToWindow(Variables.Cameras[CameraIndex].GrabOne());
                    Variables.ImageWindowDataForFunction.WindowCtrl.FitImageToWindow();
                    Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
                }
                catch { }
            }
            else if (parameter == "more")
            {
                if (beginGrab == false)
                {
                    MoreContent = "停止拍摄";
                    beginGrab = true;
                    grabImages();
                }
                else
                {
                    MoreContent = "连续拍摄";
                    beginGrab = false;
                }
            }
        }

        private bool beginGrab = false;

        private async void grabImages()
        {
            while (beginGrab)
            {
                try
                {
                    await Task.Run(() =>
                    {
                        Variables.ImageWindowDataForFunction.WindowCtrl.ShowImageToWindow(Variables.Cameras[CameraIndex].GrabOne());
                        Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
                    });

                    await 10;
                }
                catch { }
            }
        }

        public async Task<bool> Init()
        {
            await 300;
            try
            {
                //获取相机列表
                CameraNames = new ObservableCollection<string>();
                for (int i = 0; i < Variables.Cameras.Count; i++)
                    CameraNames.Add(Variables.Cameras[i].CameraName);

                CameraIndex = Variables.CurrentSubProgram.Parameters.BingGetOrAdd("CameraIndex", 0).ToString().BingToInt();
                CameraTypeIndex = Variables.CurrentSubProgram.Parameters.BingGetOrAdd(CameraIndex + ".CameraTypeIndex", 0).ToString().BingToInt();
                ExpouseTime = Variables.CurrentSubProgram.Parameters.BingGetOrAdd(CameraIndex + ".ExpouseTime", 200).ToString().BingToDouble();

                Update();
                if (Variables.Cameras[CameraIndex].IsOpened)
                    NotOpened = false;
                else
                    NotOpened = true;
                return true;
            }
            catch { return false; }
        }

        public bool Update()
        {
            Variables.CurrentSubProgram.Parameters.BingAddOrUpdate("CameraIndex", CameraIndex);
            Variables.CurrentSubProgram.Parameters.BingAddOrUpdate(CameraIndex + ".CameraTypeIndex", CameraTypeIndex);
            Variables.CurrentSubProgram.Parameters.BingAddOrUpdate(CameraIndex + ".ExpouseTime", ExpouseTime);
            return true;
        }

        private DelegateCommand _saveParam;

        public DelegateCommand SaveParam =>
            _saveParam ?? (_saveParam = new DelegateCommand(() =>
            {
                Update();
            }));
    }
}