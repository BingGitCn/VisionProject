using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace BingLibrary.Vision.Units
{
  public  class SurfaceCheck 
    {
        /// <summary>
        /// 表面检测GrayRangeRect
        /// </summary>
        /// <param name="hImage">灰度图</param>
        /// <param name="hRegion">检测区域</param>
        /// <param name="thresholdMinValue">检测结果二值化最小值</param>
        /// <param name="scaleImageSize">2</param>
        /// <param name="grayRangeRectSize">5</param>
        /// <param name="closingCircleSize">7</param>
        /// <param name="areaMinValue">结果过滤面积</param>
        /// <param name="erosionSize">检测区域内缩像素点</param>
        /// <returns></returns>
        public HRegion DefectsDetect(HImage hImage,HRegion hRegion,double thresholdMinValue = 40,double scaleImageSize=2,int grayRangeRectSize =5,double closingCircleSize =7,int areaMinValue=700, double erosionSize=5)
        {
            try {
                HRegion inspetRegion = hRegion.ErosionCircle(erosionSize);
                return hImage.ReduceDomain(hRegion).ScaleImage(scaleImageSize, 0).GrayRangeRect(grayRangeRectSize, grayRangeRectSize).ReduceDomain(inspetRegion).Threshold(thresholdMinValue, 255.0).ClosingCircle(closingCircleSize).FillUp().Connection().SelectShape(new HTuple("area"), "and", new HTuple(areaMinValue), new HTuple(160000000)).Union1();
            }
            catch { return new HRegion(); }
        }


    }
}
