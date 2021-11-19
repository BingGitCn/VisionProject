using Prism.Ioc;
using System.Windows;
using VisionProject.ViewModels;
using VisionProject.Views;

namespace VisionProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<AboutDialog, AboutDialogViewModel>(GlobalVars.DialogNames.ShowAboutWindow);
            //测试窗口
            containerRegistry.RegisterDialog<Function_Test, Function_TestViewModel>(GlobalVars.DialogNames.ShowFunctionTestWindow);
            //保存图像窗口
            containerRegistry.RegisterDialog<Function_SaveImage, Function_SaveImageViewModel>(GlobalVars.DialogNames.ShowFunctionSaveImageWindow);
        }
    }
}