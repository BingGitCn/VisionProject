using System.Windows;

namespace BingLibrary.Vision
{
    /// <summary>
    /// Interaction logic for ScriptDIalog
    /// </summary>
    public partial class ScriptDIalog : Window
    {
        public ScriptDIalog()
        {
            InitializeComponent();
        }

        public void SetCode(string code)
        {
            textEditor.Text = code;
        }

        public string GetCode()
        {
            return textEditor.Text;
        }
    }
}