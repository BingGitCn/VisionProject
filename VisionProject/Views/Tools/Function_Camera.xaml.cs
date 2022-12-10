using System.Windows.Controls;
using VisionProject.GlobalVars;

namespace VisionProject.Views
{
    /// <summary>
    /// Interaction logic for Function_Camera
    /// </summary>
    public partial class Function_Camera : UserControl
    {
        public Function_Camera()
        {
            InitializeComponent();
            Variables.ImageWindowDataForFunction = aimg.windowData;
        }
    }
}