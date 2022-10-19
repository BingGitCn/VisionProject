using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace VisionProject.HLibrary
{
    public class HalCamera
    {
        /// <summary>
        /// 静态，获取连接的相机，输入相机接口类型
        /// </summary>
        /// <param name="camType"></param>
        /// <returns></returns>
        public static List<CameraPackage> GetCameras(CameraType camType)
        {
            List<CameraPackage> cameraPackages = new List<CameraPackage>();
            HTuple hv_Information = null;
            HTuple hv_Devices = null;

            try
            {
                HOperatorSet.InfoFramegrabber(new HTuple(camType.ToString()), new HTuple("device"), out hv_Information, out hv_Devices);
            }
            catch
            {
                hv_Devices = new HTuple("get " + camType.ToString() + " failed");
            }
            try
            {
                int l = hv_Devices.SArr.Length;
                if (l > 0)
                {
                    for (int i = 0; i < l; i++)
                    {
                        CameraPackage cp = new CameraPackage();
                        cp.Name = hv_Devices.SArr[i];
                        cp.Type = CameraType.DirectShow;
                        cameraPackages.Add(cp);
                    }
                }
            }
            catch { }

            return cameraPackages;
        }

        /// <summary>
        /// 将HObject类型的图像转换程HImage类型
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        public static HImage HObjectToHImage(HObject hObject)
        {
            HImage hImage = new HImage();
            if (hObject is HImage)
            {
                HTuple channels;
                HOperatorSet.CountChannels(hObject, out channels);

                if (channels.I == 1)
                {
                    HTuple pointer, htype, width, height;
                    HOperatorSet.GetImagePointer1(hObject, out pointer, out htype, out width, out height);
                    hImage.GenImage1(htype.S, width.I, height.I, pointer.IP);
                }
                else if (channels.I == 3)
                {
                    HTuple pointerR, pointerG, pointerB, htype, width, height;
                    HOperatorSet.GetImagePointer3(hObject, out pointerR, out pointerG, out pointerB, out htype, out width, out height);
                    hImage = new HImage();
                    hImage.GenImage3(htype.S, width.I, height.I, pointerR.IP, pointerG.IP, pointerB.IP);
                }
                else
                    hImage = new HImage();
            }
            return hImage;
        }

        private HFramegrabber framegrabber { set; get; }

        /// <summary>
        /// 打开相机
        /// </summary>
        /// <param name="camName"></param>
        /// <param name="cameraType"></param>
        /// <returns></returns>
        public bool OpenCamera(string camName, CameraType cameraType)
        {
            try
            {
                framegrabber = new HFramegrabber(cameraType.ToString(), 0, 0, 0, 0, 0, 0, "default", -1, "default", -1, "false", "default", camName, 0, -1);

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
                framegrabber.Dispose();
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 设置曝光时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="check">防止参数名称出错</param>
        /// <returns></returns>
        public bool SetExpouseTime(double time, int check = 0)
        {
            try
            {
                if (check == 0)
                    framegrabber.SetFramegrabberParam("ExposureTime", time);
                else
                    framegrabber.SetFramegrabberParam("ExposureTimeAbs", time);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 设置相机参数
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public bool SetCameraParam(HTuple paramName, HTuple paramValue)
        {
            try
            {
                framegrabber.SetFramegrabberParam(paramName, paramValue);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 拍照,同步
        /// </summary>
        /// <returns></returns>
        public HImage GrabOneImage()
        {
            HImage image = new HImage();
            try
            {
                image = framegrabber.GrabImage();
            }
            catch
            {
            }
            return image;
        }
    }
}