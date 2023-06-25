using BingLibrary.FileOpreate;
using Prism.Commands;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VisionProject.GlobalVars;
using Log = BingLibrary.Logs.LogOpreate;

namespace VisionProject.ViewModels
{
    /*MES配置相关参数*/

    public partial class MainWindowViewModel
    {
        public static ObservableCollection<MesData> MesDatas = new ObservableCollection<MesData>();
        public static ObservableCollection<MesData> _mesDatasTemp = new ObservableCollection<MesData>();

        public ObservableCollection<MesData> MesDatasTemp
        {
            get { return _mesDatasTemp; }
            set { SetProperty(ref _mesDatasTemp, value); }
        }

        private string _variableName;

        public string VariableName
        {
            get { return _variableName; }
            set { SetProperty(ref _variableName, value); }
        }

        private int _selecedTabIndex;

        public int SelecedTabIndex
        {
            get { return _selecedTabIndex; }
            set { SetProperty(ref _selecedTabIndex, value); }
        }

        private MesData _selectedVariable;

        public MesData SelectedVariable
        {
            get { return _selectedVariable; }
            set { SetProperty(ref _selectedVariable, value); }
        }

        private DelegateCommand<string> _operate;

        public DelegateCommand<string> Operate =>
            _operate ?? (_operate = new DelegateCommand<string>(param => ExecuteOperate(param)));

        private void ExecuteOperate(string parameter)
        {
            switch (parameter)
            {
                case "add":
                    for (int i = 0; i < MesDatasTemp.Count; i++)
                    {
                        if (VariableName == MesDatasTemp[i].Name)
                        {
                            GlobalVars.Variables.ShowMessage("已包含该变量名称！");
                            VariableName = "";
                            return;
                        }
                    }

                    if (VariableName != "" && VariableName != null)
                    {
                        MesDatasTemp.Add(new MesData() { Name = VariableName, Value = "" });
                        VariableName = "";
                    }
                    Log.Info("MES操作，添加。");
                    break;

                case "delete":
                    if (!Variables.ShowConfirm("确认删除MES字段？"))
                        return;
                    for (int i = 0; i < MesDatasTemp.Count; i++)
                    {
                        if (MesDatasTemp[i].Name == SelectedVariable.Name)
                        {
                            MesDatasTemp.RemoveAt(i);
                            break;
                        }
                    }
                    Log.Info("MES操作，删除。");
                    break;

                case "save":
                    MesDatas.Clear();
                    for (int i = 0; i < MesDatasTemp.Count; i++)
                    {
                        MesDatas.Add(MesDatasTemp[i]);
                    }
                    Serialize.WriteJsonV2(MesDatas, Variables.BaseDirectory + "mes.config");
                    Log.Info("MES操作，保存。");
                    break;
            }
        }

        private DelegateCommand _gotMesFocus;

        public DelegateCommand GotMesFocus =>
            _gotMesFocus ?? (_gotMesFocus = new DelegateCommand(ExecuteGotMesFocus));

        private void ExecuteGotMesFocus()
        {
            if (SelecedTabIndex != 2)
            { return; }
            MesDatasTemp.Clear();
            for (int i = 0; i < MesDatas.Count; i++)
            {
                MesDatasTemp.Add(MesDatas[i]);
            }
        }

        //private DelegateCommand _saveMesData;

        //public DelegateCommand SaveMesData =>
        //    _saveMesData ?? (_saveMesData = new DelegateCommand(ExecuteSaveMesData));

        //private void ExecuteSaveMesData()
        //{
        //    try
        //    {
        //        Serialize.WriteJsonV2(MesDatas, Variables.BaseDirectory + "mes.config");
        //        Log.Info("MES设置保存成功。");
        //    }
        //    catch { Log.Error("MES设置保存失败。"); }
        //}

        private void getMESConfig()
        {
            try
            {
                MesDatas = Serialize.ReadJsonV2<ObservableCollection<MesData>>(Variables.BaseDirectory + "mes.config");
                if (MesDatas == null)
                    MesDatas = new ObservableCollection<MesData>();
                MesDatasTemp.Clear();
                for (int i = 0; i < MesDatas.Count; i++)
                {
                    MesDatasTemp.Add(MesDatas[i]);
                }
                Log.Info("MES设置加载成功。");
            }
            catch { Log.Error("MES设置加载失败。"); }
        }
    }

    //Step..
    /// <summary>
    /// mes数据，这里根据实际情况更改添加即可。
    /// </summary>
    public class MesData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { set; get; }

        //private string _name;
        //public string Name
        //{
        //    get { return _name; }
        //    set { _name = value; }
        //}
        //private string _value;
        //public string Value
        //{
        //    get { return _value; }
        //    set { _value = value; }
        //}
    }
}