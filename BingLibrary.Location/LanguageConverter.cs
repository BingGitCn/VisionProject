using System;
using System.Collections.Generic;
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
                if (Languages.ContainsKey(c.Name))
                {
                    var ci = new CultureInfo(Languages[c.Name], false);
                    return $"{ci.DisplayName} ({ci.NativeName})";
                }
                else
                {
                    switch (c.Name)
                    {
                        default:
                            return $"{c.DisplayName} ({c.NativeName})";
                    }
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Binding.DoNothing;
        }

        public Dictionary<string, string> Languages = new Dictionary<string, string>() {
            { "en","en-US"},//英语
            { "zh-cn","zh-CN"},//中文简体
            { "cht","zh-TW"},//中文繁体
            { "jp","ja-JP"},//日语
            { "kor","ko-KR"},//韩语
            { "fra","fr"},//法语
            { "spa","es"},//西班牙语
            { "th","th-TH"},//泰语
            { "ara","ar"},//阿拉伯语
            { "ru","ru"},//俄语
            { "pt","pt"},//葡萄牙语
            { "de","de"},//德语
            { "it","it"},//意大利语
            { "el","el"},//希腊语
            { "nl","nl"},//荷兰语
            { "pl","pl"},//波兰语
            { "cs","cs"},//捷克语
            { "vie","vi"},//越南语
        };
    }
}