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

            await System.Threading.Tasks.Task.Delay(1000);
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
                        await System.Threading.Tasks.Task.Run(() =>
                        {
                            Variables.CurrentProject = Serialize.ReadJsonV2<Project>(SelectProjectName.Path);
                        });

                        Programs = Variables.CurrentProject.Programs;

                        CurrentProgram.Clear();
                        if (Programs.Count > 0)
                            CurrentProgram = Variables.DeepClone(Programs[Programs.Keys.ToList()[0]]);
                        CurrentProgramDatas.Clear();
                        if (CurrentProgram.Count > 0)
                            CurrentProgramDatas = Variables.DeepClone(CurrentProgram[0].ProgramDatas);

                        ProgramsIndex = 0;
                        CurrentProgramIndex = 0;
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
                            if (Programs.Count > 0)
                                CurrentProgram = Variables.DeepClone(Programs[Programs.Keys.ToList()[0]]);
                            CurrentProgramDatas.Clear();
                            if (CurrentProgram.Count > 0)
                                CurrentProgramDatas = Variables.DeepClone(CurrentProgram[0].ProgramDatas);

                            //CurrentProgram.Clear();
                            //if (Programs.Keys.ToList().Count > 0)
                            //{
                            //    for (int i = 0; i < Programs[Programs.Keys.ToList()[0]].Count; i++)
                            //    {
                            //        CurrentProgram.Add(new SubProgram() { ProductIndex = Programs[Programs.Keys.ToList()[0]][i].ProductIndex });
                            //        for (int j = 0; j < Programs[Programs.Keys.ToList()[0]][i].ProgramDatas.Count; j++)
                            //            CurrentProgram[i].ProgramDatas.Add(Programs[Programs.Keys.ToList()[0]][i].ProgramDatas[j].Clone());
                            //    }
                            //    CurrentProgramIndex = 0;
                            //}

                            //CurrentProgramDatas.Clear();
                            //if (CurrentProgram.Count > 0)
                            //{
                            //    for (int i = 0; i < CurrentProgram[0].ProgramDatas.Count; i++)
                            //        CurrentProgramDatas.Add(CurrentProgram[0].ProgramDatas[i].Clone());
                            //    CurrentProgramDatasIndex = 0;
                            //}
                            CurrentProgramIndex = 0;

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
                                if (Programs.Count > 0)
                                    CurrentProgram = Variables.DeepClone(Programs[Programs.Keys.ToList()[0]]);
                                CurrentProgramDatas.Clear();
                                if (CurrentProgram.Count > 0)
                                    CurrentProgramDatas = Variables.DeepClone(CurrentProgram[0].ProgramDatas);

                                //CurrentProgram.Clear();
                                //if (Programs.Keys.ToList().Count > 0)
                                //{
                                //    for (int i = 0; i < Programs[Programs.Keys.ToList()[0]].Count; i++)
                                //    {
                                //        CurrentProgram.Add(new SubProgram() { ProductIndex = Programs[Programs.Keys.ToList()[0]][i].ProductIndex });
                                //        for (int j = 0; j < Programs[Programs.Keys.ToList()[0]][i].ProgramDatas.Count; j++)
                                //            CurrentProgram[i].ProgramDatas.Add(Programs[Programs.Keys.ToList()[0]][i].ProgramDatas[j].Clone());
                                //    }

                                //    CurrentProgramIndex = 0;
                                //}

                                //CurrentProgramDatas.Clear();
                                //if (CurrentProgram.Count > 0)
                                //{
                                //    for (int i = 0; i < CurrentProgram[0].ProgramDatas.Count; i++)
                                //        CurrentProgramDatas.Add(CurrentProgram[0].ProgramDatas[i].Clone());
                                //    CurrentProgramDatasIndex = 0;
                                //}

                                CurrentProgramIndex = 0;
                                ProjectName = dig_openFileDialog.SafeFileName.Replace(".lprj", "");
                                CreateDate = Variables.CurrentProject.CreateDate;
                                LastDate = Variables.CurrentProject.LastDate;
                                ProjectPath = dig_openFileDialog.FileName;

                                ProgramsName.Clear();
                                for (int i = 0; i < Programs.Keys.Count; i++)
                                    ProgramsName.Add(Programs.Keys.ToList()[i]);
                                ProgramsIndex = 0;
                                Variables.ProgramName = ProgramsName[ProgramsIndex];

                                Log.Info("打开了项目 " + ProjectName);
                            }
                        }
                        catch { }
                        break;

                    case "save":
                        try
                        {
                            // 疑问点：保存问题
                            /*
                            项目文件 主要结构
                            第一层 project
                            第二层 project 下面的 programs
                            第三层 project 下面的 programs，program 下面的 ProgramDatas

                            全局
                                Variables.CurrentProject
                                Variables.CurrentProgramData
                                Variables.CurrentConfigSet

                            界面绑定
                                CurrentProgram
                                CurrentProgramDatas
                                CurrentSelectedProgramData

                            所有编辑保存的思路都是，
                            1. 当选择 program 时，将全局的 CurrentProject 对应的 program， deepclone 给 界面的 CurrentProgram，用于绑定显示
                            2. 接下来所有的操作均改变的是 CurrentProgram ，只要不保存，原来的项目不会变。
                            3. 当选择位置时，将 CurrentProgram 对应的 ProgramDatas 给 界面的 CurrentProgramDatas，用于绑定显示
                                   编辑完成后，将 CurrentProgramDatas 再deepclone 给 CurrentProgram 的 ProgramDatas
                                   CurrentConfigSet 也是一样，编辑完成后，更新 CurrentProgram

                            4. 保存是，将 CurrentProgram deepclone 给 Variables.CurrentProject

                             */

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

                                CurrentProgram[CurrentProgramIndex].ProgramDatas = Variables.DeepClone(CurrentProgramDatas);

                                Programs[Programs.Keys.ToList()[ProgramsIndex]] = Variables.DeepClone(CurrentProgram);

                                //Programs[Programs.Keys.ToList()[ProgramsIndex]] = Variables.DeepClone(ParamSetVars);
                                Variables.CurrentProject.Programs = Programs;

                                bool isSaveOK = true;

                                try
                                {
                                    Serialize.WriteJsonV2(Variables.CurrentProject, AppDomain.CurrentDomain.BaseDirectory + "cache.lprj");
                                    var cache = Serialize.ReadJsonV2<Project>(AppDomain.CurrentDomain.BaseDirectory + "cache.lprj");
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
        private int currentProgramIndex = -1;

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

        private ObservableCollection<ProgramData> currentProgramDatas = new ObservableCollection<ProgramData>();

        public ObservableCollection<ProgramData> CurrentProgramDatas
        {
            get { return currentProgramDatas; }
            set { SetProperty(ref currentProgramDatas, value); }
        }

        private int currentProgramDatasIndex;

        public int CurrentProgramDatasIndex
        {
            get { return currentProgramDatasIndex; }
            set { SetProperty(ref currentProgramDatasIndex, value); }
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
                                                var newProgram = Variables.DeepClone(Programs[ProgramsName[ProgramsIndex]]);

                                                //var newProgram = new ObservableCollection<SubProgram>();
                                                //for (int i = 0; i < Programs[ProgramsName[ProgramsIndex]].Count; i++)
                                                //{
                                                //    var subProgram = new SubProgram();
                                                //    subProgram.ProductIndex = Programs[ProgramsName[ProgramsIndex]][i].ProductIndex;
                                                //    subProgram.ProgramDatas = new ObservableCollection<ProgramData>();
                                                //    for (int j = 0; j < Programs[ProgramsName[ProgramsIndex]][i].ProgramDatas.Count; j++)
                                                //        subProgram.ProgramDatas.Add(Programs[ProgramsName[ProgramsIndex]][i].ProgramDatas[j].Clone());
                                                //    newProgram.Add(subProgram);
                                                //}

                                                Programs.Add(ProgramName, newProgram);
                                                ProgramsName.Add(ProgramName);
                                                ProgramsIndex = Programs.Count - 1;
                                                Variables.ProgramName = ProgramsName[ProgramsIndex];

                                                Log.Info("增加了程序");
                                                ProgramName = "";
                                            }
                                            else
                                            {
                                                ProgramsName.Add(ProgramName);
                                                Programs.Add(ProgramName, new ObservableCollection<SubProgram>());
                                                ProgramsIndex = Programs.Count - 1;
                                                Variables.ProgramName = ProgramsName[ProgramsIndex];

                                                Log.Info("增加了程序");
                                                ProgramName = "";
                                            }
                                        }
                                        else
                                        {
                                            ProgramsName.Add(ProgramName);
                                            Programs.Add(ProgramName, new ObservableCollection<SubProgram>());
                                            ProgramsIndex = Programs.Count - 1;
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
                    CurrentProgram.Clear();
                    CurrentProgram = Variables.DeepClone(Programs[ProgramsName[ProgramsIndex]]);

                    ////将程序集合中选中的程序给到界面
                    //CurrentProgram.Clear();
                    //for (int i = 0; i < Programs[ProgramsName[ProgramsIndex]].Count; i++)
                    //{
                    //    CurrentProgram.Add(new SubProgram() { ProductIndex = Programs[ProgramsName[ProgramsIndex]][i].ProductIndex });
                    //    for (int j = 0; j < Programs[ProgramsName[ProgramsIndex]][i].ProgramDatas.Count; j++)
                    //        CurrentProgram[i].ProgramDatas.Add(Programs[ProgramsName[ProgramsIndex]][i].ProgramDatas[j].Clone());
                    //}

                    Variables.ProgramName = ProgramsName[ProgramsIndex];

                    if (CurrentProgram.Count > 0)
                        CurrentProgramIndex = 0;
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
                            if (Variables.ShowConfirm("确认添加新的位置？") == true)
                            {
                                if (CurrentProgram.Count > 0)
                                {
                                    CurrentProgram.Add(new SubProgram() { ProductIndex = CurrentProgram.Count() });
                                    //Variables.ConfigSets[]
                                    Log.Info("增加了新的检测位置");
                                }
                                else
                                {
                                    CurrentProgram.Add(new SubProgram() { ProductIndex = CurrentProgram.Count() });
                                    Log.Info("增加了新的检测位置");
                                }
                            }
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

                try
                {
                    Programs[Programs.Keys.ToList()[ProgramsIndex]] = Variables.DeepClone(CurrentProgram);
                }
                catch { }
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
                                CurrentProgramDatas.Add(new ProgramData() { InspectFunction = inspectName, Content = content, IsUse = true });
                            }
                        });
                    }
                    catch { }
                    break;

                case "del":
                    try
                    {
                        if (Variables.ShowConfirm("确认删除检测？") == true)
                            CurrentProgramDatas.Remove(CurrentSelectedProgramData);
                    }
                    catch { }
                    break;

                case "locate":
                    try
                    {
                        DialogParameters param = new DialogParameters
                        {
                            { "Title", "" },
                            {"ProgramsIndex",ProgramsIndex },
                            {"CurrentProgramIndex",CurrentProgramIndex },
                            {"RowDefault",400 },//疑问点：这里需要读到图片然后拿到；现场相机像素2000w，分辨率固定，可以用固定值
                            {"ColDefault",400 },
                        };
                        //这里从当前程序获取
                        Variables.CurrentConfigSet = Variables.DeepClone(CurrentProgram[CurrentProgramIndex].ProductConfigSet);

                        Variables.CurDialogService.ShowDialog(DialogNames.ShowLocationDialog, param, callback =>
                        {
                            if (callback.Result == ButtonResult.Yes)
                            {
                                //var Program = callback.Parameters.GetValue<string>("Program");
                                //var RowDefault = callback.Parameters.GetValue<string>("RowDefault");
                                //var ColDefault = callback.Parameters.GetValue<string>("ColDefault");
                                //var RowInput = callback.Parameters.GetValue<string>("RowInput");
                                //var ColInput = callback.Parameters.GetValue<string>("ColInput");
                            }
                        });

                        //设置完成后还给当前程序
                        CurrentProgram[CurrentProgramIndex].ProductConfigSet = Variables.DeepClone(Variables.CurrentConfigSet);
                    }
                    catch (Exception ex) { }
                    break;
            }

            //将界面的programdata更新到项目的程序集合中。
            try
            {
                CurrentProgram[CurrentProgramIndex].ProgramDatas = Variables.DeepClone(CurrentProgramDatas);

                //Programs[ProgramsName[ProgramsIndex]][currentProgramIndex] = JsonConvert.DeserializeObject<SubProgram>(JsonConvert.SerializeObject(CurrentProgram[CurrentProgramIndex]));

                //Programs[ProgramsName[ProgramsIndex]][currentProgramIndex].ProgramDatas.Clear();
                //for (int i = 0; i < currentProgramDatas.Count; i++)
                //    Programs[ProgramsName[ProgramsIndex]][currentProgramIndex].ProgramDatas.Add(currentProgramDatas[i]);
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
                CurrentProgramDatas = Variables.DeepClone(CurrentProgram[CurrentProgramIndex].ProgramDatas);

                //CurrentProgramDatas.Clear();

                //for (int i = 0; i < CurrentProgram[CurrentProgramIndex].ProgramDatas.Count; i++)
                //    CurrentProgramDatas.Add(CurrentProgram[CurrentProgramIndex].ProgramDatas[i].Clone());
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
                    Variables.CurrentProgramData = Variables.DeepClone(CurrentProgramDatas[CurrentProgramDatasIndex]);

                    Variables.CurDialogService.ShowDialog(DialogNames.ToolNams[param]);

                    CurrentProgramDatas[CurrentProgramDatasIndex] = Variables.DeepClone(Variables.CurrentProgramData);

                    CurrentProgram[CurrentProgramIndex].ProgramDatas = Variables.DeepClone(CurrentProgramDatas);

                    Variables.IngoreAutoHome = false;
                }
                catch (Exception ex) { }
            }));

        private ObservableCollection<ProgramData> copyDatas = new ObservableCollection<ProgramData>();
        private string copyMsg = "";
        private DelegateCommand<string> _ctrlProgramDatasOperate;

        public DelegateCommand<string> CtrlProgramDatasOperate =>
            _ctrlProgramDatasOperate ?? (_ctrlProgramDatasOperate = new DelegateCommand<string>(ExecuteCtrlProgramDatasOperate));

        private void ExecuteCtrlProgramDatasOperate(string parameter)
        {
            CurrentProgramDatas.Remove(CurrentSelectedProgramData);

            try
            {
                CurrentProgram[CurrentProgramIndex].ProgramDatas = Variables.DeepClone(CurrentProgramDatas);
            }
            catch { }

            switch (parameter)
            {
                case "copy":
                    // 深拷贝程序数据
                    copyDatas = GlobalVars.Variables.DeepClone(CurrentProgram[CurrentProgramIndex].ProgramDatas);
                    // 设置复制提示消息
                    copyMsg = "确认从【程序" + ProgramsName[ProgramsIndex] + "】【" + (CurrentProgramIndex) + "】号位置粘贴吗？";
                    break;

                case "paste":
                    // 显示确认对话框
                    bool rst = GlobalVars.Variables.ShowConfirm(copyMsg);
                    // 粘贴程序数据
                    CurrentProgram[CurrentProgramIndex].ProgramDatas = Variables.DeepClone(copyDatas);

                    CurrentProgramDatas = Variables.DeepClone(CurrentProgram[CurrentProgramIndex].ProgramDatas);
                    break;

                case "clear":
                    // 显示确认对话框，询问是否清空当前位置
                    bool isClear = GlobalVars.Variables.ShowConfirm("是否清空当前位置？");
                    if (isClear)
                    {
                        // 再次显示确认对话框，确认是否清空当前位置
                        isClear = GlobalVars.Variables.ShowConfirm("请确认是否清空当前位置？");
                        if (isClear)
                        {
                            CurrentProgramDatas.Clear();
                            CurrentProgram[CurrentProgramIndex].ProgramDatas = Variables.DeepClone(CurrentProgramDatas);
                        }
                    }
                    break;
            }
        }

        #endregion 程序编辑

        #region Run方法

        private DelegateCommand _runAll;

        public DelegateCommand RunAll =>
            _runAll ?? (_runAll = new DelegateCommand(() =>
            {
                try
                {
                    //run在 run.cs中重写，测试的话可以重新写一个runtest方法

                    // 疑问点：  图片为了测试先写死了一个，需要修改
                    string path = @"C:\Users\PC\Desktop\202211181359345410.tif";
                    var rst = new HImage(path);
                    // run(rst);//需要换图片；
                }
                catch (Exception ex) { }
            }));

        #endregion Run方法
    }

    //项目类
    public class Project
    {
        //public Dictionary<int, List<ConfigSet>> ConfigSets = new Dictionary<int, List<ConfigSet>>();

        public Dictionary<string, ObservableCollection<SubProgram>> Programs = new Dictionary<string, ObservableCollection<SubProgram>>();
        public string CreateDate = "";
        public string LastDate = "";
    }

    // 疑问点：
    /*
        ConfigSet 放到 SubProgram 中，它是跟着检测位置走的。

        SubProgram 包含
            - 当前检测位置索引
            - 当前检测位置参数
            - 当前检测位置各检测项

     */

    public class SubProgram
    {
        /// <summary>
        /// 产品索引,位置
        /// </summary>
        public int ProductIndex { set; get; } = 0;

        //对应位置的参数设置
        public ConfigSet ProductConfigSet { set; get; } = new ConfigSet();

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