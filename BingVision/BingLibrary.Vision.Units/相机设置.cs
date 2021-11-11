using HalconDotNet;
using System.Collections.Generic;

namespace BingLibrary.Vision.Units
{
    public class HalconCamera
    {
        public class CameraPackage
        {
            public string Name { set; get; }
            public cameraType Type { set; get; }
            public double ExTime { set; get; } = 1000;
        }

        /// <summary>
        /// 相机接口类型枚举
        /// </summary>
        public enum cameraType
        {
            DirectShow,
            GigeVision2,
            USB3Vision
        }

        public enum RegionType
        {
            Region,
        }

        private List<CameraPackage> cameraPackages = new List<CameraPackage>();

        public void GetCameras()
        {
            HTuple hv_Information = null;
            HTuple hv_Devices = null;

            try
            {
                HOperatorSet.InfoFramegrabber(new HTuple(cameraType.DirectShow.ToString()), new HTuple("device"), out hv_Information, out hv_Devices);
            }
            catch
            {
                hv_Devices = new HTuple("DirectShowFailed");
            }
            try
            {
                int l = hv_Devices.SArr.Length;
                if (l > 0)
                {
                    for (int i = 0; i < l; i++)
                    {
                        CameraPackage DirectShowDevice = new CameraPackage();
                        DirectShowDevice.Name = hv_Devices.SArr[i];
                        DirectShowDevice.Type = cameraType.DirectShow;
                        cameraPackages.Add(DirectShowDevice);
                    }
                }
            }
            catch { }
            try
            {
                HOperatorSet.InfoFramegrabber(new HTuple(cameraType.GigeVision2.ToString()), new HTuple("device"), out hv_Information, out hv_Devices);
            }
            catch
            {
                hv_Devices = new HTuple("GigeVision2Failed");
            }
            try
            {
                int l = hv_Devices.SArr.Length;
                if (l > 0)
                {
                    for (int i = 0; i < l; i++)
                    {
                        CameraPackage GigeVision2Device = new CameraPackage();
                        GigeVision2Device.Name = hv_Devices.SArr[i];
                        GigeVision2Device.Type = cameraType.GigeVision2;
                        cameraPackages.Add(GigeVision2Device);
                    }
                }
            }
            catch { }
        }

        private HFramegrabber Framegrabber { set; get; }
        public HImage CurrentImage { set; get; } = new HImage();
        public string HalconCameraID { set; get; }
        public string Name { set; get; }
        public cameraType CameraType { set; get; }
        public double ExpouseTime { set; get; }

        /// <summary>
        /// 初始化相机ID
        /// </summary>
        /// <param name="halconCameraID"></param>
        public void Init(string halconCameraID)
        {
            HalconCameraID = halconCameraID;
        }

        /// <summary>
        /// 打开相机
        /// </summary>
        /// <returns></returns>
        public bool OpenCamera()
        {
            try
            {
                Framegrabber = new HFramegrabber(CameraType.ToString(), 0, 0, 0, 0, 0, 0, "default", -1, "default", -1, "false", "default", Name, 0, -1);

                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 关闭相机
        /// </summary>
        /// <returns></returns>
        public bool CloseCamera()
        {
            try
            {
                Framegrabber.Dispose();
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 设置曝光时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool SetExpouseTime(double time)
        {
            try
            {
                ExpouseTime = time;
                Framegrabber.SetFramegrabberParam("ExposureTime", ExpouseTime);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 拍照
        /// </summary>
        /// <returns></returns>
        public bool GrabImage()
        {
            try
            {
                CurrentImage?.Dispose();
                CurrentImage = Framegrabber.GrabImage();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 读取一张图像
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public bool ReadImage(string imagePath)
        {
            try
            {
                CurrentImage?.Dispose();
                CurrentImage.ReadImage(imagePath);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 保存图像
        /// </summary>
        /// <param name="imageType"></param>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public bool SaveImage(string imageType, string imagePath)
        {
            try
            {
                CurrentImage.WriteImage(imageType, 0, imagePath);
                return true;
            }
            catch { return false; }
        }
    }
}