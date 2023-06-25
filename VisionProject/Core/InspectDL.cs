using VisionProject.Core;
using HalconDotNet;
using System;
using System.Collections.ObjectModel;

namespace VisionProject.Core
{
    /// <summary>
    /// 应用深度模型进行检测
    /// 日期：2021-09-01
    /// 编写：Bing
    /// </summary>
    public class Inspect
    {
        private HTuple hv_DLModelHandle = new HTuple();
        private InspectCY tcy = new InspectCY();
        private InspectSM tsm = new InspectSM();


        /// <summary>
        /// 加载模型，自动识别模型类型
        /// </summary>
        /// <param name="modelPath">模型路径</param>
        /// <returns></returns>
        public BoolResult LoadModel(string modelPath, int batchSize = 2, bool isUseGPU = false)
        {
            if (modelPath == "") return new BoolResult() { IsActionOK = false, ExMessage = "未设定模型路径" };
            try
            {
                HOperatorSet.ClearDlModel(hv_DLModelHandle);
            }
            catch { }
            try
            {
                HOperatorSet.ReadDlModel(modelPath, out hv_DLModelHandle);
                HOperatorSet.SetDlModelParam(hv_DLModelHandle, "batch_size", batchSize);
                if (isUseGPU)
                {
                    HOperatorSet.SetDlModelParam(hv_DLModelHandle, "gpu", 0);
                    HOperatorSet.SetSystem("cudnn_deterministic", "true");
                }



                return new BoolResult() { IsActionOK = true };
            }
            catch (Exception ex)
            {
                return new BoolResult() { IsActionOK = true, ExMessage = ex.Message };
            }
        }

        /// <summary>
        /// 应用模型
        /// </summary>
        /// <param name="image">输入检测图像</param>
        /// <param name="ignoreLabels">在结果中忽略的标注类型，0是背景不必设置，1一般为“正常”，new int[1]{1}</param>
        /// <returns>输出结果</returns>
        public ObservableCollection<ResultPackage> ApplyModel(out string type,HImage image, int[] ignoreLabels, bool isHighAccuracy = false)
        {
            type = "";
            //new int[1] { 1 }
            HTuple mType = new HTuple();
            ObservableCollection<HObject> hobjs = new ObservableCollection<HObject>();
            ObservableCollection<ResultPackage> Results = new ObservableCollection<ResultPackage>();
            try
            {
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "type", out mType);
                if (mType.S == "segmentation")
                {
                    hobjs = tsm.action(hv_DLModelHandle, image, ignoreLabels, isHighAccuracy);

                    for (int i = 0; i < tsm.LabelIDS.Length; i++)
                    {
                        HTuple area, r, c;
                        HOperatorSet.AreaCenter(hobjs[i], out area, out r, out c);
                        Results.Add(new ResultPackage() { LabelName = tsm.LabelIDS[i].I.ToString(), Area = area.D, Region = hobjs[i] });
                    }
                }
                else if (mType.S == "classification")
                {
                    var trp = tcy.action(hv_DLModelHandle, image);
                    Results.Add(trp);
                }
                type = mType.S;
            }
            catch (Exception ex) { }
            return Results;
        }

        /// <summary>
        /// 应用模型，语义分割下，返回图像
        /// </summary>
        /// <param name="image"></param>
        /// <param name="ignoreLabels"></param>
        /// <param name="isHighAccuracy"></param>
        /// <returns></returns>
        public HImage ApplyModelInSegmentation(HImage image, int[] ignoreLabels, bool isHighAccuracy = false)
        {
            //new int[1] { 1 }
            HTuple mType = new HTuple();
            HImage resultImage = new HImage();
            try
            {
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "type", out mType);
                if (mType.S == "segmentation")
                    resultImage = tsm.action2(hv_DLModelHandle, image, ignoreLabels, isHighAccuracy);
            }
            catch (Exception ex) { }
            return resultImage;
        }
    }

    public class BoolResult
    {
        /// <summary>
        /// 执行结果
        /// </summary>
        public bool IsActionOK { set; get; }

        /// <summary>
        /// 出错信息
        /// </summary>
        public string ExMessage { set; get; }
    }

    public class ResultPackage
    {
        /// <summary>
        /// 标注名称
        /// </summary>
        public string LabelName { set; get; } = "";

        /// <summary>
        /// 面积或置信度
        /// </summary>
        public double Area { set; get; } = 0;

        /// <summary>
        /// 语义分割检测的region
        /// </summary>
        public HObject Region { set; get; } = null;
    }
}
