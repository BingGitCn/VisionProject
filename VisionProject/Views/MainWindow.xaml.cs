using Prism.Regions;
using System.Windows;
using VisionProject.GlobalVars;

namespace VisionProject.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            Variables.WindowData1 = ImageWindow1.windowData;

            //regionManager.RegisterViewWithRegion(GlobalPrism.RegionNames.StatusRegionName, typeof(Status));
            //GlobalVars.Variables.ImageWindowData = imageWin.windowData;

        }
    }
}
