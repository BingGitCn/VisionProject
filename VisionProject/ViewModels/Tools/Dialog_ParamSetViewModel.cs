using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HalconDotNet;
using BingLibrary.Extension;
using System.Collections.ObjectModel;
using System.Text;
using VisionProject.GlobalVars;
using BingLibrary.Vision;
using VisionProject.Dialogs;
using VisionProject.Views.Tools;
using HandyControl.Tools.Extension;
using MySqlX.XDevAPI.Common;
using System.Windows.Forms;
using System.ComponentModel;

namespace VisionProject.ViewModels
{
    public class Dialog_ParamSetViewModel : BindableBase, IDialogAware //, IFunction_ViewModel_Interface
    {
        private IDialogService _dialogService;

        public Dialog_ParamSetViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        #region 窗口相关

        private string _title = "参数设置";

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public event Action<IDialogResult> RequestClose;

        private bool canClose = false;

        public bool CanCloseDialog()
        {
            return canClose;
        }

        public void OnDialogClosed()
        {
        }

        private Dictionary<string, ParamSetVar> paramDict = new Dictionary<string, ParamSetVar>();

        public void OnDialogOpened(IDialogParameters parameters)
        {
            paramDict = parameters.GetValue<Dictionary<string, ParamSetVar>>("ParamDict");
            OpenFunc();
        }

        #endregion 窗口相关

        private string _variableName;

        public string VariableName
        {
            get { return _variableName; }
            set { SetProperty(ref _variableName, value); }
        }

        private ObservableCollection<ParamSetVar> _variableList = new ObservableCollection<ParamSetVar>();

        public ObservableCollection<ParamSetVar> VariableList
        {
            get { return _variableList; }
            set { SetProperty(ref _variableList, value); }
        }

        private ParamSetVar _selectedVariable;

        public ParamSetVar SelectedVariable
        {
            get { return _selectedVariable; }
            set { SetProperty(ref _selectedVariable, value); }
        }

        private void OpenFunc()
        {
            VariableList.Clear();
            foreach (var key in paramDict.Keys)
            {
                VariableList.Add(paramDict[key]);
            }
        }

        private void CloseFunc()
        {
            paramDict.Clear();
            for (int i = 0; i < VariableList.Count; i++)
            {
                paramDict.BingAddOrUpdate(VariableList[i].Name, VariableList[i]);
            }
        }

        private DelegateCommand<string> _operate;

        public DelegateCommand<string> Operate =>
            _operate ?? (_operate = new DelegateCommand<string>(param => ExecuteOperate(param)));

        private void ExecuteOperate(string parameter)
        {
            switch (parameter)
            {
                case "add":
                    for (int i = 0; i < VariableList.Count; i++)
                    {
                        if (VariableName == VariableList[i].Name)
                        {
                            GlobalVars.Variables.ShowMessage("已包含该变量名称！");
                            VariableName = "";
                            return;
                        }
                    }

                    if (VariableName != "" && VariableName != null)
                    {
                        char firstChar = VariableName[0];
                        bool isFirstCharLetter = char.IsLetter(firstChar);
                        if (isFirstCharLetter)
                        {
                            VariableList.Add(new ParamSetVar() { Name = VariableName, Type = paramSetType.Bool, Value = "", Mark = "", SelectedIndex = 0 });
                            //Variables.ParamSetVars.Add(VariableName, new ParamSetVar() { Name = VariableName });
                        }
                        else
                            GlobalVars.Variables.ShowMessage("起始应为字母！");
                        VariableName = "";
                    }

                    break;

                case "delete":
                    for (int i = 0; i < VariableList.Count; i++)
                    {
                        if (VariableList[i].Name == SelectedVariable.Name)
                        {
                            VariableList.RemoveAt(i);
                            break;
                        }
                    }
                    break;

                case "save":
                    canClose = true;
                    CloseFunc();
                    DialogParameters param = new DialogParameters
                {
                    { "ParamDict", paramDict }
                };

                    RequestClose?.Invoke(new Prism.Services.Dialogs.DialogResult(ButtonResult.Yes, param));
                    break;
            }
        }
    }
}