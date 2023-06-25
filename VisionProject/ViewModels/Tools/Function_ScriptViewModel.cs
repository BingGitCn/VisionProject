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
using VisionProject.Views.Tools;
using HandyControl.Tools.Extension;
using DryIoc;
using Prism.Ioc;
using ImTools;

namespace VisionProject.ViewModels
{
    public class Function_ScriptViewModel : BindableBase, IDialogAware, IFunction_ViewModel_Interface
    {
        #region 窗口相关

        private string _title = "视觉脚本";

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
            //这里注意下，放在这里是每次打开窗口时执行一次，放在构造函数里，只有在第一次打开窗口时执行
            _ = Init();
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

        private string _scriptName;

        public string ScriptName
        {
            get { return _scriptName; }
            set { SetProperty(ref _scriptName, value); }
        }

        private ObservableCollection<string> _scriptNames = new ObservableCollection<string>();

        public ObservableCollection<string> ScriptNames
        {
            get { return _scriptNames; }
            set { SetProperty(ref _scriptNames, value); }
        }

        private Dictionary<string, ParamSetVar> paramDict = new Dictionary<string, ParamSetVar>();

        public async Task<bool> Init()
        {
            await 300;
            try
            {
                EngineNames = new ObservableCollection<string>();
                for (int i = 0; i < Variables.WorkEngines.Count; i++)
                    EngineNames.Add("执行引擎" + i);

                EngineIndex = Variables.CurrentProgramData.Parameters.BingGetOrAdd("EngineIndex", 0).ToString().BingToInt();
                paramDict = (Dictionary<string, ParamSetVar>)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ParamDict", new Dictionary<string, ParamSetVar>());

                if (!paramDict.ContainsKey("ROIRow1"))
                    paramDict.BingAddOrUpdate("ROIRow1", new ParamSetVar() { Name = "ROIRow1", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawRow1.ToString() });
                if (!paramDict.ContainsKey("ROIColumn1"))
                    paramDict.BingAddOrUpdate("ROIColumn1", new ParamSetVar() { Name = "ROIColumn1", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawRow1.ToString() });
                if (!paramDict.ContainsKey("ROIPhi"))
                    paramDict.BingAddOrUpdate("ROIPhi", new ParamSetVar() { Name = "ROIPhi", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawRow1.ToString() });
                if (!paramDict.ContainsKey("DrawLength1"))
                    paramDict.BingAddOrUpdate("DrawLength1", new ParamSetVar() { Name = "DrawLength1", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawRow1.ToString() });
                if (!paramDict.ContainsKey("DrawLength2"))
                    paramDict.BingAddOrUpdate("DrawLength2", new ParamSetVar() { Name = "DrawLength2", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawRow1.ToString() });

                // var paramKeys = paramDict.GetDictParam("keys", new HTuple());
                //获取脚本列表
                ScriptNames = new ObservableCollection<string>();
                var procedureNames = Variables.WorkEngines[EngineIndex].GetProcedureNames();

                for (int i = 0; i < procedureNames.Count; i++)
                {
                    ScriptNames.Add(procedureNames[i]);
                }
                ScriptName = Variables.CurrentProgramData.Parameters.BingGetOrAdd("ScriptName", "").ToString();
                //这里判断，如果当前引擎下未挂在过程则返回。
                if (ScriptNames.Count == 0) return false;

                // 脚本册立不清除，因为共用。清除多余的key，仅保留当前设置。
                // Variables.CurrentSubProgram.Parameters.Clear();
                // Update();
                return true;
            }
            catch { return false; }
        }

        private DelegateCommand _saveParam;

        public DelegateCommand SaveParam =>
            _saveParam ?? (_saveParam = new DelegateCommand(ExecuteSaveParam));

        public void ExecuteSaveParam()
        {
            Update();
        }

        public bool Update()
        {
            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("EngineIndex", EngineIndex);
            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ScriptName", ScriptName);
            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ParamDict", paramDict);
            return true;
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
            var procedureNames = Variables.WorkEngines[EngineIndex].GetProcedureNames();
            for (int i = 0; i < procedureNames.Count; i++)
                ScriptNames.Add(procedureNames[i]);

            //这里判断，如果当前引擎下未挂在过程则返回。
            if (ScriptNames.Count == 0) return;

            ScriptName = Variables.CurrentProgramData.Parameters.BingGetOrAdd("ScriptName", 0).ToString();

            //脚本册立不清除，因为共用。清除多余的key，仅保留当前设置。
            // Variables.CurrentSubProgram.Parameters.Clear();
            Update();
        }

        private DelegateCommand _selectedScript;
        public DelegateCommand SelectedScript =>
            _selectedScript ?? (_selectedScript = new DelegateCommand(ExecuteSelectedScript));

        private void ExecuteSelectedScript()
        {
            //获取输入变量值
            //  IOValue = Variables.CurrentSubProgram.Parameters.BingGetOrAdd(EngineIndex + "." +
            //  ScriptIndex1 + "." + IOIndex1 + "." + IOVariables1[IOIndex1], "").ToString();
        }

        private DelegateCommand _editScript;

        public DelegateCommand EditScript =>
            _editScript ?? (_editScript = new DelegateCommand(ExecuteEditScript));

        private void ExecuteEditScript()
        {
            try
            {
                //BingLibrary.Vision.ScriptEditPlus scriptEditPlus = new ScriptEditPlus();
                //scriptEditPlus.SetProcedurePath("D:\\Desktop\\HalconTest");

                //scriptEditPlus.CreateNew("ABC");
                //scriptEditPlus.UpdateIO("PA1", "123", ScriptEditPlus.BaseType.InputCtrl, ScriptEditPlus.SemType.String);
                //var rst = scriptEditPlus.GetIO();
                //scriptEditPlus.Save();

                Variables.scriptEdit.SetProcedurePath(AppDomain.CurrentDomain.BaseDirectory + "Projects\\Scripts" + EngineIndex);
                //打开脚本窗口
                ScriptDIalog sd = new ScriptDIalog();
                //读取并显示脚本
                sd.SetCode(Variables.scriptEdit.ReadProcedure(ScriptName));
                sd.ShowDialog();
                //保存脚本
                Variables.scriptEdit.SaveProcedure(sd.GetCode());
                sd.Close();
            }
            catch { }
        }

        private DelegateCommand _paramSet;

        public DelegateCommand ParamSet =>
            _paramSet ?? (_paramSet = new DelegateCommand(ExecuteParamSet));

        private void ExecuteParamSet()
        {
            try
            {
                DialogParameters param = new DialogParameters
                {
                    { "ParamDict", paramDict }
                };

                Variables.CurDialogService.ShowDialog(DialogNames.ShowParamSetDialog, param, callback =>
                {
                    paramDict = callback.Parameters.GetValue<Dictionary<string, ParamSetVar>>("ParamDict");
                });
            }
            catch { }
        }

        //DialogService dialogService = new DialogService(IContainerExtension containerExtension);

        private double DrawRow1 = 0;
        private double DrawColumn1 = 0;
        private double DrawPhi = 0;
        private double DrawLength1 = 0;
        private double DrawLength2 = 0;

        private string _isDrawROIEnabled;

        public string IsDrawROIEnabled
        {
            get { return _isDrawROIEnabled; }
            set { SetProperty(ref _isDrawROIEnabled, value); }
        }

        private DelegateCommand<string> _drawROI;

        public DelegateCommand<string> DrawROI =>
            _drawROI ?? (_drawROI = new DelegateCommand<string>(param => ExecuteDrawROI(param)));

        private void ExecuteDrawROI(string parameter)
        {
            try
            {
                Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();

                switch (parameter)
                {
                    case "roi":
                        if (DrawRow1 == 0 && DrawColumn1 == 0)
                        {
                            Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = true;
                            Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor("green");
                            Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor(HalconColors.橙色.ToDescription());
                            Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.DrawRectangle1(out double r1, out double c1, out double r2, out double c2);
                            DrawRow1 = r1 / 2 + r2 / 2;
                            DrawColumn1 = c1 / 2 + c2 / 2;
                            DrawPhi = 0;
                            DrawLength1 = c2 / 2 - c1 / 2;
                            DrawLength2 = r2 / 2 - r1 / 2;
                            Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = false;

                            HXLDCont hXLD = new HXLDCont();
                            hXLD.GenRectangle2ContourXld(DrawRow1, DrawColumn1, DrawPhi, DrawLength1, DrawLength2);
                            Variables.ImageWindowDataForFunction.DispObjectCtrl.AddDispObjectVar(hXLD);
                            Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
                        }
                        else
                        {
                            Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = true;
                            Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor("green");
                            Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor(HalconColors.橙色.ToDescription());
                            Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.DrawRectangle2Mod(DrawRow1, DrawColumn1, DrawPhi, DrawLength1, DrawLength2, out DrawRow1, out DrawColumn1, out DrawPhi, out DrawLength1, out DrawLength2);

                            Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = false;

                            HXLDCont hXLD = new HXLDCont();
                            hXLD.GenRectangle2ContourXld(DrawRow1, DrawColumn1, DrawPhi, DrawLength1, DrawLength2);
                            Variables.ImageWindowDataForFunction.DispObjectCtrl.AddDispObjectVar(hXLD);
                            Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
                        }

                        paramDict.BingAddOrUpdate("ROIRow1", new ParamSetVar() { Name = "ROIRow1", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawRow1.ToString(), Mark = paramDict["ROIRow1"].Mark });
                        paramDict.BingAddOrUpdate("ROIColumn1", new ParamSetVar() { Name = "ROIColumn1", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawColumn1.ToString(), Mark = paramDict["ROIColumn1"].Mark });
                        paramDict.BingAddOrUpdate("ROIPhi", new ParamSetVar() { Name = "ROIPhi", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawPhi.ToString(), Mark = paramDict["ROIPhi"].Mark });
                        paramDict.BingAddOrUpdate("DrawLength1", new ParamSetVar() { Name = "DrawLength1", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawLength1.ToString(), Mark = paramDict["DrawLength1"].Mark });
                        paramDict.BingAddOrUpdate("DrawLength2", new ParamSetVar() { Name = "DrawLength2", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawLength2.ToString(), Mark = paramDict["DrawLength2"].Mark });

                        break;

                    case "del":
                        DrawRow1 = 0;
                        DrawColumn1 = 0;
                        DrawPhi = 0;
                        DrawLength1 = 0;
                        DrawLength2 = 0;
                        break;
                }
            }
            catch { }
        }

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
                paramDict = (Dictionary<string, ParamSetVar>)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ParamDict", new Dictionary<string, ParamSetVar>());

                HDict dict = new HDict();
                dict.CreateDict();

                foreach (var kv in paramDict)
                {
                    if (kv.Value.SelectedIndex == 0)
                        dict.SetDictTuple(kv.Value.Name, new HTuple(bool.Parse(kv.Value.Value)));
                    else if (kv.Value.SelectedIndex == 1)
                        dict.SetDictTuple(kv.Value.Name, new HTuple(kv.Value.Value));
                    else if (kv.Value.SelectedIndex == 2)
                        dict.SetDictTuple(kv.Value.Name, new HTuple(double.Parse(kv.Value.Value)));
                    else if (kv.Value.SelectedIndex == 3)
                        dict.SetDictTuple(kv.Value.Name, new HTuple(int.Parse(kv.Value.Value)));
                }

                //设置图像
                //HImage hImage = new HImage("C:\\Users\\N294\\Pictures\\Camera Roll\\1186626.jpg");
                //dict.SetDictObject(hImage, "Image");

                Variables.WorkEngines[EngineIndex].SetParam(
                     ScriptName, "InputDict", dict);
                bool rst = Variables.WorkEngines[EngineIndex].InspectProcedure(ScriptName);
                //获取结果
                HDict resultDict = Variables.WorkEngines[EngineIndex].GetParam<HalconDotNet.HDict>(ScriptName, "OutputDict");
                //这里约定好对应的输出结果
                RunScriptResult = resultDict.GetDictTuple("Result").D.ToString();
            }
            catch { }
        }

        public Function_ScriptViewModel()
        {
        }//未将对象引用设置到对象的实例。
    }
}