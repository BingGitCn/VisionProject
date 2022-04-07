using BingLibrary.FileOpreate;
using BingLibrary.Logs;
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
        /// <summary>
        /// 项目合集
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
        /// 初始化项目
        /// </summary>
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

        private string _createDate ;
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
                            Program1.Clear();
                            if (Programs.Keys.ToList().Count > 0)
                                for (int i = 0; i < Programs[Programs.Keys.ToList()[0]].Count; i++)
                                    Program1.Add(Programs[Programs.Keys.ToList()[0]][i]);
                            ProgramsIndex = 0;
                            ProjectName = SelectProjectName.Name;
                            CreateDate = Variables.CurrentProject.CreateDate;
                            LastDate = Variables.CurrentProject.LastDate;
                            ProjectPath = SelectProjectName.Path;
                            Log.Info("打开了项目 "+ ProjectName);

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
                            if (SystemConfig.IsRestoreDirectory)
                                dig_openFileDialog.RestoreDirectory = true;
                            else
                                dig_openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                            dig_openFileDialog.Filter = "项目文件(*.lprj)|*.lprj";
                            if (dig_openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                Variables.CurrentProject = new Project();
                                Variables.CurrentProject = Serialize.ReadJsonV2<Project>(dig_openFileDialog.FileName);
                                Programs = Variables.CurrentProject.Programs;
                                Program1.Clear();
                                if (Programs.Keys.ToList().Count > 0)
                                    for (int i = 0; i < Programs[Programs.Keys.ToList()[0]].Count; i++)
                                        Program1.Add(Programs[Programs.Keys.ToList()[0]][i]);
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
                                    Serialize.WriteJsonV2(Variables.CurrentProject, saveFileDialog.FileName);
                                }
                            }
                            Log.Info("保存了项目");
                        }
                        catch { }
                        break;
                }
            }));

        #endregion 项目编辑

        #region 程序编辑

        /*
         * 说明：
         * Program1 为当前程序用。主要是界面交互。
         * Programs 为程序合集。
         * Variables.CurrentProgram 为全局变量，主要是用于子工具中使用。
         * 
         */

        //程序索引
        private int program1Index;

        public int Program1Index
        {
            get { return program1Index; }
            set { SetProperty(ref program1Index, value); }
        }


        //界面上的多程序
        public Dictionary<string, ObservableCollection<SubProgram>> Programs = new Dictionary<string, ObservableCollection<SubProgram>>();
        //程序列表
        private ObservableCollection<SubProgram> program1 = new ObservableCollection<SubProgram>();
        /// <summary>
        /// 界面上当前编辑的程序
        /// </summary>
        public ObservableCollection<SubProgram> Program1
        {
            get { return program1; }
            set { SetProperty(ref program1, value); }
        }

      

        //各程序索引
        private int programsIndex=0;

        public int ProgramsIndex
        {
            get { return programsIndex; }
            set { SetProperty(ref programsIndex, value); }
        }
        //各程序名称
        private ObservableCollection<string> programsName = new ObservableCollection<string>();

        public ObservableCollection<string> ProgramsName
        {
            get { return programsName; }
            set { SetProperty(ref programsName, value); }
        }

        private string _programName="";
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
                                if (ProgramName != "")
                                {
                                    if (!Programs.Keys.ToList().Contains(ProgramName))
                                    {
                                        ProgramsName.Add(ProgramName);
                                        Programs.Add(ProgramName, new ObservableCollection<SubProgram>());
                                        Variables.ProgramName = ProgramsName[ProgramsIndex];
                                        Log.Info("增加了程序");
                                    }
                                
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
                               
                                    var p = Programs[ProgramsName[ProgramsIndex]];
                                    for (int i = 0; i < p.Count; i++)
                                        Variables.CurrentProject.Parameters.Remove(p[i].ID);


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
            _selectProgram ?? (_selectProgram = new DelegateCommand(()=> {


                try
                {
                    //将程序集合中选中的程序给到界面
                    Program1.Clear();
                    for (int i = 0; i < Programs[ProgramsName[ProgramsIndex]].Count; i++)
                        Program1.Add(Programs[ProgramsName[ProgramsIndex]][i]);
                    Variables.ProgramName = ProgramsName[ProgramsIndex];
                }
                catch (Exception ex) { }
               
               

            }));

       
      
        


        //增加删除子程序
        private DelegateCommand<string> _programs1Operate;

        public DelegateCommand<string> Programs1Operate =>
            _programs1Operate ?? (_programs1Operate = new DelegateCommand<string>((string param) =>
            {
                switch (param)
                {
                    case "add":
                        try
                        {
                            SubProgram sp = new SubProgram();
                            sp.ID = Guid.NewGuid().ToString();
                            Program1.Add(sp);
                            if (Variables.CurrentProject.Parameters.ContainsKey(sp.ID))
                                Variables.CurrentProject.Parameters.Remove(sp.ID);
                            Variables.CurrentProject.Parameters.Add(sp.ID, new Dictionary<string, object>());



                        }
                        catch { }
                        break;

                    case "del":
                        try
                        {
                            if (Variables.ShowConfirm("确认删除当前子程序？") == true)
                            {
                                string id = Program1[Program1Index].ID;
                                Variables.CurrentProject.Parameters.Remove(id);
                                Program1.RemoveAt(Program1Index);
                            }
                        }
                        catch { }
                        break;
                }

                //将界面的program更新到项目的程序集合中。
                try
                {
                    Programs[ProgramsName[ProgramsIndex]].Clear();
                    for (int i = 0; i < Program1.Count; i++)
                        Programs[ProgramsName[ProgramsIndex]].Add(Program1[i]);

                  

                }
                catch (Exception ex) { }





            }));


        //Step..
        private DelegateCommand<string> _program1Config;
        public DelegateCommand<string> Program1Config =>
            _program1Config ?? (_program1Config = new DelegateCommand<string>((param)=> {
                try
                {
                    Variables.ProgramIndex = Program1Index;
                    //Program1.Clear();
                    //for (int i = 0; i < Programs[ProgramsName[ProgramsIndex]].Count; i++)
                    //    Program1.Add(Programs[ProgramsName[ProgramsIndex]][i]);


                    Variables.CurrentProgram.Clear();
                    for (int i = 0; i < Program1.Count; i++)
                        Variables.CurrentProgram.Add(Program1[i]);

                    Variables.CurrentImage1 = Variables.WindowData1.CurrentImage;

                    curDialogService.ShowDialog(DialogNames.ToolNams[param]);

                    //Program1.Clear();
                    //for (int i = 0; i < Variables.CurrentProgram.Count; i++)
                    //    Program1.Add(Variables.CurrentProgram[i]);

                    //Programs[ProgramsName[ProgramsIndex]].Clear();
                    //for (int i = 0; i < Program1.Count; i++)
                    //    Programs[ProgramsName[ProgramsIndex]].Add(Program1[i]);


                }
                catch (Exception ex) { }
            }));

       

        #endregion 程序1编辑
    }

    //项目类
    public class Project
    {

        public Dictionary<string, ObservableCollection<SubProgram>> Programs = new Dictionary<string, ObservableCollection<SubProgram>>();

        /// <summary>
        /// 检测参数
        /// </summary>
        public Dictionary<string, Dictionary<string, object>> Parameters = new Dictionary<string, Dictionary<string, object>>();
        /// <summary>
        /// 检测结果
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, Dictionary<string, object>> Results = new Dictionary<string, Dictionary<string, object>>();

        public string CreateDate = "";
        public string LastDate = "";
    }

    //程序类
    public class SubProgram
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsUse { set; get; } = true;
        /// <summary>
        /// 产品索引
        /// </summary>
        public int ProductIndex { set; get; } = 0; 

        public string InspectFunction { set; get; } = "无";
        

        public string Content { set; get; }

        /// <summary>
        /// 产品索引
        /// </summary>
        [JsonIgnore]
        public List<int> ProductIndexs { set; get; } = new List<int>() { 0, 1, 2, };
        [JsonIgnore]
        public List<string> ToolNames { set; get; } = DialogNames.ToolNams.Keys.ToList();

        public SubProgram() {

            ProductIndexs.Clear();
            for (int i = 0; i < 48; i++) ProductIndexs.Add(i);

           
        }

        /// <summary>
        /// 身份标识
        /// </summary>
        public string ID { set; get; }

    }

    public class ProjectInfo
    {
        public string Name { set; get; }
        public string Path { set; get; }
    }
}