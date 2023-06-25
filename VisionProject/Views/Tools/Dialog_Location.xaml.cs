using System.Windows.Controls;
using System.Windows.Input;

namespace VisionProject.Views
{
    /// <summary>
    /// Interaction logic for Dialog_Location
    /// </summary>
    public partial class Dialog_Location : UserControl
    {
        public Dialog_Location()
        {
            InitializeComponent();
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
