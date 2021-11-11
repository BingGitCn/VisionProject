using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Windows;
using OfficeOpenXml;
using System.IO;
using VisionProject.GlobalVars;
using System;
using OfficeOpenXml.Style;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BingLibrary.FileOpreate;
using System.Windows.Controls;
using System.Collections.Generic;
using Newtonsoft.Json;
using HalconDotNet;

namespace VisionProject.ViewModels
{
    public partial class MainWindowViewModel
    {

        private string _projectName;
        public string ProjectName
        { 
            get { return _projectName; }
            set { SetProperty(ref _projectName, value); }
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


        private DelegateCommand<string> _projectOperate;
        public DelegateCommand<string> ProjectOperate =>
            _projectOperate ?? (_projectOperate = new DelegateCommand<string>((string param) =>{
                switch (param) {
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




}
