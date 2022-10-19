using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace VisionProject.HLibrary
{
    /// <summary>
    /// 坐标
    /// </summary>
    public class CoordinatePoint
    {
        /// <summary>
        /// X 坐标
        /// </summary>
        public double X { set; get; }

        /// <summary>
        /// Y 坐标
        /// </summary>
        public double Y { set; get; }

        /// <summary>
        /// Z 坐标
        /// </summary>
        public double Z { set; get; }

        /// <summary>
        /// 角度
        /// </summary>
        public double U { set; get; }

        /// <summary>
        /// 点位注释预留
        /// </summary>
        public string Commit { set; get; }
    }

    /// <summary>
    /// 相机接口类型枚举
    /// </summary>
    public enum CameraType
    {
        DirectShow,
        GigeVision2,
        USB3Vision
    }

    /// <summary>
    /// 相机类型
    /// </summary>
    public class CameraPackage
    {
        public string Name { set; get; }
        public CameraType Type { set; get; }
        public double ExTime { set; get; } = 1000;
    }
}