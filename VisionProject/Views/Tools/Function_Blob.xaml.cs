using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using VisionProject.GlobalVars;

namespace VisionProject.Views
{
    /// <summary>
    /// Interaction logic for Function_Blob
    /// </summary>
    public partial class Function_Blob : UserControl
    {
        public Function_Blob()
        {
            InitializeComponent();
            Variables.ImageWindowDataForFunction = aimg.windowData;
            //if (c1.IsChecked == true)
            //{
            //    g11.Visibility = System.Windows.Visibility.Visible;
            //    g12.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    g11.Visibility = System.Windows.Visibility.Collapsed;
            //    g12.Visibility = System.Windows.Visibility.Collapsed;
            //}
        }

        private void cb_DropDownClosed(object sender, System.EventArgs e)
        {
            //if (cb.SelectedIndex == 0)
            //{
            //    g1.Visibility = System.Windows.Visibility.Visible;
            //    g2.Visibility = System.Windows.Visibility.Collapsed;
            //}
            //else
            //{
            //    g1.Visibility = System.Windows.Visibility.Collapsed;
            //    g2.Visibility = System.Windows.Visibility.Visible;
            //}
        }

        private void UserControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBox textBox = Keyboard.FocusedElement as TextBox;
            if (textBox != null)
            {
                TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                textBox.MoveFocus(tRequest);
            }
        }
    }

    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type _enumType;

        public Type EnumType
        {
            get { return _enumType; }
            set
            {
                if (value != _enumType)
                {
                    if (null != value)
                    {
                        var enumType = Nullable.GetUnderlyingType(value) ?? value;
                        if (!enumType.IsEnum)
                        {
                            throw new ArgumentException("Type must bu for an Enum");
                        }
                    }

                    _enumType = value;
                }
            }
        }

        public EnumBindingSourceExtension()
        {
        }

        public EnumBindingSourceExtension(Type enumType)
        {
            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == _enumType)
            {
                throw new InvalidOperationException("The EnumTYpe must be specified.");
            }

            var actualEnumType = Nullable.GetUnderlyingType(_enumType) ?? _enumType;
            var enumValues = Enum.GetValues(actualEnumType);

            if (actualEnumType == _enumType)
            {
                return enumValues;
            }

            var tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);

            return tempArray;
        }
    }
}