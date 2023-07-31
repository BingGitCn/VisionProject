using BingLibrary.Extension;
using HalconDotNet;
using System.Collections.Generic;
using VisionProject.GlobalVars;
using VisionProject.ViewModels;

namespace VisionProject.RunTools
{
    public static class Function_ScriptTool
    {
        //programData.Parameters
        public static RunResult Run(HImage image, ProgramData programData)
        {
            #region 参数获得

            var row01 = (double)programData.Parameters.BingGetOrAdd("ROIRow1", 0.0);
            var row02 = (double)programData.Parameters.BingGetOrAdd("ROIRow2", 0.0);
            var col01 = (double)programData.Parameters.BingGetOrAdd("ROIColumn1", 0.0);
            var col02 = (double)programData.Parameters.BingGetOrAdd("ROIColumn2", 0.0);
            var IsTrans = (bool)programData.Parameters.BingGetOrAdd("IsTrans", false);

            var TransBrightnessValue = (double)programData.Parameters.BingGetOrAdd("TransBrightnessValue", 128.0);
            var TransContrastValue = (double)programData.Parameters.BingGetOrAdd("TransContrastValue", 128.0);
            var TransGammaValue = (double)programData.Parameters.BingGetOrAdd("TransGammaValue", 1.0);

            var row1 = (double)programData.Parameters.BingGetOrAdd("ROIRow1", 0.0);
            var row2 = (double)programData.Parameters.BingGetOrAdd("ROIRow2", 0.0);
            var col1 = (double)programData.Parameters.BingGetOrAdd("ROIColumn1", 0.0);
            var col2 = (double)programData.Parameters.BingGetOrAdd("ROIColumn2", 0.0);

            var paramDict = (Dictionary<string, ParamSetVar>)programData.Parameters.BingGetOrAdd("ParamDict", new Dictionary<string, ParamSetVar>());

            var EngineIndex = programData.Parameters.BingGetOrAdd("EngineIndex", 0).ToString().BingToInt();

            var ScriptName = programData.Parameters.BingGetOrAdd("ScriptName", "").ToString();

            #endregion 参数获得

            RunResult runResult = new RunResult();
            // 照片传进来
            HImage runImage = new HImage();

            HImage modelImage = new HImage();
            HRegion resultRegion = new HRegion();
            bool resultBool = true;
            int num = 0;
            try
            {
                //image = ((HImage)programData.Parameters.BingGetOrAdd("ROIImage", new HImage())).CopyImage();
                if (row1 + row2 != 0)
                    runImage = image.CropRectangle1(row1, col1, row2, col2);
                else runImage = image.CopyImage();
            }
            catch { }

            //补正
            try
            {
                if (IsTrans)
                {
                    try
                    {
                        modelImage = runImage.CopyImage();
                        modelImage = modelImage.AddImage(modelImage, 0.5, TransBrightnessValue - 128);

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
                    catch { modelImage = image.CopyImage(); }

                    HTuple modelScore = new HTuple();
                    HTuple row = new HTuple(), col = new HTuple(), angle = new HTuple();
                    HHomMat2D hHomMat2D = new HHomMat2D();
                    var modelSM = (HShapeModel)programData.Parameters.BingGetOrAdd("Model", null);
                    var modelRow = (HTuple)programData.Parameters.BingGetOrAdd("ModelRow", new HTuple(0));
                    var modelCol = (HTuple)programData.Parameters.BingGetOrAdd("ModelCol", new HTuple(0));
                    var modelAngle = (HTuple)programData.Parameters.BingGetOrAdd("ModelAngle", new HTuple(0));
                    //搜索06101143
                    try
                    {
                        modelImage.FindShapeModel(modelSM, -0.39, 0.79, 0.5, 1, 0.5, "least_squares", 3, 0.5,
                          out row, out col, out angle, out modelScore);
                    }
                    catch { }

                    hHomMat2D.VectorAngleToRigid(row, col, angle, modelRow, modelCol, modelAngle);
                    runImage = runImage.AffineTransImage(hHomMat2D, "constant", "false");
                }
            }
            catch { }

            try
            {
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

                //设置图像
                //HImage hImage = new HImage("C:\\Users\\N294\\Pictures\\Camera Roll\\1186626.jpg");
                dict.SetDictObject(runImage, "Image");

                Variables.WorkEngines[EngineIndex].SetParam(
                     ScriptName, "InputDict", dict);
                bool rst = Variables.WorkEngines[EngineIndex].InspectProcedure(ScriptName);
                //获取结果
                HDict resultDict = Variables.WorkEngines[EngineIndex].GetParam<HalconDotNet.HDict>(ScriptName, "OutputDict");
                //这里约定好对应的输出结果
                //每个都要try一下，防止接收结果不对
                try
                {
                    runResult.MessageResult = resultDict.GetDictTuple("MessageResult").D.ToString();//检查一下。。0619
                }
                catch { }
                try
                {
                    resultBool = (bool)(resultDict.GetDictTuple("BoolResult"));
                }
                catch { }
                try
                {
                    runResult.NGImagePath = resultDict.GetDictTuple("NGImagePath").D.ToString();
                }
                catch { }
                Funcs funcs = new Funcs();
                runResult.ResultImage = funcs.ConvertHObjectToHImage(resultDict.GetDictObject("ResultImage"));

                try
                {
                    runResult.RegionResult = new HRegion(resultDict.GetDictObject("RunRegion"));
                }
                catch { }
                try
                {
                    runResult.RunRegion = new HRegion(resultDict.GetDictObject("ResultRegion"));
                }
                catch { }
            }
            catch { }

            runResult.BoolResult = resultBool;
            return runResult;
        }
    }

    public class Funcs
    {
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
    }
}