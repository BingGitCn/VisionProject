using BingLibrary.Extension;
using HalconDotNet;
using System;
using VisionProject.GlobalVars;
using VisionProject.ViewModels;

namespace VisionProject.RunTools
{
    public static class Function_CodeTool
    {
        //programData.Parameters
        public static RunResult Run(HImage image, ProgramData programData)
        {
            #region 参数获得

            var row1 = (double)programData.Parameters.BingGetOrAdd("ROIRow1", 0.0);
            var row2 = (double)programData.Parameters.BingGetOrAdd("ROIRow2", 0.0);
            var col1 = (double)programData.Parameters.BingGetOrAdd("ROIColumn1", 0.0);
            var col2 = (double)programData.Parameters.BingGetOrAdd("ROIColumn2", 0.0);

            var ColorIndex = int.Parse(programData.Parameters.BingGetOrAdd("ColorIndex", 0).ToString());
            var CodeIndex = int.Parse(programData.Parameters.BingGetOrAdd("CodeIndex", 0).ToString());
            var MinResultNumber = int.Parse(programData.Parameters.BingGetOrAdd("MinResultNumber", 0).ToString());

            #endregion 参数获得

            // 照片传进来
            bool resultBool = true;
            int resultLength = 0;
            try
            {
                //image = ((HImage)programData.Parameters.BingGetOrAdd("ROIImage", new HImage())).CopyImage();
                if (row1 + row2 != 0)
                    image = image.CropRectangle1(row1, col1, row2, col2);
            }
            catch { }

            RunResult runResult = new RunResult();
            try
            {
                HXLDCont inspectXLD = new HXLDCont();
                HTuple hTuple2 = new HTuple();
                HDataCode2D hDataCode2D = new HDataCode2D();
                HRegion hRegion = new HRegion();

                if (ColorIndex == 1)
                    image = image.InvertImage();

                if (CodeIndex == 0)
                {
                    HBarCode hBarCode = new HBarCode();
                    hBarCode.CreateBarCodeModel("quiet_zone", "true");
                    string resultString;
                    hRegion = hBarCode.FindBarCode(image, "auto", out resultString);
                    inspectXLD = hRegion.GenContourRegionXld("border");
                    runResult.MessageResult = "条码：" + resultString;
                    resultLength = resultString.Length;
                }
                else if (CodeIndex == 1)
                {
                    hDataCode2D.CreateDataCode2dModel("Data Matrix ECC 200", "default_parameters", "enhanced_recognition");
                    inspectXLD = hDataCode2D.FindDataCode2d(image, new HTuple(), new HTuple(), out HTuple hTuple1, out hTuple2);
                    runResult.MessageResult = "条码：" + (string)hTuple2;
                    resultLength = ((string)hTuple2).Length;
                }
                else if (CodeIndex == 2)
                {
                    hDataCode2D.CreateDataCode2dModel("QR Code", "default_parameters", "enhanced_recognition");
                    inspectXLD = hDataCode2D.FindDataCode2d(image, new HTuple(), new HTuple(), out HTuple hTuple1, out hTuple2);

                    runResult.MessageResult = "条码：" + hTuple2.S;
                    resultLength = (hTuple2.S).Length;
                }
                if (resultLength < MinResultNumber)
                    runResult.BoolResult = false;
                else runResult.BoolResult = true;
                runResult.RunRegion = inspectXLD.GenRegionContourXld("filled");
            }
            catch (Exception ex)
            {
                runResult.MessageResult = "报错：" + ex.Message;
                resultBool = false;
            }

            runResult.BoolResult = resultBool;
            //runResult.NGImagePath = "";
            return runResult;
        }
    }
}