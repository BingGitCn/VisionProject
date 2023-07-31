using BingLibrary.Extension;
using HalconDotNet;
using ImageResizer.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using VisionProject.GlobalVars;
using VisionProject.ViewModels;
using static VisionProject.ViewModels.Function_PINOneViewModel;

namespace VisionProject.RunTools
{
    public static class Function_PINOneTool
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

            var IsEnableDeep = (bool)programData.Parameters.BingGetOrAdd("IsEnableDeep", false);

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

            var DisposeROIRow1 = (double)programData.Parameters.BingGetOrAdd("DisposeROIRow1", 0.0);
            var DisposeROIRow2 = (double)programData.Parameters.BingGetOrAdd("DisposeROIRow2", 0.0);
            var DisposeROIColumn1 = (double)programData.Parameters.BingGetOrAdd("DisposeROIColumn1", 0.0);
            var DisposeROIColumn2 = (double)programData.Parameters.BingGetOrAdd("DisposeROIColumn2", 0.0);
            var RegionFoundFeatureDatas = (ObservableCollection<RegionFoundFeatureData>)programData.Parameters.BingGetOrAdd("RegionFoundFeatureDatas", new ObservableCollection<RegionFoundFeatureData>());
            var BlobFeatureDatas = (ObservableCollection<BlobFeatureData>)programData.Parameters.BingGetOrAdd("BlobFeatureDatas", new ObservableCollection<BlobFeatureData>());
            var FilterModeIndex = int.Parse(programData.Parameters.BingGetOrAdd("FilterModeIndex", 0).ToString());
            var BlobFeatureDataIndex = int.Parse(programData.Parameters.BingGetOrAdd("BlobFeatureDataIndex", 0).ToString());

            var InspectROIRow1 = (double)programData.Parameters.BingGetOrAdd("InspectROIRow1", 0.0);
            var InspectROIRow2 = (double)programData.Parameters.BingGetOrAdd("InspectROIRow2", 0.0);
            var InspectROIColumn1 = (double)programData.Parameters.BingGetOrAdd("InspectROIColumn1", 0.0);
            var InspectROIColumn12 = (double)programData.Parameters.BingGetOrAdd("InspectROIColumn2", 0.0);
            var AllowOffset = (double)(programData.Parameters.BingGetOrAdd("AllowOffset", 0.0));
            var Scale = int.Parse(programData.Parameters.BingGetOrAdd("Scale", 1).ToString());

            #endregion 参数获得

            // 照片传进来
            HImage runImage = new HImage();
            HImage modelImage = new HImage();
            HImage ImageTemp = new HImage();
            HRegion resultRegion = new HRegion();
            bool resultBool = true;
            int num = 0;
            try
            {
                //image = ((HImage)programData.Parameters.BingGetOrAdd("ROIImage", new HImage())).CopyImage();
                if (row1 + row2 != 0)
                    runImage = image.CropRectangle1(row1, col1, row2, col2);
                else runImage = image.CopyImage();
                ImageTemp = runImage.CopyImage();
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
            catch (Exception ex) { }

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
                    if (IsEnableDeep)
                    {
                        resultRegion = Variables.PinDeep.ApplyModelInSegmentation(runImage, new int[] { 1 }, true).Threshold(2.0, 2.0);
                    }
                    else if (GrayModeIndex == 0)
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
                        if (image.CountChannels() == 1)
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

            //处理
            try
            {
                HRegion region = resultRegion.Connection();

                //var w = row2 - row1; var h = col2 - col1;
                if (DisposeROIRow1 + DisposeROIRow2 != 0)
                    region = new HRegion(DisposeROIRow1, DisposeROIColumn1, DisposeROIRow2, DisposeROIColumn2).Intersection(region);
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
                    for (int i = 0; i < RegionFoundFeatureDatas.Count; i++)
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
            }
            catch { }

            //提取
            try
            {
                Dictionary<string, List<double>> blobRess = new Dictionary<string, List<double>>();
                HRegion region = resultRegion.Connection();
                HRegion regions = new HRegion(region);
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
                    resultRegion = regions.Union1().Connection();
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
                    // num = regions.Connection().CountObj();
                }
            }
            catch { }

            RunResult runResult = new RunResult();
            //判定
            try
            {
                var defaultRegion = new HRegion();
                defaultRegion.GenEmptyRegion();
                // RowAllowOffset
                var InspectFrames = (List<InspectFrame>)programData.Parameters.BingGetOrAdd("InspectFrames", new List<InspectFrame>());
                runResult.ResultImage = ImageTemp;
                runResult.MessageResult = "PIN针检测\n";
                HRegion hRegion = new HRegion();
                hRegion.GenEmptyRegion();

                List<double> distances = new List<double>();
                for (int i = 0; i < InspectFrames.Count(); i++)
                {
                    defaultRegion.GenRectangle1(InspectFrames[i].Row1, InspectFrames[i].Col1, InspectFrames[i].Row2, InspectFrames[i].Col2);

                    defaultRegion = defaultRegion.Intersection(resultRegion);
                    double col = 0, row = 0;
                    defaultRegion.AreaCenter(out row, out col);
                    Vector2 point1 = new Vector2((float)row, (float)col);
                    Vector2 point2 = new Vector2((float)InspectFrames[i].CrossRow, (float)InspectFrames[i].CrossCol);

                    double distance = Vector2.Distance(point1, point2);
                    distances.Add(distance);
                }

                var minDistance = distances.Average();
                for (int i = 0; i < distances.Count; i++)
                {
                    distances[i] = Math.Abs(distances[i] - minDistance);
                }

                for (int i = 0; i < InspectFrames.Count(); i++)
                {
                    try
                    {
                        defaultRegion.GenRectangle1(InspectFrames[i].Row1, InspectFrames[i].Col1, InspectFrames[i].Row2, InspectFrames[i].Col2);

                        defaultRegion = defaultRegion.Intersection(resultRegion);
                        double col = 0, row = 0;
                        defaultRegion.AreaCenter(out row, out col);

                        if (distances[i] > AllowOffset * Scale)
                        {
                            if (InspectFrames[i].ContainsObject)
                            {
                                resultBool = false;
                            }
                        }
                        else
                        {
                            //runResult.BoolResult = true;
                            hRegion = hRegion.Union2(defaultRegion.Intersection(resultRegion));
                        }
                        int j = i + 1;
                        if (col == 0 && row == 0)
                        {
                            runResult.MessageResult += "缺失" + "\n";
                        }
                        else
                            runResult.MessageResult += (distances[i] / Scale).ToString("N3") + "\n";
                    }
                    catch
                    {
                        runResult.MessageResult += "缺失" + "\n";
                    }
                }

                runResult.RunRegion = hRegion.Union1();

                if (row1 + row2 != 0)
                    runResult.RegionResult = new HRegion(row1, col1, row2, col2);
            }
            catch (Exception ex) { resultBool = false; runResult.MessageResult = "报错：" + ex.Message; }

            runResult.BoolResult = resultBool;
            //runResult.NGImagePath = "";
            return runResult;
        }
    }
}