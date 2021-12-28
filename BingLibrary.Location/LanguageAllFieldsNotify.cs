using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BingLibrary.Location
{
    public partial class LanguageAllFields : NotifyPropertyChanged
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
