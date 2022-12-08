using BingLibrary.Vision;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VisionProject.GlobalVars;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BingLibrary.Extension;

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

        private ObservableCollection<string> _engineNames;

        public ObservableCollection<string> EngineNames
        {
            get { return _engineNames; }
            set { SetProperty(ref _engineNames, value); }
        }

        private int _engineIndex;

        /// <summary>
        /// 引擎索引
        /// </summary>
        public int EngineIndex
        {
            get { return _engineIndex; }
            set { SetProperty(ref _engineIndex, value); }
        }

        private DelegateCommand _selectedEngine;

        public DelegateCommand SelectedEngine =>
            _selectedEngine ?? (_selectedEngine = new DelegateCommand(ExecuteSelectedEngine));

        /// <summary>
        /// 引擎选择后加载过程
        /// </summary>
        private void ExecuteSelectedEngine()
        {
            //获取脚本列表
            ScriptNames = new ObservableCollection<string>();
            for (int i = 0; i < Variables.V2Engines[EngineIndex].ProcedureNames.Count; i++)
                ScriptNames.Add(Variables.V2Engines[EngineIndex].ProcedureNames[i]);

            IOVariables1.Clear();
            IOVariables2.Clear();
            IOVariables3.Clear();
            IOVariables4.Clear();
            IOValue1 = "";
            //这里判断，如果当前引擎下未挂在过程则返回。
            if (ScriptNames.Count == 0) return;

            ScriptIndex1 = int.Parse(Variables.CurrentSubProgram.Parameters.BingGetOrAdd("ScriptIndex1", 0).ToString());

            //if (Variables.CurrentSubProgram.Parameters.ContainsKey("ScriptIndex1"))
            //    ScriptIndex1 = int.Parse(Variables.CurrentSubProgram.Parameters["ScriptIndex1"].ToString());
            //else
            //    ScriptIndex1 = 0;

            var rst = Variables.V2Engines[EngineIndex].GetProcedureInfo(ScriptNames[ScriptIndex1]);

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
            IOIndex1 = 0;
            IOIndex2 = 0;
            IOIndex3 = 0;
            IOIndex4 = 0;
            //获取输入变量值
            int c0 = IOVariables1.Count - 1;
            while (c0 > -1)
            {
                IOValue1 = Variables.CurrentSubProgram.Parameters.BingGetOrAdd(EngineIndex + "." + ScriptIndex1 + "." + c0 + "." + IOVariables1[c0], "").ToString();
                //if (Variables.CurrentSubProgram.Parameters.ContainsKey(EngineIndex + "." + ScriptIndex1 + "." + c0 + "." + IOVariables1[c0]))
                //    IOValue1 = Variables.CurrentSubProgram.Parameters[EngineIndex + "." + ScriptIndex1 + "." + c0 + "." + IOVariables1[c0]].ToString();
                c0--;
            }

            //脚本册立不清除，因为共用。清除多余的key，仅保留当前设置。
            // Variables.CurrentSubProgram.Parameters.Clear();
            Update();
        }

        public Function_ScriptTestViewModel()
        {
            _ = Init();
        }

        public async Task<bool> Init()
        {
            await Task.Delay(300);
            try
            {
                EngineNames = new ObservableCollection<string>();
                for (int i = 0; i < Variables.V2Engines.Count; i++)
                    EngineNames.Add("执行引擎" + i);

                EngineIndex = int.Parse(Variables.CurrentSubProgram.Parameters.BingGetOrAdd("EngineIndex", 0).ToString());
                //if (Variables.CurrentSubProgram.Parameters.ContainsKey("EngineIndex"))
                //    EngineIndex = int.Parse(Variables.CurrentSubProgram.Parameters["EngineIndex"].ToString());
                //else
                //    EngineIndex = 0;

                //获取脚本列表
                ScriptNames = new ObservableCollection<string>();
                for (int i = 0; i < Variables.V2Engines[EngineIndex].ProcedureNames.Count; i++)
                    ScriptNames.Add(Variables.V2Engines[EngineIndex].ProcedureNames[i]);

                IOVariables1.Clear();
                IOVariables2.Clear();
                IOVariables3.Clear();
                IOVariables4.Clear();
                IOValue1 = "";
                //这里判断，如果当前引擎下未挂在过程则返回。
                if (ScriptNames.Count == 0) return false;

                //选择挂在的过程
                ScriptIndex1 = int.Parse(Variables.CurrentSubProgram.Parameters.BingGetOrAdd("ScriptIndex1", 0).ToString());

                //if (Variables.CurrentSubProgram.Parameters.ContainsKey("ScriptIndex1"))
                //    ScriptIndex1 = int.Parse(Variables.CurrentSubProgram.Parameters["ScriptIndex1"].ToString());
                //else
                //    ScriptIndex1 = 0;

                var rst = Variables.V2Engines[EngineIndex].GetProcedureInfo(ScriptNames[ScriptIndex1]);

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
                IOIndex1 = 0;
                IOIndex2 = 0;
                IOIndex3 = 0;
                IOIndex4 = 0;
                //获取输入变量值

                int c0 = IOVariables1.Count - 1;
                while (c0 > -1)
                {
                    IOValue1 = Variables.CurrentSubProgram.Parameters.BingGetOrAdd(EngineIndex + "." + ScriptIndex1 + "." + c0 + "." + IOVariables1[c0], "").ToString();
                    //if (Variables.CurrentSubProgram.Parameters.ContainsKey(EngineIndex + "." + ScriptIndex1 + "." + c0 + "." + IOVariables1[c0]))
                    //    IOValue1 = Variables.CurrentSubProgram.Parameters[EngineIndex + "." + ScriptIndex1 + "." + c0 + "." + IOVariables1[c0]].ToString();
                    c0--;
                }

                try
                {
                    IOVariables1Names = "输入参数设定";
                    string[] lines2 = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "Projects\\Scripts" + EngineIndex + "\\" + ScriptNames[ScriptIndex1] + ".txt", Encoding.UTF8);
                    if (lines2.Length == IOVariables1.Count)
                        IOVariables1Names = lines2[IOIndex1];
                }
                catch { }

                //脚本册立不清除，因为共用。清除多余的key，仅保留当前设置。
                // Variables.CurrentSubProgram.Parameters.Clear();
                //Update();
                return true;
            }
            catch { return false; }
        }

        private string _iOVariables1Names;

        /// <summary>
        /// txt读取变量解释
        /// </summary>
        public string IOVariables1Names
        {
            get { return _iOVariables1Names; }
            set { SetProperty(ref _iOVariables1Names, value); }
        }

        private DelegateCommand _selectedScript;

        public DelegateCommand SelectedScript =>
            _selectedScript ?? (_selectedScript = new DelegateCommand(ExecuteSelectedScript));

        private void ExecuteSelectedScript()
        {
            IOVariables1.Clear();
            IOVariables2.Clear();
            IOVariables3.Clear();
            IOVariables4.Clear();
            IOValue1 = "";
            if (ScriptIndex1 < 0) return;

            var rst = Variables.V2Engines[EngineIndex].GetProcedureInfo(ScriptNames[ScriptIndex1]);

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
            IOIndex1 = 0;
            IOIndex2 = 0;
            IOIndex3 = 0;
            IOIndex4 = 0;
            //获取输入变量值
            IOValue1 = Variables.CurrentSubProgram.Parameters.BingGetOrAdd(EngineIndex + "." + ScriptIndex1 + "." + IOIndex1 + "." + IOVariables1[IOIndex1], "").ToString();

            //if (Variables.CurrentSubProgram.Parameters.ContainsKey(EngineIndex + "." + ScriptIndex1 + "." + IOIndex1 + "." + IOVariables1[IOIndex1]))
            //    IOValue1 = Variables.CurrentSubProgram.Parameters[EngineIndex + "." + ScriptIndex1 + "." + IOIndex1 + "." + IOVariables1[IOIndex1]].ToString();

            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "Projects\\Scripts" + EngineIndex + "\\" + ScriptNames[ScriptIndex1] + ".txt";
                if (File.Exists(path))
                {
                    string[] lines2 = File.ReadAllLines(path, Encoding.UTF8);
                    if (lines2.Length == IOVariables1.Count)
                        IOVariables1Names = lines2[IOIndex1];
                }
            }
            catch { }
        }

        private DelegateCommand _selectedIOVariables1;

        public DelegateCommand SelectedIOVariables1 =>
            _selectedIOVariables1 ?? (_selectedIOVariables1 = new DelegateCommand(ExecuteSelectedIOVariables1));

        //选择输入变量前先保存当前值
        private void ExecuteSelectedIOVariables1()
        {
            if (IOIndex1 < 0) return;
            //获取输入变量值
            IOValue1 = Variables.CurrentSubProgram.Parameters.BingGetOrAdd(EngineIndex + "." + ScriptIndex1 + "." + IOIndex1 + "." + IOVariables1[IOIndex1], "").ToString();

            //if (Variables.CurrentSubProgram.Parameters.ContainsKey(EngineIndex + "." + ScriptIndex1 + "." + IOIndex1 + "." + IOVariables1[IOIndex1]))
            //    IOValue1 = Variables.CurrentSubProgram.Parameters[EngineIndex + "." + ScriptIndex1 + "." + IOIndex1 + "." + IOVariables1[IOIndex1]].ToString();
            //else
            //    IOValue1 = "";

            try
            {
                IOVariables1Names = "输入参数设定";
                string path = AppDomain.CurrentDomain.BaseDirectory + "Projects\\Scripts" + EngineIndex + "\\" + ScriptNames[ScriptIndex1] + ".txt";
                if (File.Exists(path))
                {
                    string[] lines2 = File.ReadAllLines(path, Encoding.UTF8);
                    if (lines2.Length == IOVariables1.Count)
                        IOVariables1Names = lines2[IOIndex1];
                }
            }
            catch { }
        }

        public bool Update()
        {
            Variables.CurrentSubProgram.Parameters.BingAddOrUpdate("EngineIndex", EngineIndex);
            //if (Variables.CurrentSubProgram.Parameters.ContainsKey("EngineIndex"))
            //    Variables.CurrentSubProgram.Parameters["EngineIndex"] = EngineIndex;
            //else
            //    Variables.CurrentSubProgram.Parameters.Add("EngineIndex", EngineIndex);
            Variables.CurrentSubProgram.Parameters.BingAddOrUpdate("ScriptIndex1", ScriptIndex1);
            //if (Variables.CurrentSubProgram.Parameters.ContainsKey("ScriptIndex1"))
            //    Variables.CurrentSubProgram.Parameters["ScriptIndex1"] = ScriptIndex1;
            //else
            //    Variables.CurrentSubProgram.Parameters.Add("ScriptIndex1", ScriptIndex1);
            Variables.CurrentSubProgram.Parameters.BingAddOrUpdate(EngineIndex + "." + ScriptIndex1 + "." + IOIndex1 + "." + IOVariables1[IOIndex1], IOValue1);
            //if (Variables.CurrentSubProgram.Parameters.ContainsKey(EngineIndex + "." + ScriptIndex1 + "." + IOIndex1 + "." + IOVariables1[IOIndex1]))
            //    Variables.CurrentSubProgram.Parameters[EngineIndex + "." + ScriptIndex1 + "." + IOIndex1 + "." + IOVariables1[IOIndex1]] = IOValue1;
            //else
            //    Variables.CurrentSubProgram.Parameters.Add(EngineIndex + "." + ScriptIndex1 + "." + IOIndex1 + "." + IOVariables1[IOIndex1], IOValue1);

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

        private string _iOValue1;

        public string IOValue1
        {
            get { return _iOValue1; }
            set { SetProperty(ref _iOValue1, value); }
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
            try
            {
                Variables.scriptEdit.SetProcedurePath(AppDomain.CurrentDomain.BaseDirectory + "Projects\\Scripts" + EngineIndex);
                //打开脚本窗口
                ScriptDIalog sd = new ScriptDIalog();
                //读取并显示脚本
                sd.SetCode(Variables.scriptEdit.ReadProcedure(ScriptNames[ScriptIndex1]));
                sd.ShowDialog();
                //保存脚本
                Variables.scriptEdit.SaveProcedure(sd.GetCode());
                sd.Close();
            }
            catch { }
        }

        private DelegateCommand _saveParam;

        public DelegateCommand SaveParam =>
            _saveParam ?? (_saveParam = new DelegateCommand(() =>
            {
                Update();
            }));

        private string _runScriptResult;

        public string RunScriptResult
        {
            get { return _runScriptResult; }
            set { SetProperty(ref _runScriptResult, value); }
        }

        private DelegateCommand _runScript;

        public DelegateCommand RunScript =>
            _runScript ?? (_runScript = new DelegateCommand(ExecuteRunScript));

        private void ExecuteRunScript()
        {
            try
            {
                //更改脚本后，重新加载脚本，无需重启软件
                Variables.V2Engines[EngineIndex].Reload();
                for (int i = 0; i < IOVariables1.Count; i++)
                {
                    //这里数据类型都一样，按实际情况传入数据
                    Variables.V2Engines[EngineIndex].SetParam(
                        ScriptNames[ScriptIndex1],
                        IOVariables1[i],
                       new HalconDotNet.HTuple(double.Parse(Variables.CurrentSubProgram.Parameters.BingGetOrAdd(EngineIndex + "." + ScriptIndex1 + "." + i + "." + IOVariables1[i], "0").ToString())
                        ));
                }
                bool rst = Variables.V2Engines[EngineIndex].InspectProcedure(ScriptNames[ScriptIndex1]);
                //获取结果
                RunScriptResult = Variables.V2Engines[EngineIndex].GetParam<HalconDotNet.HTuple>(ScriptNames[ScriptIndex1], "Result").ToString();
            }
            catch { }
        }
    }
}