using System;
using System.Globalization;
using System.Windows.Data;

namespace BingLibrary.Location
{
    /// <summary>
    /// 转换类，用于始终显示中文语言选项
    /// </summary>
    public class LanguageConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            if (Value is CultureInfo c)
            {
                switch (c.Name)
                {
                    case "en":
                        return "English";

                    case "zh-CN":
                        return "Chinese (Simplified) (中文简体)";

                    case "zh-TW":
                        return "Chinese (Traditional) (中文繁体)";

                    default:
                        return $"{c.DisplayName} ({c.NativeName})";
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Binding.DoNothing;
        }
    }
}