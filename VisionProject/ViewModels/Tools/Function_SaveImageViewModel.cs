using HalconDotNet;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VisionProject.GlobalVars;



namespace VisionProject.ViewModels
{
    
    public class Function_SaveImageViewModel : BindableBase, IDialogAware,IFunction_ViewModel_Interface
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

       public  Function_SaveImageViewModel()
        {
          _=   Init(); 
        }
       public async Task<bool> Init()
        {
            await Task.Delay(300);
            try
            {
                if (Variables.CurrentSubProgram.Parameters.ContainsKey("SaveMode"))
                    SaveMode = int.Parse(Variables.CurrentSubProgram.Parameters["SaveMode"].ToString());
                if (Variables.CurrentSubProgram.Parameters.ContainsKey("SaveFormat"))
                    SaveFormat = int.Parse(Variables.CurrentSubProgram.Parameters["SaveFormat"].ToString());
                
                if (Variables.CurrentSubProgram.Parameters.ContainsKey("SaveCount"))
                    SaveCount = (string)Variables.CurrentSubProgram.Parameters["SaveCount"];
                if (Variables.CurrentSubProgram.Parameters.ContainsKey("SavePath"))
                    SavePath = (string)Variables.CurrentSubProgram.Parameters["SavePath"];

                //清除多余的key，仅保留当前设置。
                Variables.CurrentSubProgram.Parameters.Clear();
                Update();



                return true;

               
            }
            catch(Exception ex) {  ; return false; }
         
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
                    SavePath = folderBrowserDialog.SelectedPath ;
                   
                }

            }));
         
        

        private DelegateCommand _saveParam;
        public DelegateCommand SaveParam =>
            _saveParam ?? (_saveParam = new DelegateCommand(() => {

                Update();


            }));


      public  bool Update()
        {
            if (Variables.CurrentSubProgram.Parameters.ContainsKey("SaveMode"))
                Variables.CurrentSubProgram.Parameters["SaveMode"] = SaveMode;
            else
                Variables.CurrentSubProgram.Parameters.Add("SaveMode", SaveMode);

            if (Variables.CurrentSubProgram.Parameters.ContainsKey("SaveFormat"))
                Variables.CurrentSubProgram.Parameters["SaveFormat"] = SaveFormat;
            else
                Variables.CurrentSubProgram.Parameters.Add("SaveFormat", SaveFormat);

            if (Variables.CurrentSubProgram.Parameters.ContainsKey("SaveCount"))
                Variables.CurrentSubProgram.Parameters["SaveCount"] = SaveCount;
            else
                Variables.CurrentSubProgram.Parameters.Add("SaveCount", SaveCount);
            if (Variables.CurrentSubProgram.Parameters.ContainsKey("SavePath"))
                Variables.CurrentSubProgram.Parameters["SavePath"] = SavePath;
            else
                Variables.CurrentSubProgram.Parameters.Add("SavePath", SavePath);
            
            return true;
        }


         

        public static Dictionary<string, object> Run(HImage image,string name, Dictionary<string, object> parameters)
        {
            try
            {
                    if (!Directory.Exists(parameters["SavePath"] + "\\" + System.DateTime.Now.ToString("yyyy-MM-dd")))
                        Directory.CreateDirectory(parameters["SavePath"] + "\\" + System.DateTime.Now.ToString("yyyy-MM-dd"));
                    saveImage(image, parameters["SavePath"] + "\\" + System.DateTime.Now.ToString("yyyy-MM-dd") + "\\" + name, parameters["SaveFormat"].ToString());



            } catch { }

            return parameters;
        }

        private class FileComparer : IComparer
        {
            int IComparer.Compare(Object o1, Object o2)
            {
                FileInfo fi1 = o1 as FileInfo;
                FileInfo fi2 = o2 as FileInfo;
                return fi1.LastWriteTime.CompareTo(fi2.LastWriteTime);
            }
        }

        private static void saveImage(HImage image, string name, string f)
        {
            if (f == "0")
                image.WriteImage("bmp", new HTuple(0), new HTuple(name + ".bmp"));
            else
                image.WriteImage("jpeg", new HTuple(0), new HTuple(name + ".jpg"));
        }

       

      
    }
}
