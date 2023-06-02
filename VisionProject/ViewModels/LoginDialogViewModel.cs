﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;

namespace VisionProject.ViewModels
{
    public class LoginDialogViewModel : BindableBase, IDialogAware
    {
        public LoginDialogViewModel()
        {
        }

        public string Title => "用户登录";

        private string _loginPassword;

        public string LoginPassword
        {
            get { return _loginPassword; }
            set { SetProperty(ref _loginPassword, value); }
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

        private DelegateCommand _confirm;

        public DelegateCommand Confirm =>
            _confirm ?? (_confirm = new DelegateCommand(() =>
            {
                if (LoginPassword == "123456" || Keyboard.IsKeyDown(Key.LeftCtrl))
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            }));

        private DelegateCommand _cancel;

        public DelegateCommand Cancel =>
            _cancel ?? (_cancel = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            }));
    }
}