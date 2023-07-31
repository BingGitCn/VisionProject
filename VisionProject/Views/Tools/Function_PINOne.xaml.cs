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
    public partial class Function_PINOne : UserControl
    {
        public Function_PINOne()
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
}