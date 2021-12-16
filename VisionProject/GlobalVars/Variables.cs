﻿using BingLibrary.Communication.PLC;
using BingLibrary.Controls.Log;
using BingLibrary.Vision;
using HalconDotNet;
using System;
using System.Collections.ObjectModel;
using System.IO;
using VisionProject.ViewModels;

namespace VisionProject.GlobalVars
{
    public static class Variables
    {
        //PLC
        public static HuiChuanPLC HCPLC = new HuiChuanPLC();

        //标题
        public static string Title = "";

        //密码
        public static string CurrentPassword = "";

        //路径
        public static string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public static string StatisticDataFilePath = AppDomain.CurrentDomain.BaseDirectory + "Statistics.xlsx";

        //日志
        public static LogDateTime Logs = new LogDateTime(AppDomain.CurrentDomain.BaseDirectory + "Logs");

        //项目
        public static Project CurrentProject = new Project();

        //当前编辑的程序
        public static int ProgramIndex = 0;

        public static ObservableCollection<Program> CurrentProgram = new ObservableCollection<Program>();

        //图像窗口，需在mainwindow.cs入口指定对应的windowdata
        public static BingImageWindowData WindowData1 = new BingImageWindowData();

        public static HImage CurrentImage1 = new HImage();

        //程序编辑弹出窗口对应的windowdata
        public static BingImageWindowData ImageWindowDataForFunction = new BingImageWindowData();

        public static HImage CurrentImageForFunction = new HImage();

        //弹出窗口确认
        public static void ShowMessage(string msg)
        {
            HandyControl.Controls.MessageBox.Show(msg, "消息提示");
        }

        public static bool ShowConfirm(string msg)
        {
            if (HandyControl.Controls.MessageBox.Ask(msg, "确认操作") == System.Windows.MessageBoxResult.OK)
                return true;
            else return false;
        }

        //剩余硬盘容量
        public static string GetFreeSpace(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            DriveInfo savedFolderDrive = new DriveInfo(directory.Root.Name);
            double rst = savedFolderDrive.AvailableFreeSpace / 1024 / 1024;
            if (rst < 1024)
                return rst + "M";
            else
            {
                rst = rst / 1024.0;
                if (rst < 1024)
                    return rst.ToString("f1") + "G";
                else
                {
                    rst = rst / 1024.0;
                    return rst.ToString("f1") + "T";
                }
            }


        }

     
        //容量
        public static double GetFreeSpaceRateValue(string path) 
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            DriveInfo savedFolderDrive = new DriveInfo(directory.Root.Name);
            double rst = savedFolderDrive.AvailableFreeSpace / 1024 / 1024/1024;
            return rst;
               
         

        }
    }
}