using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using VisionProject.GlobalVars;

namespace VisionProject.ViewModels
{
    public class Dialog_ProjectSetViewModel : BindableBase, IDialogAware
    {
        public Dialog_ProjectSetViewModel()
        {
        }

        #region 参数

        private bool _isShowResultToPinTu;

        public bool IsShowResultToPinTu
        {
            get { return _isShowResultToPinTu; }
            set { SetProperty(ref _isShowResultToPinTu, value); }
        }

        private bool _checkCameraMode;

        public bool CheckCameraMode
        {
            get { return _checkCameraMode; }
            set { SetProperty(ref _checkCameraMode, value); }
        }

        private static int _rowDefault;

        public int RowDefault
        {
            get { return _rowDefault; }
            set { SetProperty(ref _rowDefault, value); }
        }

        private static int _colDefault;

        public int ColDefault
        {
            get { return _colDefault; }
            set { SetProperty(ref _colDefault, value); }
        }

        private int _rowCrop = _rowDefault;

        public int RowCrop
        {
            get { return _rowCrop; }
            set
            {
                if (value <= 0) _rowCrop = 1;
                else if (value > _rowDefault) _rowCrop = _rowDefault;
                else _rowCrop = value;
                RaisePropertyChanged(nameof(RowCrop));
            }
        }

        private int _colCrop = _colDefault;

        public int ColCrop
        {
            get { return _colCrop; }
            set
            {
                if (value <= 0) _colCrop = 1;
                else if (value > _colDefault) _colCrop = _colDefault;
                else _colCrop = value;
                RaisePropertyChanged(nameof(ColCrop));
            }
        }

        public short _plcProjectName = 0;

        public short PLCProjectName
        {
            get { return _plcProjectName; }
            set { SetProperty(ref _plcProjectName, value); }
        }

        public short _plcProjectName1 = 0;

        public short PLCProjectName1
        {
            get { return _plcProjectName1; }
            set { SetProperty(ref _plcProjectName1, value); }
        }

        public short _plcProjectName2 = 0;

        public short PLCProjectName2
        {
            get { return _plcProjectName2; }
            set { SetProperty(ref _plcProjectName2, value); }
        }

        #endregion 参数

        #region 窗口相关

        private string _title = "位置参数";

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

        private Dictionary<string, ParamSetVar> paramDict = new Dictionary<string, ParamSetVar>();

        public void OnDialogOpened(IDialogParameters parameters)
        {
            try
            {
                CheckCameraMode = Variables.CurrentProject.CameraMode == 1 ? true : false;
                RowDefault = Variables.ImageHeight;
                ColDefault = Variables.ImageWidth;
                RowCrop = Variables.CurrentProject.RowCrop;
                ColCrop = Variables.CurrentProject.ColCrop;
                PLCProjectName1 = Variables.CurrentProject.PLCProjectName1;
                PLCProjectName2 = Variables.CurrentProject.PLCProjectName2;
                IsShowResultToPinTu = Variables.CurrentProject.IsShowResultToPinTu;
            }
            catch { }
        }

        private DelegateCommand _confirm;

        public DelegateCommand Confirm =>
            _confirm ?? (_confirm = new DelegateCommand(ExecuteConfirm));

        private void ExecuteConfirm()
        {
            try
            {
                var rst = Variables.ShowConfirm("确认保存并退出？");
                if (!rst)
                {
                    return;
                }
                // 直接获取
                Variables.CurrentProject.CameraMode = CheckCameraMode == true ? 1 : 2;
                Variables.CurrentProject.RowCrop = RowCrop;
                Variables.CurrentProject.ColCrop = ColCrop;
                Variables.CurrentProject.PLCProjectName1 = PLCProjectName1;
                Variables.CurrentProject.PLCProjectName2 = PLCProjectName2;
                Variables.CurrentProject.IsShowResultToPinTu = IsShowResultToPinTu;
                RequestClose?.Invoke(new DialogResult(ButtonResult.Yes));
            }
            catch { }
        }

        private DelegateCommand _valueEdit;

        public DelegateCommand ValueEdit =>
            _valueEdit ?? (_valueEdit = new DelegateCommand(ExecuteValueEdit));

        private void ExecuteValueEdit()
        {
            try
            {
                // 这里不做处理
                return;
                //Variables.CurrentConfigSet.RowInput = RowInput;
                //Variables.CurrentConfigSet.ColInput = ColInput;
            }
            catch { }
        }

        private DelegateCommand _cancel;

        public DelegateCommand Cancel =>
            _cancel ?? (_cancel = new DelegateCommand(ExecuteCancel));

        private void ExecuteCancel()
        {
            var rst = Variables.ShowConfirm("当前未保存，请确认是否退出？");
            if (!rst)
            {
                return;
            }
            RequestClose?.Invoke(new Prism.Services.Dialogs.DialogResult(ButtonResult.No));
        }

        #endregion 窗口相关
    }
}