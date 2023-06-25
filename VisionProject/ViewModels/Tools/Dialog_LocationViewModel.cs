using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VisionProject.GlobalVars;

namespace VisionProject.ViewModels
{
    public class Dialog_LocationViewModel : BindableBase, IDialogAware
    {
        public Dialog_LocationViewModel()
        {
        }

        private ObservableCollection<string> _programs = new ObservableCollection<string>();

        public ObservableCollection<string> Programs
        {
            get { return _programs; }
            set { SetProperty(ref _programs, value); }
        }

        private int _programIndex;

        public int ProgramIndex
        {
            get { return _programIndex; }
            set { SetProperty(ref _programIndex, value); }
        }

        private int _rowDefault;

        public int RowDefault
        {
            get { return _rowDefault; }
            set { SetProperty(ref _rowDefault, value); }
        }

        private int _colDefault;

        public int ColDefault
        {
            get { return _colDefault; }
            set { SetProperty(ref _colDefault, value); }
        }

        private int _rowInput;

        public int RowInput
        {
            get { return _rowInput; }
            set { SetProperty(ref _rowInput, value); }
        }

        private int _colInput;

        public int ColInput
        {
            get { return _colInput; }
            set { SetProperty(ref _colInput, value); }
        }

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

        // 疑问点：现在默认宽高由外部传入
        // 这里需整理下，定义了全局变量 CurrentConfigSet
        public void OnDialogOpened(IDialogParameters parameters)
        {
            try
            {
                Programs.Clear();

                ProgramsIndex = int.Parse(parameters.GetValue<string>("ProgramsIndex").ToString());
                CurrentProgramIndex = int.Parse(parameters.GetValue<string>("CurrentProgramIndex").ToString());
                RowDefault = int.Parse(parameters.GetValue<string>("RowDefault").ToString());
                ColDefault = int.Parse(parameters.GetValue<string>("ColDefault").ToString());
                var program = Variables.CurrentProject.Programs.ElementAt(ProgramsIndex).Value;

                foreach (var s in program)
                    Programs.Add(s.ProductIndex.ToString());

                // 直接获取
                RowInput = Variables.CurrentConfigSet.RowInput;
                ColInput = Variables.CurrentConfigSet.ColInput;
            }
            catch { }
        }

        private int ProgramsIndex;
        private int CurrentProgramIndex;
        private DelegateCommand _selectionChanged;

        public DelegateCommand SelectionChanged =>
            _selectionChanged ?? (_selectionChanged = new DelegateCommand(ExecuteSelectionChanged));

        private void ExecuteSelectionChanged()
        {
            try
            {
                // 疑问点：位置变化，不是产品变化
                /*
                    这里的流程，
                    1. 选择产品，0或1（默认程序名称，第一个对应相机1，第二个对应相机2），都选择0时，相机1和2拍同一片产品，否则0对应第一片产品，1对应第二片产品
                    2. CurrentProgramIndex 是位置索引，所以界面上应该添加个下拉框，可以选择对应位置，
                 */
                // 直接获取
                RowInput = Variables.CurrentConfigSet.RowInput;
                ColInput = Variables.CurrentConfigSet.ColInput;
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
                // 直接获取
                Variables.CurrentConfigSet.RowInput = RowInput;
                Variables.CurrentConfigSet.ColInput = ColInput;
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
                Variables.CurrentConfigSet.RowInput = RowInput;
                Variables.CurrentConfigSet.ColInput = ColInput;
            }
            catch { }
        }

        private DelegateCommand _cancel;

        public DelegateCommand Cancel =>
            _cancel ?? (_cancel = new DelegateCommand(ExecuteCancel));

        private void ExecuteCancel()
        {
            RequestClose?.Invoke(new Prism.Services.Dialogs.DialogResult(ButtonResult.No));
        }

        #endregion 窗口相关
    }
}