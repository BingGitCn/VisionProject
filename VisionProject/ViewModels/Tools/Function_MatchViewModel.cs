using BingLibrary.Extension;
using BingLibrary.Vision;
using HalconDotNet;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using VisionProject.GlobalVars;

namespace VisionProject.ViewModels
{
    public class Function_MatchViewModel : BindableBase, IDialogAware, IFunction_ViewModel_Interface
    {
        public Function_MatchViewModel()
        {
        }

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

        private void ExecuteDoSwitchTab()
        {
            if (oldTabIndex != TabIndex)
            {
                oldTabIndex = TabIndex;
                if (TabIndex == 0)
                { }
                else if (TabIndex == 1)
                {
                    ExecuteModelOperate("trans");
                }
                else if (TabIndex == 2)
                {
                    ExecuteModelOperate("trans");
                    image?.Dispose();
                    image = resultImage.CopyImage();
                    runPreEnhance();
                }
                else if (TabIndex == 3)
                {
                    ExecuteModelOperate("trans");
                    image?.Dispose();
                    image = resultImage.CopyImage();
                    runPreEnhance();
                    image?.Dispose();
                    image = resultImage.CopyImage();
                    runGray();
                }
                else if (TabIndex == 4)
                {
                    ExecuteModelOperate("trans");
                    image?.Dispose();
                    image = resultImage.CopyImage();
                    runPreEnhance();
                    image?.Dispose();
                    image = resultImage.CopyImage();
                    runGray();
                }
            }
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
                            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ROIImage", image);
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
                if (!originalImage.IsInitialized())
                    image = ((HImage)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIImage", new HImage())).CopyImage();
                else
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
                    if (ContrastValue >= 128)
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
                            "模板行位置：" + row.D.ToString("3f") + "\r\n" +
                            "模板列位置：" + col.D.ToString("3f") + "\r\n" +
                            "模板角度：" + (angle.D * 180.0 / Math.PI).ToString("3f") + "\r\n" +
                            "模板分：" + modelScore.D);
                        break;

                    case "trans":

                        modelSM = (HShapeModel)Variables.CurrentProgramData.Parameters.BingGetOrAdd("Model", null);
                        modelRow = (HTuple)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ModelRow", new HTuple(0));
                        modelCol = (HTuple)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ModelCol", new HTuple(0));
                        modelAngle = (HTuple)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ModelAngle", new HTuple(0));

                        modelImage.FindShapeModel(modelSM, -0.39, 0.79, 0.5, 1, 0.5, "least_squares", 3, 0.5,
                          out row, out col, out angle, out modelScore);

                        hHomMat2D.VectorAngleToRigid(row, col, angle, modelRow, modelCol, modelAngle);
                        resultImage = resultImage.AffineTransImage(hHomMat2D, "constant", "false");

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

        private DelegateCommand _resetPreEnhance;

        public DelegateCommand ResetPreEnhance =>
            _resetPreEnhance ?? (_resetPreEnhance = new DelegateCommand(ExecuteResetPreEnhance));

        private void ExecuteResetPreEnhance()
        {
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

            try
            {
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsPreEnhance", IsPreEnhance);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("BrightnessValue", BrightnessValue);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ContrastValue", ContrastValue);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("GammaValue", GammaValue);
            }
            catch { }
        }

        #endregion 预处理

        #region 二值处理

        private bool isGray;

        public bool IsGray
        {
            get { return isGray; }
            set { SetProperty(ref isGray, value); }
        }

        private DelegateCommand _resetGray;

        public DelegateCommand ResetGray =>
            _resetGray ?? (_resetGray = new DelegateCommand(ExecuteResetGray));

        private void ExecuteResetGray()
        {
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

        private void runGray()
        {
            Variables.ImageWindowDataForFunction.WindowCtrl.DrawMode = HalconDrawing.fill;
            if (!isInitlized)
                return;

            resultRegion.GenEmptyRegion();
            try
            {
                if (IsGray)
                {
                    if (GrayModeIndex == 0)
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

            Variables.ImageWindowDataForFunction.WindowCtrl.ShowImageToWindow(resultImage.CopyImage());
            Variables.ImageWindowDataForFunction.ROICtrl.Clear();
            Variables.ImageWindowDataForFunction.ROICtrl.AddROI(new BingLibrary.Vision.ROIRegion(resultRegion) { ROIColor = BingLibrary.Vision.HalconColors.蓝色 });
            Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();

            try
            {
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsGray", IsGray);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("GrayModeIndex", GrayModeIndex);

                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueS1", ValueS1);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueS2", ValueS2);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueS3", ValueS3);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueS4", ValueS4);

                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueE1", ValueE1);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueE2", ValueE2);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueE3", ValueE3);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ValueE4", ValueE4);

                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsEnable1", IsEnable1);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsEnable2", IsEnable2);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsEnable3", IsEnable3);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsEnable4", IsEnable4);

                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsReverse11", IsReverse1);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsReverse12", IsReverse2);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsReverse13", IsReverse3);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("IsReverse14", IsReverse4);
            }
            catch { }
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
            ExecuteResetGray();
        }

        private DelegateCommand _grayValueChanged;

        public DelegateCommand GrayValueChanged =>
            _grayValueChanged ?? (_grayValueChanged = new DelegateCommand(ExecuteGrayValueChanged));

        private void ExecuteGrayValueChanged()
        {
            runGray();
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

        #region 判定

        private int inspectIndex;

        public int InspectIndex
        {
            get { return inspectIndex; }
            set { SetProperty(ref inspectIndex, value); }
        }

        private DelegateCommand<string> _inspectROIOperate;

        public DelegateCommand<string> InspectROIOperate =>
            _inspectROIOperate ?? (_inspectROIOperate = new DelegateCommand<string>(ExecuteInspectROIOperate));

        private void ExecuteInspectROIOperate(string parameter)
        {
            Variables.ImageWindowDataForFunction.WindowCtrl.DrawMode = HalconDrawing.margin;
            NotDrawIng = false;
            try
            {
                var row1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectROIRow" + parameter + "1", 0.0);
                var row2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectROIRow" + parameter + "2", 0.0);
                var col1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectROIColumn" + parameter + "1", 0.0);
                var col2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectROIColumn" + parameter + "2", 0.0);

                if ((row1 + row2) == 0)
                {
                    Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = true;

                    Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor(HalconColors.橙色.ToDescription());
                    Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.DrawRectangle1(out row1, out col1, out row2, out col2);

                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("InspectROIRow" + parameter + "1", row1);
                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("InspectROIRow" + parameter + "2", row2);
                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("InspectROIColumn" + parameter + "1", col1);
                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("InspectROIColumn" + parameter + "2", col2);

                    Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                    Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = false;
                }
                else
                {
                    Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = true;
                    Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor(HalconColors.橙色.ToDescription());
                    Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.DrawRectangle1Mod(row1, col1, row2, col2, out row1, out col1, out row2, out col2);

                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("InspectROIRow" + parameter + "1", row1);
                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("InspectROIRow" + parameter + "2", row2);
                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("InspectROIColumn" + parameter + "1", col1);
                    Variables.CurrentProgramData.Parameters.BingAddOrUpdate("InspectROIColumn" + parameter + "2", col2);

                    Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                    Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = false;
                }
            }
            catch { }

            Variables.ImageWindowDataForFunction.WindowCtrl.DrawMode = HalconDrawing.fill;
            Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
            NotDrawIng = true;
        }

        private HNCCModel nCCModel = new HNCCModel();
        private DelegateCommand _nccRegiste;

        public DelegateCommand NccRegiste =>
            _nccRegiste ?? (_nccRegiste = new DelegateCommand(ExecuteNccRegiste));

        private void ExecuteNccRegiste()
        {
            try
            {
                var rst = Variables.ShowConfirm("是否注册标准检测？");

                var row1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectROIRow01", 0.0);
                var row2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectROIRow02", 0.0);
                var col1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectROIColumn01", 0.0);
                var col2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectROIColumn02", 0.0);

                HTuple paramterValue = new HTuple();
                HTuple paramterName = resultImage.ReduceDomain(new HRegion(row1, col1, row2, col2)).DetermineShapeModelParams(
                   new HTuple("auto"), -0.39, 0.79, new HTuple(0.9), new HTuple(1.1), "auto",
                   "use_polarity", new HTuple("auto"), new HTuple("auto"), new HTuple("all"), out paramterValue
                    );

                Dictionary<string, HTuple> dict = new Dictionary<string, HTuple>();
                for (int i = 0; i < paramterName.SArr.Length; i++)
                    dict.Add(paramterName.SArr[i], paramterValue[i]);

                nCCModel = resultImage.ReduceDomain(new HRegion(row1, col1, row2, col2)).CreateNccModel(
                       dict["num_levels"], -0.39, 0.79, dict["angle_step"], "use_polarity");

                HTuple nccModelRow = new HTuple(); HTuple nccModelCol = new HTuple(); HTuple nccModelAngle = new HTuple(); HTuple nccModelScore = new HTuple();
                resultImage.FindNccModel(nCCModel, -0.39, 0.79, 0.5, 1, 0.5, "true", 3,
                    out nccModelRow, out nccModelCol, out nccModelAngle, out nccModelScore);

                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("NccModel", nCCModel);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("NccModelRow", nccModelRow);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("NccModelCol", nccModelCol);
                Variables.CurrentProgramData.Parameters.BingAddOrUpdate("NccModelAngle", nccModelAngle);
            }
            catch (Exception ex) { Variables.ShowMessage("注册失败：" + ex.Message); }
        }

        private DelegateCommand<string> _testRun;

        public DelegateCommand<string> TestRun =>
            _testRun ?? (_testRun = new DelegateCommand<string>(ExecuteTestRun));

        private void ExecuteTestRun(string parameter)
        {
            try
            {
                switch (parameter)
                {
                    case "0":

                        var nccModel = (HNCCModel)Variables.CurrentProgramData.Parameters.BingGetOrAdd("NccModel", null);
                        HTuple nccModelRow = new HTuple(); HTuple nccModelCol = new HTuple(); HTuple nccModelAngle = new HTuple(); HTuple nccModelScore = new HTuple();
                        resultImage.FindNccModel(nccModel, -0.39, 0.79, 0.5, 1, 0.5, "true", 3,
                            out nccModelRow, out nccModelCol, out nccModelAngle, out nccModelScore);

                        var row01 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectROIRow01", 0.0);
                        var row02 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectROIRow02", 0.0);
                        var col01 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectROIColumn01", 0.0);
                        var col02 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectROIColumn02", 0.0);

                        Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetDraw("margin");
                        Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor("green");
                        Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.DispRectangle1(row01, col01, row02, col02);
                        Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetDraw("fill");

                        CurrentNCCScore = nccModelScore.D.ToString("f3");
                        break;

                    case "1":

                        var row11 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectROIRow11", 0.0);
                        var row12 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectROIRow12", 0.0);
                        var col11 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectROIColumn11", 0.0);
                        var col12 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectROIColumn12", 0.0);
                        var region = new HRegion(row11, col11, row12, col12).Intersection(resultRegion);

                        Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetDraw("margin");
                        Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor("green");
                        Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.DispRectangle1(row11, col11, row12, col12);
                        Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetDraw("fill");
                        CurrentArea = region.Area.D.ToString();

                        break;
                }
            }
            catch { }
        }

        private string currentNCCScore;

        public string CurrentNCCScore
        {
            get { return currentNCCScore; }
            set { SetProperty(ref currentNCCScore, value); }
        }

        private string currentArea;

        public string CurrentArea
        {
            get { return currentArea; }
            set { SetProperty(ref currentArea, value); }
        }

        //

        #endregion 判定

        public string Title => "图像比对";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
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
            BrightnessValue = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("BrightnessValue", 128.0);
            ContrastValue = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ContrastValue", 128.0);
            GammaValue = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("GammaValue", 1.0);

            IsGray = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsGray", false);

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
            ValueS1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS1", 0).ToString());
            ValueS2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS2", 0).ToString());
            ValueS3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS3", 0).ToString());
            ValueS4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueS4", 0).ToString());
            ValueE1 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE1", 0).ToString());
            ValueE2 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE2", 0).ToString());
            ValueE3 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE3", 0).ToString());
            ValueE4 = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ValueE4", 0).ToString());
            IsEnable1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable1", false);
            IsEnable2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable2", false);
            IsEnable3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable3", false);
            IsEnable4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsEnable4", false);
            IsReverse1 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse11", false);
            IsReverse2 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse12", false);
            IsReverse3 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse13", false);
            IsReverse4 = (bool)Variables.CurrentProgramData.Parameters.BingGetOrAdd("IsReverse14", false);
            //if ((ValueE1 + ValueE2 + ValueE3 + ValueE4) == 0)
            //{
            //    ExecuteResetGray();
            //}

            await Task.Delay(200);
            isInitlized = true;
            return true;
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