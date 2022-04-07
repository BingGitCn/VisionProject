using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using BingLibrary.Vision.Engine;
using System.Collections.ObjectModel;
using VisionProject.GlobalVars;
using System.Threading.Tasks;

namespace VisionProject.ViewModels
{
    public class Function_ScriptTestViewModel : BindableBase, IDialogAware,IFunction_ViewModel_Interface
    {
        #region 窗口相关

        private string _title = "脚本测试。";

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
        public Function_ScriptTestViewModel()
        {
           
            _=Init();
        }
        public async Task<bool> Init()
        {
            await Task.Delay(300);
            try
            {
                IOVariables1.Clear();
                IOVariables2.Clear();
                IOVariables3.Clear();
                IOVariables4.Clear();
                var rst = Variables.V2Engine.GetProcedureInfo("lvba");

                for (int i = 0; i < rst.InputCtrlParamNames.Count; i++)
                {
                    IOVariables1.Add(rst.InputCtrlParamNames[i]);
                }
                for (int i = 0; i < rst.InputIconicParamNames.Count; i++)
                {
                    IOVariables2.Add(rst.InputIconicParamNames[i]);
                }
                for (int i = 0; i < rst.OutputCtrlParamNames.Count; i++)
                {
                    IOVariables3.Add(rst.OutputCtrlParamNames[i]);
                }
                for (int i = 0; i < rst.OutputIconicParamNames.Count; i++)
                {
                    IOVariables4.Add(rst.OutputIconicParamNames[i]);
                }

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

        private ObservableCollection<string> _iOVariables1=new ObservableCollection<string>();
        public ObservableCollection<string> IOVariables1
        {
            get { return _iOVariables1; }
            set { SetProperty(ref _iOVariables1, value); }
        }

        private int _iOIndex1;
        public int IOIndex1
        {
            get { return _iOIndex1; }
            set { SetProperty(ref _iOIndex1, value); }
        }


        private ObservableCollection<string> _iOVariables2 = new ObservableCollection<string>();
        public ObservableCollection<string> IOVariables2
        {
            get { return _iOVariables2; }
            set { SetProperty(ref _iOVariables2, value); }
        }

        private int _iOIndex2;
        public int IOIndex2
        {
            get { return _iOIndex2; }
            set { SetProperty(ref _iOIndex2, value); }
        }

        private ObservableCollection<string> _iOVariables3 = new ObservableCollection<string>();
        public ObservableCollection<string> IOVariables3
        {
            get { return _iOVariables3; }
            set { SetProperty(ref _iOVariables3, value); }
        }

        private int _iOIndex3;
        public int IOIndex3
        {
            get { return _iOIndex3; }
            set { SetProperty(ref _iOIndex3, value); }
        }

        private ObservableCollection<string> _iOVariables4 = new ObservableCollection<string>();
        public ObservableCollection<string> IOVariables4
        {
            get { return _iOVariables4; }
            set { SetProperty(ref _iOVariables4, value); }
        }

        private int _iOIndex4;
        public int IOIndex4
        {
            get { return _iOIndex4; }
            set { SetProperty(ref _iOIndex4, value); }
        }

    }
}
