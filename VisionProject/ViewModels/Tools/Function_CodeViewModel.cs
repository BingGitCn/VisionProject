using BingLibrary.Extension;
using BingLibrary.Vision;
using HalconDotNet;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.IO;
using System.Threading.Tasks;
using VisionProject.GlobalVars;
using VisionProject.RunTools;

namespace VisionProject.ViewModels
{
    public class Function_CodeViewModel : BindableBase, IDialogAware, IFunction_ViewModel_Interface
    {
        public Function_CodeViewModel()
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
                    // runCode();
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
                        { }
                        else
                        {
                            image = originalImage.CropRectangle1((int)row01, (int)col01, (int)row02, (int)col02);
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

        #region 条码识别

        public bool boolResult = true;

        private DelegateCommand _testRun;

        public DelegateCommand TestRun =>
            _testRun ?? (_testRun = new DelegateCommand(ExecuteTestRun));

        private void ExecuteTestRun()
        {
            try
            {
                var rst = Function_CodeTool.Run(originalImage, Variables.CurrentProgramData);

                var row1 = (int)((double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow1", 0.0));
                var col1 = (int)((double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn1", 0.0));
                IdentifyResult = rst.MessageResult;
                var region = rst.RunRegion;
                Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                Variables.ImageWindowDataForFunction.ROICtrl.Clear();
                Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetDraw("margin");
                Variables.ImageWindowDataForFunction.WindowCtrl.hWindowControlWPF.HalconWindow.SetColor("green");
                Variables.ImageWindowDataForFunction.DispObjectCtrl.AddDispObjectVar(region);
                Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
            }
            catch { }

            // runCode();
        }

        private DelegateCommand _save;

        public DelegateCommand Save =>
            _save ?? (_save = new DelegateCommand(ExecuteSave));

        private void ExecuteSave()
        {
            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("MinResultNumber", MinResultNumber);

            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("CodeIndex", CodeIndex);
            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ColorIndex", ColorIndex);
        }

        private void runCode()
        {
            HXLDCont inspectXLD = new HXLDCont();
            HTuple hTuple2 = new HTuple();
            HDataCode2D hDataCode2D = new HDataCode2D();
            HRegion hRegion = new HRegion();
            try
            {
                var row1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow1", 0.0);
                var row2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIRow2", 0.0);
                var col1 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn1", 0.0);
                var col2 = (double)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIColumn2", 0.0);

                if ((row1 + row2) == 0)
                {
                    image = originalImage;
                }
                else
                {
                    image = originalImage.CropRectangle1(row1, col1, row2, col2);
                }

                if (ColorIndex == 1)
                    image = image.InvertImage();

                if (CodeIndex == 0)
                {
                    HBarCode hBarCode = new HBarCode();
                    hBarCode.CreateBarCodeModel("quiet_zone", "true");
                    string resultString;
                    hRegion = hBarCode.FindBarCode(image, "auto", out resultString);
                    inspectXLD = hRegion.GenContourRegionXld("border");
                    IdentifyResult = resultString;
                }
                else if (CodeIndex == 1)
                {
                    hDataCode2D.CreateDataCode2dModel("Data Matrix ECC 200", "default_parameters", "enhanced_recognition");
                    inspectXLD = hDataCode2D.FindDataCode2d(image, new HTuple(), new HTuple(), out HTuple hTuple1, out hTuple2);
                    IdentifyResult = (string)hTuple2;
                }
                else if (CodeIndex == 2)
                {
                    hDataCode2D.CreateDataCode2dModel("QR Code", "default_parameters", "enhanced_recognition");
                    inspectXLD = hDataCode2D.FindDataCode2d(image, new HTuple(), new HTuple(), out HTuple hTuple1, out hTuple2);
                    IdentifyResult = (string)hTuple2;
                }
                Variables.ImageWindowDataForFunction.ROICtrl.Clear();
                Variables.ImageWindowDataForFunction.DispObjectCtrl.AddDispObjectVar(inspectXLD, HalconColors.海军蓝色七五成);
                Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
            }
            catch
            {
                IdentifyResult = "查找失败，请选择合适的图片或条码类型";
                boolResult = false;
            }

            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("CodeIndex", CodeIndex);
            Variables.CurrentProgramData.Parameters.BingAddOrUpdate("ColorIndex", ColorIndex);
        }

        private int _colorIndex;

        public int ColorIndex
        {
            get { return _colorIndex; }
            set { SetProperty(ref _colorIndex, value); }
        }

        private int _codeIndex;

        public int CodeIndex
        {
            get { return _codeIndex; }
            set { SetProperty(ref _codeIndex, value); }
        }

        private string _identifyResult;

        public string IdentifyResult
        {
            get { return _identifyResult; }
            set { SetProperty(ref _identifyResult, value); }
        }

        private int _minResultNumber;

        public int MinResultNumber
        {
            get { return _minResultNumber; }
            set { SetProperty(ref _minResultNumber, value); }
        }

        #endregion 条码识别

        public string Title => "条码识别";

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
            ColorIndex = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("ColorIndex", 0).ToString());
            CodeIndex = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("CodeIndex", 0).ToString());
            MinResultNumber = int.Parse(Variables.CurrentProgramData.Parameters.BingGetOrAdd("MinResultNumber", 0).ToString());
            try
            {
                ///0729 疑问，这里的图片初始化是指的什么？
                if (!originalImage.IsInitialized())
                {
                    string imagePath = "";
                    imagePath = Variables.CurrentProgramData.Parameters.BingGetOrAdd("ROIImage", Guid.NewGuid().ToString()).ToString();

                    image = new HImage(Variables.ProjectImagesPath + imagePath + ".bmp");
                }

                Variables.ImageWindowDataForFunction.DispObjectCtrl.ClearDispObjects();
                Variables.ImageWindowDataForFunction.ROICtrl.Clear();
                Variables.ImageWindowDataForFunction.WindowCtrl.ShowImageToWindow(image);
                Variables.ImageWindowDataForFunction.WindowCtrl.Repaint();
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

        public bool Update()
        {
            return true;
        }
    }
}