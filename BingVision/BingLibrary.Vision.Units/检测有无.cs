using HalconDotNet;

namespace BingLibrary.Vision.Units
{
    public class FindObject
    { 
        /// <summary>
        /// 检测有无
        /// </summary>
        /// <param name="image">检测区域的图像</param>
        /// <param name="isFindWhite">是否找白，否则找黑</param>
        /// <param name="Threshold">阈值，中间值128</param>
        /// <returns></returns>
        public static double Find(HImage image, ThresholdType thresholdType, double Threshold)
        {
            return image.Threshold(thresholdType == ThresholdType.White ? Threshold : 0, thresholdType == ThresholdType.White ? 255 : Threshold).Area.D;
        }
    }
} 