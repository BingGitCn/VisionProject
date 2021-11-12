using BingLibrary.FileOpreate;
using HalconDotNet;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using VisionProject.GlobalVars;

namespace VisionProject.ViewModels
{
    public partial class MainWindowViewModel
    {
        private ObservableCollection<ProjectInfo> _projectNames = new ObservableCollection<ProjectInfo>();
        public ObservableCollection<ProjectInfo> ProjectNames
        {
            get { return _projectNames; }
            set { SetProperty(ref _projectNames, value); }
        }

        private ProjectInfo _selectProjectName=new ProjectInfo();
        public ProjectInfo SelectProjectName
        {
            get { return _selectProjectName; }
            set { SetProperty(ref _selectProjectName, value); }
        }

        private  void initProjects()
        {
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

        private int _projectIndex;
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

        int projectIndex = -1;

        private DelegateCommand<string> _openProject;
        public DelegateCommand<string> OpenProject =>
            _openProject ?? (_openProject = new DelegateCommand<string>((string param) => {

                switch (param) {
                    case "close":
                        try
                        {
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
                            //Programs = Variables.CurrentProject.Programs;
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
                            dig_openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                            dig_openFileDialog.Filter = "项目文件(*.lprj)|*.lprj";
                            if (dig_openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                Variables.CurrentProject = Serialize.ReadJsonV2<Project>(dig_openFileDialog.FileName);
                                //Programs = Variables.CurrentProject.Programs;
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
                            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                            saveFileDialog.Filter = "项目文件(*.lprj)|*.lprj";
                            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                // GlobalVars.Variables.CurrentProject.Programs = Programs;
                                Serialize.WriteJsonV2(Variables.CurrentProject, saveFileDialog.FileName);

                                if (Variables.CurrentProject.CreateDate == "")
                                {
                                    Variables.CurrentProject.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    CreateDate = Variables.CurrentProject.CreateDate;
                                }
                                Variables.CurrentProject.LastDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                LastDate = Variables.CurrentProject.LastDate;
                                ProjectPath = saveFileDialog.FileName;
                            }
                        }
                        catch { }
                        break;
                }
            }));
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

        public Dictionary<string, object> Parameters = new Dictionary<string, object>();

        [JsonIgnore]
        public Dictionary<string, object> Results = new Dictionary<string, object>();
    }

    public class ProjectInfo:BindableBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value); }
        }
    }
}