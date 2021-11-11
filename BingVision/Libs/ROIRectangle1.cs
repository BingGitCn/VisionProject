using HalconDotNet;
using System;

namespace BingLibrary.Vision
{
    [Serializable]
    public class ROIRectangle1 : ROI
    {
        //public string ROIName { get; set; }
        //public int ROIId { get; set; }
        public double row1, col1;   // upper left

        public double row2, col2;   // lower right
        public double midR, midC;   // midpoint

        //public bool SizeEnable=true;
        /// <summary>Constructor</summary>
        public ROIRectangle1()
        {
            NumHandles = 5; // 4 corner points + midpoint
            activeHandleIdx = 4;
        }

        /// <summary>Creates a new ROI instance at the mouse position</summary>
        /// <param name="midX">
        /// x (=column) coordinate for interactive ROI
        /// </param>
        /// <param name="midY">
        /// y (=row) coordinate for interactive ROI
        /// </param>
        public override void createROI(double midX, double midY)
        {
            midR = midY;
            midC = midX;

            row1 = midR - 50;
            col1 = midC - 50;
            row2 = midR + 50;
            col2 = midC + 50;
        }

        public override void createROIRect1(double r1, double c1, double r2, double c2)
        {
            row1 = r1;
            col1 = c1;
            row2 = r2;
            col2 = c2;
        }

        private int smallregionwidth = 10;//4边小矩形的大小
        private HTuple hv_Font = new HTuple();
        private HTuple hv_OS = new HTuple();

        /// <summary>Paints the ROI into the supplied window</summary>
        /// <param name="window">HALCON window</param>
        public override void draw(HalconDotNet.HWindow window)
        {
            window.DispRectangle1(row1, col1, row2, col2);
            if (SizeEnable && ShowRect)
            {
                midR = ((row2 - row1) / 2) + row1;
                midC = ((col2 - col1) / 2) + col1;

                window.DispCircle(row1, col1, 50);
                window.DispCircle(row1, col2, 50);
                window.DispCircle(row2, col2, 50);
                window.DispCircle(row2, col1, 50);
                window.DispCircle(midR, midC, 50);

                try
                {
                    hv_Font.Dispose();
                    HOperatorSet.QueryFont(window, out hv_Font);
                    hv_OS.Dispose();
                    HOperatorSet.GetSystem("operating_system", out hv_OS);
                    if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetFont(window, (hv_Font.TupleSelect(0)) + "-" + "64");
                        }
                    }
                    else
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetFont(window, (hv_Font.TupleSelect(0)) + "-" + "64");
                        }
                    }
                    HOperatorSet.DispText(window, ROIName, "image", new HTuple(midR),
                        new HTuple(midC), "black", "box_color", "#ffffff77");
                }
                catch { }
            }
        }

        public override double distToClosestHandle(double x, double y)
        {
            double max = 10000;
            double[] val = new double[NumHandles];

            midR = ((row2 - row1) / 2) + row1;
            midC = ((col2 - col1) / 2) + col1;

            val[0] = HMisc.DistancePp(y, x, row1, col1); // upper left
            val[1] = HMisc.DistancePp(y, x, row1, col2); // upper right
            val[2] = HMisc.DistancePp(y, x, row2, col2); // lower right
            val[3] = HMisc.DistancePp(y, x, row2, col1); // lower left
            val[4] = HMisc.DistancePp(y, x, midR, midC); // midpoint

            for (int i = 0; i < NumHandles; i++)
            {
                if (val[i] < max)
                {
                    max = val[i];
                    activeHandleIdx = i;
                }
            }// end of for
            return val[activeHandleIdx];
        }

        public override double distToClosestROI(double x, double y)
        {
            HTuple dismax, dismin = 0;
            //这算法根本不对啊！还不如自己算的
            //HOperatorSet.DistancePr(getRegion(), y, x, out dismin, out dismax);
            if (y >= row1 && y <= row2 && x >= col1 && x <= col2)
                dismin = 0;
            else
                dismin = -1;
            //System.Diagnostics.Debug.Print(dismin + "," + dismax);
            //Console.WriteLine("r1:" + row1 + "c1:" + col1 + "r2:" + row2 + "c2:" + col2 + "y:" + y + "x:" + x+"dismin:"+dismin.D);
            return dismin;
        }

        public override void displayActive(HalconDotNet.HWindow window)
        {
            if (!SizeEnable || !ShowRect)
                return;
            switch (activeHandleIdx)
            {
                case 0:
                    //window.DispRectangle2(row1, col1, 0, smallregionwidth, smallregionwidth);
                    window.DispCircle(row1, col1, 50);
                    break;

                case 1:
                    // window.DispRectangle2(row1, col2, 0, smallregionwidth, smallregionwidth);
                    window.DispCircle(row1, col2, 50);
                    break;

                case 2:
                    // window.DispRectangle2(row2, col2, 0, smallregionwidth, smallregionwidth);
                    window.DispCircle(row2, col2, 50);
                    break;

                case 3:
                    // window.DispRectangle2(row2, col1, 0, smallregionwidth, smallregionwidth);
                    window.DispCircle(row2, col1, 50);
                    break;

                case 4:
                    // window.DispRectangle2(midR, midC, 0, smallregionwidth, smallregionwidth);
                    window.DispCircle(midR, midC, 50);
                    break;
            }
        }

        public override HRegion getRegion()
        {
            HRegion region = new HRegion();
            region.GenRectangle1(row1, col1, row2, col2);
            return region;
        }

        public override HTuple getModelData()
        {
            return new HTuple(new double[] { row1, col1, row2, col2 });
        }

        public override void moveByHandle(double newX, double newY)
        {
            if (SizeEnable == false)
                return;
            double len1, len2;
            double tmp;

            switch (activeHandleIdx)
            {
                case 0: // upper left
                    row1 = newY;
                    col1 = newX;
                    break;

                case 1: // upper right
                    row1 = newY;
                    col2 = newX;
                    break;

                case 2: // lower right
                    row2 = newY;
                    col2 = newX;
                    break;

                case 3: // lower left
                    row2 = newY;
                    col1 = newX;
                    break;

                case 4: // midpoint
                    len1 = ((row2 - row1) / 2);
                    len2 = ((col2 - col1) / 2);

                    row1 = newY - len1;
                    row2 = newY + len1;

                    col1 = newX - len2;
                    col2 = newX + len2;

                    break;
            }

            if (row2 <= row1)
            {
                tmp = row1;
                row1 = row2;
                row2 = tmp;
            }

            if (col2 <= col1)
            {
                tmp = col1;
                col1 = col2;
                col2 = tmp;
            }

            midR = ((row2 - row1) / 2) + row1;
            midC = ((col2 - col1) / 2) + col1;
        }//end of method

        public override void show()
        {
            System.Diagnostics.Debug.Print(midR + "," + midC + "," + row1 + "," + col1 + "," + row2 + "," + col2);
        }
    }
}