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

            var TransBrightnessValue = (double)programData.Parameters.BingGetOrAdd("TransBrightnessValue", 128.0);
            var TransContrastValue = (double)programData.Parameters.BingGetOrAdd("TransContrastValue", 128.0);
            var TransGammaValue = (double)programData.Parameters.BingGetOrAdd("TransGammaValue", 1.0);
            var IsPreEnhance = (bool)programData.Parameters.BingGetOrAdd("IsPreEnhance", false);
            var BrightnessValue = (double)programData.Parameters.BingGetOrAdd("BrightnessValue", 128.0);
            var ContrastValue = (double)programData.Parameters.BingGetOrAdd("ContrastValue", 128.0);
            var GammaValue = (double)programData.Parameters.BingGetOrAdd("GammaValue", 1.0);
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

                        var nccModel = (HNCCModel)programData.Parameters.BingGetOrAdd("NccModel", null);//NCC模型
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

                        var inspectRegionPath = programData.Parameters.BingGetOrAdd("InspectRegion", Guid.NewGuid().ToString() + ".reg").ToString();
                        if (File.Exists(Variables.ProjectObjectPath + inspectRegionPath))
                            defaultRegion.ReadRegion(Variables.ProjectObjectPath + inspectRegionPath);
                        var region = defaultRegion.Intersection(resultRegion).Union1();

                        var CurrentArea = region.Area;

                        ResultScoreMin = (double)programData.Parameters.BingGetOrAdd("ResultAreaMin", 100.0);
                        ResultScoreMax = (double)programData.Parameters.BingGetOrAdd("ResultAreaMax", 100.0);

                        if (CurrentArea < ResultScoreMin || CurrentArea > ResultScoreMax)
                            resultBool = false;
                        else { resultBool = true; }
                        runResult.RunRegion = region;
                        if (row1 + row2 != 0)
                            runResult.RegionResult = new HRegion(row1, col1, row2, col2);
                        string content = "";
                        if (programData.Content == null)
                            content = "【未定义缺陷】";
                        else
                        {
                            if (programData.Content == "")
                                content = "【未定义缺陷】";
                            else
                            {
                                content = "【" + programData.Content + "】";
                            }
                        }
                        runResult.MessageResult = content + "面积：" + CurrentArea;

                        break;

                    case "2":
                        var InspectRow1 = (double)programData.Parameters.BingGetOrAdd("InspectRow1", 0.0);
                        var InspectColumn1 = (double)programData.Parameters.BingGetOrAdd("InspectColumn1", 0.0);
                        var InspectRow2 = (double)programData.Parameters.BingGetOrAdd("InspectRow2", 0.0);
                        var InspectColumn2 = (double)programData.Parameters.BingGetOrAdd("InspectColumn2", 0.0);

                        HTuple inputHDict = new HTuple(), outputHDict = new HTuple();
                        HOperatorSet.CreateDict(out inputHDict);
                        HOperatorSet.SetDictTuple(inputHDict, "ROIRow1", new HTuple(InspectRow1));
                        HOperatorSet.SetDictTuple(inputHDict, "ROIRow2", new HTuple(InspectRow2));
                        HOperatorSet.SetDictTuple(inputHDict, "ROIColumn1", new HTuple(InspectColumn1));
                        HOperatorSet.SetDictTuple(inputHDict, "ROIColumn2", new HTuple(InspectColumn2));
                        HOperatorSet.SetDictObject(runImage, inputHDict, "Image");

                        Bing_Metrology(inputHDict, out outputHDict);

                        HTuple result;
                        HOperatorSet.GetDictTuple(outputHDict, "Result", out result);
                        if (result > 0)
                        {
                            HTuple hv_fr, hv_er, hv_fc, hv_ec;
                            HOperatorSet.GetDictTuple(outputHDict, "Row1", out hv_fr);
                            HOperatorSet.GetDictTuple(outputHDict, "Row2", out hv_er);
                            HOperatorSet.GetDictTuple(outputHDict, "Column1", out hv_fc);
                            HOperatorSet.GetDictTuple(outputHDict, "Column2", out hv_ec);

                            HTuple hv_fr_r1, hv_fr_r2, hv_fc_c1, hv_fc_c2;
                            HOperatorSet.GetDictTuple(outputHDict, "fr_r1", out hv_fr_r1);
                            HOperatorSet.GetDictTuple(outputHDict, "fr_r2", out hv_fr_r2);
                            HOperatorSet.GetDictTuple(outputHDict, "fc_c1", out hv_fc_c1);
                            HOperatorSet.GetDictTuple(outputHDict, "fc_c2", out hv_fc_c2);
                            HTuple hv_er_r1, hv_er_r2, hv_ec_c1, hv_ec_c2;
                            HOperatorSet.GetDictTuple(outputHDict, "er_r1", out hv_er_r1);
                            HOperatorSet.GetDictTuple(outputHDict, "er_r2", out hv_er_r2);
                            HOperatorSet.GetDictTuple(outputHDict, "ec_c1", out hv_ec_c1);
                            HOperatorSet.GetDictTuple(outputHDict, "ec_c2", out hv_ec_c2);

                            HRegion hRegion = new HRegion();
                            HRegion hRegionTemp = new HRegion();
                            hRegion.GenRegionLine(hv_fr, hv_fc, hv_er, hv_ec);
                            try
                            {
                                hRegionTemp.GenRegionLine(hv_fr_r1, hv_fc_c1, hv_fr_r2, hv_fc_c2);
                                hRegion = hRegion.Union2(hRegionTemp);
                                hRegionTemp.GenRegionLine(hv_er_r1, hv_ec_c1, hv_er_r2, hv_ec_c2);
                                hRegion = hRegion.Union2(hRegionTemp);
                            }
                            catch { }

                            runResult.RunRegion = hRegion;
                        }
                        var CurrentDistance = result.D;
                        ResultScoreMin = (double)programData.Parameters.BingGetOrAdd("ResultDistanceMin", 100.0);
                        ResultScoreMax = (double)programData.Parameters.BingGetOrAdd("ResultDistanceMax", 100.0);

                        if (CurrentDistance < ResultScoreMin || CurrentDistance > ResultScoreMax)
                            resultBool = false;
                        else { resultBool = true; }

                        if (programData.Content == null)
                            content = "【未定义缺陷】";
                        else
                        {
                            if (programData.Content == "")
                                content = "【未定义缺陷】";
                            else
                            {
                                content = "【" + programData.Content + "】";
                            }
                        }
                        runResult.MessageResult = content + "尺寸：" + CurrentDistance;
                        if (row1 + row2 != 0)
                            runResult.RegionResult = new HRegion(row1, col1, row2, col2);

                        break;
                }
            }
            catch (Exception ex) { resultBool = false; runResult.MessageResult = "报错：" + ex.Message; }

            runResult.BoolResult = resultBool;
            return runResult;
        }

        private static void Bing_Metrology(HTuple hv_InputDict, out HTuple hv_OutputDict)
        {
            // Local iconic variables

            HObject ho_Image;

            // Local control variables

            HTuple hv_allKeys = new HTuple(), hv_ROIRow1 = new HTuple();
            HTuple hv_ROIColumn1 = new HTuple(), hv_ROIRow2 = new HTuple();
            HTuple hv_ROIColumn2 = new HTuple(), hv_Phi = new HTuple();
            HTuple hv_AmplitudeThreshold = new HTuple(), hv_RoiWidthLen2 = new HTuple();
            HTuple hv_TmpCtrl_Row = new HTuple(), hv_TmpCtrl_Column = new HTuple();
            HTuple hv_TmpCtrl_Dr = new HTuple(), hv_TmpCtrl_Dc = new HTuple();
            HTuple hv_TmpCtrl_Phi = new HTuple(), hv_TmpCtrl_Len1 = new HTuple();
            HTuple hv_TmpCtrl_Len2 = new HTuple(), hv_MsrHandle_Measure_01_0 = new HTuple();
            HTuple hv_Row_Measure_01_0 = new HTuple(), hv_Column_Measure_01_0 = new HTuple();
            HTuple hv_Amplitude_Measure_01_0 = new HTuple(), hv_Distance_Measure_01_0 = new HTuple();
            HTuple hv_fr = new HTuple(), hv_fc = new HTuple(), hv_Length = new HTuple();
            HTuple hv_er = new HTuple(), hv_ec = new HTuple(), hv_Distance = new HTuple();
            HTuple hv_fr_r1 = new HTuple(), hv_fc_c1 = new HTuple();
            HTuple hv_fr_r2 = new HTuple(), hv_fc_c2 = new HTuple();
            HTuple hv_er_r1 = new HTuple(), hv_ec_c1 = new HTuple();
            HTuple hv_er_r2 = new HTuple(), hv_ec_c2 = new HTuple();
            // Initialize local and output iconic variables
            HOperatorSet.GenEmptyObj(out ho_Image);
            hv_OutputDict = new HTuple();
            //这里可以约定所有脚本，一个输入字典，一个输出字典
            //字典可以存图像，region，tuple等
            hv_OutputDict.Dispose();
            HOperatorSet.CreateDict(out hv_OutputDict);
            //获取所有key，备用
            hv_allKeys.Dispose();
            HOperatorSet.GetDictParam(hv_InputDict, "keys", new HTuple(), out hv_allKeys);

            //获取对应参数
            hv_ROIRow1.Dispose();
            HOperatorSet.GetDictTuple(hv_InputDict, "ROIRow1", out hv_ROIRow1);
            hv_ROIColumn1.Dispose();
            HOperatorSet.GetDictTuple(hv_InputDict, "ROIColumn1", out hv_ROIColumn1);
            hv_ROIRow2.Dispose();
            HOperatorSet.GetDictTuple(hv_InputDict, "ROIRow2", out hv_ROIRow2);
            hv_ROIColumn2.Dispose();
            HOperatorSet.GetDictTuple(hv_InputDict, "ROIColumn2", out hv_ROIColumn2);

            ho_Image.Dispose();
            HOperatorSet.GetDictObject(out ho_Image, hv_InputDict, "Image");

            hv_Phi.Dispose();
            HOperatorSet.LineOrientation(hv_ROIRow1, hv_ROIColumn1, hv_ROIRow2, hv_ROIColumn2,
                out hv_Phi);

            hv_AmplitudeThreshold.Dispose();
            hv_AmplitudeThreshold = 20;
            hv_RoiWidthLen2.Dispose();
            hv_RoiWidthLen2 = 5;

            hv_TmpCtrl_Row.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TmpCtrl_Row = 0.5 * (hv_ROIRow1 + hv_ROIRow2);
            }
            hv_TmpCtrl_Column.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TmpCtrl_Column = 0.5 * (hv_ROIColumn1 + hv_ROIColumn2);
            }
            hv_TmpCtrl_Dr.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TmpCtrl_Dr = hv_ROIRow1 - hv_ROIRow2;
            }
            hv_TmpCtrl_Dc.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TmpCtrl_Dc = hv_ROIColumn2 - hv_ROIColumn1;
            }
            hv_TmpCtrl_Phi.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TmpCtrl_Phi = hv_TmpCtrl_Dr.TupleAtan2(
                    hv_TmpCtrl_Dc);
            }
            hv_TmpCtrl_Len1.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TmpCtrl_Len1 = 0.5 * ((((hv_TmpCtrl_Dr * hv_TmpCtrl_Dr) + (hv_TmpCtrl_Dc * hv_TmpCtrl_Dc))).TupleSqrt()
                    );
            }
            hv_TmpCtrl_Len2.Dispose();
            hv_TmpCtrl_Len2 = new HTuple(hv_RoiWidthLen2);
            hv_MsrHandle_Measure_01_0.Dispose();
            HTuple w, h;
            HOperatorSet.GetImageSize(ho_Image, out w, out h);
            HOperatorSet.GenMeasureRectangle2(hv_TmpCtrl_Row, hv_TmpCtrl_Column, hv_TmpCtrl_Phi,
                hv_TmpCtrl_Len1, hv_TmpCtrl_Len2, w, h, "nearest_neighbor", out hv_MsrHandle_Measure_01_0);
            hv_Row_Measure_01_0.Dispose(); hv_Column_Measure_01_0.Dispose(); hv_Amplitude_Measure_01_0.Dispose(); hv_Distance_Measure_01_0.Dispose();
            HOperatorSet.MeasurePos(ho_Image, hv_MsrHandle_Measure_01_0, 1, hv_AmplitudeThreshold,
                "all", "all", out hv_Row_Measure_01_0, out hv_Column_Measure_01_0, out hv_Amplitude_Measure_01_0,
                out hv_Distance_Measure_01_0);

            if ((int)(new HTuple((new HTuple(hv_Row_Measure_01_0.TupleLength())).TupleGreaterEqual(
                2))) != 0)
            {
                hv_fr.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_fr = hv_Row_Measure_01_0.TupleSelect(
                        0);
                }
                hv_fc.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_fc = hv_Column_Measure_01_0.TupleSelect(
                        0);
                }

                hv_Length.Dispose();
                HOperatorSet.TupleLength(hv_Row_Measure_01_0, out hv_Length);
                hv_er.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_er = hv_Row_Measure_01_0.TupleSelect(
                        hv_Length - 1);
                }
                hv_ec.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ec = hv_Column_Measure_01_0.TupleSelect(
                        hv_Length - 1);
                }

                hv_Distance.Dispose();
                HOperatorSet.DistancePp(hv_fr, hv_fc, hv_er, hv_ec, out hv_Distance);

                //起点
                hv_fr_r1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_fr_r1 = hv_fr - ((hv_Phi.TupleCos()
                        ) * 20);
                }
                hv_fc_c1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_fc_c1 = hv_fc - ((hv_Phi.TupleSin()
                        ) * 20);
                }
                //终点
                hv_fr_r2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_fr_r2 = hv_fr + ((hv_Phi.TupleCos()
                        ) * 20);
                }
                hv_fc_c2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_fc_c2 = hv_fc + ((hv_Phi.TupleSin()
                        ) * 20);
                }

                //起点
                hv_er_r1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_er_r1 = hv_er - ((hv_Phi.TupleCos()
                        ) * 20);
                }
                hv_ec_c1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ec_c1 = hv_ec - ((hv_Phi.TupleSin()
                        ) * 20);
                }
                //终点
                hv_er_r2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_er_r2 = hv_er + ((hv_Phi.TupleCos()
                        ) * 20);
                }
                hv_ec_c2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ec_c2 = hv_ec + ((hv_Phi.TupleSin()
                        ) * 20);
                }

                HOperatorSet.SetDictTuple(hv_OutputDict, "Result", hv_Distance);

                HOperatorSet.SetDictTuple(hv_OutputDict, "Row1", hv_fr);
                HOperatorSet.SetDictTuple(hv_OutputDict, "Row2", hv_er);
                HOperatorSet.SetDictTuple(hv_OutputDict, "Column1", hv_fc);
                HOperatorSet.SetDictTuple(hv_OutputDict, "Column2", hv_ec);
                // 线段断点
                HOperatorSet.SetDictTuple(hv_OutputDict, "fr_r1", hv_fr_r1);
                HOperatorSet.SetDictTuple(hv_OutputDict, "fr_r2", hv_fr_r2);
                HOperatorSet.SetDictTuple(hv_OutputDict, "fc_c1", hv_fc_c1);
                HOperatorSet.SetDictTuple(hv_OutputDict, "fc_c2", hv_fc_c2);

                HOperatorSet.SetDictTuple(hv_OutputDict, "er_r1", hv_er_r1);
                HOperatorSet.SetDictTuple(hv_OutputDict, "er_r2", hv_er_r2);
                HOperatorSet.SetDictTuple(hv_OutputDict, "ec_c1", hv_ec_c1);
                HOperatorSet.SetDictTuple(hv_OutputDict, "ec_c2", hv_ec_c2);
            }
            else
            {
                HOperatorSet.SetDictTuple(hv_OutputDict, "Result", 0);
            }

            ho_Image.Dispose();

            hv_allKeys.Dispose();
            hv_ROIRow1.Dispose();
            hv_ROIColumn1.Dispose();
            hv_ROIRow2.Dispose();
            hv_ROIColumn2.Dispose();
            hv_Phi.Dispose();
            hv_AmplitudeThreshold.Dispose();
            hv_RoiWidthLen2.Dispose();
            hv_TmpCtrl_Row.Dispose();
            hv_TmpCtrl_Column.Dispose();
            hv_TmpCtrl_Dr.Dispose();
            hv_TmpCtrl_Dc.Dispose();
            hv_TmpCtrl_Phi.Dispose();
            hv_TmpCtrl_Len1.Dispose();
            hv_TmpCtrl_Len2.Dispose();
            hv_MsrHandle_Measure_01_0.Dispose();
            hv_Row_Measure_01_0.Dispose();
            hv_Column_Measure_01_0.Dispose();
            hv_Amplitude_Measure_01_0.Dispose();
            hv_Distance_Measure_01_0.Dispose();
            hv_fr.Dispose();
            hv_fc.Dispose();
            hv_Length.Dispose();
            hv_er.Dispose();
            hv_ec.Dispose();
            hv_Distance.Dispose();
            hv_fr_r1.Dispose();
            hv_fc_c1.Dispose();
            hv_fr_r2.Dispose();
            hv_fc_c2.Dispose();
            hv_er_r1.Dispose();
            hv_ec_c1.Dispose();
            hv_er_r2.Dispose();
            hv_ec_c2.Dispose();

            return;
        }
    }
}