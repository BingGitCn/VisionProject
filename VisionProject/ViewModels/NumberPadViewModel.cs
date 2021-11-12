using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VisionProject.ViewModels
{
    


    public class NumberPadViewModel : BindableBase
    {
        private bool _open;
        public bool Open
        {
            get { return _open; }
            set { SetProperty(ref _open, value); 
                if (_open)
                    DisplayValue = "";
            }
        }

        private string _displayValue;
        public string DisplayValue
        {
            get { return _displayValue; }
            set { SetProperty(ref _displayValue, value); }
        }


        private string _value;
        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }



        private DelegateCommand<string> _padPress;
        public DelegateCommand<string> PadPress =>
            _padPress ?? (_padPress = new DelegateCommand<string>((string param)=> {
                switch (param)
                {
                    case "Cancel":
                        Open = false;
                        break;
                    case "OK":
                        Open = false;
                        if (DisplayValue != "")
                        {
                            if (DisplayValue[DisplayValue.Length - 1] == '.')
                                DisplayValue = DisplayValue.Replace(".", "");
                        }
                        Value = DisplayValue;
                        if (CallBack != null)
                            CallBack(Value);
                        break;
                    case "0":
                        if (DisplayValue != "0")
                            DisplayValue += "0";
                        break;
                    case "1":
                        DisplayValue += "1";
                        break;
                    case "2":
                        DisplayValue += "2";
                        break;
                    case "3":
                        DisplayValue += "3";
                        break;
                    case "4":
                        DisplayValue += "4";
                        break;
                    case "5":
                        DisplayValue += "5";
                        break;
                    case "6":
                        DisplayValue += "6";
                        break;
                    case "7":
                        DisplayValue += "7";
                        break;
                    case "8":
                        DisplayValue += "8";
                        break;
                    case "9":
                        DisplayValue += "9";
                        break;
                    case "-":
                        if (DisplayValue != "")
                        {
                            if (DisplayValue.Contains("-"))
                                DisplayValue = DisplayValue.Replace("-", "");
                            else
                                DisplayValue = "-" + DisplayValue;
                        }
                        break;
                    case ".":
                        if (DisplayValue == "")
                        {

                        }
                        else if (!DisplayValue.Contains("."))
                            DisplayValue += ".";
                        break;
                    case "Back":
                        if (DisplayValue != "")
                        {
                            DisplayValue = DisplayValue.Remove(DisplayValue.Length - 1);
                        }
                        break;
                }



            }));

      



        public object Target;
      
        public void Task_PadPress(string param)
        {
            switch (param)
            {
                case "Cancel":
                    Open = false;
                    break;
                case "OK":
                    Open = false;
                    if (DisplayValue != "")
                    {
                        if (DisplayValue[DisplayValue.Length - 1] == '.')
                            DisplayValue = DisplayValue.Replace(".", "");
                    }
                    Value = DisplayValue;
                    if (CallBack != null)
                        CallBack(Value);
                    break;
                case "0":
                    if (DisplayValue != "0")
                        DisplayValue += "0";
                    break;
                case "1":
                    DisplayValue += "1";
                    break;
                case "2":
                    DisplayValue += "2";
                    break;
                case "3":
                    DisplayValue += "3";
                    break;
                case "4":
                    DisplayValue += "4";
                    break;
                case "5":
                    DisplayValue += "5";
                    break;
                case "6":
                    DisplayValue += "6";
                    break;
                case "7":
                    DisplayValue += "7";
                    break;
                case "8":
                    DisplayValue += "8";
                    break;
                case "9":
                    DisplayValue += "9";
                    break;
                case "-":
                    if (DisplayValue != "")
                    {
                        if (DisplayValue.Contains("-"))
                            DisplayValue = DisplayValue.Replace("-", "");
                        else
                            DisplayValue = "-" + DisplayValue;
                    }
                    break;
                case ".":
                    if (DisplayValue == "")
                    {

                    }
                    else if (!DisplayValue.Contains("."))
                        DisplayValue += ".";
                    break;
                case "Back":
                    if (DisplayValue != "")
                    {
                        DisplayValue = DisplayValue.Remove(DisplayValue.Length - 1);
                    }
                    break;
            }
        }
        public delegate void ProcessDelegate(string Result);
        public ProcessDelegate CallBack;

    }
}
