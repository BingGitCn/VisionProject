using BingLibrary.FileOpreate;
using Prism.Commands;
using VisionProject.GlobalVars;
using Log = BingLibrary.Logs.LogOpreate;
using System;
using System.IO;

namespace VisionProject.ViewModels
{
    /*MES配置相关参数*/

    public partial class MainWindowViewModel
    {
        private MesData _mesData = new MesData();

        public MesData MESData
        {
            get { return _mesData; }
            set { SetProperty(ref _mesData, value); }
        }

        private DelegateCommand _saveMesData;

        public DelegateCommand SaveMesData =>
            _saveMesData ?? (_saveMesData = new DelegateCommand(ExecuteSaveMesData));

        private void ExecuteSaveMesData()
        {
            try
            {
                Serialize.WriteJsonV2(MESData, Variables.BaseDirectory + "mes.config");
                Log.Info("MES设置保存成功。");
            }
            catch { Log.Error("MES设置保存失败。"); }
        }

        private void getMESConfig()
        {
            try
            {
                MESData = Serialize.ReadJsonV2<MesData>(Variables.BaseDirectory + "mes.config");
                if (MESData == null)
                    MESData = new MesData();
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
        public string OperateName { get; set; }
        public string MachineName { get; set; }
    }
}