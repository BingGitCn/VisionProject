﻿using Prism.Ioc;
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
                    for (int i = 0; i <GlobalVars.Variables.Cameras.Count; i++)
                    {
                        try
                        {
                            GlobalVars.Variables.Cameras[i].CloseCamera();
                        }
                        catch { }
                    }


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

            //创建检测方法选择窗口
            containerRegistry.RegisterDialog<Dialog_InspectCreate, Dialog_InspectCreateViewModel>(GlobalVars.DialogNames.ShowInspectCreateDialog);

            //脚本参数设置窗口
            containerRegistry.RegisterDialog<Dialog_ParamSet, Dialog_ParamSetViewModel>(GlobalVars.DialogNames.ShowParamSetDialog);
            //位置参数设置窗口
            containerRegistry.RegisterDialog<Dialog_Location, Dialog_LocationViewModel>(GlobalVars.DialogNames.ShowLocationDialog);
            //项目设置设置窗口
            containerRegistry.RegisterDialog<Dialog_ProjectSet, Dialog_ProjectSetViewModel>(GlobalVars.DialogNames.ShowProjectSetDialog);
            //脚本工具窗口
            //containerRegistry.RegisterDialog<Function_ScriptTest, Function_ScriptTestViewModel>(GlobalVars.DialogNames.ToolNams["脚本工具"]);
            containerRegistry.RegisterDialog<Function_Match, Function_MatchViewModel>(GlobalVars.DialogNames.ToolNams["图像比对"]);
            containerRegistry.RegisterDialog<Function_Script, Function_ScriptViewModel>(GlobalVars.DialogNames.ToolNams["视觉脚本"]);
            containerRegistry.RegisterDialog<Function_Blob, Function_BlobViewModel>(GlobalVars.DialogNames.ToolNams["Blob分析"]);
            containerRegistry.RegisterDialog<Function_PINOne, Function_PINOneViewModel>(GlobalVars.DialogNames.ToolNams["PIN针检测"]);
            containerRegistry.RegisterDialog<Function_Code, Function_CodeViewModel>(GlobalVars.DialogNames.ToolNams["条码识别"]);
        }
    }
}