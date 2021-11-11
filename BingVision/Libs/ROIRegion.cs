using HalconDotNet;

namespace BingLibrary.Vision
{
    public class ROIRegion : ROI
    {
        public HRegion mCurHRegion;

        public ROIRegion(HRegion r)
        {
            mCurHRegion = r;
        }

        public override void draw(HalconDotNet.HWindow window)
        {
            try
            {
                window.DispRegion(mCurHRegion);
            }
            catch { }
        }
    }
}