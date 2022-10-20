using BingLibrary.Vision;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VisionProject.GlobalVars;

namespace VisionProject.ViewModels
{
    public class Function_ScriptTestViewModel : BindableBase, IDialogAware, IFunction_ViewModel_Interface
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
            _ = Init();
        }

        public async Task<bool> Init()
        {
            await Task.Delay(300);
            try
            {
                //获取脚本列表
                ScriptNames = new ObservableCollection<string>();
                for (int i = 0; i < Variables.V2Engine.ProcedureNames.Count; i++)
                    ScriptNames.Add(Variables.V2Engine.ProcedureNames[i]);

                if (Variables.CurrentSubProgram.Parameters.ContainsKey("ScriptIndex1"))
                    ScriptIndex1 = int.Parse(Variables.CurrentSubProgram.Parameters["ScriptIndex1"].ToString());

                IOVariables1.Clear();
                IOVariables2.Clear();
                IOVariables3.Clear();
                IOVariables4.Clear();
                var rst = Variables.V2Engine.GetProcedureInfo(ScriptNames[ScriptIndex1]);

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

                //清除多余的key，仅保留当前设置。
                Variables.CurrentSubProgram.Parameters.Clear();
                Update();
                return true;
            }
            catch { return false; }
        }

        public bool Update()
        {
            if (Variables.CurrentSubProgram.Parameters.ContainsKey("ScriptIndex1"))
                Variables.CurrentSubProgram.Parameters["ScriptIndex1"] = ScriptIndex1;
            else
                Variables.CurrentSubProgram.Parameters.Add("ScriptIndex1", ScriptIndex1);
            return true;
        }

        private int _scriptIndex1;

        public int ScriptIndex1
        {
            get { return _scriptIndex1; }
            set { SetProperty(ref _scriptIndex1, value); }
        }

        private ObservableCollection<string> _scriptNames = new ObservableCollection<string>();

        public ObservableCollection<string> ScriptNames
        {
            get { return _scriptNames; }
            set { SetProperty(ref _scriptNames, value); }
        }

        private ObservableCollection<string> _iOVariables1 = new ObservableCollection<string>();

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

        private DelegateCommand _editScript;

        public DelegateCommand EditScript =>
            _editScript ?? (_editScript = new DelegateCommand(ExecuteEditScript));

        private void ExecuteEditScript()
        {
            Variables.scriptEdit.SetProcedurePath(AppDomain.CurrentDomain.BaseDirectory + "Projects\\Scripts");
            //打开脚本窗口
            ScriptDIalog sd = new ScriptDIalog();
            //读取并显示脚本
            sd.SetCode(Variables.scriptEdit.ReadProcedure(ScriptNames[ScriptIndex1]));
            sd.ShowDialog();
            //保存脚本
            Variables.scriptEdit.SaveProcedure(sd.GetCode());
            sd.Close();
        }

        private DelegateCommand _saveParam;

        public DelegateCommand SaveParam =>
            _saveParam ?? (_saveParam = new DelegateCommand(() =>
            {
                Update();
            }));
    }
}