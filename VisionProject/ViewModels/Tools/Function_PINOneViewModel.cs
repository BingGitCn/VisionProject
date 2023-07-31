﻿using BingLibrary.Extension;
using BingLibrary.Vision;
using HalconDotNet;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using VisionProject.GlobalVars;
using VisionProject.RunTools;

namespace VisionProject.ViewModels
{
    public class Function_PINOneViewModel : BindableBase, IDialogAware, IFunction_ViewModel_Interface
    {
        public Function_PINOneViewModel()
        {
        }

        public bool Result = false;
        private int tabIndex;

        public int TabIndex
        {
            get { return tabIndex; }
            set { SetProperty(ref tabIndex, value); }
        }

        private int oldTabIndex = -1;
        private DelegateCommand _doSwitchTab;

        public DelegateCommand DoSwitchTab =>
            _doSwitchTab ?? (_doSwitchTab = new DelegateCommand(ExecuteDoSwitchTab));

        private HRegion regionTemp;

        private void ExecuteDoSwitchTab()
        {
            try
            {
                var row1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow1", 0.0);
                var row2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow2", 0.0);
                var col1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn1", 0.0);
                var col2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn2", 0.0);
                if (row1 + row2 == 0)
                {
                    image = originalImage.CopyImage();
                }
                else
                {
                    image = originalImage.CropRectangle1(row1, col1, row2, col2);
                }
                resultImage = image.CopyImage();
            }
            catch { }

            try
            {
                if (oldTabIndex != TabIndex)
                {
                    oldTabIndex = TabIndex;

                    Variables.ImageWindowDataForFunction.ROICtrl.Clear();
                    Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                    Variables.ImageWindowDataForFunction.MessageCtrl.ClearMessages();
                    if (TabIndex == 0)
                    {
                        try
                        {
                            string imagePath = "";
                            //0729del
                            //imagePath = Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIImage", Guid.NewGuid().ToString()).ToString();
                            //image = new HImage(Variables.ProjectImagesPath + imagePath + ".bmp");
                            //imagePath = Variables.CurrentProgramData.Parameters.BingGetOrAdd("OriginalImage", Guid.NewGuid().ToString()).ToString();
                            imagePath = Variables.CurrentProgramData.Parameters.BingGetOrAdd("OriginalImage", Guid.NewGuid().ToString()).ToString();

                            //originalImage = new HImage(Variables.ProjectImagesPath + imagePath + ".bmp");
                            originalImage = new HImage(Variables.ProjectImagesPath + imagePath + ".bmp");
                            var row1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow1", 0.0);
                            var row2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow2", 0.0);
                            var col1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn1", 0.0);
                            var col2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn2", 0.0);
                            if (row1 + row2 == 0)
                            {
                                image = originalImage.CopyImage();
                            }
                            else
                            {
                                image = originalImage.CropRectangle1(row1, col1, row2, col2);
                            }
                            Variables.ImageWindowDataForFunction.WindowCtrl.ShowImageToWindow(image.CopyImage());
                            Variables.ImageWindowDataForFunction.WindowCtrl.FitImageToWindow();
                            Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
                            resultImage = image.CopyImage();
                        }
                        catch { }
                    }
                    else if (TabIndex == 1)
                    {
                        try
                        {
                            if (IsTrans)
                                ExecuteModelOperate("trans");
                        }
                        catch { }
                    }
                    else if (TabIndex == 2)
                    {
                        try
                        {
                            if (isTrans)
                                ExecuteModelOperate("trans");
                            image?.Dispose();
                            image = resultImage.CopyImage();
                            if (IsPreEnhance)
                            {
                                preEnhanceAdd();
                                runPreEnhance();
                            }
                        }
                        catch { }
                    }
                    else if (TabIndex == 3)
                    {
                        try
                        {
                            if (IsTrans)
                                ExecuteModelOperate("trans");
                            image?.Dispose();
                            image = resultImage.CopyImage();
                            if (IsPreEnhance)
                            {
                                preEnhanceAdd();
                                runPreEnhance();
                            }
                            image?.Dispose();
                            image = resultImage.CopyImage();
                            runGray();
                        }
                        catch { }
                    }
                    else if (TabIndex == 4)
                    {
                        try
                        {
                            if (IsTrans)
                                ExecuteModelOperate("trans");
                            image?.Dispose();
                            image = resultImage.CopyImage();
                            if (IsPreEnhance)
                            {
                                preEnhanceAdd();
                                runPreEnhance();
                            }
                            image?.Dispose();
                            image = resultImage.CopyImage();
                            grayAdd();
                            runGray();

                            regionTemp?.Dispose();
                            regionTemp = resultRegion.Clone();
                            RegionFoundFeatureDataIndex = RegionFoundFeatureDatas.Count - 1;
                            runDispose();

                            //regionTemp?.Dispose();
                            //regionTemp = resultRegion.Clone();
                            //runExtract();
                        }
                        catch { }
                    }
                    else if (TabIndex == 5)
                    {
                        try
                        {
                            if (IsTrans)
                                ExecuteModelOperate("trans");
                            image?.Dispose();
                            image = resultImage.CopyImage();
                            if (IsPreEnhance)
                            {
                                preEnhanceAdd();
                                runPreEnhance();
                            }
                            image?.Dispose();
                            image = resultImage.CopyImage();
                            grayAdd();
                            runGray();

                            regionTemp?.Dispose();
                            regionTemp = resultRegion.Clone();
                            RegionFoundFeatureDataIndex = RegionFoundFeatureDatas.Count - 1;
                            runDispose();

                            regionTemp?.Dispose();
                            regionTemp = resultRegion.Clone();
                            runExtract();
                        }
                        catch { }
                    }
                    else if (TabIndex == 6)
                    {
                        try
                        {
                            if (IsTrans)
                                ExecuteModelOperate("trans");
                            image?.Dispose();
                            image = resultImage.CopyImage();
                            if (IsPreEnhance)
                            {
                                preEnhanceAdd();
                                runPreEnhance();
                            }
                            image?.Dispose();
                            image = resultImage.CopyImage();
                            grayAdd();
                            runGray();

                            regionTemp?.Dispose();
                            regionTemp = resultRegion.Clone();
                            RegionFoundFeatureDataIndex = RegionFoundFeatureDatas.Count - 1;
                            runDispose();

                            regionTemp?.Dispose();
                            regionTemp = resultRegion.Clone();
                            runExtract();
                        }
                        catch { }

                        inspectFrames = (List<InspectFrame>)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectFrames", new List<InspectFrame>());
                        Variables.ImageWindowDataForFunction.ROICtrl.Clear();
                        for (int i = 0; i < inspectFrames.Count; i++)
                        {
                            ROIRectangle1 rOIRectangle1 = new ROIRectangle1();
                            rOIRectangle1.row1 = inspectFrames[i].Row1;
                            rOIRectangle1.col1 = inspectFrames[i].Col1;
                            rOIRectangle1.row2 = inspectFrames[i].Row2;
                            rOIRectangle1.col2 = inspectFrames[i].Col2;
                            if (inspectFrames[i].ContainsObject)
                                rOIRectangle1.ROIColor = BingLibrary.Vision.HalconColors.蓝色;
                            else
                                rOIRectangle1.ROIColor = BingLibrary.Vision.HalconColors.红色;
                            Variables.ImageWindowDataForFunction.ROICtrl.AddROI(rOIRectangle1);
                        }
                        Variables.ImageWindowDataForFunction.WindowCtrl.DrawMode = HalconDrawing.margin;
                        Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
                    }
                }
            }
            catch { }
        }

        #region 图像选择

        private HImage originalImage = new HImage();
        private HImage image = new HImage();

        private HImage resultImage = new HImage();

        private bool notDrawIng = true;

        public bool NotDrawIng
        {
            get { return notDrawIng; }
            set { SetProperty(ref notDrawIng, value); }
        }

        private DelegateCommand<string> _imageOperate;

        public DelegateCommand<string> ImageOperate =>
            _imageOperate ?? (_imageOperate = new DelegateCommand<string>(ExecuteImageOperate));

        private void ExecuteImageOperate(string parameter)
        {
            Variables.ImageWindowDataForFunction.WindowCtrl.DrawMode = HalconDrawing.margin;
            switch (parameter)
            {
                case "select":
                    try
                    {
                        var rst = openImageDialog();
                        if (rst != "")
                        {
                            originalImage?.Dispose();
                            originalImage = new HImage(rst);
                            Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                            Variables.ImageWindowDataForFunction.ROICtrl.Clear();
                            Variables.ImageWindowDataForFunction.WindowCtrl.ShowImageToWindow(originalImage.CopyImage());
                            Variables.ImageWindowDataForFunction.WindowCtrl.FitImageToWindow();

                            var row1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow1", 0.0);
                            var row2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow2", 0.0);
                            var col1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn1", 0.0);
                            var col2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn2", 0.0);
                            if (row1 + row2 == 0)
                            {
                            }
                            else
                            {
                                image = originalImage.CropRectangle1(row1, col1, row2, col2);
                                Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                                Variables.ImageWindowDataForFunction.DispObjectCtrl.AddDispObjectVar(new HRegion(row1, col1, row2, col2));
                                Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();

                                Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = false;
                            }
                        }
                    }
                    catch { }
                    break;

                case "original":
                    try
                    {
                        if (!originalImage.IsInitialized())
                        {
                            string imagePath = "";
                            imagePath = Variables.CurrentProgramData.Parameters.BingGetOrAdd("OriginalImage", Guid.NewGuid().ToString()).ToString();

                            originalImage = new HImage(Variables.ProjectImagesPath + imagePath + ".bmp");
                        }

                        var row1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow1", 0.0);
                        var row2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow2", 0.0);
                        var col1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn1", 0.0);
                        var col2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn2", 0.0);
                        if (row1 + row2 == 0)
                        {
                        }
                        else
                        {
                            image = originalImage.CropRectangle1(row1, col1, row2, col2);
                            Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                            Variables.ImageWindowDataForFunction.DispObjectCtrl.AddDispObjectVar(new HRegion(row1, col1, row2, col2));
                            Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();

                            Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = false;
                        }
                        Variables.ImageWindowDataForFunction.WindowCtrl.ShowImageToWindow(originalImage.CopyImage());
                        Variables.ImageWindowDataForFunction.WindowCtrl.FitImageToWindow();
                    }
                    catch { }
                    break;
            }
        }

        private DelegateCommand<string> _rOIOperate;

        public DelegateCommand<string> ROIOperate =>
            _rOIOperate ?? (_rOIOperate = new DelegateCommand<string>(ExecuteROIOperate));

        private void ExecuteROIOperate(string parameter)
        {
            Variables.ImageWindowDataForFunction.WindowCtrl.DrawMode = HalconDrawing.margin;
            switch (parameter)
            {
                case "roiDraw":
                    ExecuteImageOperate("original");
                    NotDrawIng = false;
                    var row1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow1", 0.0);
                    var row2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow2", 0.0);
                    var col1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn1", 0.0);
                    var col2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn2", 0.0);

                    if ((row1 + row2) == 0)
                    {
                        Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = true;

                        Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor(HalconColors.橙色.ToDescription());
                        Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.DrawRectangle1(out row1, out col1, out row2, out col2);

                        Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ROIRow1", row1);
                        Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ROIRow2", row2);
                        Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ROIColumn1", col1);
                        Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ROIColumn2", col2);

                        Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                        Variables.ImageWindowDataForFunction.DispObjectCtrl.AddDispObjectVar(new HRegion(row1, col1, row2, col2));
                        Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();

                        Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = false;
                    }
                    else
                    {
                        Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = true;
                        Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor(HalconColors.橙色.ToDescription());
                        Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.DrawRectangle1Mod(row1, col1, row2, col2, out row1, out col1, out row2, out col2);

                        Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ROIRow1", row1);
                        Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ROIRow2", row2);
                        Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ROIColumn1", col1);
                        Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ROIColumn2", col2);

                        Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                        Variables.ImageWindowDataForFunction.DispObjectCtrl.AddDispObjectVar(new HRegion(row1, col1, row2, col2));
                        Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();

                        Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = false;
                    }

                    NotDrawIng = true;
                    break;

                case "roiRegiste":
                    var rst = Variables.ShowConfirm("是否注册为ROI基准图像？");
                    if (rst)
                    {
                        var row01 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow1", 0.0);
                        var row02 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow2", 0.0);
                        var col01 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn1", 0.0);
                        var col02 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn2", 0.0);

                        if ((row01 + row02) == 0)
                        { }
                        else
                        {
                            image = originalImage.CropRectangle1(row01, col01, row02, col02);
                            string imagePath = "";
                            try
                            {
                                imagePath = Variables.CurrentProgramData.Parameters.BingGetOrAdd("OriginalImage", Guid.NewGuid().ToString()).ToString();
                                if (System.IO.File.Exists(Variables.ProjectImagesPath + imagePath + ".bmp"))
                                    System.IO.File.Delete(Variables.ProjectImagesPath + imagePath + ".bmp");
                                //0729del
                                //imagePath = Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIImage", Guid.NewGuid().ToString()).ToString();
                                //if (System.IO.File.Exists(Variables.ProjectImagesPath + imagePath + ".bmp"))
                                //    System.IO.File.Delete(Variables.ProjectImagesPath + imagePath + ".bmp");
                            }
                            catch { }
                            //0729del
                            //imagePath = Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIImage", Guid.NewGuid().ToString()).ToString();
                            //image.WriteImage("bmp", 0, Variables.ProjectImagesPath + imagePath + ".bmp");

                            imagePath = Variables.CurrentProgramData.Parameters.BingGetOrAdd("OriginalImage", Guid.NewGuid().ToString()).ToString();
                            originalImage.WriteImage("bmp", 0, Variables.ProjectImagesPath + imagePath + ".bmp");

                            //Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ROIImage", image);
                            //Variables.CurrentProgramData.Parameters.BingAddOrUpdate("OriginalImage", originalImage);
                            Variables.ImageWindowDataForFunction.WindowCtrl.ShowImageToWindow(image.CopyImage());
                            Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                            Variables.ImageWindowDataForFunction.WindowCtrl.FitImageToWindow();
                        }
                    }

                    break;
            }
        }

        private string openImageDialog()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "所有文件|*.*|Tiff文件|*.tif|BMP文件|*.bmp|Jpeg文件|*.jpg";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DereferenceLinks = false;
            openFileDialog.AutoUpgradeEnabled = true;
            System.Windows.Forms.DialogResult result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return "";
            }
            string fileName = openFileDialog.FileName;
            return fileName;
        }

        #endregion 图像选择

        #region 图像矫正

        private DelegateCommand _transResetPreEnhance;

        public DelegateCommand TransResetPreEnhance =>
            _transResetPreEnhance ?? (_transResetPreEnhance = new DelegateCommand(ExecutetransResetPreEnhance));

        private void ExecutetransResetPreEnhance()
        {
            TransBrightnessValue = 128;
            TransContrastValue = 128;
            TransGammaValue = 1.0;
            runTransPreEnhance();
        }

        private bool isTrans;

        public bool IsTrans
        {
            get { return isTrans; }
            set { SetProperty(ref isTrans, value); }
        }

        private double transBrightnessValue = 128;

        public double TransBrightnessValue
        {
            get { return transBrightnessValue; }
            set
            {
                transBrightnessValue = value;
                RaisePropertyChanged(nameof(TransBrightnessValue));
            }
        }

        private double transContrastValue = 128;

        public double TransContrastValue
        {
            get { return transContrastValue; }
            set
            {
                transContrastValue = value;
                RaisePropertyChanged(nameof(TransContrastValue));
            }
        }

        private double transGammaValue = 1;

        public double TransGammaValue
        {
            get { return transGammaValue; }
            set
            {
                transGammaValue = value;
                RaisePropertyChanged(nameof(TransGammaValue));
            }
        }

        private DelegateCommand _transPreEnhanceValueChanged;

        public DelegateCommand TransPreEnhanceValueChanged =>
            _transPreEnhanceValueChanged ?? (_transPreEnhanceValueChanged = new DelegateCommand(ExecuteTransPreEnhanceValueChanged));

        private void ExecuteTransPreEnhanceValueChanged()
        {
            try
            {
                runTransPreEnhance();
            }
            catch { }
        }

        private HImage modelImage = new HImage();

        private void runTransPreEnhance()
        {
            if (!isInitlized)
                return;
            try
            {
                ///0729 疑问，这里的图片初始化是指的什么？
                if (!originalImage.IsInitialized())
                {
                    string imagePath = "";
                    imagePath = Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIImage", Guid.NewGuid().ToString()).ToString();

                    image = new HImage(Variables.ProjectImagesPath + imagePath + ".bmp");
                }
                {
                    var row1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow1", 0.0);
                    var row2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow2", 0.0);
                    var col1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn1", 0.0);
                    var col2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn2", 0.0);
                    if (row1 + row2 == 0)
                    {
                        image = originalImage.CopyImage();
                    }
                    else
                    {
                        image = originalImage.CropRectangle1(row1, col1, row2, col2);
                    }
                }
                resultImage = image.CopyImage();
                if (!isTrans)
                {
                }
                else
                {
                    modelImage = image.AddImage(image, 0.5, TransBrightnessValue - 128);
                    if (TransContrastValue >= 128)
                    {
                        double max = 383.0 - TransContrastValue;
                        double min = TransContrastValue - 128.0;

                        double mult = 255.0 / (max - min);
                        double add = -mult * min;
                        modelImage = modelImage.ScaleImage(mult, add);
                    }
                    else
                    {
                        double max = 127.0 + TransContrastValue;
                        double min = 128.0 - TransContrastValue;

                        double mult = (2 * TransContrastValue - 1) / 255.0;
                        double add = 128 - TransContrastValue;
                        modelImage = modelImage.ScaleImage(mult, add);
                    }
                    modelImage = modelImage.GammaImage(TransGammaValue, 0, 0, 255.0, "true");
                }
            }
            catch
            {
                if (!image.IsInitialized())
                    modelImage = new HImage();
                else
                    modelImage = image.CopyImage();
            }

            Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
            Variables.ImageWindowDataForFunction.ROICtrl.Clear();
            Variables.ImageWindowDataForFunction.WindowCtrl.ShowImageToWindow(isTrans ? modelImage : resultImage);
            Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();

            try
            {
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsTrans", IsTrans);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("TransBrightnessValue", TransBrightnessValue);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("TransContrastValue", TransContrastValue);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("TransGammaValue", TransGammaValue);
            }
            catch { }
        }

        private HRegion modelRegion = new HRegion();

        private DelegateCommand<string> _modelRegionOperate;

        public DelegateCommand<string> ModelRegionOperate =>
            _modelRegionOperate ?? (_modelRegionOperate = new DelegateCommand<string>(ExecuteModelRegionOperate));

        private void ExecuteModelRegionOperate(string parameter)
        {
            try
            {
                switch (parameter)
                {
                    case "draw":
                        NotDrawIng = false;
                        if (!modelRegion.IsInitialized())
                            modelRegion.GenEmptyRegion();

                        Variables.ImageWindowDataForFunction.WindowCtrl.DrawMode = HalconDrawing.margin;
                        Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = true;
                        Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor(HalconColors.橙色.ToDescription());
                        HRegion tempRegion = Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.DrawRegion();
                        Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                        modelRegion = modelRegion.Union2(tempRegion);
                        Variables.ImageWindowDataForFunction.DispObjectCtrl.AddDispObjectVar(modelRegion);
                        Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
                        Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = false;
                        break;

                    case "clear":
                        modelRegion = new HRegion();
                        modelRegion.GenEmptyRegion();
                        Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                        Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
                        break;
                }
            }
            catch { }
            NotDrawIng = true;
        }

        private HTuple modelRow = new HTuple(), modelCol = new HTuple(), modelAngle = new HTuple(), modelScore = new HTuple();
        private HShapeModel modelSM = new HShapeModel();

        private DelegateCommand<string> _modelOperate;

        public DelegateCommand<string> ModelOperate =>
             _modelOperate ?? (_modelOperate = new DelegateCommand<string>(ExecuteModelOperate));

        private void ExecuteModelOperate(string parameter)
        {
            try
            {
                runTransPreEnhance();

                HTuple row = new HTuple(), col = new HTuple(), angle = new HTuple();
                HHomMat2D hHomMat2D = new HHomMat2D();
                switch (parameter)
                {
                    case "create":
                        HTuple paramterValue = new HTuple();
                        HTuple paramterName = modelImage.ReduceDomain(modelRegion).DetermineShapeModelParams(
                           new HTuple("auto"), -0.39, 0.79, new HTuple(0.9), new HTuple(1.1), "auto",
                           "use_polarity", new HTuple("auto"), new HTuple("auto"), new HTuple("all"), out paramterValue
                            );

                        Dictionary<string, HTuple> dict = new Dictionary<string, HTuple>();
                        for (int i = 0; i < paramterName.SArr.Length; i++)
                            dict.Add(paramterName.SArr[i], paramterValue[i]);

                        modelSM = modelImage.ReduceDomain(modelRegion).CreateShapeModel(
                               dict["num_levels"], -0.39, 0.79, dict["angle_step"], dict["optimization"], "use_polarity",
                                dict["contrast_low"].TupleConcat(dict["contrast_high"]), dict["min_contrast"]);

                        modelImage.FindShapeModel(modelSM, -0.39, 0.79, 0.5, 1, 0.5, "least_squares", 3, 0.5,
                            out modelRow, out modelCol, out modelAngle, out modelScore);

                        Variables.ShowMessage("创建成功！\r\n模板分：" + modelScore.D);
                        break;

                    case "registe":
                        var rst = Variables.ShowConfirm("是否注册为模板？");
                        Variables.CurrentProgramData.Parameters.BingAddOrUpdate("Model", modelSM);
                        Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ModelRow", modelRow);
                        Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ModelCol", modelCol);
                        Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ModelAngle", modelAngle);

                        break;

                    case "find":

                        modelSM = (HShapeModel)Variables.CurrentProgramData.Parameters.BingGetOrAdd("Model", null);
                        modelRow = (HTuple)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ModelRow", new HTuple(0));
                        modelCol = (HTuple)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ModelCol", new HTuple(0));
                        modelAngle = (HTuple)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ModelAngle", new HTuple(0));

                        var con = modelSM.GetShapeModelContours(1);

                        modelImage.FindShapeModel(modelSM, -0.39, 0.79, 0.5, 1, 0.5, "least_squares", 3, 0.5,
                          out row, out col, out angle, out modelScore);

                        hHomMat2D.VectorAngleToRigid(0, 0, 0, row, col, angle);
                        var rstCon = con.AffineTransContourXld(hHomMat2D);
                        Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                        Variables.ImageWindowDataForFunction.DispObjectCtrl.AddDispObjectVar(rstCon);
                        Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
                        Variables.ShowMessage("查找成功！\r\n" +
                            "模板行位置：" + row.D.ToString("N3") + "\r\n" +
                            "模板列位置：" + col.D.ToString("N3") + "\r\n" +
                            "模板角度：" + (angle.D * 180.0 / Math.PI).ToString("N3") + "\r\n" +
                            "模板分：" + modelScore.D);
                        break;

                    case "trans":

                        modelSM = (HShapeModel)Variables.CurrentProgramData.Parameters.BingGetOrAdd("Model", null);
                        modelRow = (HTuple)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ModelRow", new HTuple(0));
                        modelCol = (HTuple)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ModelCol", new HTuple(0));
                        modelAngle = (HTuple)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ModelAngle", new HTuple(0));
                        //搜索06101143
                        try
                        {
                            modelImage.FindShapeModel(modelSM, -0.39, 0.79, 0.5, 1, 0.5, "least_squares", 3, 0.5,
                              out row, out col, out angle, out modelScore);
                        }
                        catch { }

                        hHomMat2D.VectorAngleToRigid(row, col, angle, modelRow, modelCol, modelAngle);
                        resultImage = image.AffineTransImage(hHomMat2D, "constant", "false");

                        Variables.ImageWindowDataForFunction.WindowCtrl.ShowImageToWindow(resultImage.CopyImage());
                        Variables.ImageWindowDataForFunction.WindowCtrl.FitImageToWindow();
                        Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();

                        break;
                }
            }
            catch { }
        }

        #endregion 图像矫正

        #region 预处理

        private void preEnhanceAdd()
        {
            BrightnessValue = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("BrightnessValue", 128.0);
            ContrastValue = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ContrastValue", 128.0);
            GammaValue = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("GammaValue", 1.0);
            runPreEnhance();
        }

        private DelegateCommand _savePreEnhance;

        public DelegateCommand SavePreEnhance =>
            _savePreEnhance ?? (_savePreEnhance = new DelegateCommand(ExecuteSavePreEnhance));

        private void ExecuteSavePreEnhance()
        {
            var rst = Variables.ShowConfirm("是否保存当前设置？");
            if (!rst)
            {
                return;
            }
            try
            {
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsPreEnhance", IsPreEnhance);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("BrightnessValue", BrightnessValue);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ContrastValue", ContrastValue);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("GammaValue", GammaValue);
            }
            catch { }
        }

        private DelegateCommand _revokePreEnhance;

        public DelegateCommand RevokePreEnhance =>
            _revokePreEnhance ?? (_revokePreEnhance = new DelegateCommand(ExecuteRevokePreEnhance));

        private void ExecuteRevokePreEnhance()
        {
            var rst = Variables.ShowConfirm("是否还原为最近一次保存的设置？");
            if (!rst)
            {
                return;
            }
            IsPreEnhance = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsPreEnhance", false);
            BrightnessValue = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("BrightnessValue", 128.0);
            ContrastValue = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ContrastValue", 128.0);
            GammaValue = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("GammaValue", 1.0);
            runPreEnhance();
        }

        private DelegateCommand _resetPreEnhance;

        public DelegateCommand ResetPreEnhance =>
            _resetPreEnhance ?? (_resetPreEnhance = new DelegateCommand(ExecuteResetPreEnhance));

        private void ExecuteResetPreEnhance()
        {
            var rst = Variables.ShowConfirm("是否还原为默认设置？");
            if (!rst)
            {
                return;
            }
            BrightnessValue = 128;
            ContrastValue = 128;
            GammaValue = 1.0;
            runPreEnhance();
        }

        private bool isPreEnhance;

        public bool IsPreEnhance
        {
            get { return isPreEnhance; }
            set { SetProperty(ref isPreEnhance, value); }
        }

        private double brightnessValue = 128;

        public double BrightnessValue
        {
            get { return brightnessValue; }
            set
            {
                if (value < 1) { brightnessValue = 1; }
                else if (value > 255) { brightnessValue = 255; }
                else { brightnessValue = value; }
                //SetProperty(ref BrightnessValue, value);
                RaisePropertyChanged(nameof(BrightnessValue));
            }
        }

        private double contrastValue = 128;

        public double ContrastValue
        {
            get { return contrastValue; }
            set
            {
                if (value < 1) { contrastValue = 1; }
                else if (value > 255) { contrastValue = 255; }
                else { contrastValue = value; }
                //  SetProperty(ref contrastValue, value);
                RaisePropertyChanged(nameof(ContrastValue));
            }
        }

        private double gammaValue = 1;

        public double GammaValue
        {
            get { return gammaValue; }
            set
            {
                if (value < 0.1) { gammaValue = 0.1; }
                else if (value > 10.0) { gammaValue = 10; }
                else { gammaValue = value; }
                RaisePropertyChanged(nameof(GammaValue));
                //SetProperty(ref gammaValue, value);
            }
        }

        private DelegateCommand _preEnhanceValueChanged;

        public DelegateCommand PreEnhanceValueChanged =>
            _preEnhanceValueChanged ?? (_preEnhanceValueChanged = new DelegateCommand(ExecutePreEnhanceValueChanged));

        private void ExecutePreEnhanceValueChanged()
        {
            try
            {
                runPreEnhance();
            }
            catch { }
        }

        private void runPreEnhance()
        {
            if (!isInitlized)
                return;
            try
            {
                {
                    resultImage = image.AddImage(image, 0.5, BrightnessValue - 128);
                    if (ContrastValue >= 128)
                    {
                        double max = 383.0 - ContrastValue;
                        double min = ContrastValue - 128.0;

                        double mult = 255.0 / (max - min);
                        double add = -mult * min;
                        resultImage = resultImage.ScaleImage(mult, add);
                    }
                    else
                    {
                        double max = 127.0 + ContrastValue;
                        double min = 128.0 - ContrastValue;

                        double mult = (2 * ContrastValue - 1) / 255.0;
                        double add = 128 - ContrastValue;
                        resultImage = resultImage.ScaleImage(mult, add);
                    }
                    resultImage = resultImage.GammaImage(GammaValue, 0, 0, 255.0, "true");
                }
            }
            catch
            {
                if (!image.IsInitialized())
                    resultImage = new HImage();
                else
                    resultImage = image.CopyImage();
            }

            Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
            Variables.ImageWindowDataForFunction.ROICtrl.Clear();
            Variables.ImageWindowDataForFunction.WindowCtrl.ShowImageToWindow(resultImage);
            Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();

            //try
            //{
            //    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsPreEnhance", IsPreEnhance);
            //    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("BrightnessValue", BrightnessValue);
            //    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ContrastValue", ContrastValue);
            //    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("GammaValue", GammaValue);
            //}
            //catch { }
        }

        #endregion 预处理

        #region 二值处理

        //private bool isGray;

        //public bool IsGray
        //{
        //    get { return isGray; }
        //    set { SetProperty(ref isGray, value); }
        //}        private DelegateCommand _revokeGray;
        private DelegateCommand _saveGray;

        public DelegateCommand SaveGray =>
            _saveGray ?? (_saveGray = new DelegateCommand(ExecuteSaveGray));

        /// <summary>
        /// 保存过滤
        /// </summary>
        private void ExecuteSaveGray()
        {
            var rst = Variables.ShowConfirm("是否保存当前设置？");
            if (!rst)
            {
                return;
            }
            try
            {
                //Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsGray", IsGray);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("GrayModeIndex", GrayModeIndex);

                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueS1", ValueS1);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueS2", ValueS2);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueS3", ValueS3);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueS4", ValueS4);

                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueE1", ValueE1);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueE2", ValueE2);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueE3", ValueE3);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueE4", ValueE4);

                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsEnableDeep", IsEnableDeep);

                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsEnable1", IsEnable1);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsEnable2", IsEnable2);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsEnable3", IsEnable3);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsEnable4", IsEnable4);

                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsReverse11", IsReverse1);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsReverse12", IsReverse2);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsReverse13", IsReverse3);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsReverse14", IsReverse4);

                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueS1" + GrayModeIndex.ToString(), ValueS1);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueS2" + GrayModeIndex.ToString(), ValueS2);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueS3" + GrayModeIndex.ToString(), ValueS3);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueS4" + GrayModeIndex.ToString(), ValueS4);

                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueE1" + GrayModeIndex.ToString(), ValueE1);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueE2" + GrayModeIndex.ToString(), ValueE2);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueE3" + GrayModeIndex.ToString(), ValueE3);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueE4" + GrayModeIndex.ToString(), ValueE4);

                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsEnable1" + GrayModeIndex.ToString(), IsEnable1);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsEnable2" + GrayModeIndex.ToString(), IsEnable2);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsEnable3" + GrayModeIndex.ToString(), IsEnable3);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsEnable4" + GrayModeIndex.ToString(), IsEnable4);

                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsReverse11" + GrayModeIndex.ToString(), IsReverse1);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsReverse12" + GrayModeIndex.ToString(), IsReverse2);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsReverse13" + GrayModeIndex.ToString(), IsReverse3);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsReverse14" + GrayModeIndex.ToString(), IsReverse4);
            }
            catch { }
        }

        private DelegateCommand _revokeGray;

        public DelegateCommand RevokeGray =>
            _revokeGray ?? (_revokeGray = new DelegateCommand(ExecuteRevokeGray));

        /// <summary>
        /// 撤销过滤
        /// </summary>
        private void ExecuteRevokeGray()
        {
            var rst = Variables.ShowConfirm("是否还原为最近一次保存的设置？");
            if (!rst)
            {
                return;
            }
            if (GrayModeIndex == 0)
            {
                Minimum1 = 0; Maximum1 = 100;
                Minimum2 = 0; Maximum2 = 100;
                Minimum3 = 0; Maximum3 = 100;
                Minimum4 = 0; Maximum4 = 255;
            }
            else if (GrayModeIndex == 1)
            {
                Minimum1 = 0; Maximum1 = 255;
                Minimum2 = 0; Maximum2 = 255;
                Minimum3 = 0; Maximum3 = 255;
                Minimum4 = 0; Maximum4 = 255;
            }
            else if (GrayModeIndex == 2)
            {
                Minimum1 = 0; Maximum1 = 255;
                Minimum2 = 0; Maximum2 = 255;
                Minimum3 = 0; Maximum3 = 255;
                Minimum4 = 0; Maximum4 = 255;
            }
            ValueS1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS1" + GrayModeIndex.ToString(), 0).ToString());
            ValueS2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS2" + GrayModeIndex.ToString(), 0).ToString());
            ValueS3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS3" + GrayModeIndex.ToString(), 0).ToString());
            ValueS4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS4" + GrayModeIndex.ToString(), 0).ToString());
            ValueE1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE1" + GrayModeIndex.ToString(), 0).ToString());
            ValueE2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE2" + GrayModeIndex.ToString(), 0).ToString());
            ValueE3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE3" + GrayModeIndex.ToString(), 0).ToString());
            ValueE4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE4" + GrayModeIndex.ToString(), 0).ToString());

            IsEnableDeep = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnableDeep", false);
            IsEnable1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable1" + GrayModeIndex.ToString(), false);
            IsEnable2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable2" + GrayModeIndex.ToString(), false);
            IsEnable3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable3" + GrayModeIndex.ToString(), false);
            IsEnable4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable4" + GrayModeIndex.ToString(), false);
            IsReverse1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse11" + GrayModeIndex.ToString(), false);
            IsReverse2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse12" + GrayModeIndex.ToString(), false);
            IsReverse3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse13" + GrayModeIndex.ToString(), false);
            IsReverse4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse14" + GrayModeIndex.ToString(), false);

            runGray();
        }

        private DelegateCommand _resetGray;

        public DelegateCommand ResetGray =>
            _resetGray ?? (_resetGray = new DelegateCommand(ExecuteResetGray));

        private void ExecuteResetGray()
        {
            var rst = Variables.ShowConfirm("是否还原为默认设置？");
            if (!rst)
            {
                return;
            }
            IsEnableDeep = false;
            if (GrayModeIndex == 0)
            {
                Minimum1 = 0; Maximum1 = 100;
                Minimum2 = 0; Maximum2 = 100;
                Minimum3 = 0; Maximum3 = 100;
                Minimum4 = 0; Maximum4 = 255;
            }
            else if (GrayModeIndex == 1)
            {
                Minimum1 = 0; Maximum1 = 255;
                Minimum2 = 0; Maximum2 = 255;
                Minimum3 = 0; Maximum3 = 255;
                Minimum4 = 0; Maximum4 = 255;
            }
            else if (GrayModeIndex == 2)
            {
                Minimum1 = 0; Maximum1 = 255;
                Minimum2 = 0; Maximum2 = 255;
                Minimum3 = 0; Maximum3 = 255;
                Minimum4 = 0; Maximum4 = 255;
            }
            if (GrayModeIndex == 0)
            {
                IsEnable1 = true;
                ValueS1 = 0;
                ValueE1 = 30;
                IsEnable2 = true;
                ValueS2 = 0;
                ValueE2 = 59;
                IsEnable3 = true;
                ValueS3 = 0;
                ValueE3 = 11;
                IsEnable4 = true;
                ValueS4 = 0;
                ValueE4 = 128;

                IsReverse1 = false; IsReverse2 = false; IsReverse3 = false; IsReverse4 = false;
            }
            if (GrayModeIndex == 1)
            {
                IsEnable1 = true;
                ValueS1 = 0;
                ValueE1 = 128;
                IsEnable2 = true;
                ValueS2 = 0;
                ValueE2 = 255;
                IsEnable3 = true;
                ValueS3 = 0;
                ValueE3 = 255;
                IsEnable4 = false;
                ValueS4 = 0;
                ValueE4 = 255;

                IsReverse1 = false; IsReverse2 = false; IsReverse3 = false; IsReverse4 = false;
            }
            if (GrayModeIndex == 2)
            {
                IsEnable1 = true;
                ValueS1 = 0;
                ValueE1 = 128;
                IsEnable2 = true;
                ValueS2 = 0;
                ValueE2 = 255;
                IsEnable3 = true;
                ValueS3 = 0;
                ValueE3 = 255;
                IsEnable4 = false;
                ValueS4 = 0;
                ValueE4 = 255;

                IsReverse1 = false; IsReverse2 = false; IsReverse3 = false; IsReverse4 = false;
            }
            runGray();
        }

        private HRegion resultRegion = new HRegion();

        private void grayAdd()
        {
            GrayModeIndex = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("GrayModeIndex", 0).ToString());
            if (GrayModeIndex == 0)
            {
                Minimum1 = 0; Maximum1 = 100;
                Minimum2 = 0; Maximum2 = 100;
                Minimum3 = 0; Maximum3 = 100;
                Minimum4 = 0; Maximum4 = 255;
            }
            else if (GrayModeIndex == 1)
            {
                Minimum1 = 0; Maximum1 = 255;
                Minimum2 = 0; Maximum2 = 255;
                Minimum3 = 0; Maximum3 = 255;
                Minimum4 = 0; Maximum4 = 255;
            }
            else if (GrayModeIndex == 2)
            {
                Minimum1 = 0; Maximum1 = 255;
                Minimum2 = 0; Maximum2 = 255;
                Minimum3 = 0; Maximum3 = 255;
                Minimum4 = 0; Maximum4 = 255;
            }
            ValueS1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS1" + GrayModeIndex.ToString(), 0).ToString());
            ValueS2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS2" + GrayModeIndex.ToString(), 0).ToString());
            ValueS3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS3" + GrayModeIndex.ToString(), 0).ToString());
            ValueS4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS4" + GrayModeIndex.ToString(), 0).ToString());
            ValueE1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE1" + GrayModeIndex.ToString(), 0).ToString());
            ValueE2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE2" + GrayModeIndex.ToString(), 0).ToString());
            ValueE3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE3" + GrayModeIndex.ToString(), 0).ToString());
            ValueE4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE4" + GrayModeIndex.ToString(), 0).ToString());
            IsEnable1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable1" + GrayModeIndex.ToString(), false);
            IsEnable2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable2" + GrayModeIndex.ToString(), false);
            IsEnable3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable3" + GrayModeIndex.ToString(), false);
            IsEnable4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable4" + GrayModeIndex.ToString(), false);
            IsReverse1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse11" + GrayModeIndex.ToString(), false);
            IsReverse2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse12" + GrayModeIndex.ToString(), false);
            IsReverse3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse13" + GrayModeIndex.ToString(), false);
            IsReverse4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse14" + GrayModeIndex.ToString(), false);
        }

        private void runGray()
        {
            //var row1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("DisposeROIRow1", 0.0);

            Variables.ImageWindowDataForFunction.WindowCtrl.DrawMode = HalconDrawing.fill;
            if (!isInitlized)
                return;

            resultRegion.GenEmptyRegion();
            try
            {
                if (true)
                {
                    if (IsEnableDeep)
                    {
                        resultImage = image.CopyImage();
                        resultRegion = Variables.PinDeep.ApplyModelInSegmentation(resultImage, new int[] { 1 }, true).Threshold(2.0, 2.0);
                    }
                    else if (GrayModeIndex == 0)
                    {
                        if (image.CountChannels() == 1)
                        {
                            resultImage = image.CopyImage();
                            if (!IsReverse4)
                            {
                                resultRegion = resultImage.Threshold((double)ValueS4, (double)ValueE4);
                            }
                            else
                            {
                                resultRegion = resultImage.Threshold((new HTuple(0)).TupleConcat(new HTuple(ValueE4)), (new HTuple(ValueS4)).TupleConcat(255));
                                resultRegion = resultRegion.Union1();
                            }
                        }
                        else
                        {
                            HImage image1, image2, image3;
                            image1 = image.Decompose3(out image2, out image3);
                            var rstImage1 = image1.AddImage(image1, ValueE1 * 0.5 / 100.0, 0);
                            var rstImage2 = image2.AddImage(image2, ValueE2 * 0.5 / 100.0, 0);
                            var rstImage3 = image3.AddImage(image3, ValueE3 * 0.5 / 100.0, 0);

                            resultImage = rstImage1.AddImage(rstImage2, 1.0, 0);
                            resultImage = resultImage.AddImage(rstImage3, 1.0, 0);

                            if (IsEnable4)
                            {
                                if (!IsReverse4)
                                {
                                    resultRegion = resultImage.Threshold((double)ValueS4, (double)ValueE4);
                                }
                                else
                                {
                                    resultRegion = resultImage.Threshold((new HTuple(0)).TupleConcat(new HTuple(ValueE4)), (new HTuple(ValueS4)).TupleConcat(255));
                                }
                            }
                        }
                    }
                    else if (GrayModeIndex == 1)
                    {
                        if (image.CountChannels() == 1)
                        {
                            resultImage = image.CopyImage();
                            if (!IsReverse4)
                            {
                                resultRegion = resultImage.Threshold((double)ValueS4, (double)ValueE4);
                            }
                            else
                            {
                                resultRegion = resultImage.Threshold((new HTuple(0)).TupleConcat(new HTuple(ValueE4)), (new HTuple(ValueS4)).TupleConcat(255));
                                resultRegion = resultRegion.Union1();
                            }
                        }
                        else
                        {
                            HImage image1, image2, image3;
                            image1 = image.Decompose3(out image2, out image3);

                            HRegion resultRegion1 = new HRegion(); resultRegion1.GenEmptyRegion();
                            HRegion resultRegion2 = new HRegion(); resultRegion2.GenEmptyRegion();
                            HRegion resultRegion3 = new HRegion(); resultRegion3.GenEmptyRegion();

                            resultImage = image1.Compose3(image2, image3);
                            if (IsEnable1)
                            {
                                if (!IsReverse1)
                                {
                                    resultRegion1 = image1.Threshold((double)ValueS1, (double)ValueE1);
                                }
                                else
                                {
                                    resultRegion1 = image1.Threshold((new HTuple(0)).TupleConcat(new HTuple(ValueE1)), (new HTuple(ValueS1)).TupleConcat(255));
                                    resultRegion1 = resultRegion1.Union1();
                                }
                            }
                            else
                            {
                                resultRegion1 = image1.Threshold(0.0, 255.0);
                            }

                            if (IsEnable2)
                            {
                                if (!IsReverse2)
                                {
                                    resultRegion2 = image2.Threshold((double)ValueS2, (double)ValueE2);
                                }
                                else
                                {
                                    resultRegion2 = image2.Threshold((new HTuple(0)).TupleConcat(new HTuple(ValueE2)), (new HTuple(ValueS2)).TupleConcat(255));
                                    resultRegion2 = resultRegion2.Union1();
                                }
                            }
                            else
                            {
                                resultRegion2 = image2.Threshold(0.0, 255.0);
                            }

                            if (IsEnable3)
                            {
                                if (!IsReverse3)
                                {
                                    resultRegion3 = image3.Threshold((double)ValueS3, (double)ValueE3);
                                }
                                else
                                {
                                    resultRegion3 = image3.Threshold((new HTuple(0)).TupleConcat(new HTuple(ValueE3)), (new HTuple(ValueS3)).TupleConcat(255));
                                    resultRegion3 = resultRegion3.Union1();
                                }
                            }
                            else
                            {
                                resultRegion3 = image3.Threshold(0.0, 255.0);
                            }

                            resultRegion = resultRegion1.Intersection(resultRegion2);
                            resultRegion = resultRegion.Intersection(resultRegion3);
                        }
                    }
                    else if (GrayModeIndex == 2)
                    {
                        if (image.CountChannels() == 1)
                        {
                            resultImage = image.CopyImage();
                            if (!IsReverse4)
                            {
                                resultRegion = resultImage.Threshold((double)ValueS4, (double)ValueE4);
                            }
                            else
                            {
                                resultRegion = resultImage.Threshold((new HTuple(0)).TupleConcat(new HTuple(ValueE4)), (new HTuple(ValueS4)).TupleConcat(255));
                                resultRegion = resultRegion.Union1();
                            }
                        }
                        else
                        {
                            HImage image1, image2, image3;
                            image1 = image.Decompose3(out image2, out image3);
                            HImage tImage1, tImage2, tImage3;
                            tImage1 = image1.TransFromRgb(image2, image3, out tImage2, out tImage3, "hsv");

                            HRegion resultRegion1 = new HRegion(); resultRegion1.GenEmptyRegion();
                            HRegion resultRegion2 = new HRegion(); resultRegion2.GenEmptyRegion();
                            HRegion resultRegion3 = new HRegion(); resultRegion3.GenEmptyRegion();

                            resultImage = image1.Compose3(image2, image3);

                            if (!IsReverse1)
                            {
                                resultRegion1 = tImage1.Threshold((double)ValueS1, (double)ValueE1);
                            }
                            else
                            {
                                resultRegion1 = tImage1.Threshold((new HTuple(0)).TupleConcat(new HTuple(ValueE1)), (new HTuple(ValueS1)).TupleConcat(255));
                                resultRegion1 = resultRegion1.Union1();
                            }

                            HImage hImage = tImage2.ReduceDomain(resultRegion1);

                            if (!IsReverse2)
                            {
                                resultRegion2 = hImage.Threshold((double)ValueS2, (double)ValueE2);
                            }
                            else
                            {
                                resultRegion2 = hImage.Threshold((new HTuple(0)).TupleConcat(new HTuple(ValueE2)), (new HTuple(ValueS2)).TupleConcat(255));
                                resultRegion2 = resultRegion2.Union1();
                            }

                            HImage vImage = tImage3.ReduceDomain(resultRegion2);

                            if (!IsReverse3)
                            {
                                resultRegion3 = vImage.Threshold((double)ValueS3, (double)ValueE3);
                                resultRegion = resultRegion3;
                            }
                            else
                            {
                                resultRegion3 = vImage.Threshold((new HTuple(0)).TupleConcat(new HTuple(ValueE3)), (new HTuple(ValueS3)).TupleConcat(255));
                                resultRegion3 = resultRegion3.Union1();
                                resultRegion = resultRegion3;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            ///   ///
            Variables.ImageWindowDataForFunction.WindowCtrl.ShowImageToWindow(resultImage.CopyImage());///因为被释放掉了

            Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
            Variables.ImageWindowDataForFunction.DispObjectCtrl.AddDispObjectVar(resultRegion);
            Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
            //Variables.ImageWindowDataForFunction.ROICtrl.Clear();
            //Variables.ImageWindowDataForFunction.ROICtrl.AddROI(new BingLibrary.Vision.ROIRegion(resultRegion) { ROIColor = BingLibrary.Vision.HalconColors.蓝色 });
            //Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
        }

        private int grayModeIndex;

        public int GrayModeIndex
        {
            get { return grayModeIndex; }
            set
            {
                SetProperty(ref grayModeIndex, value);
            }
        }

        private DelegateCommand _grayModeChanged;

        public DelegateCommand GrayModeChanged =>
            _grayModeChanged ?? (_grayModeChanged = new DelegateCommand(ExecuteGrayModeChanged));

        private void ExecuteGrayModeChanged()
        {
            if (GrayModeIndex == 0)
            {
                Minimum1 = 0; Maximum1 = 100;
                Minimum2 = 0; Maximum2 = 100;
                Minimum3 = 0; Maximum3 = 100;
                Minimum4 = 0; Maximum4 = 255;

                ValueS1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS1" + GrayModeIndex.ToString(), 0).ToString());
                ValueS2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS2" + GrayModeIndex.ToString(), 0).ToString());
                ValueS3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS3" + GrayModeIndex.ToString(), 0).ToString());
                ValueS4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS4" + GrayModeIndex.ToString(), 0).ToString());
                ValueE1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE1" + GrayModeIndex.ToString(), 30).ToString());
                ValueE2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE2" + GrayModeIndex.ToString(), 59).ToString());
                ValueE3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE3" + GrayModeIndex.ToString(), 11).ToString());
                ValueE4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE4" + GrayModeIndex.ToString(), 128).ToString());
                IsEnable1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable1" + GrayModeIndex.ToString(), true);
                IsEnable2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable2" + GrayModeIndex.ToString(), true);
                IsEnable3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable3" + GrayModeIndex.ToString(), true);
                IsEnable4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable4" + GrayModeIndex.ToString(), true);
                IsReverse1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse11" + GrayModeIndex.ToString(), false);
                IsReverse2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse12" + GrayModeIndex.ToString(), false);
                IsReverse3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse13" + GrayModeIndex.ToString(), false);
                IsReverse4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse14" + GrayModeIndex.ToString(), false);
            }
            else if (GrayModeIndex == 1)
            {
                Minimum1 = 0; Maximum1 = 255;
                Minimum2 = 0; Maximum2 = 255;
                Minimum3 = 0; Maximum3 = 255;
                Minimum4 = 0; Maximum4 = 255;

                ValueS1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS1" + GrayModeIndex.ToString(), 0).ToString());
                ValueS2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS2" + GrayModeIndex.ToString(), 0).ToString());
                ValueS3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS3" + GrayModeIndex.ToString(), 0).ToString());
                ValueS4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS4" + GrayModeIndex.ToString(), 0).ToString());
                ValueE1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE1" + GrayModeIndex.ToString(), 128).ToString());
                ValueE2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE2" + GrayModeIndex.ToString(), 255).ToString());
                ValueE3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE3" + GrayModeIndex.ToString(), 255).ToString());
                ValueE4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE4" + GrayModeIndex.ToString(), 255).ToString());
                IsEnable1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable1" + GrayModeIndex.ToString(), true);
                IsEnable2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable2" + GrayModeIndex.ToString(), true);
                IsEnable3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable3" + GrayModeIndex.ToString(), true);
                IsEnable4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable4" + GrayModeIndex.ToString(), false);
                IsReverse1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse11" + GrayModeIndex.ToString(), false);
                IsReverse2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse12" + GrayModeIndex.ToString(), false);
                IsReverse3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse13" + GrayModeIndex.ToString(), false);
                IsReverse4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse14" + GrayModeIndex.ToString(), false);
            }
            else if (GrayModeIndex == 2)
            {
                Minimum1 = 0; Maximum1 = 255;
                Minimum2 = 0; Maximum2 = 255;
                Minimum3 = 0; Maximum3 = 255;
                Minimum4 = 0; Maximum4 = 255;

                ValueS1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS1" + GrayModeIndex.ToString(), 0).ToString());
                ValueS2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS2" + GrayModeIndex.ToString(), 0).ToString());
                ValueS3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS3" + GrayModeIndex.ToString(), 0).ToString());
                ValueS4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS4" + GrayModeIndex.ToString(), 0).ToString());
                ValueE1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE1" + GrayModeIndex.ToString(), 128).ToString());
                ValueE2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE2" + GrayModeIndex.ToString(), 255).ToString());
                ValueE3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE3" + GrayModeIndex.ToString(), 255).ToString());
                ValueE4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE4" + GrayModeIndex.ToString(), 255).ToString());
                IsEnable1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable1" + GrayModeIndex.ToString(), true);
                IsEnable2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable2" + GrayModeIndex.ToString(), true);
                IsEnable3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable3" + GrayModeIndex.ToString(), true);
                IsEnable4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable4" + GrayModeIndex.ToString(), false);
                IsReverse1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse11" + GrayModeIndex.ToString(), false);
                IsReverse2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse12" + GrayModeIndex.ToString(), false);
                IsReverse3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse13" + GrayModeIndex.ToString(), false);
                IsReverse4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse14" + GrayModeIndex.ToString(), false);
            }
            runGray();
        }

        private DelegateCommand _grayValueChanged;

        public DelegateCommand GrayValueChanged =>
            _grayValueChanged ?? (_grayValueChanged = new DelegateCommand(ExecuteGrayValueChanged));

        private void ExecuteGrayValueChanged()
        {
            runGray();
        }

        private bool isEnableDeep;

        public bool IsEnableDeep
        {
            get { return isEnableDeep; }
            set { SetProperty(ref isEnableDeep, value); }
        }

        private bool isEnable1;

        public bool IsEnable1
        {
            get { return isEnable1; }
            set { SetProperty(ref isEnable1, value); RaisePropertyChanged(nameof(IsEnable1)); }
        }

        private bool isEnable2;

        public bool IsEnable2
        {
            get { return isEnable2; }
            set { SetProperty(ref isEnable2, value); RaisePropertyChanged(nameof(IsEnable2)); }
        }

        private bool isEnable3;

        public bool IsEnable3
        {
            get { return isEnable3; }
            set { SetProperty(ref isEnable3, value); RaisePropertyChanged(nameof(IsEnable3)); }
        }

        private bool isEnable4;

        public bool IsEnable4
        {
            get { return isEnable4; }
            set { SetProperty(ref isEnable4, value); RaisePropertyChanged(nameof(IsEnable4)); }
        }

        private double maximum1;

        public double Maximum1
        {
            get { return maximum1; }
            set
            {
                maximum1 = value;
                RaisePropertyChanged(nameof(Maximum1));
                //    SetProperty(ref maximum1, value);
            }
        }

        private double minimum1;

        public double Minimum1
        {
            get { return minimum1; }
            set
            {
                minimum1 = value;
                RaisePropertyChanged(nameof(Minimum1));
                // SetProperty(ref minimum1, value); }
            }
        }

        private double maximum2;

        public double Maximum2
        {
            get { return maximum2; }
            set
            {
                maximum2 = value;
                RaisePropertyChanged(nameof(Maximum2));
                // SetProperty(ref maximum2, value); }
            }
        }

        private double minimum2;

        public double Minimum2
        {
            get { return minimum2; }
            set
            {
                minimum2 = value;
                RaisePropertyChanged(nameof(Minimum2));
                //SetProperty(ref minimum2, value); }
            }
        }

        private double maximum3;

        public double Maximum3
        {
            get { return maximum3; }
            set
            {
                maximum3 = value;
                RaisePropertyChanged(nameof(Maximum3));
                //  SetProperty(ref maximum3, value);
            }
        }

        private double minimum3;

        public double Minimum3
        {
            get { return minimum3; }
            set
            {
                minimum3 = value;
                RaisePropertyChanged(nameof(Minimum3));
                //SetProperty(ref minimum3, value);
            }
        }

        private double maximum4;

        public double Maximum4
        {
            get { return maximum4; }
            set
            {
                maximum4 = value;
                RaisePropertyChanged(nameof(Maximum4));
                //SetProperty(ref maximum4, value);
            }
        }

        private double minimum4;

        public double Minimum4
        {
            get { return minimum4; }
            set
            {
                minimum4 = value;
                RaisePropertyChanged(nameof(Minimum4));
                // SetProperty(ref minimum4, value);
            }
        }

        private int valueS1;

        public int ValueS1
        {
            get { return valueS1; }
            set { SetProperty(ref valueS1, value); RaisePropertyChanged(nameof(ValueS1)); }
        }

        private int valueE1;

        public int ValueE1
        {
            get { return valueE1; }
            set { SetProperty(ref valueE1, value); RaisePropertyChanged(nameof(ValueE1)); }
        }

        private int valueS2;

        public int ValueS2
        {
            get { return valueS2; }
            set { SetProperty(ref valueS2, value); RaisePropertyChanged(nameof(ValueS2)); }
        }

        private int valueE2;

        public int ValueE2
        {
            get { return valueE2; }
            set { SetProperty(ref valueE2, value); RaisePropertyChanged(nameof(ValueE2)); }
        }

        private int valueS3;

        public int ValueS3
        {
            get { return valueS3; }
            set
            {
                SetProperty(ref valueS3, value);
                RaisePropertyChanged(nameof(ValueS3));
            }
        }

        private int valueE3;

        public int ValueE3
        {
            get { return valueE3; }
            set
            {
                SetProperty(ref valueE3, value);
                RaisePropertyChanged(nameof(ValueE3));
            }
        }

        private int valueS4;

        public int ValueS4
        {
            get { return valueS4; }
            set { SetProperty(ref valueS4, value); RaisePropertyChanged(nameof(ValueS4)); }
        }

        private int valueE4;

        public int ValueE4
        {
            get { return valueE4; }
            set { SetProperty(ref valueE4, value); RaisePropertyChanged(nameof(ValueE4)); }
        }

        private bool isReverse1;

        public bool IsReverse1
        {
            get { return isReverse1; }
            set { SetProperty(ref isReverse1, value); RaisePropertyChanged(nameof(IsReverse1)); }
        }

        private bool isReverse2;

        public bool IsReverse2
        {
            get { return isReverse2; }
            set { SetProperty(ref isReverse2, value); RaisePropertyChanged(nameof(IsReverse2)); }
        }

        private bool isReverse3;

        public bool IsReverse3
        {
            get { return isReverse3; }
            set { SetProperty(ref isReverse3, value); RaisePropertyChanged(nameof(IsReverse3)); }
        }

        private bool isReverse4;

        public bool IsReverse4
        {
            get { return isReverse4; }
            set { SetProperty(ref isReverse4, value); RaisePropertyChanged(nameof(IsReverse4)); }
        }

        #endregion 二值处理

        #region 处理

        private void runDispose()
        {
            try
            {
                HRegion region = regionTemp.Connection();

                var row1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("DisposeROIRow1", 0.0);
                var row2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("DisposeROIRow2", 0.0);
                var col1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("DisposeROIColumn1", 0.0);
                var col2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("DisposeROIColumn2", 0.0);
                //var w = row2 - row1; var h = col2 - col1;
                var w = row1; var h = col1;
                if (row1 + row2 != 0)
                    region = new HRegion(row1, col1, row2, col2).Intersection(region);
                // 回来
                //HTuple hTuple1 = new HTuple();
                //HTuple hTuple2 = new HTuple();
                //HTuple hTuple3 = new HTuple();
                //hTuple1 = hTuple1.TupleConcat("width", "height");
                //hTuple2 = hTuple2.TupleConcat(w, h);
                //hTuple3 = hTuple3.TupleConcat(w + 3, h + 3);
                //regionTemp = regionTempp.SelectShape(hTuple1, "or", hTuple2, hTuple3);
                //region = region.Difference(regionTempp).Connection();

                if (RegionFoundFeatureDatas.Count != 0)
                {
                    for (int i = 0; i < RegionFoundFeatureDataIndex + 1; i++)
                    {
                        switch (RegionFoundFeatureDatas[i].Feature)
                        {
                            case RegionFoundFeatures.圆形开运算:
                                region = region.OpeningCircle(RegionFoundFeatureDatas[i].Radius);
                                break;

                            case RegionFoundFeatures.矩形开运算:
                                region = region.OpeningRectangle1(RegionFoundFeatureDatas[i].Width, RegionFoundFeatureDatas[i].Height);
                                break;

                            case RegionFoundFeatures.圆形闭运算:
                                region = region.ClosingCircle(RegionFoundFeatureDatas[i].Radius);
                                break;

                            case RegionFoundFeatures.矩形闭运算:
                                region = region.ClosingRectangle1(RegionFoundFeatureDatas[i].Width, RegionFoundFeatureDatas[i].Height);
                                break;

                            case RegionFoundFeatures.圆形膨胀:
                                region = region.DilationCircle(RegionFoundFeatureDatas[i].Radius);
                                break;

                            case RegionFoundFeatures.矩形膨胀:
                                region = region.DilationRectangle1(RegionFoundFeatureDatas[i].Width, RegionFoundFeatureDatas[i].Height);
                                break;

                            case RegionFoundFeatures.圆形腐蚀:
                                region = region.ErosionCircle(RegionFoundFeatureDatas[i].Radius);
                                break;

                            case RegionFoundFeatures.矩形腐蚀:
                                region = region.ErosionRectangle1(RegionFoundFeatureDatas[i].Width, RegionFoundFeatureDatas[i].Height);
                                break;

                            case RegionFoundFeatures.填充孔洞:
                                region = region.FillUp();
                                break;

                            case RegionFoundFeatures.形状变换:
                                switch (RegionFoundFeatureDatas[i].TransIndex)
                                {
                                    case 0:
                                        region = region.ShapeTrans("convex");
                                        break;

                                    case 1:
                                        region = region.ShapeTrans("ellipse");
                                        break;

                                    case 2:
                                        region = region.ShapeTrans("outer_circle");
                                        break;

                                    case 3:
                                        region = region.ShapeTrans("inner_circle");
                                        break;

                                    case 4:
                                        region = region.ShapeTrans("rectangle1");
                                        break;

                                    case 5:
                                        region = region.ShapeTrans("rectangle2");
                                        break;

                                    case 6:
                                        region = region.ShapeTrans("inner_rectangle1");
                                        break;

                                    case 7:
                                        region = region.ShapeTrans("inner_center");
                                        break;

                                    default:
                                        break;
                                }
                                break;

                            case RegionFoundFeatures.连通:
                                region = region.Connection();
                                break;

                            case RegionFoundFeatures.联合:
                                region = region.Union1();
                                break;

                            default:
                                break;
                        }
                    }
                }

                resultRegion = region.Union1().Connection();

                Variables.ImageWindowDataForFunction.ROICtrl.Clear();
                Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                Variables.ImageWindowDataForFunction.DispObjectCtrl.AddDispObjectVar(resultRegion);
                //Variables.ImageWindowDataForFunction.ROICtrl.AddROI(new BingLibrary.Vision.ROIRegion(resultRegion) { ROIColor = BingLibrary.Vision.HalconColors.蓝色 });
                Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
            }
            catch { }
            isTestRuning = false;
        }

        private DelegateCommand _disposeROIOperate;

        public DelegateCommand DisposeROIOperate =>
            _disposeROIOperate ?? (_disposeROIOperate = new DelegateCommand(ExecuteDisposeROIOperate));

        private void ExecuteDisposeROIOperate()
        {
            Variables.ImageWindowDataForFunction.WindowCtrl.DrawMode = HalconDrawing.margin;
            NotDrawIng = false;
            try
            {
                var row1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("DisposeROIRow1", 0.0);
                var row2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("DisposeROIRow2", 0.0);
                var col1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("DisposeROIColumn1", 0.0);
                var col2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("DisposeROIColumn2", 0.0);

                if ((row1 + row2) == 0)
                {
                    Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = true;

                    Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor(HalconColors.橙色.ToDescription());
                    Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.DrawRectangle1(out row1, out col1, out row2, out col2);

                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("DisposeROIRow1", row1);
                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("DisposeROIRow2", row2);
                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("DisposeROIColumn1", col1);
                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("DisposeROIColumn2", col2);

                    Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                    Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = false;
                }
                else
                {
                    Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = true;
                    Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor(HalconColors.橙色.ToDescription());
                    Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.DrawRectangle1Mod(row1, col1, row2, col2, out row1, out col1, out row2, out col2);

                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("DisposeROIRow1", row1);
                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("DisposeROIRow2", row2);
                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("DisposeROIColumn1", col1);
                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("DisposeROIColumn2", col2);

                    Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                    Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = false;
                }
            }
            catch { Result = false; }

            Variables.ImageWindowDataForFunction.WindowCtrl.DrawMode = HalconDrawing.fill;
            Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
            NotDrawIng = true;
        }

        private DelegateCommand _regionEditSelection;

        public DelegateCommand RegionEditSelection =>
    _regionEditSelection ?? (_regionEditSelection = new DelegateCommand(ExecuteRegionEditSelection));

        private void ExecuteRegionEditSelection()
        {
            try
            {
                CircleVisibility = Visibility.Collapsed;
                RectVisibility = Visibility.Collapsed;
                TransVisibility = Visibility.Collapsed;

                if (RegionFoundFeatureDataIndex < 0)
                    RegionFoundFeatureDataIndex = 0;
                if (RegionFoundFeatureDataIndex >= RegionFoundFeatureDatas.Count)
                    RegionFoundFeatureDataIndex = RegionFoundFeatureDatas.Count - 1;

                if (RegionFoundFeatureDatas[RegionFoundFeatureDataIndex].Name.Contains("圆形"))
                {
                    CircleVisibility = Visibility.Visible;
                }
                else if (RegionFoundFeatureDatas[RegionFoundFeatureDataIndex].Name.Contains("矩形"))
                {
                    RectVisibility = Visibility.Visible;
                }
                else if (RegionFoundFeatureDatas[RegionFoundFeatureDataIndex].Name == "形状变换")
                {
                    TransVisibility = Visibility.Visible;
                }

                RegionFoundFeatureDatas[RegionFoundFeatureDataIndex].Radius = Radius;
                RegionFoundFeatureDatas[RegionFoundFeatureDataIndex].Width = Width;
                RegionFoundFeatureDatas[RegionFoundFeatureDataIndex].Height = Height;
                RegionFoundFeatureDatas[RegionFoundFeatureDataIndex].TransIndex = TransIndex;
                runDispose();
                //TestRun();
            }
            catch
            {
            }
        }

        private DelegateCommand _regionSelectionChanged;

        public DelegateCommand RegionSelectionChanged =>
    _regionSelectionChanged ?? (_regionSelectionChanged = new DelegateCommand(ExecuteRegionSelectionChanged));

        private void ExecuteRegionSelectionChanged()
        {
            try
            {
                CircleVisibility = Visibility.Collapsed;
                RectVisibility = Visibility.Collapsed;
                TransVisibility = Visibility.Collapsed;

                if (RegionFoundFeatureDataIndex < 0)
                    RegionFoundFeatureDataIndex = 0;
                if (RegionFoundFeatureDataIndex >= RegionFoundFeatureDatas.Count)
                    RegionFoundFeatureDataIndex = RegionFoundFeatureDatas.Count - 1;

                if (RegionFoundFeatureDatas[RegionFoundFeatureDataIndex].Name.Contains("圆形"))
                {
                    CircleVisibility = Visibility.Visible;
                }
                else if (RegionFoundFeatureDatas[RegionFoundFeatureDataIndex].Name.Contains("矩形"))
                {
                    RectVisibility = Visibility.Visible;
                }
                else if (RegionFoundFeatureDatas[RegionFoundFeatureDataIndex].Name == "形状变换")
                {
                    TransVisibility = Visibility.Visible;
                }

                Radius = RegionFoundFeatureDatas[RegionFoundFeatureDataIndex].Radius;
                Width = RegionFoundFeatureDatas[RegionFoundFeatureDataIndex].Width;
                Height = RegionFoundFeatureDatas[RegionFoundFeatureDataIndex].Height;
                TransIndex = RegionFoundFeatureDatas[RegionFoundFeatureDataIndex].TransIndex;
                runDispose();// TestRun();
            }
            catch
            {
            }
        }

        private DelegateCommand<string> _regionFoundOperater;

        public DelegateCommand<string> RegionFoundOperater =>
    _regionFoundOperater ?? (_regionFoundOperater = new DelegateCommand<string>(ExecuteRegionFoundOperater));

        private void ExecuteRegionFoundOperater(string parameter)
        {
            switch (parameter)
            {
                case "add":
                    if (RegionFoundFeature == RegionFoundFeatures.圆形开运算)
                    {
                        RegionFoundFeatureDatas.Add(new RegionFoundFeatureData()
                        {
                            Feature = RegionFoundFeature,
                            Name = RegionFoundFeature.ToDescription(),
                            Radius = 3.5,
                            Width = 0,
                            Height = 0,
                            TransIndex = 0
                        });
                    }
                    else if (RegionFoundFeature == RegionFoundFeatures.圆形腐蚀)
                    {
                        RegionFoundFeatureDatas.Add(new RegionFoundFeatureData()
                        {
                            Feature = RegionFoundFeature,
                            Name = RegionFoundFeature.ToDescription(),
                            Radius = 3.5,
                            Width = 0,
                            Height = 0,
                            TransIndex = 0
                        });
                    }
                    else if (RegionFoundFeature == RegionFoundFeatures.圆形膨胀)
                    {
                        RegionFoundFeatureDatas.Add(new RegionFoundFeatureData()
                        {
                            Feature = RegionFoundFeature,
                            Name = RegionFoundFeature.ToDescription(),
                            Radius = 3.5,
                            Width = 0,
                            Height = 0,
                            TransIndex = 0
                        });
                    }
                    else if (RegionFoundFeature == RegionFoundFeatures.圆形闭运算)
                    {
                        RegionFoundFeatureDatas.Add(new RegionFoundFeatureData()
                        {
                            Feature = RegionFoundFeature,
                            Name = RegionFoundFeature.ToDescription(),
                            Radius = 3.5,
                            Width = 0,
                            Height = 0,
                            TransIndex = 0
                        });
                    }
                    else if (RegionFoundFeature == RegionFoundFeatures.填充孔洞)
                    {
                        RegionFoundFeatureDatas.Add(new RegionFoundFeatureData()
                        {
                            Feature = RegionFoundFeature,
                            Name = RegionFoundFeature.ToDescription(),
                            Radius = 0,
                            Width = 0,
                            Height = 0,
                            TransIndex = 0
                        });
                    }
                    else if (RegionFoundFeature == RegionFoundFeatures.形状变换)
                    {
                        RegionFoundFeatureDatas.Add(new RegionFoundFeatureData()
                        {
                            Feature = RegionFoundFeature,
                            Name = RegionFoundFeature.ToDescription(),
                            Radius = 0,
                            Width = 3,
                            Height = 3,
                            TransIndex = 0
                        });
                    }
                    else if (RegionFoundFeature == RegionFoundFeatures.矩形开运算)
                    {
                        RegionFoundFeatureDatas.Add(new RegionFoundFeatureData()
                        {
                            Feature = RegionFoundFeature,
                            Name = RegionFoundFeature.ToDescription(),
                            Radius = 0,
                            Width = 3,
                            Height = 3,
                            TransIndex = 0
                        });
                    }
                    else if (RegionFoundFeature == RegionFoundFeatures.矩形腐蚀)
                    {
                        RegionFoundFeatureDatas.Add(new RegionFoundFeatureData()
                        {
                            Feature = RegionFoundFeature,
                            Name = RegionFoundFeature.ToDescription(),
                            Radius = 0,
                            Width = 3,
                            Height = 3,
                            TransIndex = 0
                        });
                    }
                    else if (RegionFoundFeature == RegionFoundFeatures.矩形膨胀)
                    {
                        RegionFoundFeatureDatas.Add(new RegionFoundFeatureData()
                        {
                            Feature = RegionFoundFeature,
                            Name = RegionFoundFeature.ToDescription(),
                            Radius = 0,
                            Width = 3,
                            Height = 3,
                            TransIndex = 0
                        });
                    }
                    else if (RegionFoundFeature == RegionFoundFeatures.矩形闭运算)
                    {
                        RegionFoundFeatureDatas.Add(new RegionFoundFeatureData()
                        {
                            Feature = RegionFoundFeature,
                            Name = RegionFoundFeature.ToDescription(),
                            Radius = 0,
                            Width = 3,
                            Height = 3,
                            TransIndex = 0
                        });
                    }
                    else if (RegionFoundFeature == RegionFoundFeatures.连通)
                    {
                        RegionFoundFeatureDatas.Add(new RegionFoundFeatureData()
                        {
                            Feature = RegionFoundFeature,
                            Name = RegionFoundFeature.ToDescription(),
                            Radius = 0,
                            Width = 3,
                            Height = 3,
                            TransIndex = 0
                        });
                    }
                    else if (RegionFoundFeature == RegionFoundFeatures.联合)
                    {
                        RegionFoundFeatureDatas.Add(new RegionFoundFeatureData()
                        {
                            Feature = RegionFoundFeature,
                            Name = RegionFoundFeature.ToDescription(),
                            Radius = 0,
                            Width = 3,
                            Height = 3,
                            TransIndex = 0
                        });
                    }
                    RegionFoundFeatureDataIndex = RegionFoundFeatureDatas.Count - 1;
                    break;

                case "remove":
                    RegionFoundFeatureDatas.RemoveAt(RegionFoundFeatureDataIndex);
                    if (RegionFoundFeatureDatas.Count == 0)
                    {
                        CircleVisibility = Visibility.Collapsed;
                        RectVisibility = Visibility.Collapsed;
                        TransVisibility = Visibility.Collapsed;
                    }
                    if (RegionFoundFeatureDataIndex >= RegionFoundFeatureDatas.Count)
                        RegionFoundFeatureDataIndex = RegionFoundFeatureDatas.Count - 1;
                    if (RegionFoundFeatureDataIndex < 0)
                        RegionFoundFeatureDataIndex = 0;
                    break;

                case "moveUp":
                    var selectedIndex = RegionFoundFeatureDataIndex;
                    var selectedItem = RegionFoundFeatureDatas[RegionFoundFeatureDataIndex];

                    if (selectedItem == null || selectedIndex <= 0)
                        return;
                    RegionFoundFeatureDatas.RemoveAt(selectedIndex);
                    RegionFoundFeatureDatas.Insert(selectedIndex - 1, selectedItem);
                    RegionFoundFeatureDataIndex = selectedIndex - 1;
                    break;

                case "moveDown":
                    selectedIndex = RegionFoundFeatureDataIndex;
                    selectedItem = RegionFoundFeatureDatas[RegionFoundFeatureDataIndex];

                    if (selectedItem == null || selectedIndex == -1 || selectedIndex == RegionFoundFeatureDatas.Count - 1)
                        return;
                    RegionFoundFeatureDatas.RemoveAt(selectedIndex);
                    RegionFoundFeatureDatas.Insert(selectedIndex + 1, selectedItem);
                    RegionFoundFeatureDataIndex = selectedIndex + 1;
                    break;
            }
            runDispose();//TestRun();
            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("RegionFoundFeatureDatas", RegionFoundFeatureDatas);
        }

        private double _radius;

        public double Radius
        {
            get { return _radius; }
            set { SetProperty(ref _radius, value); }
        }

        private int _width;

        public int Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }
        }

        private int _height;

        public int Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }

        private int _transIndex;

        public int TransIndex
        {
            get { return _transIndex; }
            set { SetProperty(ref _transIndex, value); }
        }

        private int _regionFoundFeatureDataIndex;

        public int RegionFoundFeatureDataIndex
        {
            get { return _regionFoundFeatureDataIndex; }
            set { SetProperty(ref _regionFoundFeatureDataIndex, value); }
        }

        private Visibility _circleVisibility = Visibility.Collapsed;

        public Visibility CircleVisibility
        {
            get { return _circleVisibility; }
            set { SetProperty(ref _circleVisibility, value); }
        }

        private Visibility _rectVisibility = Visibility.Collapsed;

        public Visibility RectVisibility
        {
            get { return _rectVisibility; }
            set { SetProperty(ref _rectVisibility, value); }
        }

        private Visibility _transVisibility = Visibility.Collapsed;

        public Visibility TransVisibility
        {
            get { return _transVisibility; }
            set { SetProperty(ref _transVisibility, value); }
        }

        private RegionFoundFeatures _regionFoundFeature = RegionFoundFeatures.圆形开运算;

        public RegionFoundFeatures RegionFoundFeature
        {
            get { return _regionFoundFeature; }
            set { SetProperty(ref _regionFoundFeature, value); }
        }

        public class RegionFoundFeatureData
        {
            public string Name { set; get; } = "";
            public RegionFoundFeatures Feature { set; get; } = RegionFoundFeatures.圆形开运算;
            public double Radius { set; get; } = 3.5;
            public int Width { set; get; } = 3;
            public int Height { set; get; } = 3;
            public int TransIndex { set; get; } = 0;
        }

        private ObservableCollection<RegionFoundFeatureData> _regionFoundFeatureDatas = new ObservableCollection<RegionFoundFeatureData>();

        public ObservableCollection<RegionFoundFeatureData> RegionFoundFeatureDatas
        {
            get { return _regionFoundFeatureDatas; }
            set { SetProperty(ref _regionFoundFeatureDatas, value); }
        }

        #endregion 处理

        #region 提取

        private DelegateCommand<string> _filterOperater;

        public DelegateCommand<string> FilterOperater =>
    _filterOperater ?? (_filterOperater = new DelegateCommand<string>(ExecuteFilterOperater));

        private void ExecuteFilterOperater(string parameter)
        {
            switch (parameter)
            {
                case "add":
                    ValueVisibility = Visibility.Visible;
                    BlobFeatureDatas.Add(new BlobFeatureData()
                    {
                        Feature = BlobFeature,
                        Name = BlobFeature.ToString(),
                        MinValue = 0,
                        MaxValue = 1000,
                    });
                    BlobFeatureDataIndex = BlobFeatureDatas.Count - 1;
                    break;

                case "remove":
                    BlobFeatureDatas.RemoveAt(BlobFeatureDataIndex);
                    if (BlobFeatureDatas.Count == 0)
                    {
                        ValueVisibility = Visibility.Collapsed;
                    }
                    if (BlobFeatureDataIndex >= BlobFeatureDatas.Count)
                        BlobFeatureDataIndex = BlobFeatureDatas.Count - 1;
                    if (BlobFeatureDataIndex < 0)
                        BlobFeatureDataIndex = 0;
                    break;
            }
            runExtract();
            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("BlobFeatureDatas", BlobFeatureDatas);
        }

        private DelegateCommand _selectionChanged;

        public DelegateCommand SelectionChanged =>
    _selectionChanged ?? (_selectionChanged = new DelegateCommand(ExecuteSelectionChanged));

        private void ExecuteSelectionChanged()
        {
            try
            {
                if (BlobFeatureDatas.Count != 0)
                    ValueVisibility = Visibility.Visible;
                if (BlobFeatureDataIndex < 0)
                    BlobFeatureDataIndex = 0;
                if (BlobFeatureDataIndex >= BlobFeatureDatas.Count)
                    BlobFeatureDataIndex = BlobFeatureDatas.Count - 1;

                MinValue = BlobFeatureDatas[BlobFeatureDataIndex].MinValue;
                MaxValue = BlobFeatureDatas[BlobFeatureDataIndex].MaxValue;

                runExtract();
            }
            catch
            {
            }
        }

        private double row1Temp, row2Temp, col1Temp, col2Temp;
        private DelegateCommand _drawTemp;

        public DelegateCommand DrawTemp =>
            _drawTemp ?? (_drawTemp = new DelegateCommand(ExecuteDrawTemp));

        private void ExecuteDrawTemp()
        {
            Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = true;

            Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor(HalconColors.橙色.ToDescription());
            Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.DrawRectangle1(out row1Temp, out col1Temp, out row2Temp, out col2Temp);
            Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
            try
            {
                runExtract();
            }
            catch (Exception)
            {
                throw;
            }
            var rstCon = new HRegion(row1Temp, col1Temp, row2Temp, col2Temp).GenContourRegionXld("border");
            Variables.ImageWindowDataForFunction.DispObjectCtrl.AddDispObjectVar(rstCon);
            Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
        }

        private DelegateCommand _testRunExtract;

        public DelegateCommand TestRunExtract =>
    _testRunExtract ?? (_testRunExtract = new DelegateCommand(ExecuteTestRunExtract));

        private void ExecuteTestRunExtract()
        {
            try
            {
                runExtract();
            }
            catch { }
        }

        private DelegateCommand _editSelection;

        public DelegateCommand EditSelection =>
    _editSelection ?? (_editSelection = new DelegateCommand(ExecuteEditSelection));

        private void ExecuteEditSelection()
        {
            try
            {
                if (BlobFeatureDataIndex < 0)
                    BlobFeatureDataIndex = 0;
                if (BlobFeatureDataIndex >= BlobFeatureDatas.Count)
                    BlobFeatureDataIndex = BlobFeatureDatas.Count - 1;

                BlobFeatureDatas[BlobFeatureDataIndex].MinValue = MinValue;
                BlobFeatureDatas[BlobFeatureDataIndex].MaxValue = MaxValue;

                runExtract();
            }
            catch
            {
            }
        }

        private bool isTestRuning = false;

        private void runExtract()
        {
            if (!isInitlized)
                return;

            if (isTestRuning)
            {
                return;
            }
            else
            {
                isTestRuning = true;
            }
            try
            {
                Dictionary<string, List<double>> blobRess = new Dictionary<string, List<double>>();
                HRegion region = regionTemp.Connection();
                HRegion regions = new HRegion(region);
                HRegion regionrefer = new HRegion(row1Temp, col1Temp, row2Temp, col2Temp);

                regions.GenEmptyRegion();
                if (BlobFeatureDatas.Count == 0)
                    regions = regions.ConcatObj(region);
                else
                {
                    HTuple hNames = new HTuple();
                    HTuple hMins = new HTuple();
                    HTuple hMaxs = new HTuple();
                    double min = -1, max = -1;
                    for (int i = 0; i < BlobFeatureDatas.Count; i++)
                    {
                        if (i == BlobFeatureDataIndex)
                        {
                            if (FilterModeIndex == 2 || FilterModeIndex == 3)
                            {
                                continue;
                            }
                        }
                        hNames = hNames.TupleConcat(BlobFeatureDatas[i].Feature.ToDescription());
                        hMins = hMins.TupleConcat(BlobFeatureDatas[i].MinValue);
                        hMaxs = hMaxs.TupleConcat(BlobFeatureDatas[i].MaxValue);
                    }

                    if (FilterModeIndex == 0)
                    {
                        HTuple featureName = new HTuple(BlobFeatureDatas[BlobFeatureDataIndex].Feature.ToDescription());
                        HTuple featureValue = region.RegionFeatures(featureName);
                        min = featureValue.TupleMin();
                        max = featureValue.TupleMax();

                        regions = region.SelectShape(hNames, "or", hMins, hMaxs);
                    }
                    else if (FilterModeIndex == 1)
                    {
                        HTuple featureName = new HTuple(BlobFeatureDatas[BlobFeatureDataIndex].Feature.ToDescription());
                        HTuple featureValue = region.RegionFeatures(featureName);

                        min = featureValue.TupleMin();
                        max = featureValue.TupleMax();
                        regions = region.SelectShape(hNames, "or", hMins, hMaxs);
                        regions = region.Difference(regions);
                    }
                    else if (FilterModeIndex == 2)
                    {
                        HTuple featureName = new HTuple(BlobFeatureDatas[BlobFeatureDataIndex].Feature.ToDescription());
                        HTuple featureValue = new HTuple();
                        if (BlobFeatureDatas.Count != 1)
                        {
                            regions = region.SelectShape(hNames, "and", hMins, hMaxs).Connection();
                            featureValue = regions.RegionFeatures(featureName);
                        }
                        else featureValue = region.RegionFeatures(featureName);
                        min = featureValue.TupleMin();
                        max = featureValue.TupleMax();

                        hNames = hNames.TupleConcat(BlobFeatureDatas[BlobFeatureDataIndex].Feature.ToDescription());
                        hMins = hMins.TupleConcat(BlobFeatureDatas[BlobFeatureDataIndex].MinValue);
                        hMaxs = hMaxs.TupleConcat(BlobFeatureDatas[BlobFeatureDataIndex].MaxValue);
                        regions = region.SelectShape(hNames, "and", hMins, hMaxs);
                    }
                    else if (FilterModeIndex == 3)
                    {
                        HTuple featureName = new HTuple(BlobFeatureDatas[BlobFeatureDataIndex].Feature.ToDescription());
                        HTuple featureValue = new HTuple();
                        if (BlobFeatureDatas.Count != 1)
                        {
                            regions = region.SelectShape(hNames, "and", hMins, hMaxs).Connection();
                            featureValue = regions.RegionFeatures(featureName);
                        }
                        else featureValue = region.RegionFeatures(featureName);
                        min = featureValue.TupleMin();
                        max = featureValue.TupleMax();

                        hNames = hNames.TupleConcat(BlobFeatureDatas[BlobFeatureDataIndex].Feature.ToDescription());
                        hMins = hMins.TupleConcat(BlobFeatureDatas[BlobFeatureDataIndex].MinValue);
                        hMaxs = hMaxs.TupleConcat(BlobFeatureDatas[BlobFeatureDataIndex].MaxValue);
                        regions = region.SelectShape(hNames, "and", hMins, hMaxs);
                    }

                    if (row1Temp + row2Temp != 0)
                    {
                        HTuple featureName = new HTuple(BlobFeatureDatas[BlobFeatureDataIndex].Feature.ToDescription());
                        regionrefer = regionrefer.Intersection(region).Connection();
                        HTuple featureValue = regionrefer.RegionFeatures(featureName);
                        min = featureValue.TupleMin();
                        max = featureValue.TupleMax();
                    }
                    resultRegion = regions;
                    HTuple unique = Unique(hNames);
                    for (int i = 0; i < unique.Length; i++)
                    {
                        string temp = unique[i];
                        HTuple featureResult = regions.RegionFeatures(new HTuple(temp));
                        {
                            //blobRess.Add(new BlobRes()  { FeatureName = unique[i], Values = ConvertHTupleToList(featureResult) });
                            blobRess.Add(unique[i], ConvertHTupleToList(featureResult));
                        }
                    }

                    num = regions.Connection().CountObj();

                    Text = "    区域内特征参数最小为：" + Math.Round(min).ToString() + " , 最大为： " + Math.Round(max).ToString();
                }
            }
            catch { }
            isTestRuning = false;
            //Variables.ImageWindowDataForFunction.WindowCtrl.ShowImageToWindow(resultImage.CopyImage());///因为被释放掉了
            Variables.ImageWindowDataForFunction.ROICtrl.Clear();
            Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
            Variables.ImageWindowDataForFunction.DispObjectCtrl.AddDispObjectVar(resultRegion);
            //Variables.ImageWindowDataForFunction.ROICtrl.AddROI(new BingLibrary.Vision.ROIRegion(resultRegion) { ROIColor = BingLibrary.Vision.HalconColors.蓝色 });
            Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();

            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("FilterModeIndex", FilterModeIndex);
            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("BlobFeatureDatas", BlobFeatureDatas);
            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("BlobFeatureDataIndex", BlobFeatureDataIndex);
            //Variables.CurrentProgramData.Parameters.BingAddOrUpdate("resultRegion", resultRegion);
        }

        private int num = 0;

        public static List<double> ConvertHTupleToList(HTuple htuple)
        {
            List<double> list = new List<double>();
            for (int i = 0; i < htuple.Length; i++)
            {
                list.Add(htuple[i]);
            }
            return list;
        }

        public static HTuple Unique(HTuple htuple)
        {
            HTuple tuple = new HTuple();
            HashSet<string> seen = new HashSet<string>();
            for (int i = 0; i < htuple.Length; i++)
            {
                string item = htuple[i];
                if (!seen.Contains(item))
                {
                    seen.Add(item);
                    tuple = tuple.TupleConcat(item);
                }
            }
            return tuple;
        }

        private int _filterModeIndex = 0;

        public int FilterModeIndex
        {
            get { return _filterModeIndex; }
            set { SetProperty(ref _filterModeIndex, value); }
        }

        public class BlobFeatureData
        {
            public string Name { set; get; } = "";
            public BlobFeatures Feature { set; get; } = BlobFeatures.面积;
            public double MinValue { set; get; } = 1.0;
            public double MaxValue { set; get; } = 1000.0;
        }

        public ObservableCollection<BlobFeatureData> _blobFeatureDatas = new ObservableCollection<BlobFeatureData>();

        public ObservableCollection<BlobFeatureData> BlobFeatureDatas
        {
            get { return _blobFeatureDatas; }
            set { SetProperty(ref _blobFeatureDatas, value); }
        }

        private BlobFeatures _blobFeature = BlobFeatures.面积;

        public BlobFeatures BlobFeature
        {
            get { return _blobFeature; }
            set { SetProperty(ref _blobFeature, value); }
        }

        private int _blobFeatureDataIndex = 0;

        public int BlobFeatureDataIndex
        {
            get { return _blobFeatureDataIndex; }
            set { SetProperty(ref _blobFeatureDataIndex, value); }
        }

        private double _minValue = 1;

        public double MinValue
        {
            get { return Math.Round(_minValue, 2); }
            set
            {
                if (value < 0) _minValue = 0;
                else if (value > _maxValue)
                    _minValue = _maxValue;
                else _minValue = value;
                RaisePropertyChanged(nameof(MinValue));
            }
        }

        private double _maxValue = 1000;

        public double MaxValue
        {
            get { return Math.Round(_maxValue, 2); }
            set
            {
                if (value < 0) { _maxValue = 0; }
                else if (value < _minValue)
                    _maxValue = _minValue;
                else { _maxValue = value; }
                RaisePropertyChanged(nameof(MaxValue));
            }
        }

        private string _text;

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        private Visibility _valueVisibility = Visibility.Collapsed;

        public Visibility ValueVisibility
        {
            get { return _valueVisibility; }
            set { SetProperty(ref _valueVisibility, value); }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        #endregion 提取

        #region 判定

        private double _allowOffset = 0;

        public double AllowOffset
        {
            get { return _allowOffset; }
            set
            {
                SetProperty(ref _allowOffset, value);
            }
        }

        private int _scale = 1;

        public int Scale
        {
            get { return _scale; }
            set
            {
                SetProperty(ref _scale, value);
            }
        }

        private DelegateCommand<string> _inspectROIOperate;

        public DelegateCommand<string> InspectROIOperate =>
            _inspectROIOperate ?? (_inspectROIOperate = new DelegateCommand<string>(ExecuteInspectROIOperate));

        private void ExecuteInspectROIOperate(string parameter)
        {
            Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = true;
            Variables.ImageWindowDataForFunction.WindowCtrl.DrawMode = HalconDrawing.margin;
            NotDrawIng = false;
            try
            {
                //List<InspectFrame> InspectFrames = new List<InspectFrame>();
                switch (parameter)
                {
                    case "1":
                        {
                            HRegion RegionTemp = new HRegion();
                            RegionTemp.GenEmptyRegion();
                            inspectFrames = (List<InspectFrame>)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectFrames", new List<InspectFrame>());
                            double r1 = 0, c1 = 0, r2 = 0, c2 = 0;
                            // (  (ROIRectangle1)   Variables.ImageWindowDataForFunction.ROICtrl.GetActiveROI()[0]).ro;
                            Variables.ImageWindowDataForFunction.WindowCtrl.CanEdit = true;
                            Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.DrawRectangle1(out r1, out c1, out r2, out c2);

                            double crossRow = 0, crossCol = 0;
                            double area = 0;
                            RegionTemp.GenRectangle1(r1, c1, r2, c2);
                            ROIRectangle1 rOIRectangle1 = new ROIRectangle1();
                            rOIRectangle1.createROIRect1(r1, c1, r2, c2);
                            rOIRectangle1.ROIColor = BingLibrary.Vision.HalconColors.蓝色;
                            RegionTemp = RegionTemp.Intersection(resultRegion);
                            try
                            {
                                area = RegionTemp.AreaCenter(out crossRow, out crossCol);
                            }
                            catch { }
                            if ((RegionTemp.Connection()).CountObj() == 1 && area > 1)
                            {
                                inspectFrames.Add(new InspectFrame() { Row1 = r1, Row2 = r2, Col1 = c1, Col2 = c2, ContainsObject = true, CrossRow = crossRow, CrossCol = crossCol });
                                rOIRectangle1.ROIColor = BingLibrary.Vision.HalconColors.蓝色;
                                Variables.ImageWindowDataForFunction.ROICtrl.AddROI(rOIRectangle1);
                                Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
                            }
                            else
                            {
                                inspectFrames.Add(new InspectFrame() { Row1 = r1, Row2 = r2, Col1 = c1, Col2 = c2, ContainsObject = false, CrossRow = crossRow, CrossCol = crossCol });
                                rOIRectangle1.ROIColor = BingLibrary.Vision.HalconColors.红色;
                                Variables.ImageWindowDataForFunction.ROICtrl.AddROI(rOIRectangle1);
                                Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
                            }
                        }
                        break;

                    case "2":
                        {
                            int index = Variables.ImageWindowDataForFunction.ROICtrl.GetActiveROIIdx();
                            Variables.ImageWindowDataForFunction.ROICtrl.RemoveActiveRoi();

                            inspectFrames.RemoveAt(index);
                        }
                        break;
                }
            }
            catch { }
            Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = false;
            Variables.ImageWindowDataForFunction.WindowCtrl.DrawMode = HalconDrawing.margin;
            Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
            NotDrawIng = true;
        }

        private int _inspectWidth = 60;

        public int InspectWidth
        {
            get { return _inspectWidth; }
            set { SetProperty(ref _inspectWidth, value); }
        }

        private int _inspectHeight = 60;

        public int InspectHeight
        {
            get { return _inspectHeight; }
            set { SetProperty(ref _inspectHeight, value); }
        }

        private DelegateCommand _oneStepToInspect;

        public DelegateCommand OneStepToInspect =>
            _oneStepToInspect ?? (_oneStepToInspect = new DelegateCommand(ExecuteOneStepToInspect));

        private void ExecuteOneStepToInspect()
        {
            bool rst = Variables.ShowConfirm("请确认好设置的单个Pin检测框的宽高！");
            if (rst)
            {
                rst = Variables.ShowConfirm("是否清除已有的检测框？");
                if (rst)
                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("InspectFrames", new List<InspectFrame>());

                Variables.ShowMessage("绘制包含需要自动设置的Pin的检测框。");
                HRegion RegionTemp = new HRegion();
                RegionTemp.GenEmptyRegion();
                inspectFrames = (List<InspectFrame>)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectFrames", new List<InspectFrame>());
                double r1 = 0, c1 = 0, r2 = 0, c2 = 0;
                // (  (ROIRectangle1)   Variables.ImageWindowDataForFunction.ROICtrl.GetActiveROI()[0]).ro;
                Variables.ImageWindowDataForFunction.WindowCtrl.CanEdit = true;
                NotDrawIng = false;
                Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = true;

                Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.DrawRectangle1(out r1, out c1, out r2, out c2);
                Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = false;
                NotDrawIng = true;
                double crossRow = 0, crossCol = 0;
                double area = 0;
                RegionTemp.GenRectangle1(r1, c1, r2, c2);

                var regions = RegionTemp.Intersection(resultRegion).Union1().Connection();
                HTuple rs, cs;
                regions.AreaCenter(out rs, out cs);

                Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                Variables.ImageWindowDataForFunction.MessageCtrl.ClearMessages();
                Variables.ImageWindowDataForFunction.ROICtrl.Clear();
                for (int i = 0; i < rs.DArr.Length; i++)
                {
                    ROIRectangle1 rOIRectangle1 = new ROIRectangle1();
                    rOIRectangle1.createROIRect1(rs[i] - InspectHeight, cs[i] - InspectHeight, rs[i] + InspectHeight, cs[i] + InspectHeight);
                    rOIRectangle1.ROIColor = BingLibrary.Vision.HalconColors.蓝色;
                    inspectFrames.Add(new InspectFrame() { Row1 = rs[i] - InspectHeight, Row2 = rs[i] + InspectHeight, Col1 = cs[i] - InspectHeight, Col2 = cs[i] + InspectHeight, ContainsObject = true, CrossRow = rs[i], CrossCol = cs[i] });
                    Variables.ImageWindowDataForFunction.ROICtrl.AddROI(rOIRectangle1);
                    Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
                }

                Variables.ShowMessage("创建完成，请做进一步调整。");
            }
        }

        private DelegateCommand _testRunJudge;

        public DelegateCommand TestRunJudge =>
            _testRunJudge ?? (_testRunJudge = new DelegateCommand(ExecuteTestRunJudge));

        private void ExecuteTestRunJudge()
        {
            try
            {
                var rst = Function_PINOneTool.Run(originalImage, Variables.CurrentProgramData);

                string[] results = rst.MessageResult.Split('\n');
                //CurNum = rst.MessageResult;
                var region = rst.RunRegion;

                var row1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow1", 0.0);
                var col1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn1", 0.0);
                var Frames = (List<InspectFrame>)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectFrames", new List<InspectFrame>());
                //HImage showImage = new HImage();
                //if (row1 + row2 != 0)
                //    showImage = image.CropRectangle1(row1, col1, row2, col2);
                //else showImage = image.CopyImage();
                //Variables.ImageWindowDataForFunction.WindowCtrl.ShowImageToWindow(showImage);

                Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                Variables.ImageWindowDataForFunction.MessageCtrl.ClearMessages();
                Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetDraw("margin");
                Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor("green");
                Variables.ImageWindowDataForFunction.DispObjectCtrl.AddDispObjectVar(region);
                region.MoveRegion(-(int)row1, -(int)col1);
                for (int i = 0; i < Frames.Count; i++)
                {
                    double row = Frames[i].Row1;
                    double col = Frames[i].Col1;
                    double distance = 0;
                    bool b = double.TryParse(results[i + 1], out distance);
                    //results[i].Split("");
                    if (!Frames[i].ContainsObject)
                    {
                        Variables.ImageWindowDataForFunction.MessageCtrl.AddMessageVar("", (int)row, (int)col, 36, BingLibrary.Vision.HalconColors.红色);
                    }
                    else if (distance < AllowOffset)
                    {
                        if (results[i + 1] == "缺失")
                            Variables.ImageWindowDataForFunction.MessageCtrl.AddMessageVar(results[i + 1], (int)row, (int)col, 20, BingLibrary.Vision.HalconColors.红色);
                        else
                            Variables.ImageWindowDataForFunction.MessageCtrl.AddMessageVar(results[i + 1], (int)row, (int)col, 20, BingLibrary.Vision.HalconColors.蓝色);
                    }
                    else { Variables.ImageWindowDataForFunction.MessageCtrl.AddMessageVar(results[i + 1], (int)row, (int)col, 20, BingLibrary.Vision.HalconColors.红色); }
                }
                Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
            }
            catch { }
        }

        private List<InspectFrame> inspectFrames = new List<InspectFrame> { };

        #endregion 判定

        public string Title => "PIN针检测";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            if (NotDrawIng)
                return true;
            else
            {
                Variables.ShowMessage("请先右键完成绘制");
                return false;
            }
        }

        private bool isInitlized = false;

        public async Task<bool> Init()
        {
            isInitlized = false;
            await Task.Delay(300);
            try
            {
                IsTrans = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsTrans", false);
                TransBrightnessValue = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("TransBrightnessValue", 128.0);
                TransContrastValue = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("TransContrastValue", 128.0);
                TransGammaValue = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("TransGammaValue", 1.0);
            }
            catch { }

            IsPreEnhance = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsPreEnhance", false);
            IsEnableDeep = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnableDeep", false);
            BrightnessValue = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("BrightnessValue", 128.0);
            ContrastValue = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ContrastValue", 128.0);
            GammaValue = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("GammaValue", 1.0);

            //IsGray = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsGray", false);

            GrayModeIndex = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("GrayModeIndex", 0).ToString());
            if (GrayModeIndex == 0)
            {
                Minimum1 = 0; Maximum1 = 100;
                Minimum2 = 0; Maximum2 = 100;
                Minimum3 = 0; Maximum3 = 100;
                Minimum4 = 0; Maximum4 = 255;

                ValueS1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS1" + GrayModeIndex.ToString(), 0).ToString());
                ValueS2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS2" + GrayModeIndex.ToString(), 0).ToString());
                ValueS3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS3" + GrayModeIndex.ToString(), 0).ToString());
                ValueS4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS4" + GrayModeIndex.ToString(), 0).ToString());
                ValueE1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE1" + GrayModeIndex.ToString(), 30).ToString());
                ValueE2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE2" + GrayModeIndex.ToString(), 59).ToString());
                ValueE3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE3" + GrayModeIndex.ToString(), 11).ToString());
                ValueE4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE4" + GrayModeIndex.ToString(), 128).ToString());
                IsEnable1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable1" + GrayModeIndex.ToString(), true);
                IsEnable2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable2" + GrayModeIndex.ToString(), true);
                IsEnable3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable3" + GrayModeIndex.ToString(), true);
                IsEnable4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable4" + GrayModeIndex.ToString(), true);
                IsReverse1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse11" + GrayModeIndex.ToString(), false);
                IsReverse2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse12" + GrayModeIndex.ToString(), false);
                IsReverse3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse13" + GrayModeIndex.ToString(), false);
                IsReverse4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse14" + GrayModeIndex.ToString(), false);
            }
            else if (GrayModeIndex == 1)
            {
                Minimum1 = 0; Maximum1 = 255;
                Minimum2 = 0; Maximum2 = 255;
                Minimum3 = 0; Maximum3 = 255;
                Minimum4 = 0; Maximum4 = 255;

                ValueS1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS1" + GrayModeIndex.ToString(), 0).ToString());
                ValueS2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS2" + GrayModeIndex.ToString(), 0).ToString());
                ValueS3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS3" + GrayModeIndex.ToString(), 0).ToString());
                ValueS4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS4" + GrayModeIndex.ToString(), 0).ToString());
                ValueE1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE1" + GrayModeIndex.ToString(), 128).ToString());
                ValueE2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE2" + GrayModeIndex.ToString(), 255).ToString());
                ValueE3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE3" + GrayModeIndex.ToString(), 255).ToString());
                ValueE4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE4" + GrayModeIndex.ToString(), 255).ToString());
                IsEnable1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable1" + GrayModeIndex.ToString(), true);
                IsEnable2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable2" + GrayModeIndex.ToString(), true);
                IsEnable3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable3" + GrayModeIndex.ToString(), true);
                IsEnable4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable4" + GrayModeIndex.ToString(), false);
                IsReverse1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse11" + GrayModeIndex.ToString(), false);
                IsReverse2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse12" + GrayModeIndex.ToString(), false);
                IsReverse3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse13" + GrayModeIndex.ToString(), false);
                IsReverse4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse14" + GrayModeIndex.ToString(), false);
            }
            else if (GrayModeIndex == 2)
            {
                Minimum1 = 0; Maximum1 = 255;
                Minimum2 = 0; Maximum2 = 255;
                Minimum3 = 0; Maximum3 = 255;
                Minimum4 = 0; Maximum4 = 255;

                ValueS1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS1" + GrayModeIndex.ToString(), 0).ToString());
                ValueS2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS2" + GrayModeIndex.ToString(), 0).ToString());
                ValueS3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS3" + GrayModeIndex.ToString(), 0).ToString());
                ValueS4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS4" + GrayModeIndex.ToString(), 0).ToString());
                ValueE1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE1" + GrayModeIndex.ToString(), 128).ToString());
                ValueE2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE2" + GrayModeIndex.ToString(), 255).ToString());
                ValueE3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE3" + GrayModeIndex.ToString(), 255).ToString());
                ValueE4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE4" + GrayModeIndex.ToString(), 255).ToString());
                IsEnable1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable1" + GrayModeIndex.ToString(), true);
                IsEnable2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable2" + GrayModeIndex.ToString(), true);
                IsEnable3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable3" + GrayModeIndex.ToString(), true);
                IsEnable4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable4" + GrayModeIndex.ToString(), false);
                IsReverse1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse11" + GrayModeIndex.ToString(), false);
                IsReverse2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse12" + GrayModeIndex.ToString(), false);
                IsReverse3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse13" + GrayModeIndex.ToString(), false);
                IsReverse4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse14" + GrayModeIndex.ToString(), false);
            }

            AllowOffset = (double)(Variables.CurrentProgramData.Parameters.BingGetOrAdd("AllowOffset", 0.0));
            Scale = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("Scale", 0).ToString());

            FilterModeIndex = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("FilterModeIndex", 0).ToString());

            RegionFoundFeatureDatas = (ObservableCollection<RegionFoundFeatureData>)Variables.CurrentProgramData.Parameters.BingGetOrAdd("RegionFoundFeatureDatas", new ObservableCollection<RegionFoundFeatureData>());
            //RegionFoundFeatureDataIndex = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("RegionFoundFeatureDataIndex", 0).ToString());
            ExecuteRegionSelectionChanged();
            BlobFeatureDatas = (ObservableCollection<BlobFeatureData>)Variables.CurrentProgramData.Parameters.BingGetOrAdd("BlobFeatureDatas", new ObservableCollection<BlobFeatureData>());
            BlobFeatureDataIndex = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("BlobFeatureDataIndex", 0).ToString());
            ExecuteSelectionChanged();

            await Task.Delay(200);
            isInitlized = true;
            DoSwitchTab.Execute();
            return true;
        }

        private DelegateCommand<string> _save;

        public DelegateCommand<string> Save =>
            _save ?? (_save = new DelegateCommand<string>(ExecuteSave));

        private void ExecuteSave(string parameter)
        {
            switch (parameter)
            {
                case "0":
                    Variables.ImageWindowDataForFunction.ROICtrl.Clear();
                    for (int i = 0; i < inspectFrames.Count; i++)
                    {
                        ROIRectangle1 rOIRectangle1 = new ROIRectangle1();
                        rOIRectangle1.row1 = inspectFrames[i].Row1;
                        rOIRectangle1.col1 = inspectFrames[i].Col1;
                        rOIRectangle1.row2 = inspectFrames[i].Row2;
                        rOIRectangle1.col2 = inspectFrames[i].Col2;
                        if (inspectFrames[i].ContainsObject)
                            rOIRectangle1.ROIColor = BingLibrary.Vision.HalconColors.蓝色;
                        else
                            rOIRectangle1.ROIColor = BingLibrary.Vision.HalconColors.红色;
                        Variables.ImageWindowDataForFunction.ROICtrl.AddROI(rOIRectangle1);
                    }
                    Variables.ImageWindowDataForFunction.WindowCtrl.DrawMode = HalconDrawing.margin;
                    Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("InspectFrames", inspectFrames);
                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("AllowOffset", AllowOffset);
                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("Scale", Scale);
                    break;

                case "1":
                    var rst = Variables.ShowConfirm("是否注册为检测基准模板？");
                    if (rst)
                    {
                        HRegion RegionTemp = new HRegion();
                        RegionTemp.GenEmptyRegion();
                        var rolList = Variables.ImageWindowDataForFunction.ROICtrl.GetROIList();
                        inspectFrames.Clear();
                        for (int i = 0; i < rolList.Count; i++)
                        {
                            var v = (ROIRectangle1)rolList[i];
                            RegionTemp.GenRectangle1(v.row1, v.col1, v.row2, v.col2);
                            RegionTemp = RegionTemp.Intersection(resultRegion);
                            double area = 0;
                            try
                            {
                                area = RegionTemp.AreaCenter(out double crossRow, out double crossCol);
                            }
                            catch { }
                            if ((RegionTemp.Connection()).CountObj() != 1 || area <= 1)
                            {
                                inspectFrames.Add(new InspectFrame() { Row1 = v.row1, Row2 = v.row2, Col1 = v.col1, Col2 = v.col2, ContainsObject = false, CrossRow = 0, CrossCol = 0 });
                            }
                            else
                            {
                                double cr = 0, cc = 0;
                                try
                                {
                                    RegionTemp.AreaCenter(out cr, out cc);
                                }
                                catch { }
                                inspectFrames.Add(new InspectFrame() { Row1 = v.row1, Row2 = v.row2, Col1 = v.col1, Col2 = v.col2, ContainsObject = true, CrossRow = cr, CrossCol = cc });
                            }
                        }
                    }
                    break;
            }
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _ = Init();
        }

        public bool Update()
        {
            return true;
        }
    }
}