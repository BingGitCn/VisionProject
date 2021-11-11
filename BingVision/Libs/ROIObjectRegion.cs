using HalconDotNet;

namespace BingLibrary.Vision
{
    public class ROIObjectRegion : ROI
    {
        public HObject mCurHRegion;

        public ROIObjectRegion(HObject r)
        {
            mCurHRegion = r;
        }

        public override void draw(HalconDotNet.HWindow window)
        {
            try
            {
                window.DispObj(mCurHRegion);
            }
            catch { }
        }
    }
}