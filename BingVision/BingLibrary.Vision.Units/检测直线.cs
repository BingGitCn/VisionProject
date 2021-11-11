using HalconDotNet;

namespace BingLibrary.Vision.Units
{
    public class DetectLine
    {
        /// <summary>
        /// 找直线
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
        public static double[] FindLine(HImage image, double Row1, double Col1, double Row2, double Col2, TransitionType Transition, int Elements, int ActiveElements, double DetectHeight, SelectType select, double Sigma, int Threshold)
        {
            double[] results = new double[5];

            // Stack for temporary objects
            HObject[] OTemp = new HObject[20];

            // Local iconic variables

            HObject ho_Regions, ho_Rectangle = null, ho_Arrow1 = null;
            HObject ho_Line, ho_Contour = null;

            // Local control variables

            HTuple hv_Width = null, hv_Height = null, hv_ResultRow = null;
            HTuple hv_ResultColumn = null, hv_ATan = null, hv_Deg1 = null;
            HTuple hv_Deg = null, hv_i = null, hv_RowC = new HTuple();
            HTuple hv_ColC = new HTuple(), hv_Distance = new HTuple();
            HTuple hv_MsrHandle_Measure = new HTuple(), hv_RowEdge = new HTuple();
            HTuple hv_ColEdge = new HTuple(), hv_Amplitude = new HTuple();
            HTuple hv_tRow = new HTuple(), hv_tCol = new HTuple();
            HTuple hv_t = new HTuple(), hv_Number = new HTuple(), hv_j = new HTuple();
            HTuple hv_Length = null, hv_Nr = new HTuple(), hv_Nc = new HTuple();
            HTuple hv_Dist = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_Select_COPY_INP_TMP = select.ToString();
            HTuple hv_Transition_COPY_INP_TMP = Transition.ToString();

            // Initialize local and output iconic variables
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Arrow1);
            HOperatorSet.GenEmptyObj(out ho_Line);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HTuple hv_RowL1 = new HTuple();
            HTuple hv_ColL1 = new HTuple();
            HTuple hv_RowL2 = new HTuple();
            HTuple hv_ColL2 = new HTuple();
            //Bing
            //20170705
            HOperatorSet.GetImageSize(image, out hv_Width, out hv_Height);
            ho_Regions.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Regions);
            hv_ResultRow = new HTuple();
            hv_ResultColumn = new HTuple();
            HOperatorSet.TupleAtan2((-Row2) + Row1, Col2 - Col1, out hv_ATan);
            HOperatorSet.TupleDeg(hv_ATan, out hv_Deg1);
            hv_ATan = hv_ATan + ((new HTuple(90)).TupleRad());
            HOperatorSet.TupleDeg(hv_ATan, out hv_Deg);

            HTuple end_val11 = Elements;
            HTuple step_val11 = 1;
            for (hv_i = 1; hv_i.Continue(end_val11, step_val11); hv_i = hv_i.TupleAdd(step_val11))
            {
                hv_RowC = Row1 + (((Row2 - Row1) * hv_i) / (Elements + 1));
                hv_ColC = Col1 + (((Col2 - Col1) * hv_i) / (Elements + 1));
                if ((int)((new HTuple((new HTuple((new HTuple(hv_RowC.TupleGreater(hv_Height - 1))).TupleOr(
                    new HTuple(hv_RowC.TupleLess(0))))).TupleOr(new HTuple(hv_ColC.TupleGreater(
                    hv_Width - 1))))).TupleOr(new HTuple(hv_ColC.TupleLess(0)))) != 0)
                {
                    continue;
                }
                HOperatorSet.DistancePp(Row1, Col1, Row2, Col2, out hv_Distance);
                if (Elements != 0)
                {
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC,
                        hv_Deg.TupleRad(), DetectHeight / 2, hv_Distance / 2);
                }
                else
                {
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_RowC, hv_ColC,
                        hv_Deg.TupleRad(), DetectHeight / 2, hv_Distance / Elements);
                }

                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Regions, ho_Rectangle, out ExpTmpOutVar_0);
                    ho_Regions.Dispose();
                    ho_Regions = ExpTmpOutVar_0;
                }
                if ((int)(new HTuple(hv_i.TupleEqual(1))) != 0)
                {
                    hv_RowL2 = hv_RowC + ((DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                    hv_RowL1 = hv_RowC - ((DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                    hv_ColL2 = hv_ColC + ((DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                    hv_ColL1 = hv_ColC - ((DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                    ho_Arrow1.Dispose();
                    gen_arrow_contour_xld(out ho_Arrow1, hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2,
                        25, 25);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_Regions, ho_Arrow1, out ExpTmpOutVar_0);
                        ho_Regions.Dispose();
                        ho_Regions = ExpTmpOutVar_0;
                    }
                }
                HOperatorSet.GenMeasureRectangle2(hv_RowC, hv_ColC, hv_Deg.TupleRad(), DetectHeight / 2,
                    (hv_Distance / Elements) / 2, hv_Width, hv_Height, "nearest_neighbor", out hv_MsrHandle_Measure);

                if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("negative"))) != 0)
                {
                    hv_Transition_COPY_INP_TMP = "negative";
                }
                else
                {
                    if ((int)(new HTuple(hv_Transition_COPY_INP_TMP.TupleEqual("positive"))) != 0)
                    {
                        hv_Transition_COPY_INP_TMP = "positive";
                    }
                    else
                    {
                        hv_Transition_COPY_INP_TMP = "all";
                    }
                }

                if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("first"))) != 0)
                {
                    hv_Select_COPY_INP_TMP = "first";
                }
                else
                {
                    if ((int)(new HTuple(hv_Select_COPY_INP_TMP.TupleEqual("last"))) != 0)
                    {
                        hv_Select_COPY_INP_TMP = "last";
                    }
                    else
                    {
                        hv_Select_COPY_INP_TMP = "all";
                    }
                }

                HOperatorSet.MeasurePos(image, hv_MsrHandle_Measure, Sigma, Threshold,
                    hv_Transition_COPY_INP_TMP, hv_Select_COPY_INP_TMP, out hv_RowEdge, out hv_ColEdge,
                    out hv_Amplitude, out hv_Distance);

                HOperatorSet.CloseMeasure(hv_MsrHandle_Measure);
                hv_tRow = 0;
                hv_tCol = 0;
                hv_t = 0;
                HOperatorSet.TupleLength(hv_RowEdge, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleLess(1))) != 0)
                {
                    continue;
                }
                HTuple end_val65 = hv_Number - 1;
                HTuple step_val65 = 1;
                for (hv_j = 0; hv_j.Continue(end_val65, step_val65); hv_j = hv_j.TupleAdd(step_val65))
                {
                    if ((int)(new HTuple(((((hv_Amplitude.TupleSelect(hv_j))).TupleAbs())).TupleGreater(
                        hv_t))) != 0)
                    {
                        hv_tRow = hv_RowEdge.TupleSelect(hv_j);
                        hv_tCol = hv_ColEdge.TupleSelect(hv_j);
                        hv_t = ((hv_Amplitude.TupleSelect(hv_j))).TupleAbs();
                    }
                }
                if ((int)(new HTuple(hv_t.TupleGreater(0))) != 0)
                {
                    hv_ResultRow = hv_ResultRow.TupleConcat(hv_tRow);
                    hv_ResultColumn = hv_ResultColumn.TupleConcat(hv_tCol);
                }
            }
            HOperatorSet.TupleLength(hv_ResultRow, out hv_Number);

            hv_RowL1 = 0;
            hv_ColL1 = 0;
            hv_RowL2 = 0;
            hv_ColL2 = 0;
            ho_Line.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Line);
            HOperatorSet.TupleLength(hv_ResultColumn, out hv_Length);

            if (ActiveElements != 0)
            {
                ho_Contour.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_ResultRow, hv_ResultColumn);
                HOperatorSet.FitLineContourXld(ho_Contour, "tukey", ActiveElements, 0, 5,
                    2, out hv_RowL1, out hv_ColL1, out hv_RowL2, out hv_ColL2, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                HOperatorSet.TupleLength(hv_Dist, out hv_Length1);
                if ((int)(new HTuple(hv_Length1.TupleLess(1))) != 0)
                {
                    ho_Regions.Dispose();
                    ho_Rectangle.Dispose();
                    ho_Arrow1.Dispose();
                    ho_Line.Dispose();
                    ho_Contour.Dispose();
                }
                ho_Line.Dispose();
            }
            ho_Regions.Dispose();
            ho_Rectangle.Dispose();
            ho_Arrow1.Dispose();
            ho_Line.Dispose();
            ho_Contour.Dispose();
            HTuple hv_Angle;
            HOperatorSet.AngleLx(hv_RowL1, hv_ColL1, hv_RowL2, hv_ColL2, out hv_Angle);

            results[0] = hv_RowL1.D;
            results[1] = hv_ColL1.D;
            results[2] = hv_RowL2.D;
            results[3] = hv_ColL2.D;
            results[4] = hv_Angle.D;
            return results;
        }


       

        /// <summary>
        /// 创建箭头
        /// </summary>
        /// <param name="ho_Arrow"></param>
        /// <param name="hv_Row1"></param>
        /// <param name="hv_Column1"></param>
        /// <param name="hv_Row2"></param>
        /// <param name="hv_Column2"></param>
        /// <param name="hv_HeadLength"></param>
        /// <param name="hv_HeadWidth"></param>
        private static void gen_arrow_contour_xld(out HObject ho_Arrow, HTuple hv_Row1, HTuple hv_Column1, HTuple hv_Row2, HTuple hv_Column2, HTuple hv_HeadLength, HTuple hv_HeadWidth)
        {
            // Stack for temporary objects
            HObject[] OTemp = new HObject[20];

            // Local iconic variables

            HObject ho_TempArrow = null;

            // Local control variables

            HTuple hv_Length = null, hv_ZeroLengthIndices = null;
            HTuple hv_DR = null, hv_DC = null, hv_HalfHeadWidth = null;
            HTuple hv_RowP1 = null, hv_ColP1 = null, hv_RowP2 = null;
            HTuple hv_ColP2 = null, hv_Index = null;
            // Initialize local and output iconic variables
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            HOperatorSet.GenEmptyObj(out ho_TempArrow);
            //This procedure generates arrow shaped XLD contours,
            //pointing from (Row1, Column1) to (Row2, Column2).
            //If starting and end point are identical, a contour consisting
            //of a single point is returned.
            //
            //input parameteres:
            //Row1, Column1: Coordinates of the arrows' starting points
            //Row2, Column2: Coordinates of the arrows' end points
            //HeadLength, HeadWidth: Size of the arrow heads in pixels
            //
            //output parameter:
            //Arrow: The resulting XLD contour
            //
            //The input tuples Row1, Column1, Row2, and Column2 have to be of
            //the same length.
            //HeadLength and HeadWidth either have to be of the same length as
            //Row1, Column1, Row2, and Column2 or have to be a single element.
            //If one of the above restrictions is violated, an error will occur.
            //
            //
            //Init
            ho_Arrow.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            //
            //Calculate the arrow length
            HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Length);
            //
            //Mark arrows with identical start and end point
            //(set Length to -1 to avoid division-by-zero exception)
            hv_ZeroLengthIndices = hv_Length.TupleFind(0);
            if ((int)(new HTuple(hv_ZeroLengthIndices.TupleNotEqual(-1))) != 0)
            {
                if (hv_Length == null)
                    hv_Length = new HTuple();
                hv_Length[hv_ZeroLengthIndices] = -1;
            }
            //
            //Calculate auxiliary variables.
            hv_DR = (1.0 * (hv_Row2 - hv_Row1)) / hv_Length;
            hv_DC = (1.0 * (hv_Column2 - hv_Column1)) / hv_Length;
            hv_HalfHeadWidth = hv_HeadWidth / 2.0;
            //
            //Calculate end points of the arrow head.
            hv_RowP1 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) + (hv_HalfHeadWidth * hv_DC);
            hv_ColP1 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) - (hv_HalfHeadWidth * hv_DR);
            hv_RowP2 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) - (hv_HalfHeadWidth * hv_DC);
            hv_ColP2 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) + (hv_HalfHeadWidth * hv_DR);
            //
            //Finally create output XLD contour for each input point pair
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Length.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
            {
                if ((int)(new HTuple(((hv_Length.TupleSelect(hv_Index))).TupleEqual(-1))) != 0)
                {
                    //Create_ single points for arrows with identical start and end point
                    ho_TempArrow.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_TempArrow, hv_Row1.TupleSelect(hv_Index),
                        hv_Column1.TupleSelect(hv_Index));
                }
                else
                {
                    //Create arrow contour
                    ho_TempArrow.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_TempArrow, ((((((((((hv_Row1.TupleSelect(
                        hv_Index))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                        hv_RowP1.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                        hv_RowP2.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)),
                        ((((((((((hv_Column1.TupleSelect(hv_Index))).TupleConcat(hv_Column2.TupleSelect(
                        hv_Index)))).TupleConcat(hv_ColP1.TupleSelect(hv_Index)))).TupleConcat(
                        hv_Column2.TupleSelect(hv_Index)))).TupleConcat(hv_ColP2.TupleSelect(
                        hv_Index)))).TupleConcat(hv_Column2.TupleSelect(hv_Index)));
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Arrow, ho_TempArrow, out ExpTmpOutVar_0);
                    ho_Arrow.Dispose();
                    ho_Arrow = ExpTmpOutVar_0;
                }
            }
            ho_TempArrow.Dispose();

            return;
        }
    }
}