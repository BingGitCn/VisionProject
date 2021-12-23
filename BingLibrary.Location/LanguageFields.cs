using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace BingLibrary.Location
{
    /// <summary>
    /// 用于页面需要切换语言的属性绑定，此仅用作切换语言显示用，不可参与软件逻辑操作。
    /// </summary>
    public partial class LanguageFields
    {
        public string Home
        {
            get => Get();
            set => Set(value);
        }
        public string Project
        {
            get => Get();
            set => Set(value);
        }
        public string Setting
        {
            get => Get();
            set => Set(value);
        }
        public string Statistic
        {
            get => Get();
            set => Set(value);
        }
        public string Log
        {
            get => Get();
            set => Set(value);
        }
        public string About
        {
            get => Get();
            set => Set(value);
        }

        public string ShowCurrentProjectName
        {
            get => Get();
            set => Set(value);
        }
        public string ShowRunResult
        {
            get => Get();
            set => Set(value);
        }
        public string ShowRunStatus
        {
            get => Get();
            set => Set(value);
        }
        public string ShowRunStatistic
        {
            get => Get();
            set => Set(value);
        }
        public string ButtonClearRunStatistic
        {
            get => Get();
            set => Set(value);
        }
        public string SelectProject
        {
            get => Get();
            set => Set(value);
        }
        public string Login
        {
            get => Get();
            set => Set(value);
        }
        public string ProjectManagement
        {
            get => Get();
            set => Set(value);
        }

        public string ButtonOpen
        {
            get => Get();
            set => Set(value);
        }
        public string ButtonSave
        {
            get => Get();
            set => Set(value);
        }
        public string ProjectPath
        {
            get => Get();
            set => Set(value);
        }
        public string CreateDate
        {
            get => Get();
            set => Set(value);
        }
        public string UpdateDate
        {
            get => Get();
            set => Set(value);
        }
        public string ProgramEdit
        {
            get => Get();
            set => Set(value);
        }
        public string ButtonAdd
        {
            get => Get();
            set => Set(value);
        }
        public string ButtonDelete
        {
            get => Get();
            set => Set(value);
        }
        public string Dragging
        {
            get => Get();
            set => Set(value);
        }
        public string DataGridDate
        {
            get => Get();
            set => Set(value);
        }
        public string DataGridOK
        {
            get => Get();
            set => Set(value);
        }
        public string DataGridNG
        {
            get => Get();
            set => Set(value);
        }
        public string DataGridAll
        {
            get => Get();
            set => Set(value);
        }
        public string DataGridRate
        {
            get => Get();
            set => Set(value);
        }

        public string ButtonRefresh
        {
            get => Get();
            set => Set(value);
        }
        public string ButtonExport
        {
            get => Get();
            set => Set(value);
        }
        public string ButtonUpdatePassword
        {
            get => Get();
            set => Set(value);
        }
        public string ShowTextWarn
        {
            get => Get();
            set => Set(value);
        }
        public string IDUser
        {
            get => Get();
            set => Set(value);
        }
        public string IDTechnician
        {
            get => Get();
            set => Set(value);
        }
        public string IDEngineer
        {
            get => Get();
            set => Set(value);
        }
        public string IDAdministrator
        {
            get => Get();
            set => Set(value);
        }
        public string SoftName
        {
            get => Get();
            set => Set(value);
        }
        public string UserLevel
        {
            get => Get();
            set => Set(value);
        }
        public string OldPassword
        {
            get => Get();
            set => Set(value);
        }
        public string NewPassword
        {
            get => Get();
            set => Set(value);
        }
        public string RememberParh
        {
            get => Get();
            set => Set(value);
        }
        public string ShowTextWarn2
        {
            get => Get();
            set => Set(value);
        }
        public string Drive
        {
            get => Get();
            set => Set(value);
        }
        public string AlarmThreshold
        {
            get => Get();
            set => Set(value);
        }
        public string Language
        {
            get => Get();
            set => Set(value);
        }
  

    }

    public partial class LanguageFields : NotifyPropertyChanged
    {
        protected virtual string GetValue(string Key) => "";

        protected virtual void SetValue(string Key, string Value)
        { }

        private string Get([CallerMemberName] string PropertyName = null)
        {
            return GetValue(PropertyName);
        }

        private void Set(string Value, [CallerMemberName] string PropertyName = null)
        {
            SetValue(PropertyName, Value);

            RaisePropertyChanged(PropertyName);
        }

        public virtual event Action<CultureInfo> LanguageChanged;
    }
}