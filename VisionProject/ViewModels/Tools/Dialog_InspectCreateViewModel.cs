using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VisionProject.GlobalVars;

namespace VisionProject.ViewModels
{
    public class Dialog_InspectCreateViewModel : BindableBase, IDialogAware
    {
        public Dialog_InspectCreateViewModel()
        {
        }

        private ObservableCollection<string> inspectNames = new ObservableCollection<string>();

        public ObservableCollection<string> InspectNames
        {
            get { return inspectNames; }
            set { SetProperty(ref inspectNames, value); }
        }

        private int inspectNamesIndex;

        public int InspectNamesIndex
        {
            get { return inspectNamesIndex; }
            set { SetProperty(ref inspectNamesIndex, value); }
        }

        private string content;

        public string Content
        {
            get { return content; }
            set { SetProperty(ref content, value); }
        }

        #region 窗口相关

        private string _title = "添加检测";

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
            InspectNames.Clear();
            foreach (var s in DialogNames.ToolNams.Keys)
                InspectNames.Add(s);
            InspectNamesIndex = 0;
        }

        private DelegateCommand _confirm;

        public DelegateCommand Confirm =>
            _confirm ?? (_confirm = new DelegateCommand(ExecuteConfirm));

        private void ExecuteConfirm()
        {
            DialogParameters param = new DialogParameters
            {
                { "InspectName", InspectNames[inspectNamesIndex] },
                { "Content", Content }
            };

            RequestClose?.Invoke(new Prism.Services.Dialogs.DialogResult(ButtonResult.Yes, param));
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