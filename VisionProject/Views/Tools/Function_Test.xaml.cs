using System.Windows.Controls;
using VisionProject.GlobalVars;

namespace VisionProject.Views
{
    /// <summary>
    /// Interaction logic for Function_Test
    /// </summary>
    public partial class Function_Test : UserControl
    {
        public Function_Test()
        {
            InitializeComponent();
            Variables.ImageWindowDataForFunction = aimg.windowData;
        }
    }
}