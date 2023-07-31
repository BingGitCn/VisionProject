﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using VisionProject.GlobalVars;

namespace VisionProject.ViewModels
{
    public class Dialog_LocationViewModel : BindableBase, IDialogAware
    {
        public Dialog_LocationViewModel()
        {
        }
        #region 参数
        private ObservableCollection<string> _programs = new ObservableCollection<string>();

        public ObservableCollection<string> Programs
        {
            get { return _programs; }
            set { SetProperty(ref _programs, value); }
        }

        private int _programIndex;

        public int ProgramIndex
        {
            get { return _programIndex; }
            set { SetProperty(ref _programIndex, value); }
        }


        private int _rowInput;

        public int RowInput
        {
            get { return _rowInput; }
            set { SetProperty(ref _rowInput, value); }
        }

        private int _colInput;

        public int ColInput
        {
            get { return _colInput; }
            set { SetProperty(ref _colInput, value); }
        }


      


        private int _row1;
        public int Row1
        {
            get { return _row1; }
            set { SetProperty(ref _row1, value); }
        }
        private int _row2;
        public int Row2
        {
            get { return _row2; }
            set { SetProperty(ref _row2, value); }
        }

        private int _column1;
        public int Column1
        {
            get { return _column1; }
            set { SetProperty(ref _column1, value); }
        }
        private int _column2;
        public int Column2
        {
            get { return _column2; }
            set { SetProperty(ref _column2, value); }
        }
   
        #endregion



        #region 窗口相关

        private string _title = "位置参数";

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

        private Dictionary<string, ParamSetVar> paramDict = new Dictionary<string, ParamSetVar>();

        // 疑问点：现在默认宽高由外部传入
        // 这里需整理下，定义了全局变量 CurrentConfigSet
        public void OnDialogOpened(IDialogParameters parameters)
        {
            try
            {
                Programs.Clear();

                ProgramsIndex = int.Parse(parameters.GetValue<string>("ProgramsIndex").ToString());
                CurrentProgramIndex = int.Parse(parameters.GetValue<string>("CurrentProgramIndex").ToString());
                var program = Variables.CurrentProject.Programs.ElementAt(ProgramsIndex).Value;

                foreach (var s in program)
                    Programs.Add(s.ProductIndex.ToString());
             
                // 直接获取
                RowInput = Variables.CurrentConfigSet.RowInput;
                ColInput = Variables.CurrentConfigSet.ColInput;
                Row1 = Variables.CurrentConfigSet.Row1;
                Column1 = Variables.CurrentConfigSet.Column1;
                Row2= Variables.CurrentConfigSet.Row2;
                Column2 = Variables.CurrentConfigSet.Column2;
            }
            catch { }
        }

        private int ProgramsIndex;
        private int CurrentProgramIndex;

        private DelegateCommand _confirm;

        public DelegateCommand Confirm =>
            _confirm ?? (_confirm = new DelegateCommand(ExecuteConfirm));

        private void ExecuteConfirm()
        {
            try
            {
                var rst = Variables.ShowConfirm("确认保存并退出？");
                if (!rst)
                {
                    return;
                }
                // 直接获取
                Variables.CurrentConfigSet.RowInput = RowInput;
                Variables.CurrentConfigSet.ColInput = ColInput;
                 Variables.CurrentConfigSet.Row1= Row1;
                 Variables.CurrentConfigSet.Column1= Column1;
                Variables.CurrentConfigSet.Row2 = Row2;
                Variables.CurrentConfigSet.Column2 = Column2;

                RequestClose?.Invoke(new DialogResult(ButtonResult.Yes));
            }
            catch { }
        }

        private DelegateCommand _valueEdit;

        public DelegateCommand ValueEdit =>
            _valueEdit ?? (_valueEdit = new DelegateCommand(ExecuteValueEdit));

        private void ExecuteValueEdit()
        {
            try
            {
                // 这里不做处理
                return;
                //Variables.CurrentConfigSet.RowInput = RowInput;
                //Variables.CurrentConfigSet.ColInput = ColInput;
            }
            catch { }
        }

        private DelegateCommand _cancel;

        public DelegateCommand Cancel =>
            _cancel ?? (_cancel = new DelegateCommand(ExecuteCancel));

        private void ExecuteCancel()
        {
            var rst = Variables.ShowConfirm("当前未保存，请确认是否退出？");
            if (!rst)
            {
                return;
            }
            RequestClose?.Invoke(new Prism.Services.Dialogs.DialogResult(ButtonResult.No));
        }

        #endregion 窗口相关
    }
}