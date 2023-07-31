using System.Windows.Controls;
using System.Windows.Input;

namespace VisionProject.Views
{
    /// <summary>
    /// Interaction logic for Dialog_Join
    /// </summary>
    public partial class Dialog_Join : UserControl
    {
        public Dialog_Join()
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