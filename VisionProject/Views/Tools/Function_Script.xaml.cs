using System.Windows.Controls;
using VisionProject.GlobalVars;

namespace VisionProject.Views
{
    /// <summary>
    /// Interaction logic for Function_Script
    /// </summary>
    public partial class Function_Script : UserControl
    {
        public Function_Script()
        {
            InitializeComponent();
            Variables.ImageWindowDataForFunction = aimg.windowData;
        }
    }
}