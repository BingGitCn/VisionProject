using System.Windows.Controls;
using VisionProject.GlobalVars;

namespace VisionProject.Views
{
    /// <summary>
    /// Interaction logic for Function_Match
    /// </summary>
    public partial class Function_Match : UserControl
    {
        public Function_Match()
        {
            InitializeComponent();
            Variables.ImageWindowDataForFunction = aimg.windowData;
        }

        private void cb_DropDownClosed(object sender, System.EventArgs e)
        {
            if (cb.SelectedIndex == 0)
            {
                g1.Visibility = System.Windows.Visibility.Visible;
                g2.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                g1.Visibility = System.Windows.Visibility.Collapsed;
                g2.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void TabItem_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (cb.SelectedIndex == 0)
            {
                g1.Visibility = System.Windows.Visibility.Visible;
                g2.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                g1.Visibility = System.Windows.Visibility.Collapsed;
                g2.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}