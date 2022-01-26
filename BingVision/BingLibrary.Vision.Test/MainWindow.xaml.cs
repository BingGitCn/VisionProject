using System.Windows;

namespace BingLibrary.Vision.Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private BingImageWindowData windowData = new BingImageWindowData();

        public MainWindow()
        {
            InitializeComponent();
            windowData = win.windowData;
        }
    }
}