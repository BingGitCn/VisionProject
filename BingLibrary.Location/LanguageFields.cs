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
        public string About
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