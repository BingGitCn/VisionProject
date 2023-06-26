using BingLibrary.Extension;
using HalconDotNet;
using System;
using System.IO;
using VisionProject.GlobalVars;
using VisionProject.ViewModels;

namespace VisionProject.RunTools
{
    public static class Function_MatchTool
    {
        //programData.Parameters
        public static RunResult Run(HImage image, ProgramData programData)
        {
            #region 参数获得

            var row1 = (double)programData.Parameters.BingGetOrAdd("ROIRow1", 0.0);
            var row2 = (double)programData.Parameters.BingGetOrAdd("ROIRow2", 0.0);
            var col1 = (double)programData.Parameters.BingGetOrAdd("ROIColumn1", 0.0);
            var col2 = (double)programData.Parameters.BingGetOrAdd("ROIColumn2", 0.0);
            var IsTrans = (bool)programData.Parameters.BingGetOrAdd("IsTrans", false);
            var TransBrightnessValue = (double)programData.Parameters.BingGetOrAdd("TransBrightnessValue", 128);
            var TransContrastValue = (double)programData.Parameters.BingGetOrAdd("TransContrastValue", 128);
            var TransGammaValue = (double)programData.Parameters.BingGetOrAdd("TransGammaValue", 1.0);
            var IsPreEnhance = (bool)programData.Parameters.BingGetOrAdd("IsPreEnhance", false);
            var BrightnessValue = (double)programData.Parameters.BingGetOrAdd("BrightnessValue", 128.0);
            var ContrastValue = (double)programData.Parameters.BingGetOrAdd("ContrastValue", 128.0);
            var GammaValue = (double)programData.Parameters.BingGetOrAdd("GammaValue", 1.0);
            var IsSaveNG = (bool)programData.Parameters.BingGetOrAdd("IsSaveNG", false);
            var GrayModeIndex = int.Parse(programData.Parameters.BingGetOrAdd("GrayModeIndex", 0).ToString());

            var ValueS1 = int.Parse(programData.Parameters.BingGetOrAdd("ValueS1", 0).ToString());
            var ValueS2 = int.Parse(programData.Parameters.BingGetOrAdd("ValueS2", 0).ToString());
            var ValueS3 = int.Parse(programData.Parameters.BingGetOrAdd("ValueS3", 0).ToString());
            var ValueS4 = int.Parse(programData.Parameters.BingGetOrAdd("ValueS4", 0).ToString());
            var ValueE1 = int.Parse(programData.Parameters.BingGetOrAdd("ValueE1", 0).ToString());
            var ValueE2 = int.Parse(programData.Parameters.BingGetOrAdd("ValueE2", 0).ToString());
            var ValueE3 = int.Parse(programData.Parameters.BingGetOrAdd("ValueE3", 0).ToString());
            var ValueE4 = int.Parse(programData.Parameters.BingGetOrAdd("ValueE4", 0).ToString());
            var IsEnable1 = (bool)programData.Parameters.BingGetOrAdd("IsEnable1", false);
            var IsEnable2 = (bool)programData.Parameters.BingGetOrAdd("IsEnable2", false);
            var IsEnable3 = (bool)programData.Parameters.BingGetOrAdd("IsEnable3", false);
            var IsEnable4 = (bool)programData.Parameters.BingGetOrAdd("IsEnable4", false);
            var IsReverse1 = (bool)programData.Parameters.BingGetOrAdd("IsReverse11", false);
            var IsReverse2 = (bool)programData.Parameters.BingGetOrAdd("IsReverse12", false);
            var IsReverse3 = (bool)programData.Parameters.BingGetOrAdd("IsReverse13", false);
            var IsReverse4 = (bool)programData.Parameters.BingGetOrAdd("IsReverse14", false);

            var InspectROIRow1 = (double)programData.Parameters.BingGetOrAdd("InspectROIRow1", 0.0);
            var InspectROIRow2 = (double)programData.Parameters.BingGetOrAdd("InspectROIRow2", 0.0);
            var InspectROIColumn1 = (double)programData.Parameters.BingGetOrAdd("InspectROIColumn1", 0.0);
            var InspectROIColumn12 = (double)programData.Parameters.BingGetOrAdd("InspectROIColumn12", 0.0);
            //programData.Parameters.BingAddOrUpdate("NCCScore", 1.0);
            //programData.Parameters.BingAddOrUpdate("ResultScoreMin", 0.1);
            //programData.Parameters.BingAddOrUpdate("ResultScoreMax", 1.0);
            var NCCScore = (double)programData.Parameters.BingGetOrAdd("NCCScore", 0.0);
            var ResultScoreMin = (double)programData.Parameters.BingGetOrAdd("ResultScoreMin", 0.0);
            var ResultScoreMax = (double)programData.Parameters.BingGetOrAdd("ResultScoreMax", 0.0);
            //var MinNum = (double)programData.Parameters.BingGetOrAdd("MinNum", 0.0);
            //var MaxNum = (double)programData.Parameters.BingGetOrAdd("MaxNum", 0.0);
            var InspectIndex = int.Parse(programData.Parameters.BingGetOrAdd("InspectIndex", 0).ToString());

            #endregion 参数获得

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

            //调节（预处理）
            if (IsPreEnhance)
            {
                try
                {
                    {
                        runImage = runImage.AddImage(runImage, 0.5, BrightnessValue - 128);
                        if (ContrastValue >= 128)
                        {
                            double max = 383.0 - ContrastValue;
                            double min = ContrastValue - 128.0;

                            double mult = 255.0 / (max - min);
                            double add = -mult * min;
                            runImage = runImage.ScaleImage(mult, add);
                        }
                        else
                        {
                            double max = 127.0 + ContrastValue;
                            double min = 128.0 - ContrastValue;

                            double mult = (2 * ContrastValue - 1) / 255.0;
                            double add = 128 - ContrastValue;
                            runImage = runImage.ScaleImage(mult, add);
                        }
                        runImage = runImage.GammaImage(GammaValue, 0, 0, 255.0, "true");
                    }
                }
                catch
                {
                }
            }

            //过滤（二值化）
            try
            {
                resultRegion.GenEmptyRegion();
                if (true)
                {
                    if (GrayModeIndex == 0)
                    {
                        if (runImage.CountChannels() == 1)
                        {
                            if (!IsReverse4)
                            {
                                resultRegion = runImage.Threshold((double)ValueS4, (double)ValueE4);
                            }
                            else
                            {
                                resultRegion = runImage.Threshold((new HTuple(0)).TupleConcat(new HTuple(ValueE4)), (new HTuple(ValueS4)).TupleConcat(255));
                                resultRegion = resultRegion.Union1();
                            }
                        }
                        else
                        {
                            HImage image1, image2, image3;
                            image1 = runImage.Decompose3(out image2, out image3);
                            var rstImage1 = image1.AddImage(image1, ValueE1 * 0.5 / 100.0, 0);
                            var rstImage2 = image2.AddImage(image2, ValueE2 * 0.5 / 100.0, 0);
                            var rstImage3 = image3.AddImage(image3, ValueE3 * 0.5 / 100.0, 0);

                            runImage = rstImage1.AddImage(rstImage2, 1.0, 0);
                            runImage = runImage.AddImage(rstImage3, 1.0, 0);

                            if (IsEnable4)
                            {
                                if (!IsReverse4)
                                {
                                    resultRegion = runImage.Threshold((double)ValueS4, (double)ValueE4);
                                }
                                else
                                {
                                    resultRegion = runImage.Threshold((new HTuple(0)).TupleConcat(new HTuple(ValueE4)), (new HTuple(ValueS4)).TupleConcat(255));
                                }
                            }
                        }
                    }
                    else if (GrayModeIndex == 1)
                    {
                        if (runImage.CountChannels() == 1)
                        {
                            if (!IsReverse4)
                            {
                                resultRegion = runImage.Threshold((double)ValueS4, (double)ValueE4);
                            }
                            else
                            {
                                resultRegion = runImage.Threshold((new HTuple(0)).TupleConcat(new HTuple(ValueE4)), (new HTuple(ValueS4)).TupleConcat(255));
                                resultRegion = resultRegion.Union1();
                            }
                        }
                        else
                        {
                            HImage image1, image2, image3;
                            image1 = runImage.Decompose3(out image2, out image3);

                            HRegion resultRegion1 = new HRegion(); resultRegion1.GenEmptyRegion();
                            HRegion resultRegion2 = new HRegion(); resultRegion2.GenEmptyRegion();
                            HRegion resultRegion3 = new HRegion(); resultRegion3.GenEmptyRegion();

                            runImage = image1.Compose3(image2, image3);
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
                        if (runImage.CountChannels() == 1)
                        {
                            if (!IsReverse4)
                            {
                                resultRegion = runImage.Threshold((double)ValueS4, (double)ValueE4);
                            }
                            else
                            {
                                resultRegion = runImage.Threshold((new HTuple(0)).TupleConcat(new HTuple(ValueE4)), (new HTuple(ValueS4)).TupleConcat(255));
                                resultRegion = resultRegion.Union1();
                            }
                        }
                        else
                        {
                            HImage image1, image2, image3;
                            image1 = runImage.Decompose3(out image2, out image3);
                            HImage tImage1, tImage2, tImage3;
                            tImage1 = image1.TransFromRgb(image2, image3, out tImage2, out tImage3, "hsv");

                            HRegion resultRegion1 = new HRegion(); resultRegion1.GenEmptyRegion();
                            HRegion resultRegion2 = new HRegion(); resultRegion2.GenEmptyRegion();
                            HRegion resultRegion3 = new HRegion(); resultRegion3.GenEmptyRegion();

                            runImage = image1.Compose3(image2, image3);

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

            RunResult runResult = new RunResult();
            //判定
            try
            {
                switch (InspectIndex.ToString())
                {
                    case "0":

                        var nccModel = (HNCCModel)Variables.CurrentProgramData.Parameters.BingGetOrAdd("NccModel", null);
                        HTuple nccModelRow = new HTuple(); HTuple nccModelCol = new HTuple(); HTuple nccModelAngle = new HTuple(); HTuple nccModelScore = new HTuple();
                        runImage.FindNccModel(nccModel, -0.39, 0.79, 0.5, 1, 0.5, "true", 4,
                            out nccModelRow, out nccModelCol, out nccModelAngle, out nccModelScore);

                        if (nccModelScore < NCCScore)
                            resultBool = false;
                        else { resultBool = true; }
                        if (row1 + row2 != 0)
                            runResult.RegionResult = new HRegion(row1, col1, row2, col2);
                        runResult.MessageResult = "得分：" + nccModelScore;
                        break;

                    case "1":

                        var defaultRegion = new HRegion();
                        defaultRegion.GenEmptyRegion();

                        var inspectRegionPath = Variables.CurrentProgramData.Parameters.BingGetOrAdd("InspectRegion", Variables.ProjectObjectPath + Guid.NewGuid().ToString() + ".reg").ToString();
                        if (File.Exists(inspectRegionPath))
                            defaultRegion.ReadRegion(inspectRegionPath);
                        var region = defaultRegion.Intersection(resultRegion).Union1();

                        var CurrentArea = region.Area;
                        if (CurrentArea < ResultScoreMin || CurrentArea > ResultScoreMax)
                            resultBool = false;
                        else { resultBool = true; }
                        runResult.RunRegion = region;
                        if (row1 + row2 != 0)
                            runResult.RegionResult = new HRegion(row1, col1, row2, col2);
                        runResult.MessageResult = "面积：" + CurrentArea;

                        break;
                }
            }
            catch (Exception ex) { resultBool = false; runResult.MessageResult = "报错：" + ex.Message; }

            runResult.BoolResult = resultBool;
            return runResult;
        }
    }
}