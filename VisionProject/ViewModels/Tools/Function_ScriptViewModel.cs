using BingLibrary.Extension;
using BingLibrary.Vision;
using HalconDotNet;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using VisionProject.GlobalVars;

namespace VisionProject.ViewModels
{
    public class Function_ScriptViewModel : BindableBase, IDialogAware, IFunction_ViewModel_Interface
    {
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

            if (oldTabIndex != TabIndex)
            {
                oldTabIndex = TabIndex;
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
                else if (TabIndex == 1 || TabIndex == 2)
                {
                    try
                    {
                        if (IsTrans)
                            ExecuteModelOperate("trans");
                    }
                    catch { }
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
                            image = originalImage.CopyImage();
                        else
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

                        modelImage.FindShapeModel(modelSM, -0.39, 0.79, 0.5, 1, 0.5, "least_squares", 3, 0.5,
                          out row, out col, out angle, out modelScore);

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

        #region 窗口相关

        private string _title = "视觉脚本";

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

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
            try
            {
                EngineNames = new ObservableCollection<string>();
                for (int i = 0; i < Variables.WorkEngines.Count; i++)
                    EngineNames.Add("执行引擎" + i);

                EngineIndex = Variables.CurrentProgramData.Parameters.BingGetOrAdd("EngineIndex", 0).ToString().BingToInt();
                paramDict = (Dictionary<string, ParamSetVar>)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ParamDict", new Dictionary<string, ParamSetVar>());

                if (!paramDict.ContainsKey("ROIRow1"))
                    paramDict.BingAddOrUpdate("ROIRow1", new ParamSetVar() { Name = "ROIRow1", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawRow1.ToString() });
                if (!paramDict.ContainsKey("ROIColumn1"))
                    paramDict.BingAddOrUpdate("ROIColumn1", new ParamSetVar() { Name = "ROIColumn1", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawRow1.ToString() });
                if (!paramDict.ContainsKey("ROIPhi"))
                    paramDict.BingAddOrUpdate("ROIPhi", new ParamSetVar() { Name = "ROIPhi", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawRow1.ToString() });
                if (!paramDict.ContainsKey("DrawLength1"))
                    paramDict.BingAddOrUpdate("DrawLength1", new ParamSetVar() { Name = "DrawLength1", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawRow1.ToString() });
                if (!paramDict.ContainsKey("DrawLength2"))
                    paramDict.BingAddOrUpdate("DrawLength2", new ParamSetVar() { Name = "DrawLength2", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawRow1.ToString() });

                // var paramKeys = paramDict.GetDictParam("keys", new HTuple());
                //获取脚本列表
                ScriptNames = new ObservableCollection<string>();
                var procedureNames = Variables.WorkEngines[EngineIndex].GetProcedureNames();

                for (int i = 0; i < procedureNames.Count; i++)
                {
                    ScriptNames.Add(procedureNames[i]);
                }
                ScriptName = Variables.CurrentProgramData.Parameters.BingGetOrAdd("ScriptName", "").ToString();
            }
            catch { }

            await Task.Delay(200);
            isInitlized = true;
            DoSwitchTab.Execute();
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _ = Init();
        }

        #endregion 窗口相关

        private ObservableCollection<string> _engineNames;

        public ObservableCollection<string> EngineNames
        {
            get { return _engineNames; }
            set { SetProperty(ref _engineNames, value); }
        }

        private int _engineIndex;

        /// <summary>
        /// 引擎索引
        /// </summary>
        public int EngineIndex
        {
            get { return _engineIndex; }
            set { SetProperty(ref _engineIndex, value); }
        }

        private string _scriptName;

        public string ScriptName
        {
            get { return _scriptName; }
            set { SetProperty(ref _scriptName, value); }
        }

        private ObservableCollection<string> _scriptNames = new ObservableCollection<string>();

        public ObservableCollection<string> ScriptNames
        {
            get { return _scriptNames; }
            set { SetProperty(ref _scriptNames, value); }
        }

        private Dictionary<string, ParamSetVar> paramDict = new Dictionary<string, ParamSetVar>();

        private DelegateCommand _saveParam;

        public DelegateCommand SaveParam =>
            _saveParam ?? (_saveParam = new DelegateCommand(ExecuteSaveParam));

        public void ExecuteSaveParam()
        {
            Update();
        }

        private DelegateCommand _openFile;

        public DelegateCommand OpenFile =>
            _openFile ?? (_openFile = new DelegateCommand(ExecuteOpenFile));

        public void ExecuteOpenFile()
        {
            string folderPath = AppDomain.CurrentDomain.BaseDirectory + "Projects\\Scripts0";

            try
            {
                Process.Start(folderPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("无法打开文件夹: " + ex.Message);
            }
        }

        public bool Update()
        {
            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("EngineIndex", EngineIndex);
            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ScriptName", ScriptName);
            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ParamDict", paramDict);
            return true;
        }

        private DelegateCommand _selectedEngine;

        public DelegateCommand SelectedEngine =>
            _selectedEngine ?? (_selectedEngine = new DelegateCommand(ExecuteSelectedEngine));

        /// <summary>
        /// 引擎选择后加载过程
        /// </summary>
        private void ExecuteSelectedEngine()
        {
            //获取脚本列表
            ScriptNames = new ObservableCollection<string>();
            var procedureNames = Variables.WorkEngines[EngineIndex].GetProcedureNames();
            for (int i = 0; i < procedureNames.Count; i++)
                ScriptNames.Add(procedureNames[i]);

            //这里判断，如果当前引擎下未挂在过程则返回。
            if (ScriptNames.Count == 0) return;

            ScriptName = Variables.CurrentProgramData.Parameters.BingGetOrAdd("ScriptName", 0).ToString();

            //脚本册立不清除，因为共用。清除多余的key，仅保留当前设置。
            // Variables.CurrentSubProgram.Parameters.Clear();
            Update();
        }

        private DelegateCommand _selectedScript;

        public DelegateCommand SelectedScript =>
            _selectedScript ?? (_selectedScript = new DelegateCommand(ExecuteSelectedScript));

        private void ExecuteSelectedScript()
        {
            //获取输入变量值
            //  IOValue = Variables.CurrentSubProgram.Parameters.BingGetOrAdd(EngineIndex + "." +
            //  ScriptIndex1 + "." + IOIndex1 + "." + IOVariables1[IOIndex1], "").ToString();
        }

        private DelegateCommand _editScript;

        public DelegateCommand EditScript =>
            _editScript ?? (_editScript = new DelegateCommand(ExecuteEditScript));

        private void ExecuteEditScript()
        {
            try
            {
                //BingLibrary.Vision.ScriptEditPlus scriptEditPlus = new ScriptEditPlus();
                //scriptEditPlus.SetProcedurePath("D:\\Desktop\\HalconTest");

                //scriptEditPlus.CreateNew("ABC");
                //scriptEditPlus.UpdateIO("PA1", "123", ScriptEditPlus.BaseType.InputCtrl, ScriptEditPlus.SemType.String);
                //var rst = scriptEditPlus.GetIO();
                //scriptEditPlus.Save();

                Variables.scriptEdit.SetProcedurePath(AppDomain.CurrentDomain.BaseDirectory + "Projects\\Scripts" + EngineIndex);
                //打开脚本窗口
                ScriptDIalog sd = new ScriptDIalog();
                //读取并显示脚本
                sd.SetCode(Variables.scriptEdit.ReadProcedure(ScriptName));
                sd.ShowDialog();
                //保存脚本
                Variables.scriptEdit.SaveProcedure(sd.GetCode());
                sd.Close();
            }
            catch { }
        }

        private DelegateCommand _paramSet;

        public DelegateCommand ParamSet =>
            _paramSet ?? (_paramSet = new DelegateCommand(ExecuteParamSet));

        private void ExecuteParamSet()
        {
            try
            {
                DialogParameters param = new DialogParameters
                {
                    { "ParamDict", paramDict }
                };

                Variables.CurDialogService.ShowDialog(DialogNames.ShowParamSetDialog, param, callback =>
                {
                    paramDict = callback.Parameters.GetValue<Dictionary<string, ParamSetVar>>("ParamDict");
                });
            }
            catch { }
        }

        //DialogService dialogService = new DialogService(IContainerExtension containerExtension);

        private double DrawRow1 = 0;
        private double DrawColumn1 = 0;
        private double DrawPhi = 0;
        private double DrawLength1 = 0;
        private double DrawLength2 = 0;

        private string _isDrawROIEnabled;

        public string IsDrawROIEnabled
        {
            get { return _isDrawROIEnabled; }
            set { SetProperty(ref _isDrawROIEnabled, value); }
        }

        private DelegateCommand<string> _drawROI;

        public DelegateCommand<string> DrawROI =>
            _drawROI ?? (_drawROI = new DelegateCommand<string>(param => ExecuteDrawROI(param)));

        private void ExecuteDrawROI(string parameter)
        {
            try
            {
                Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();

                switch (parameter)
                {
                    case "roi":
                        if (DrawRow1 == 0 && DrawColumn1 == 0)
                        {
                            Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = true;
                            Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor("green");
                            Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor(HalconColors.橙色.ToDescription());
                            Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.DrawRectangle1(out double r1, out double c1, out double r2, out double c2);
                            DrawRow1 = r1 / 2 + r2 / 2;
                            DrawColumn1 = c1 / 2 + c2 / 2;
                            DrawPhi = 0;
                            DrawLength1 = c2 / 2 - c1 / 2;
                            DrawLength2 = r2 / 2 - r1 / 2;
                            Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = false;

                            HXLDCont hXLD = new HXLDCont();
                            hXLD.GenRectangle2ContourXld(DrawRow1, DrawColumn1, DrawPhi, DrawLength1, DrawLength2);
                            Variables.ImageWindowDataForFunction.DispObjectCtrl.AddDispObjectVar(hXLD);
                            Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
                        }
                        else
                        {
                            Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = true;
                            Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor("green");
                            Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor(HalconColors.橙色.ToDescription());
                            Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.DrawRectangle2Mod(DrawRow1, DrawColumn1, DrawPhi, DrawLength1, DrawLength2, out DrawRow1, out DrawColumn1, out DrawPhi, out DrawLength1, out DrawLength2);

                            Variables.ImageWindowDataForFunction.WindowCtrl.IsDrawing = false;

                            HXLDCont hXLD = new HXLDCont();
                            hXLD.GenRectangle2ContourXld(DrawRow1, DrawColumn1, DrawPhi, DrawLength1, DrawLength2);
                            Variables.ImageWindowDataForFunction.DispObjectCtrl.AddDispObjectVar(hXLD);
                            Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
                        }

                        paramDict.BingAddOrUpdate("ROIRow1", new ParamSetVar() { Name = "ROIRow1", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawRow1.ToString(), Mark = paramDict["ROIRow1"].Mark });
                        paramDict.BingAddOrUpdate("ROIColumn1", new ParamSetVar() { Name = "ROIColumn1", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawColumn1.ToString(), Mark = paramDict["ROIColumn1"].Mark });
                        paramDict.BingAddOrUpdate("ROIPhi", new ParamSetVar() { Name = "ROIPhi", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawPhi.ToString(), Mark = paramDict["ROIPhi"].Mark });
                        paramDict.BingAddOrUpdate("DrawLength1", new ParamSetVar() { Name = "DrawLength1", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawLength1.ToString(), Mark = paramDict["DrawLength1"].Mark });
                        paramDict.BingAddOrUpdate("DrawLength2", new ParamSetVar() { Name = "DrawLength2", SelectedIndex = 2, Type = paramSetType.Double, Value = DrawLength2.ToString(), Mark = paramDict["DrawLength2"].Mark });

                        break;

                    case "del":
                        DrawRow1 = 0;
                        DrawColumn1 = 0;
                        DrawPhi = 0;
                        DrawLength1 = 0;
                        DrawLength2 = 0;
                        break;
                }
            }
            catch { }
        }

        private string _runScriptResult;

        public string RunScriptResult
        {
            get { return _runScriptResult; }
            set { SetProperty(ref _runScriptResult, value); }
        }

        private DelegateCommand _runScript;

        public DelegateCommand RunScript =>
            _runScript ?? (_runScript = new DelegateCommand(ExecuteRunScript));

        private void ExecuteRunScript()
        {
            try
            {
                paramDict = (Dictionary<string, ParamSetVar>)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ParamDict", new Dictionary<string, ParamSetVar>());

                HDict dict = new HDict();
                dict.CreateDict();

                foreach (var kv in paramDict)
                {
                    if (kv.Value.SelectedIndex == 0)
                        dict.SetDictTuple(kv.Value.Name, new HTuple(bool.Parse(kv.Value.Value)));
                    else if (kv.Value.SelectedIndex == 1)
                        dict.SetDictTuple(kv.Value.Name, new HTuple(kv.Value.Value));
                    else if (kv.Value.SelectedIndex == 2)
                        dict.SetDictTuple(kv.Value.Name, new HTuple(double.Parse(kv.Value.Value)));
                    else if (kv.Value.SelectedIndex == 3)
                        dict.SetDictTuple(kv.Value.Name, new HTuple(int.Parse(kv.Value.Value)));
                }

                // 设置图像
                // HImage hImage = new HImage("C:\\Users\\N294\\Pictures\\Camera Roll\\1186626.jpg");
                // dict.SetDictObject(hImage, "Image");

                Variables.WorkEngines[EngineIndex].SetParam(
                     ScriptName, "InputDict", dict);
                bool rst = Variables.WorkEngines[EngineIndex].InspectProcedure(ScriptName);
                //获取结果
                HDict resultDict = Variables.WorkEngines[EngineIndex].GetParam<HalconDotNet.HDict>(ScriptName, "OutputDict");
                //这里约定好对应的输出结果
                RunScriptResult = "输出结果：" + resultDict.GetDictTuple("MessageResult").D.ToString();

                HImage hImage = ConvertHObjectToHImage(resultDict.GetDictObject("ResultImage"));

                Variables.ImageWindowDataForFunction.WindowCtrl.ShowImageToWindow(hImage);
            }
            catch { }
        }

        public HImage ConvertHObjectToHImage(HObject hObject)
        {
            HImage hImage = new HImage();
            try
            {
                HTuple pointer, type, width, height;
                //下面这个语句的PNG即为HObject类型
                HOperatorSet.GetImagePointer1(hObject, out pointer, out type, out width, out height);
                //这样HObject就转成HImage了
                hImage.GenImage1(type, width, height, pointer);
            }
            catch { }
            return hImage;
        }

        public Function_ScriptViewModel()
        {
        }//未将对象引用设置到对象的实例。
    }
}