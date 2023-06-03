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
    }
}