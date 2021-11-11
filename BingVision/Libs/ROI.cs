using HalconDotNet;
using System;

namespace BingLibrary.Vision
{
    public enum ROIColors
    {
        red, green, blue, violet, gray, cyan, magenta, yelow, coral, pink, orange, gold, navy
    }

    [Serializable]
    public class ROI
    {
        public string ROIName { get; set; }

        public string ROIResult { set; get; }

        public string ROIColor { set; get; }

        public int ROIId { get; set; }
        private bool mSizeEnable = true;
        public bool SizeEnable { get { return mSizeEnable; } set { mSizeEnable = value; } }
        private bool mShowRect = false;
        public bool ShowRect { get { return mShowRect; } set { mShowRect = value; } }

        public virtual void show()
        {
        }

        protected int NumHandles;

        protected int activeHandleIdx;

        protected int OperatorFlag;

        public HTuple flagLineStyle = new HTuple();//halcon13修改=new htuple(),否则报null错误

        public const int POSITIVE_FLAG = ROIController.MODE_ROI_POS;

        public const int NEGATIVE_FLAG = ROIController.MODE_ROI_NEG;

        public const int ROI_TYPE_LINE = 10;
        public const int ROI_TYPE_CIRCLE = 11;
        public const int ROI_TYPE_CIRCLEARC = 12;
        public const int ROI_TYPE_RECTANCLE1 = 13;
        public const int ROI_TYPE_RECTANGLE2 = 14;

        [NonSerialized]
        protected HTuple posOperation = new HTuple();

        [NonSerialized]
        protected HTuple negOperation = new HTuple(new int[] { 2, 2 });

        public ROI()
        {
        }

        public virtual void createROI(double midX, double midY)
        {
        }

        public virtual void createROIRect1(double r1, double c1, double r2, double c2)
        {
        }

        public virtual void draw(HalconDotNet.HWindow window)
        {
        }

        public virtual double distToClosestHandle(double x, double y)
        {
            return 0.0;
        }

        //判断鼠标是否在ROI内
        public virtual double distToClosestROI(double x, double y)
        {
            return -1.0;
        }

        public virtual void displayActive(HalconDotNet.HWindow window)
        {
        }

        public virtual void moveByHandle(double x, double y)
        {
        }

        public virtual HRegion getRegion()
        {
            return null;
        }

        public virtual double getDistanceFromStartPoint(double row, double col)
        {
            return 0.0;
        }

        public virtual HTuple getModelData()
        {
            return null;
        }

        public int getNumHandles()
        {
            return NumHandles;
        }

        public int getActHandleIdx()
        {
            return activeHandleIdx;
        }

        public int getOperatorFlag()
        {
            return OperatorFlag;
        }

        public void setOperatorFlag(int flag)
        {
            OperatorFlag = flag;

            switch (OperatorFlag)
            {
                case ROI.POSITIVE_FLAG:
                    flagLineStyle = posOperation;
                    break;

                case ROI.NEGATIVE_FLAG:
                    flagLineStyle = negOperation;
                    break;

                default:
                    flagLineStyle = posOperation;
                    break;
            }
        }
    }
}