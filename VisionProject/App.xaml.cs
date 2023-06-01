using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Windows;
using VisionProject.ViewModels;
using VisionProject.Views;
using VisionProject.Views.Tools;

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

        protected override void OnInitialized()
        {
            var dialog = Container.Resolve<IDialogService>();
            dialog.ShowDialog(GlobalVars.DialogNames.ShowLoginWindow, callback =>
            {
                if (callback.Result != ButtonResult.OK)
                {
                    Environment.Exit(0);
                    return;
                }
                //给主窗体传值
                base.OnInitialized();
            });
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //Step..

            //关于窗口
            containerRegistry.RegisterDialog<AboutDialog, AboutDialogViewModel>(GlobalVars.DialogNames.ShowAboutWindow);
            //登录窗口
            containerRegistry.RegisterDialog<LoginDialog, LoginDialogViewModel>(GlobalVars.DialogNames.ShowLoginWindow);

            //脚本工具窗口
            containerRegistry.RegisterDialog<Function_ScriptTest, Function_ScriptTestViewModel>(GlobalVars.DialogNames.ToolNams["脚本工具"]);
            containerRegistry.RegisterDialog<Function_Script, Function_ScriptViewModel>(GlobalVars.DialogNames.ToolNams["视觉脚本"]);

            //相机操作窗口
            containerRegistry.RegisterDialog<Function_Camera, Function_CameraViewModel>(GlobalVars.DialogNames.ToolNams["相机操作"]);

            //参数设置弹窗
            containerRegistry.RegisterDialog<Dialog_ParamSet, Dialog_ParamSetViewModel>(GlobalVars.DialogNames.ToolNams["参数设置"]);
        }
    }
}