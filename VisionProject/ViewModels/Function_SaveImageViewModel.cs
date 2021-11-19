using HalconDotNet;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using VisionProject.GlobalVars;

namespace VisionProject.ViewModels
{
    public class Function_SaveImageViewModel : BindableBase, IDialogAware
    {
        #region 窗口相关

        private string _title = "保存图像";

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

        public Function_SaveImageViewModel()
        {
            init();
        }
        private async void init()
        {
            await Task.Delay(300);
            try
            {
                if (Variables.CurrentProgram[Variables.ProgramIndex].Parameters.ContainsKey("SaveMode"))
                    SaveMode = int.Parse(Variables.CurrentProgram[Variables.ProgramIndex]
                    .Parameters["SaveMode"].ToString());
                if (Variables.CurrentProgram[Variables.ProgramIndex].Parameters.ContainsKey("SaveFormat"))
                    SaveFormat = int.Parse(Variables.CurrentProgram[Variables.ProgramIndex]
                    .Parameters["SaveFormat"].ToString());
                if (Variables.CurrentProgram[Variables.ProgramIndex].Parameters.ContainsKey("SaveCount"))
                    SaveCount = (string)Variables.CurrentProgram[Variables.ProgramIndex]
                    .Parameters["SaveCount"];
                if (Variables.CurrentProgram[Variables.ProgramIndex].Parameters.ContainsKey("SavePath"))
                    SavePath = (string)Variables.CurrentProgram[Variables.ProgramIndex]
                    .Parameters["SavePath"];

              
            }
            catch(Exception ex) { }
        }

        private int _saveMode;
        public int SaveMode 
        {
            get { return _saveMode; }
            set { SetProperty(ref _saveMode, value); }
        }

        private int _saveFormat; 
        public int SaveFormat
        {
            get { return _saveFormat; }
            set { SetProperty(ref _saveFormat, value); }
        }

        private string _saveCount; 
        public string SaveCount
        {
            get { return _saveCount; }
            set { SetProperty(ref _saveCount, value); }
        }

        private string _savePath;
        public string SavePath
        { 
            get { return _savePath; }
            set { SetProperty(ref _savePath, value); }
        }


        private DelegateCommand _selectPath; 
        public DelegateCommand SelectPath =>
            _selectPath ?? (_selectPath = new DelegateCommand(()=> {
                System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                folderBrowserDialog.Description = "请选择保存的文件路径";
                var dlg = folderBrowserDialog.ShowDialog();
                if (dlg == System.Windows.Forms.DialogResult.OK)
                {
                    SavePath = folderBrowserDialog.SelectedPath + "\\";
                   
                }

            }));
         

        private DelegateCommand _saveParam;
        public DelegateCommand SaveParam =>
            _saveParam ?? (_saveParam = new DelegateCommand(() => {
                if (Variables.CurrentProgram[Variables.ProgramIndex].Parameters.ContainsKey("SaveMode"))
                    Variables.CurrentProgram[Variables.ProgramIndex].Parameters["SaveMode"] = SaveMode;
                else
                    Variables.CurrentProgram[Variables.ProgramIndex].Parameters.Add("SaveMode", SaveMode);

                if (Variables.CurrentProgram[Variables.ProgramIndex].Parameters.ContainsKey("SaveFormat"))
                    Variables.CurrentProgram[Variables.ProgramIndex].Parameters["SaveFormat"] = SaveFormat;
                else
                    Variables.CurrentProgram[Variables.ProgramIndex].Parameters.Add("SaveFormat", SaveFormat);

                if (Variables.CurrentProgram[Variables.ProgramIndex].Parameters.ContainsKey("SaveCount"))
                    Variables.CurrentProgram[Variables.ProgramIndex].Parameters["SaveCount"] = SaveCount;
                else
                    Variables.CurrentProgram[Variables.ProgramIndex].Parameters.Add("SaveCount", SaveCount);
                if (Variables.CurrentProgram[Variables.ProgramIndex].Parameters.ContainsKey("SavePath"))
                    Variables.CurrentProgram[Variables.ProgramIndex].Parameters["SavePath"] = SavePath;
                else
                    Variables.CurrentProgram[Variables.ProgramIndex].Parameters.Add("SavePath", SavePath);

               

            }));

    }
}
