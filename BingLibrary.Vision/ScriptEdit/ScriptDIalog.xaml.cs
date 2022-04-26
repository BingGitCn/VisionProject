using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Windows;
using System.Windows.Threading;
using System.Xml;

namespace BingLibrary.Vision
{
    /// <summary>
    /// Interaction logic for ScriptDIalog
    /// </summary>
    public partial class ScriptDIalog : Window
    {
        private readonly IniFoldingStrategy foldingStrategy = new IniFoldingStrategy();

        public ScriptDIalog()
        {
            InitializeComponent();
            XmlReader reader = XmlReader.Create("Halcon.xshd");
            textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

            var foldingManager = FoldingManager.Install(textEditor.TextArea);

            var foldingTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            foldingTimer.Tick += (s, e)
               => foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
            foldingTimer.Start();

            textEditor.FontSize = double.Parse(fsize.Text);
            textEditor.IsReadOnly = true;
        }

        public void SetCode(string code)
        {
            textEditor.Text = code;
        }

        public string GetCode()
        {
            return textEditor.Text;
        }

        private void fsize_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                textEditor.FontSize = double.Parse(fsize.Text);
            }
            catch { }
        }

        private void number_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                textEditor.ShowLineNumbers = true;
            }
            catch { }
        }

        private void number_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                textEditor.ShowLineNumbers = false;
            }
            catch { }
        }

        private void edit_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                textEditor.IsReadOnly = false;
            }
            catch { }
        }

        private void edit_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                textEditor.IsReadOnly = true;
            }
            catch { }
        }
    }
}