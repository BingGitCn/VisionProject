using BingLibrary.FileOpreate;
using HalconDotNet;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using VisionProject.GlobalVars;
using Log = BingLibrary.Logs.LogOpreate;

namespace VisionProject.ViewModels
{
    /*项目相关*/

    public partial class MainWindowViewModel
    {
        #region 项目编辑

        private ObservableCollection<ProjectInfo> _projectNames = new ObservableCollection<ProjectInfo>();

        /// <summary>
        /// 项目名称合集
        /// </summary>
        public ObservableCollection<ProjectInfo> ProjectNames
        {
            get { return _projectNames; }
            set { SetProperty(ref _projectNames, value); }
        }

        private ProjectInfo _selectProjectName = new ProjectInfo();

        /// <summary>
        /// 选择的项目
        /// </summary>
        public ProjectInfo SelectProjectName
        {
            get { return _selectProjectName; }
            set { SetProperty(ref _selectProjectName, value); }
        }

        /// <summary>
        /// 初始化项目,主界面显示所有项目
        /// </summary>
        private async void initProjects()
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Projects"))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Projects");
            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Projects");
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo fileInfo = new FileInfo(files[i]);
                if (fileInfo.Extension == ".lprj")
                    ProjectNames.Add(new ProjectInfo() { Name = fileInfo.Name.Replace(".lprj", ""), Path = files[i] });
            }

            await System.Threading.Tasks.Task.Delay(5000);
            if (SystemConfig.IsLoadProject)
                if (SystemConfig.ProjectIndex != -1)
                {
                    try
                    {
                        HOperatorSet.SetSystem(new HTuple("clip_region"), new HTuple("false"));
                        ProjectIndex = SystemConfig.ProjectIndex;
                        if (!File.Exists(SelectProjectName.Path))
                        {
                            return;
                        }
                        Variables.CurrentProject = Serialize.ReadJsonV2<Project>(SelectProjectName.Path);
                        Programs = Variables.CurrentProject.Programs;
                        CurrentProgram.Clear();
                        if (Programs.Keys.ToList().Count > 0)
                            for (int i = 0; i < Programs[Programs.Keys.ToList()[0]].Count; i++)
                                CurrentProgram.Add(Programs[Programs.Keys.ToList()[0]][i]);
                        ProgramsIndex = 0;
                        ProjectName = SelectProjectName.Name;
                        CreateDate = Variables.CurrentProject.CreateDate;
                        LastDate = Variables.CurrentProject.LastDate;
                        ProjectPath = SelectProjectName.Path;
                        Log.Info("打开了项目 " + ProjectName);

                        ProgramsName.Clear();
                        for (int i = 0; i < Programs.Keys.Count; i++)
                            ProgramsName.Add(Programs.Keys.ToList()[i]);

                        Variables.ProgramName = ProgramsName[ProgramsIndex];
                    }
                    catch { }
                }
        }

        private string _projectName;

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName
        {
            get { return _projectName; }
            set { SetProperty(ref _projectName, value); }
        }

        private int _projectIndex = -1;

        /// <summary>
        /// 项目索引
        /// </summary>
        public int ProjectIndex
        {
            get { return _projectIndex; }
            set { SetProperty(ref _projectIndex, value); }
        }

        private string _projectPath;

        /// <summary>
        /// 项目路径
        /// </summary>
        public string ProjectPath
        {
            get { return _projectPath; }
            set { SetProperty(ref _projectPath, value); }
        }

        private string _createDate;

        /// <summary>
        /// 项目创建日期
        /// </summary>
        public string CreateDate
        {
            get { return _createDate; }
            set { SetProperty(ref _createDate, value); }
        }

        private string _lastDate;

        /// <summary>
        /// 项目更新日期
        /// </summary>
        public string LastDate
        {
            get { return _lastDate; }
            set { SetProperty(ref _lastDate, value); }
        }

        private int projectIndex = -1;

        private DelegateCommand<string> _openProject;

        /// <summary>
        /// 打开项目
        /// </summary>
        public DelegateCommand<string> OpenProject =>
            _openProject ?? (_openProject = new DelegateCommand<string>((string param) =>
            {
                switch (param)
                {
                    case "close":
                        try
                        {
                            if (ProjectIndex == -1) return;
                            if (Variables.ShowConfirm("请确认设备处于非工作状态？") == false)
                            {
                                ProjectIndex = projectIndex;
                                return;
                            }
                            if (Variables.ShowConfirm("防止勿操作，请确认设备处于非工作状态？") == false)
                            {
                                ProjectIndex = projectIndex;
                                return;
                            }
                            if (Variables.ShowConfirm("已就绪，确认进行下一步操作？") == false)
                            {
                                ProjectIndex = projectIndex;
                                return;
                            }

                            HOperatorSet.SetSystem(new HTuple("clip_region"), new HTuple("false"));

                            Variables.CurrentProject = Serialize.ReadJsonV2<Project>(SelectProjectName.Path);
                            Programs = Variables.CurrentProject.Programs;
                            CurrentProgram.Clear();
                            if (Programs.Keys.ToList().Count > 0)
                                for (int i = 0; i < Programs[Programs.Keys.ToList()[0]].Count; i++)
                                    CurrentProgram.Add(Programs[Programs.Keys.ToList()[0]][i]);
                            ProgramsIndex = 0;
                            ProjectName = SelectProjectName.Name;
                            CreateDate = Variables.CurrentProject.CreateDate;
                            LastDate = Variables.CurrentProject.LastDate;
                            ProjectPath = SelectProjectName.Path;
                            Log.Info("打开了项目 " + ProjectName);

                            if (SystemConfig.IsLoadProject == true)
                                try
                                {
                                    SystemConfig.ProjectIndex = ProjectIndex;
                                    Serialize.WriteJsonV2(SystemConfig, Variables.BaseDirectory + "system.config");
                                    Log.Info("系统设置保存成功。");
                                }
                                catch { Log.Error("系统设置保存失败。"); }
                            else
                                try
                                {
                                    SystemConfig.ProjectIndex = -1;
                                    Serialize.WriteJsonV2(SystemConfig, Variables.BaseDirectory + "system.config");
                                    Log.Info("系统设置保存成功。");
                                }
                                catch { Log.Error("系统设置保存失败。"); }
                            ProgramsName.Clear();
                            for (int i = 0; i < Programs.Keys.Count; i++)
                                ProgramsName.Add(Programs.Keys.ToList()[i]);

                            Variables.ProgramName = ProgramsName[ProgramsIndex];
                        }
                        catch { }
                        break;

                    case "open":
                        projectIndex = ProjectIndex;
                        break;
                }
            }));

        private DelegateCommand<string> _projectOperate;

        public DelegateCommand<string> ProjectOperate =>
            _projectOperate ?? (_projectOperate = new DelegateCommand<string>((string param) =>
            {
                switch (param)
                {
                    case "open":
                        try
                        {
                            HOperatorSet.SetSystem(new HTuple("clip_region"), new HTuple("false"));
                            System.Windows.Forms.OpenFileDialog dig_openFileDialog = new System.Windows.Forms.OpenFileDialog();
                            dig_openFileDialog.Title = "请选择项目文件路径";
                            dig_openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Projects";

                            dig_openFileDialog.Filter = "项目文件(*.lprj)|*.lprj";
                            if (dig_openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                Variables.CurrentProject = new Project();
                                Variables.CurrentProject = Serialize.ReadJsonV2<Project>(dig_openFileDialog.FileName);
                                Programs = Variables.CurrentProject.Programs;
                                CurrentProgram.Clear();
                                if (Programs.Keys.ToList().Count > 0)
                                    for (int i = 0; i < Programs[Programs.Keys.ToList()[0]].Count; i++)
                                        CurrentProgram.Add(Programs[Programs.Keys.ToList()[0]][i]);
                                ProgramsIndex = 0;
                                ProjectName = dig_openFileDialog.SafeFileName.Replace(".lprj", "");
                                CreateDate = Variables.CurrentProject.CreateDate;
                                LastDate = Variables.CurrentProject.LastDate;
                                ProjectPath = dig_openFileDialog.FileName;

                                ProgramsName.Clear();
                                for (int i = 0; i < Programs.Keys.Count; i++)
                                    ProgramsName.Add(Programs.Keys.ToList()[i]);

                                Variables.ProgramName = ProgramsName[ProgramsIndex];

                                Log.Info("打开了项目 " + ProjectName);
                            }
                        }
                        catch { }
                        break;

                    case "save":
                        try
                        {
                            //bool rst = Variables.ShowConfirm("是否要保存文件？");
                            if (true)
                            {
                                if (ProjectPath == "" || ProjectPath == null)
                                {
                                    System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                                    saveFileDialog.Title = ("请选择项目文件路径");

                                    saveFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Projects";

                                    saveFileDialog.Filter = "项目文件(*.lprj)|*.lprj";
                                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                    {
                                        ProjectPath = saveFileDialog.FileName;
                                    }
                                }
                                if (Variables.CurrentProject.CreateDate == "")
                                {
                                    Variables.CurrentProject.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    CreateDate = Variables.CurrentProject.CreateDate;
                                }
                                Variables.CurrentProject.LastDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                LastDate = Variables.CurrentProject.LastDate;

                                Variables.CurrentProject.Programs = Programs;

                                bool isSaveOK = true;

                                try
                                {
                                    Serialize.WriteJsonV2(Variables.CurrentProject, AppDomain.CurrentDomain.BaseDirectory + "cache.lprj");
                                    Serialize.ReadJsonV2<Project>(AppDomain.CurrentDomain.BaseDirectory + "cache.lprj");
                                }
                                catch (Exception ex)
                                {
                                    isSaveOK = false;
                                    Variables.ShowMessage("保存文件出错，当前项目文件未受影响。" + "\r\n" + ex.Message);
                                }

                                if (isSaveOK)
                                {
                                    Serialize.WriteJsonV2(Variables.CurrentProject, ProjectPath);
                                }
                            }
                            Variables.ShowMessage("保存文件成功。");
                            Log.Info("保存了项目");
                        }
                        catch (Exception ex) { Variables.ShowMessage("保存文件出错，当前项目文件未受影响。" + "\r\n" + ex.Message); }
                        break;

                    case "saveas":
                        try
                        {
                            bool rst = Variables.ShowConfirm("是否要保存文件？");
                            if (rst)
                            {
                                System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                                saveFileDialog.Title = ("请选择项目文件路径");

                                saveFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Projects";

                                saveFileDialog.Filter = "项目文件(*.lprj)|*.lprj";
                                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    ProjectPath = saveFileDialog.FileName;
                                }

                                if (Variables.CurrentProject.CreateDate == "")
                                {
                                    Variables.CurrentProject.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    CreateDate = Variables.CurrentProject.CreateDate;
                                }
                                Variables.CurrentProject.LastDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                LastDate = Variables.CurrentProject.LastDate;

                                Variables.CurrentProject.Programs = Programs;

                                bool isSaveOK = true;

                                try
                                {
                                    Serialize.WriteJsonV2(Variables.CurrentProject, AppDomain.CurrentDomain.BaseDirectory + "cache.lprj");
                                    Serialize.ReadJsonV2<Project>(AppDomain.CurrentDomain.BaseDirectory + "cache.lprj");
                                }
                                catch (Exception ex)
                                {
                                    isSaveOK = false;
                                    Variables.ShowMessage("保存文件出错，当前项目文件未受影响。" + "\r\n" + ex.Message);
                                }

                                if (isSaveOK)
                                {
                                    Serialize.WriteJsonV2(Variables.CurrentProject, ProjectPath);
                                }
                            }
                            Variables.ShowMessage("保存文件成功。");
                            Log.Info("保存了项目");
                        }
                        catch (Exception ex) { Variables.ShowMessage("保存文件出错，当前项目文件未受影响。" + "\r\n" + ex.Message); }
                        break;
                }
            }));

        #endregion 项目编辑

        #region 程序编辑

        /*
         * 说明：
         * currentProgram 为当前程序用。主要是界面交互。
         * Programs 为程序合集。
         * Variables.CurrentProgram 为全局变量，主要是用于子工具中使用。
         *
         */

        //界面上的多程序
        public Dictionary<string, ObservableCollection<SubProgram>> Programs = new Dictionary<string, ObservableCollection<SubProgram>>();

        //程序索引
        private int currentProgramIndex;

        public int CurrentProgramIndex
        {
            get { return currentProgramIndex; }
            set { SetProperty(ref currentProgramIndex, value); }
        }

        //程序列表
        private ObservableCollection<SubProgram> currentProgram = new ObservableCollection<SubProgram>();

        /// <summary>
        /// 界面上当前编辑的程序
        /// </summary>
        public ObservableCollection<SubProgram> CurrentProgram
        {
            get { return currentProgram; }
            set { SetProperty(ref currentProgram, value); }
        }

        //各程序索引
        private int programsIndex = 0;

        public int ProgramsIndex
        {
            get { return programsIndex; }
            set { SetProperty(ref programsIndex, value); }
        }

        private ProgramData currentSelectedProgramData;

        public ProgramData CurrentSelectedProgramData
        {
            get { return currentSelectedProgramData; }
            set { SetProperty(ref currentSelectedProgramData, value); }
        }

        private ObservableCollection<ProgramData> currentprogramDatas = new ObservableCollection<ProgramData>();

        public ObservableCollection<ProgramData> CurrentProgramDatas
        {
            get { return currentprogramDatas; }
            set { SetProperty(ref currentprogramDatas, value); }
        }

        private int currentprogramDatasIndex;

        public int CurrentProgramDatasIndex
        {
            get { return currentprogramDatasIndex; }
            set { SetProperty(ref currentprogramDatasIndex, value); }
        }

        //各程序名称
        private ObservableCollection<string> programsName = new ObservableCollection<string>();

        public ObservableCollection<string> ProgramsName
        {
            get { return programsName; }
            set { SetProperty(ref programsName, value); }
        }

        private string _programName = "";

        public string ProgramName
        {
            get { return _programName; }
            set { SetProperty(ref _programName, value); }
        }

        //增加删除程序,多个程序
        private DelegateCommand<string> _programsOperate;

        public DelegateCommand<string> ProgramsOperate =>
            _programsOperate ?? (_programsOperate = new DelegateCommand<string>((string param) =>
            {
                switch (param)
                {
                    case "add":
                        try
                        {
                            if (Variables.ShowConfirm("确认添加新程序？") == true)
                            {
                                if (ProgramName != "" && ProgramName.Replace(" ", "") != "")
                                {
                                    if (!Programs.Keys.ToList().Contains(ProgramName))
                                    {
                                        if (ProgramsName.Count > 0)
                                        {
                                            var ns = ProgramsName[ProgramsIndex];
                                            if (Variables.ShowConfirm("是否复制【" + ns + "】程序到新程序【" + ProgramName + "】？") == true)
                                            {
                                                var newProgram = new ObservableCollection<SubProgram>();
                                                for (int i = 0; i < Programs[ProgramsName[ProgramsIndex]].Count; i++)
                                                {
                                                    var subProgram = new SubProgram();
                                                    subProgram.ProductIndex = Programs[ProgramsName[ProgramsIndex]][i].ProductIndex;
                                                    subProgram.ProgramDatas = new ObservableCollection<ProgramData>();
                                                    for (int j = 0; j < Programs[ProgramsName[ProgramsIndex]][i].ProgramDatas.Count; j++)
                                                        subProgram.ProgramDatas.Add(Programs[ProgramsName[ProgramsIndex]][i].ProgramDatas[j].Clone());
                                                    newProgram.Add(subProgram);
                                                }

                                                Programs.Add(ProgramName, newProgram);
                                                ProgramsName.Add(ProgramName);
                                                ProgramName = "";
                                            }
                                            else
                                            {
                                                ProgramsName.Add(ProgramName);
                                                Programs.Add(ProgramName, new ObservableCollection<SubProgram>());
                                                Variables.ProgramName = ProgramsName[ProgramsIndex];
                                                Log.Info("增加了程序");
                                                ProgramName = "";
                                            }
                                        }
                                        else
                                        {
                                            ProgramsName.Add(ProgramName);
                                            Programs.Add(ProgramName, new ObservableCollection<SubProgram>());
                                            Variables.ProgramName = ProgramsName[ProgramsIndex];
                                            Log.Info("增加了程序");
                                            ProgramName = "";
                                        }
                                    }
                                    else
                                    {
                                        Variables.ShowMessage("名称不能重复！");
                                    }
                                }
                                else
                                {
                                    Variables.ShowMessage("名称不能为空！");
                                }
                            }
                        }
                        catch { }
                        break;

                    case "del":
                        try
                        {   //删除确认
                            if (Variables.ShowConfirm("确认删除当前程序？") == true)
                            {
                                Programs.Remove(ProgramsName[ProgramsIndex]);
                                ProgramsName.Remove(ProgramsName[ProgramsIndex]);
                                Variables.ProgramName = ProgramsName[ProgramsIndex];
                                Log.Info("删除了程序");
                            }
                        }
                        catch { }
                        break;
                }
            }));

        private DelegateCommand _selectProgram;

        public DelegateCommand SelectProgram =>
            _selectProgram ?? (_selectProgram = new DelegateCommand(() =>
            {
                try
                {
                    //将程序集合中选中的程序给到界面
                    CurrentProgram.Clear();
                    for (int i = 0; i < Programs[ProgramsName[ProgramsIndex]].Count; i++)
                        CurrentProgram.Add(Programs[ProgramsName[ProgramsIndex]][i]);
                    Variables.ProgramName = ProgramsName[ProgramsIndex];
                }
                catch (Exception ex) { }
            }));

        //增加删除子程序
        private DelegateCommand<string> _programOperate;

        public DelegateCommand<string> ProgramOperate =>
            _programOperate ?? (_programOperate = new DelegateCommand<string>((string param) =>
            {
                if (ProgramsName.Count == 0)
                {
                    Variables.ShowMessage("请选择或创建一个程序！");
                    return;
                }

                switch (param)
                {
                    case "add":
                        try
                        {
                            CurrentProgram.Add(new SubProgram() { ProductIndex = CurrentProgram.Count() });
                        }
                        catch { }
                        break;

                    case "del":
                        try
                        {
                            if (Variables.ShowConfirm("禁止直接删除！\r\n将删除最后一行程序？") == true)
                                CurrentProgram.RemoveAt(CurrentProgram.Count - 1);
                        }
                        catch { }
                        break;
                }

                //将界面的program更新到项目的程序集合中。
                try
                {
                    Programs[ProgramsName[ProgramsIndex]].Clear();
                    for (int i = 0; i < CurrentProgram.Count; i++)
                        Programs[ProgramsName[ProgramsIndex]].Add(CurrentProgram[i]);
                }
                catch (Exception ex) { }
            }));

        //各位置子程序删减
        private DelegateCommand<string> _programDatasOperate;

        public DelegateCommand<string> ProgramDatasOperate =>
            _programDatasOperate ?? (_programDatasOperate = new DelegateCommand<string>(ExecuteProgramDatasOperate));

        private void ExecuteProgramDatasOperate(string parameter)
        {
            if (ProgramsName.Count == 0)
            {
                Variables.ShowMessage("请选择或创建一个程序！");
                return;
            }
            if (CurrentProgramIndex < 0 || CurrentProgramIndex >= CurrentProgram.Count)
            {
                Variables.ShowMessage("请选择或添加一个位置！");
                return;
            }
            //禁止自动返回主界面
            Variables.IngoreAutoHome = true;
            switch (parameter)
            {
                case "add":
                    try
                    {
                        DialogParameters param = new DialogParameters
                        {
                            { "Title", "" }
                        };
                        Variables.CurDialogService.ShowDialog(DialogNames.ShowInspectCreateDialog, param, callback =>
                        {
                            if (callback.Result == ButtonResult.Yes)
                            {
                                var inspectName = callback.Parameters.GetValue<string>("InspectName");
                                var content = callback.Parameters.GetValue<string>("Content");
                                currentprogramDatas.Add(new ProgramData() { InspectFunction = inspectName, Content = content, IsUse = true });
                            }
                        });
                    }
                    catch { }
                    break;

                case "del":
                    try
                    {
                        if (Variables.ShowConfirm("确认删除检测？") == true)
                            currentprogramDatas.Remove(CurrentSelectedProgramData);
                    }
                    catch { }
                    break;
            }

            //将界面的programdata更新到项目的程序集合中。
            try
            {
                Programs[ProgramsName[ProgramsIndex]][currentProgramIndex].ProgramDatas.Clear();
                for (int i = 0; i < currentprogramDatas.Count; i++)
                    Programs[ProgramsName[ProgramsIndex]][currentProgramIndex].ProgramDatas.Add(currentprogramDatas[i]);
            }
            catch { }
            Variables.IngoreAutoHome = false;
        }

        private DelegateCommand _updateSelectedCurrentProgram;

        public DelegateCommand UpdateSelectedCurrentProgram =>
            _updateSelectedCurrentProgram ?? (_updateSelectedCurrentProgram = new DelegateCommand(ExecuteUpdateSelectedCurrentProgram));

        private void ExecuteUpdateSelectedCurrentProgram()
        {
            try
            {
                CurrentProgramDatas.Clear();

                for (int i = 0; i < Programs[ProgramsName[ProgramsIndex]][currentProgramIndex].ProgramDatas.Count; i++)
                    CurrentProgramDatas.Add(Programs[ProgramsName[ProgramsIndex]][currentProgramIndex].ProgramDatas[i]);
            }
            catch { }
        }

        //Step..
        private DelegateCommand<string> _programDataConfig;

        public DelegateCommand<string> ProgramDataConfig =>
            _programDataConfig ?? (_programDataConfig = new DelegateCommand<string>((param) =>
            {
                try
                {
                    //禁止自动返回主界面
                    Variables.IngoreAutoHome = true;
                    Variables.CurrentProgramData = CurrentProgram[CurrentProgramIndex].ProgramDatas[CurrentProgramDatasIndex];

                    Variables.CurDialogService.ShowDialog(DialogNames.ToolNams[param]);
                    CurrentProgram[CurrentProgramIndex].ProgramDatas[CurrentProgramDatasIndex] = Variables.CurrentProgramData.Clone();

                    Variables.IngoreAutoHome = false;
                }
                catch (Exception ex) { }
            }));

        #endregion 程序编辑
    }

    //项目类
    public class Project
    {
        public Dictionary<string, ObservableCollection<SubProgram>> Programs = new Dictionary<string, ObservableCollection<SubProgram>>();
        public string CreateDate = "";
        public string LastDate = "";
    }

    public class SubProgram
    {
        /// <summary>
        /// 产品索引
        /// </summary>
        public int ProductIndex { set; get; } = 0;

        public ObservableCollection<ProgramData> ProgramDatas { set; get; } = new ObservableCollection<ProgramData>();
    }

    //程序类
    public class ProgramData
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsUse { set; get; } = true;

        public string InspectFunction { set; get; } = "无";

        /// <summary>
        /// 备注
        /// </summary>
        public string Content { set; get; }

        /// <summary>
        /// 检测参数
        /// </summary>
        public Dictionary<string, object> Parameters = new Dictionary<string, object>();

        /// <summary>
        /// 检测结果
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, object> Results = new Dictionary<string, object>();

        public ProgramData Clone()
        {
            return (ProgramData)this.MemberwiseClone();
        }
    }

    public class ProjectInfo
    {
        public string Name { set; get; }
        public string Path { set; get; }
    }
}