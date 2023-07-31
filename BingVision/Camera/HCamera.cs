using HalconDotNet;

namespace BingLibrary.Vision
{
    public class HCamera
    {
        private HFramegrabber hFramegrabber = new HFramegrabber();
        public double ExpouseTime { set; get; }
        public int CameraType { set; get; } = 0;
        public string CameraName { set; get; }

        public bool IsOpened { set; get; } = false;

        public HCamera(string name)
        {
            CameraName = name;
        }

        public bool OpenCamera()
        {
            try
            {
                hFramegrabber = new HFramegrabber(CameraType == 0 ? "GigEVision2" : CameraType == 1 ? "DirectShow" : "USB3Vision", 1, 1, 0, 0, 0, 0, "default", 8, "default", -1, "false", "default", CameraName, 0, -1);
                SetExpouseTime(ExpouseTime);
                IsOpened = true;
                return true;
            }
            catch { IsOpened = false; return false; }
        }

        public HImage GrabOne()
        {
            try
            {
                HImage image = hFramegrabber.GrabImage();

                return image;
            }
            catch { return null; }
        }

        public bool CloseCamera()
        {
            try
            {
                hFramegrabber.CloseFramegrabber();
                IsOpened = false;
                return true;
            }
            catch { IsOpened = false; return false; }
        }

        public bool SetExpouseTime(double time)
        {
            try
            {
                ExpouseTime = time;
                hFramegrabber.SetFramegrabberParam("ExposureTime", ExpouseTime);
                return true;
            }
            catch { return false; }
        }
    }
}