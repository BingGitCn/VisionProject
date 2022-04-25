using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisionProject.ViewModels
{
    public class Function_Test1ViewModel : BindableBase, IDialogAware,IFunction_ViewModel_Interface
    {
        public Function_Test1ViewModel()
        {

        }

        public Task<bool> Init()
        {
            throw new NotImplementedException();
        }

        public bool Update()
        {
            throw new NotImplementedException();
        }



        #region 窗口相关

        private string _title = "测试窗口1，无其它用途。";

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
    }
}
