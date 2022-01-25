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

namespace VisionProject.ViewModels
{
    public partial class MainWindowViewModel
    {
        #region 项目编辑

        private ObservableCollection<ProjectInfo> _projectNames = new ObservableCollection<ProjectInfo>();

        public ObservableCollection<ProjectInfo> ProjectNames
        {
            get { return _projectNames; }
            set { SetProperty(ref _projectNames, value); }
        }

        private ProjectInfo _selectProjectName = new ProjectInfo();

        public ProjectInfo SelectProjectName
        {
            get { return _selectProjectName; }
            set { SetProperty(ref _selectProjectName, value); }
        }

        private void initProjects()
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
        }

        private string _projectName;

        public string ProjectName
        {
            get { return _projectName; }
            set { SetProperty(ref _projectName, value); }
        }

        private int _projectIndex = -1;

        public int ProjectIndex
        {
            get { return _projectIndex; }
            set { SetProperty(ref _projectIndex, value); }
        }

        private string _projectPath;

        public string ProjectPath
        {
            get { return _projectPath; }
            set { SetProperty(ref _projectPath, value); }
        }

        private string _createDate;

        public string CreateDate
        {
            get { return _createDate; }
            set { SetProperty(ref _createDate, value); }
        }

        private string _lastDate;

        public string LastDate
        {
            get { return _lastDate; }
            set { SetProperty(ref _lastDate, value); }
        }

        private int projectIndex = -1;

        private DelegateCommand<string> _openProject;

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
                            Programs1 = Variables.CurrentProject.Programs1;
                            ProjectName = SelectProjectName.Name;
                            CreateDate = Variables.CurrentProject.CreateDate;
                            LastDate = Variables.CurrentProject.LastDate;
                            ProjectPath = SelectProjectName.Path;
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
                            if (SystemConfig.IsRestoreDirectory)
                                dig_openFileDialog.RestoreDirectory = true;
                            else
                                dig_openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                            dig_openFileDialog.Filter = "项目文件(*.lprj)|*.lprj";
                            if (dig_openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                Variables.CurrentProject = Serialize.ReadJsonV2<Project>(dig_openFileDialog.FileName);
                                Programs1 = Variables.CurrentProject.Programs1;
                                ProjectName = dig_openFileDialog.SafeFileName.Replace(".lprj", "");
                                CreateDate = Variables.CurrentProject.CreateDate;
                                LastDate = Variables.CurrentProject.LastDate;
                                ProjectPath = dig_openFileDialog.FileName;
                            }
                        }
                        catch { }
                        break;

                    case "save":
                        try
                        {
                            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                            saveFileDialog.Title = ("请选择项目文件路径");
                            if (SystemConfig.IsRestoreDirectory)
                                saveFileDialog.RestoreDirectory = true;
                            else
                                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                            saveFileDialog.Filter = "项目文件(*.lprj)|*.lprj";
                            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                if (Variables.CurrentProject.CreateDate == "")
                                {
                                    Variables.CurrentProject.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    CreateDate = Variables.CurrentProject.CreateDate;
                                }
                                Variables.CurrentProject.LastDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                LastDate = Variables.CurrentProject.LastDate;
                                ProjectPath = saveFileDialog.FileName;

                                Variables.CurrentProject.Programs1 = Programs1;

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
                                    Serialize.WriteJsonV2(Variables.CurrentProject, saveFileDialog.FileName);
                                }
                            }
                        }
                        catch { }
                        break;
                }
            }));

        #endregion 项目编辑

        #region 程序1编辑

        //程序索引
        private int program1Index;

        public int Program1Index
        {
            get { return program1Index; }
            set { SetProperty(ref program1Index, value); }
        }

        //程序列表
        private ObservableCollection<Program> programs1 = new ObservableCollection<Program>();

        public ObservableCollection<Program> Programs1
        {
            get { return programs1; }
            set { SetProperty(ref programs1, value); }
        }

        //当前选择的程序
        private string program1;

        public string Program1
        {
            get { return program1; }
            set { SetProperty(ref program1, value); }
        }

        //增加删除程序
        private DelegateCommand<string> _programs1Operate;

        public DelegateCommand<string> Programs1Operate =>
            _programs1Operate ?? (_programs1Operate = new DelegateCommand<string>((string param) =>
            {
                switch (param)
                {
                    case "add":
                        try
                        {
                            Programs1.Add(new Program());
                        }
                        catch { }
                        break;

                    case "del":
                        try
                        {
                            Programs1.RemoveAt(program1Index);
                        }
                        catch { }
                        break;
                }
            }));


        //Step..
        private DelegateCommand<string> _program1Config;
        public DelegateCommand<string> Program1Config =>
            _program1Config ?? (_program1Config = new DelegateCommand<string>((param)=> {
                try
                {
                    Variables.ProgramIndex = Program1Index;
                    Variables.CurrentProgram.Clear();
                    for (int i = 0; i < Programs1.Count; i++)
                        Variables.CurrentProgram.Add(Programs1[i]);

                    Variables.CurrentImage1 = Variables.WindowData1.CurrentImage;

                    curDialogService.ShowDialog(DialogNames.ToolNams[param]);
                    
                    Programs1.Clear();
                    for (int i = 0; i < Variables.CurrentProgram.Count; i++)
                        Programs1.Add(Variables.CurrentProgram[i]);
                }
                catch (Exception ex) { }
            }));

       

        ////程序编辑
        //private DelegateCommand _program1Config;

        ////这里，如果有多个程序列表编辑，先将对应程序给到全局变量程序，然后编辑是在全局变量中完成，编辑完成后再给对应的程序

        //public DelegateCommand Program1Config =>
        //    _program1Config ?? (_program1Config = new DelegateCommand(() =>
        //    {
        //        try
        //        {
        //            Variables.ProgramIndex = Program1Index;
        //            Variables.CurrentProgram.Clear();
        //            for (int i = 0; i < Programs1.Count; i++)
        //                Variables.CurrentProgram.Add(Programs1[i]);

        //            Variables.CurrentImage1 = Variables.WindowData1.CurrentImage;

        //            if (Programs1[Program1Index].InspectFunction == "无")
        //            {
        //                curDialogService.ShowDialog(DialogNames.ShowFunctionTestWindow);
        //            }
        //            if (Programs1[Program1Index].InspectFunction == "保存图像")
        //            {
        //                curDialogService.ShowDialog(DialogNames.ShowFunctionSaveImageWindow);
        //                Programs1.Clear();
        //                for (int i = 0; i < Variables.CurrentProgram.Count; i++)
        //                    Programs1.Add(Variables.CurrentProgram[i]);
        //            }
        //        }
        //        catch (Exception ex) { }
        //    }));

        #endregion 程序1编辑
    }

    //项目类
    public class Project
    {
        public ObservableCollection<Program> Programs1 = new ObservableCollection<Program>();

        public string CreateDate = "";
        public string LastDate = "";
    }

    //程序类
    public class Program
    {
        public bool IsUse { set; get; } = true;
        public int ProductIndex { set; get; } = 0; 

        public string InspectFunction { set; get; } = "无";

        public Dictionary<string, object> Parameters = new Dictionary<string, object>();

        [JsonIgnore]
        public Dictionary<string, object> Results = new Dictionary<string, object>();

        public string Content { set; get; }

        public List<int> ProductIndexs { set; get; } = new List<int>() { 0,1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
        21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48};

        public List<string> ToolNames { set; get; } = DialogNames.ToolNams.Keys.ToList();
    }

    public class ProjectInfo
    {
        public string Name { set; get; }
        public string Path { set; get; }
    }
}