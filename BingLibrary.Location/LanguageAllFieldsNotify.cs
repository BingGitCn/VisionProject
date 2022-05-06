using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace BingLibrary.Location
{
    public class LanguageNotify : NotifyPropertyChanged
    {
        public virtual string GetValue(string Key) => "";

        public virtual void SetValue(string Key, string Value)
        { }

        public string Get([CallerMemberName] string PropertyName = null)
        {
            return GetValue(PropertyName);
        }

        public void Set(string Value, [CallerMemberName] string PropertyName = null)
        {
            SetValue(PropertyName, Value);

            RaisePropertyChanged(PropertyName);
        }

        public virtual event Action<CultureInfo> LanguageChanged;
    }
}