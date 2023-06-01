using System.Windows.Controls;
using VisionProject.GlobalVars;

namespace VisionProject.Views
{
    /// <summary>
    /// Interaction logic for Function_ScriptTest
    /// </summary>
    public partial class Function_ScriptTest : UserControl
    {
        public Function_ScriptTest()
        {
            InitializeComponent();
            Variables.ImageWindowDataForFunction = aimg.windowData;
        }
    }
}