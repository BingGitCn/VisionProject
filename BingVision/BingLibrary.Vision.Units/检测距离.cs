using HalconDotNet;

namespace BingLibrary.Vision.Units
{
    public class Measure 
    {
        /// <summary>
        /// 测量一个物体的长度间距
        /// </summary>
        /// <param name="image">当前图像</param>
        /// <param name="Row">Rectangle2参数</param>
        /// <param name="Column">Rectangle2参数</param>
        /// <param name="Phi">Rectangle2参数</param>
        /// <param name="Length1">Rectangle2参数</param>
        /// <param name="Length2">Rectangle2参数</param>
        /// <param name="Sigma">Sigma,默认设1即可</param>
        /// <param name="Threshold">阈值，对比度高则高，反之则低，经验值10</param>
        /// <param name="Transition1">起始检测变换，positive,negative,all</param>
        /// <param name="Transition2">结束检测变换，positive,negative,all</param>
        /// <returns></returns>
        public static double[] MeasureSpace(HImage image, double Row, double Column, double Phi, double Length1, double Length2, double Sigma, double Threshold, TransitionType Transition1, TransitionType Transition2)
        {
            double[] results = new double[5];
            HTuple hv_Width, hv_Height;
            image.GetImageSize(out hv_Width, out hv_Height);
            HMeasure MeasureHandle = new HMeasure(Row, Column, Phi, Length1, Length2, hv_Width, hv_Height, "nearest_neighbor");
            HTuple hv_RowM1, hv_ColumnM1, hv_AmplitudeMeasure1, hv_DistanceMeasure1;
            HTuple hv_RowM2, hv_ColumnM2, hv_AmplitudeMeasure2, hv_DistanceMeasure2;
            HTuple hv_Distance;
            image.MeasurePos(MeasureHandle, Sigma, Threshold, Transition1.ToString(), "first", out hv_RowM1, out hv_ColumnM1, out hv_AmplitudeMeasure1, out hv_DistanceMeasure1);
            image.MeasurePos(MeasureHandle, Sigma, Threshold, Transition2.ToString(), "last", out hv_RowM2, out hv_ColumnM2, out hv_AmplitudeMeasure2, out hv_DistanceMeasure2);
            HOperatorSet.DistancePp(hv_RowM1, hv_ColumnM1, hv_RowM2, hv_ColumnM2, out hv_Distance);
            MeasureHandle?.Dispose();
            results[0] = hv_RowM1.D;
            results[1] = hv_ColumnM1.D;
            results[2] = hv_RowM2.D;
            results[3] = hv_ColumnM2.D;
            results[4] = hv_Distance.D;
            return results;
        }

        /// <summary>
        /// 检测宽度
        /// </summary>
        /// <param name="image">当前图像</param>
        /// <param name="Row1">检测直线起点</param>
        /// <param name="Col1">检测直线起点</param>
        /// <param name="Row2">检测直线终点</param>
        /// <param name="Col2">检测直线终点</param>
        /// <param name="Transition">检测变换，positive,negative,all</param>
        /// <param name="Elements">检测元素个数</param>
        /// <param name="ActiveElements">生效的元素个数</param>
        /// <param name="DetectHeight">检测范围高度</param>
        /// <param name="select">选择直线，first,last,max</param>
        /// <param name="Sigma">Sigma,默认设置1即可</param>
        /// <param name="Threshold">阈值，对比度高则高，反之则低，经验值30</param>
        /// <returns></returns>
        public static double GetLineDistance(double Row1, double Col1, double Row2, double Col2, double Row11, double Col11, double Row21, double Col21, int Elements)
        {

            double rst = 0;
            HTuple hv_RowL1 = new HTuple();
            HTuple hv_ColL1 = new HTuple();
            HTuple hv_RowL2 = new HTuple();
            HTuple hv_ColL2 = new HTuple();
            //Bing
            //20170705

            HTuple hv_ResultRow = new HTuple();
            HTuple hv_ResultColumn = new HTuple();
            HTuple hv_ATan, hv_Deg1, hv_Deg, hv_i;
            HOperatorSet.TupleAtan2((-Row2) + Row1, Col2 - Col1, out hv_ATan);
            HOperatorSet.TupleDeg(hv_ATan, out hv_Deg1);
            hv_ATan = hv_ATan + ((new HTuple(90)).TupleRad());
            HOperatorSet.TupleDeg(hv_ATan, out hv_Deg);

            HTuple end_val11 = Elements;
            HTuple step_val11 = 1;
            HTuple hv_RowC, hv_ColC;
            int c = 0;
            for (hv_i = 1; hv_i.Continue(end_val11, step_val11); hv_i = hv_i.TupleAdd(step_val11))
            {
                hv_RowC = Row1 + (((Row2 - Row1) * hv_i) / (Elements + 1));
                hv_ColC = Col1 + (((Col2 - Col1) * hv_i) / (Elements + 1));
                HTuple distance;
                HOperatorSet.DistancePl(hv_RowC, hv_ColC, Row11, Col11, Row21, Col21, out distance);
                rst += distance.D;
                c++;

            }
            return rst / c;
        }
    }


}