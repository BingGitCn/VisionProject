using System.Windows.Controls;
using VisionProject.GlobalVars;

namespace VisionProject.Views
{
    /// <summary>
    /// Interaction logic for Function_Cls
    /// </summary>
    public partial class Function_Cls : UserControl
    {
        public Function_Cls()
        {
            InitializeComponent();
            Variables.ImageWindowDataForFunction = aimg.windowData;
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

        //private void cb_DropDownClosed(object sender, System.EventArgs e)
        //{
        //    if (cb.SelectedIndex == 0)
        //    {
        //        g1.Visibility = System.Windows.Visibility.Visible;
        //        g2.Visibility = System.Windows.Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        g1.Visibility = System.Windows.Visibility.Collapsed;
        //        g2.Visibility = System.Windows.Visibility.Visible;
        //    }
        //}
    }
}