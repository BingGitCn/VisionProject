using HalconDotNet;

namespace VisionProject.Core
{
    public class InspectCY
    {
        public HTuple hv_ExpDefaultWinHandle;

        public void display_feature_maps(HObject ho_Image, HTuple hv_ChosenLayers, HTuple hv_LayerIndex,
            HTuple hv_ImageIndex, HTuple hv_LayerTypes, HTuple hv_SelectedFeatureMaps, HTuple hv_SelectedFeatureMapDepths,
            HTuple hv_WindowDict)
        {
            // Local iconic variables

            HObject ho_SelectedFeatureMapsObj, ho_FeatureMap1;
            HObject ho_FeatureMap2, ho_FeatureMap3;

            // Local control variables

            HTuple hv_ClassNames = new HTuple(), hv_WindowHandle1 = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_Text = new HTuple(), hv_SelectedChannels = new HTuple();
            HTuple hv_SelectedDepthsOfLayer = new HTuple(), hv_WindowHandle2 = new HTuple();
            HTuple hv_LineColors = new HTuple(), hv_WindowHandle3 = new HTuple();
            HTuple hv_WindowHandle4 = new HTuple();
            // Initialize local and output iconic variables
            HOperatorSet.GenEmptyObj(out ho_SelectedFeatureMapsObj);
            HOperatorSet.GenEmptyObj(out ho_FeatureMap1);
            HOperatorSet.GenEmptyObj(out ho_FeatureMap2);
            HOperatorSet.GenEmptyObj(out ho_FeatureMap3);
            try
            {
                //This procedure displays the inferred image and the selected
                //feature maps.

                //For visualization purpose set class names.
                hv_ClassNames.Dispose();
                hv_ClassNames = new HTuple();
                hv_ClassNames[0] = "good";
                hv_ClassNames[1] = "crack";
                hv_ClassNames[2] = "contamination";

                //Show original image.
                //dev_open_window(...);
                HOperatorSet.SetDictTuple(hv_WindowDict, "original_image", hv_WindowHandle1);
                hv_Row.Dispose(); hv_Column.Dispose(); hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetWindowExtents(hv_ExpDefaultWinHandle, out hv_Row, out hv_Column,
                    out hv_Width, out hv_Height);
                HOperatorSet.DispObj(ho_Image, hv_ExpDefaultWinHandle);
                hv_Text.Dispose();
                hv_Text = "Original image";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = " ";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "Class: " + (hv_ClassNames.TupleSelect(
                    hv_ImageIndex));
                HOperatorSet.DispText(hv_ExpDefaultWinHandle, hv_Text, "window", "top", "left",
                    ((new HTuple("red")).TupleConcat("black")).TupleConcat("black"), new HTuple(),
                    new HTuple());

                //Get the depths of the selected layer.
                hv_SelectedChannels.Dispose();
                HOperatorSet.GetDictParam(hv_SelectedFeatureMapDepths, "keys", new HTuple(),
                    out hv_SelectedChannels);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SelectedDepthsOfLayer.Dispose();
                    HOperatorSet.GetDictTuple(hv_SelectedFeatureMapDepths, hv_SelectedChannels.TupleSelect(
                        hv_LayerIndex), out hv_SelectedDepthsOfLayer);
                }

                //Show feature maps.
                //dev_open_window(...);
                HOperatorSet.SetDictTuple(hv_WindowDict, "feature_map_1", hv_WindowHandle2);
                //Get image objects from dict.
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_SelectedFeatureMapsObj.Dispose();
                    HOperatorSet.GetDictObject(out ho_SelectedFeatureMapsObj, hv_SelectedFeatureMaps,
                        hv_ChosenLayers.TupleSelect(hv_LayerIndex));
                }
                //Get feature map or selected layer.
                ho_FeatureMap1.Dispose();
                HOperatorSet.SelectObj(ho_SelectedFeatureMapsObj, out ho_FeatureMap1, 1);
                HOperatorSet.DispObj(ho_FeatureMap1, hv_ExpDefaultWinHandle);
                hv_Text.Dispose();
                hv_Text = "Feature Map ";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = " ";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = ("Selected layer: '" + (hv_ChosenLayers.TupleSelect(
                    hv_LayerIndex))) + "' ";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = ("Type of layer: '" + (hv_LayerTypes.TupleSelect(
                    hv_LayerIndex))) + "' ";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "Selected depth: " + (hv_SelectedDepthsOfLayer.TupleSelect(
                    0));

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_LineColors.Dispose();
                    HOperatorSet.TupleGenConst(new HTuple(hv_Text.TupleLength()), "white", out hv_LineColors);
                }
                if (hv_LineColors == null)
                    hv_LineColors = new HTuple();
                hv_LineColors[0] = "red";
                if (hv_LineColors == null)
                    hv_LineColors = new HTuple();
                hv_LineColors[HTuple.TupleGenSequence(1, 6, 1)] = "black";
                HOperatorSet.DispText(hv_ExpDefaultWinHandle, hv_Text, "window", "top", "left",
                    hv_LineColors, new HTuple(), new HTuple());

                //dev_open_window(...);
                HOperatorSet.SetDictTuple(hv_WindowDict, "feature_map_2", hv_WindowHandle3);
                //Get feature map or selected layer.
                ho_FeatureMap2.Dispose();
                HOperatorSet.SelectObj(ho_SelectedFeatureMapsObj, out ho_FeatureMap2, 2);
                HOperatorSet.DispObj(ho_FeatureMap2, hv_ExpDefaultWinHandle);
                hv_Text.Dispose();
                hv_Text = "Feature Map ";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = " ";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = ("Selected layer: '" + (hv_ChosenLayers.TupleSelect(
                    hv_LayerIndex))) + "' ";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = ("Type of layer: '" + (hv_LayerTypes.TupleSelect(
                    hv_LayerIndex))) + "' ";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "Selected depth: " + (hv_SelectedDepthsOfLayer.TupleSelect(
                    1));
                HOperatorSet.DispText(hv_ExpDefaultWinHandle, hv_Text, "window", "top", "left",
                    hv_LineColors, new HTuple(), new HTuple());

                //dev_open_window(...);
                HOperatorSet.SetDictTuple(hv_WindowDict, "feature_map_3", hv_WindowHandle4);
                //Get feature map or selected layer.
                ho_FeatureMap3.Dispose();
                HOperatorSet.SelectObj(ho_SelectedFeatureMapsObj, out ho_FeatureMap3, 3);
                HOperatorSet.DispObj(ho_FeatureMap3, hv_ExpDefaultWinHandle);
                hv_Text.Dispose();
                hv_Text = "Feature Map ";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = " ";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = ("Selected layer: '" + (hv_ChosenLayers.TupleSelect(
                    hv_LayerIndex))) + "' ";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = ("Type of layer: '" + (hv_LayerTypes.TupleSelect(
                    hv_LayerIndex))) + "' ";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "Selected depth: " + (hv_SelectedDepthsOfLayer.TupleSelect(
                    2));
                HOperatorSet.DispText(hv_ExpDefaultWinHandle, hv_Text, "window", "top", "left",
                    hv_LineColors, new HTuple(), new HTuple());
                if ((int)((new HTuple(hv_LayerIndex.TupleEqual(1))).TupleAnd(new HTuple(hv_ImageIndex.TupleEqual(
                    2)))) != 0)
                {
                    HOperatorSet.DispText(hv_ExpDefaultWinHandle, "End of the example", "window",
                        "bottom", "right", "black", new HTuple(), new HTuple());
                }
                else
                {
                    HOperatorSet.DispText(hv_ExpDefaultWinHandle, "Press F5 to continue", "window",
                        "bottom", "right", "black", new HTuple(), new HTuple());
                }
                ho_SelectedFeatureMapsObj.Dispose();
                ho_FeatureMap1.Dispose();
                ho_FeatureMap2.Dispose();
                ho_FeatureMap3.Dispose();

                hv_ClassNames.Dispose();
                hv_WindowHandle1.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Text.Dispose();
                hv_SelectedChannels.Dispose();
                hv_SelectedDepthsOfLayer.Dispose();
                hv_WindowHandle2.Dispose();
                hv_LineColors.Dispose();
                hv_WindowHandle3.Dispose();
                hv_WindowHandle4.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_SelectedFeatureMapsObj.Dispose();
                ho_FeatureMap1.Dispose();
                ho_FeatureMap2.Dispose();
                ho_FeatureMap3.Dispose();

                hv_ClassNames.Dispose();
                hv_WindowHandle1.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Text.Dispose();
                hv_SelectedChannels.Dispose();
                hv_SelectedDepthsOfLayer.Dispose();
                hv_WindowHandle2.Dispose();
                hv_LineColors.Dispose();
                hv_WindowHandle3.Dispose();
                hv_WindowHandle4.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Short Description: Get the single feature maps of a selected model layer.
        public void get_feature_maps(HObject ho_SampleFeatureMaps, HTuple hv_SelectedFeatureMapDepths,
            HTuple hv_LayerIndex, out HTuple hv_SelectedFeatureMaps)
        {
            // Stack for temporary objects
            HObject[] OTemp = new HObject[20];

            // Local iconic variables

            HObject ho_SingleFeatureMaps, ho_SingleFeatureMap = null;

            // Local control variables

            HTuple hv_NumChannels = new HTuple(), hv_SelectedChannels = new HTuple();
            HTuple hv_SelectedDepthsOfLayer = new HTuple(), hv_Idx = new HTuple();
            // Initialize local and output iconic variables
            HOperatorSet.GenEmptyObj(out ho_SingleFeatureMaps);
            HOperatorSet.GenEmptyObj(out ho_SingleFeatureMap);
            hv_SelectedFeatureMaps = new HTuple();
            try
            {
                //This procedure gets the single feature maps of the selected layer and
                //saves them in a dictionary.

                hv_NumChannels.Dispose();
                HOperatorSet.CountChannels(ho_SampleFeatureMaps, out hv_NumChannels);
                hv_SelectedChannels.Dispose();
                HOperatorSet.GetDictParam(hv_SelectedFeatureMapDepths, "keys", new HTuple(),
                    out hv_SelectedChannels);

                hv_SelectedFeatureMaps.Dispose();
                HOperatorSet.CreateDict(out hv_SelectedFeatureMaps);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SelectedDepthsOfLayer.Dispose();
                    HOperatorSet.GetDictTuple(hv_SelectedFeatureMapDepths, hv_SelectedChannels.TupleSelect(
                        hv_LayerIndex), out hv_SelectedDepthsOfLayer);
                }
                ho_SingleFeatureMaps.Dispose();
                HOperatorSet.GenEmptyObj(out ho_SingleFeatureMaps);
                for (hv_Idx = 0; (int)hv_Idx <= (int)((new HTuple(hv_SelectedDepthsOfLayer.TupleLength()
                    )) - 1); hv_Idx = (int)hv_Idx + 1)
                {
                    if ((int)(new HTuple(((hv_SelectedDepthsOfLayer.TupleSelect(hv_Idx))).TupleGreater(
                        hv_NumChannels))) != 0)
                    {
                        throw new HalconException("Your set depth is too large! Please have a look at the channel depth of the chosen layer.");
                    }
                    //Get the feature maps.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_SingleFeatureMap.Dispose();
                        HOperatorSet.AccessChannel(ho_SampleFeatureMaps, out ho_SingleFeatureMap,
                            hv_SelectedDepthsOfLayer.TupleSelect(hv_Idx));
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_SingleFeatureMaps, ho_SingleFeatureMap, out ExpTmpOutVar_0
                            );
                        ho_SingleFeatureMaps.Dispose();
                        ho_SingleFeatureMaps = ExpTmpOutVar_0;
                    }
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetDictObject(ho_SingleFeatureMaps, hv_SelectedFeatureMaps, hv_SelectedChannels.TupleSelect(
                        hv_LayerIndex));
                }
                ho_SingleFeatureMaps.Dispose();
                ho_SingleFeatureMap.Dispose();

                hv_NumChannels.Dispose();
                hv_SelectedChannels.Dispose();
                hv_SelectedDepthsOfLayer.Dispose();
                hv_Idx.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_SingleFeatureMaps.Dispose();
                ho_SingleFeatureMap.Dispose();

                hv_NumChannels.Dispose();
                hv_SelectedChannels.Dispose();
                hv_SelectedDepthsOfLayer.Dispose();
                hv_Idx.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Short Description: Get the type for a selected model layer.
        public void get_layer_types_from_summary(HTuple hv_ChosenLayers, HTuple hv_NetworkSummary,
            out HTuple hv_LayerTypes)
        {
            // Local iconic variables

            // Local control variables

            HTuple hv_LayerIndex = new HTuple(), hv_SelectedLayerInfo = new HTuple();
            HTuple hv_RegExpression = new HTuple(), hv_LayerType = new HTuple();
            // Initialize local and output iconic variables
            hv_LayerTypes = new HTuple();
            try
            {
                //This procedure gets the type specification for the selected model layers.

                hv_LayerTypes.Dispose();
                hv_LayerTypes = new HTuple();
                for (hv_LayerIndex = 0; (int)hv_LayerIndex <= (int)((new HTuple(hv_ChosenLayers.TupleLength()
                    )) - 1); hv_LayerIndex = (int)hv_LayerIndex + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_SelectedLayerInfo.Dispose();
                        HOperatorSet.TupleRegexpSelect(hv_NetworkSummary, (hv_ChosenLayers.TupleSelect(
                            hv_LayerIndex)) + ";", out hv_SelectedLayerInfo);
                    }
                    hv_RegExpression.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_RegExpression = (hv_ChosenLayers.TupleSelect(
                            hv_LayerIndex)) + ";[ ]+([a-zA-Z0-9-]+)";
                    }
                    hv_LayerType.Dispose();
                    HOperatorSet.TupleRegexpMatch(hv_SelectedLayerInfo, hv_RegExpression, out hv_LayerType);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_LayerTypes = hv_LayerTypes.TupleConcat(
                                hv_LayerType);
                            hv_LayerTypes.Dispose();
                            hv_LayerTypes = ExpTmpLocalVar_LayerTypes;
                        }
                    }
                }

                hv_LayerIndex.Dispose();
                hv_SelectedLayerInfo.Dispose();
                hv_RegExpression.Dispose();
                hv_LayerType.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                hv_LayerIndex.Dispose();
                hv_SelectedLayerInfo.Dispose();
                hv_RegExpression.Dispose();
                hv_LayerType.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: File / Misc
        // Short Description: This procedure removes a directory recursively.
        public void remove_dir_recursively(HTuple hv_DirName)
        {
            // Local control variables

            HTuple hv_Dirs = new HTuple(), hv_I = new HTuple();
            HTuple hv_Files = new HTuple();
            // Initialize local and output iconic variables
            try
            {
                //Recursively delete all subdirectories.
                hv_Dirs.Dispose();
                HOperatorSet.ListFiles(hv_DirName, "directories", out hv_Dirs);
                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Dirs.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        remove_dir_recursively(hv_Dirs.TupleSelect(hv_I));
                    }
                }
                //Delete all files.
                hv_Files.Dispose();
                HOperatorSet.ListFiles(hv_DirName, "files", out hv_Files);
                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Files.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.DeleteFile(hv_Files.TupleSelect(hv_I));
                    }
                }
                //Remove empty directory.
                HOperatorSet.RemoveDir(hv_DirName);

                hv_Dirs.Dispose();
                hv_I.Dispose();
                hv_Files.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                hv_Dirs.Dispose();
                hv_I.Dispose();
                hv_Files.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Main procedure
        public ResultPackage action(HTuple hv_DLModelHandle, HImage image)
        {
            ResultPackage trp = new ResultPackage();
            // Stack for temporary objects
            HObject[] OTemp = new HObject[20];

            // Local iconic variables

            HObject ho_Image, ho_ImagePreprocessedByte;
            HObject ho_ImagePreprocessed;

            // Local control variables

            HTuple hv_ImageDimensions = new HTuple();
            HTuple hv_ClassNames = new HTuple(), hv_ClassIds = new HTuple();
            HTuple hv_DLSample = new HTuple(), hv_DLResult = new HTuple();
            HTuple hv_Confidences = new HTuple(), hv_PredictClasses = new HTuple();
            HTuple hv_Max = new HTuple(), hv_IndexMax = new HTuple();
            HTuple hv_Text = new HTuple();
            // Initialize local and output iconic variables
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImagePreprocessedByte);
            HOperatorSet.GenEmptyObj(out ho_ImagePreprocessed);
            try
            {
                ho_Image.Dispose();
                ho_Image = image;

                HOperatorSet.SetDlModelParam(hv_DLModelHandle, "batch_size", 16);

                hv_ImageDimensions.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_dimensions", out hv_ImageDimensions);
                hv_ClassNames.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_names", out hv_ClassNames);
                hv_ClassIds.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_ids", out hv_ClassIds);

                hv_DLSample.Dispose();
                HOperatorSet.CreateDict(out hv_DLSample);

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_ImagePreprocessedByte.Dispose();
                    HOperatorSet.ZoomImageSize(ho_Image, out ho_ImagePreprocessedByte, hv_ImageDimensions.TupleSelect(
                        0), hv_ImageDimensions.TupleSelect(1), "constant");
                }
                ho_ImagePreprocessed.Dispose();
                HOperatorSet.ConvertImageType(ho_ImagePreprocessedByte, out ho_ImagePreprocessed,
                    "real");
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ScaleImage(ho_ImagePreprocessed, out ExpTmpOutVar_0, 1, -127);
                    ho_ImagePreprocessed.Dispose();
                    ho_ImagePreprocessed = ExpTmpOutVar_0;
                }
                HOperatorSet.SetDictObject(ho_ImagePreprocessed, hv_DLSample, "image");
                hv_DLResult.Dispose();
                HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSample, new HTuple(), out hv_DLResult);
                hv_Confidences.Dispose();
                HOperatorSet.GetDictTuple(hv_DLResult, "classification_confidences", out hv_Confidences);
                hv_PredictClasses.Dispose();
                HOperatorSet.GetDictTuple(hv_DLResult, "classification_class_names", out hv_PredictClasses);
                hv_Max.Dispose();
                HOperatorSet.TupleMax(hv_Confidences, out hv_Max);
                hv_IndexMax.Dispose();
                HOperatorSet.TupleFind(hv_Confidences, hv_Max, out hv_IndexMax);

                hv_Text.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    trp.LabelName = hv_PredictClasses.TupleSelect(hv_IndexMax).S;
                    trp.Area = double.Parse(hv_Max.TupleString(".3f").S);

                    hv_Text = ((hv_PredictClasses.TupleSelect(
                        hv_IndexMax)) + new HTuple(", ")) + (hv_Max.TupleString(".3f"));
                }
            }
            catch (HalconException HDevExpDefaultException)
            {
                // ho_Image.Dispose();
                ho_ImagePreprocessedByte.Dispose();
                ho_ImagePreprocessed.Dispose();

                hv_DLModelHandle.Dispose();
                hv_ImageDimensions.Dispose();
                hv_ClassNames.Dispose();
                hv_ClassIds.Dispose();
                hv_DLSample.Dispose();
                hv_DLResult.Dispose();
                hv_Confidences.Dispose();
                hv_PredictClasses.Dispose();
                hv_Max.Dispose();
                hv_IndexMax.Dispose();
                return trp;
            }
            //ho_Image.Dispose();
            ho_ImagePreprocessedByte.Dispose();
            ho_ImagePreprocessed.Dispose();

            hv_ImageDimensions.Dispose();
            hv_ClassNames.Dispose();
            hv_ClassIds.Dispose();
            hv_DLSample.Dispose();
            hv_DLResult.Dispose();
            hv_Confidences.Dispose();
            hv_PredictClasses.Dispose();
            hv_Max.Dispose();
            hv_IndexMax.Dispose();
            return trp;
        }

        public void InitHalcon()
        {
            // Default settings used in HDevelop
            HOperatorSet.SetSystem("width", 512);
            HOperatorSet.SetSystem("height", 512);
        }

        public void RunHalcon(HTuple Window)
        {
            hv_ExpDefaultWinHandle = Window;
            // action();
        }
    }//分类测试
}
