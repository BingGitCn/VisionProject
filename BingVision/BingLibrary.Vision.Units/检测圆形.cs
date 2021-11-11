using HalconDotNet;

namespace BingLibrary.Vision.Units
{
    public class DetectCircle
    {
        /// <summary>
        /// 找圆
        /// </summary>
        /// <param name="Image">当前图像</param>
        /// <param name="CRow">检测圆的圆心</param>
        /// <param name="CColumn">检测圆的圆心</param>
        /// <param name="CRadius">检测圆的半径</param>
        /// <param name="Transition">检测变换，positive,negative,all</param>
        /// <param name="Direct">检测方向，inner,outer</param>
        /// <param name="Elements">检测元素个数</param>
        /// <param name="ActiveElements">生效的元素个数</param>
        /// <param name="DetectHeight">检测范围高度</param>
        /// <param name="select">选择圆，first,last,max</param>
        /// <param name="Sigma">Sigma,默认设1即可</param>
        /// <param name="Threshold">阈值，对比度高则高，反之则低，经验值30</param>
        /// <param name="ArcType">类型，圆弧或圆，arc,circle</param>
        /// <returns></returns>
        public static double[] FindCircle(HImage Image, double CRow, double CColumn, double CRadius, TransitionType Transition, string Direct, int Elements, int ActiveElements, double DetectHeight, SelectType select, double Sigma, int Threshold, string ArcType = "circle")
        {
            double[] results = new double[3];

            // Stack for temporary objects
            HObject[] OTemp = new HObject[20];

            // Local iconic variables

            HObject ho_Regions, ho_ContCircle, ho_Arrow1 = null;
            HObject ho_Circle, ho_Contour = null;

            // Local control variables

            HTuple hv_Width = null, hv_Height = null;
            HTuple hv_ResultRow = null, hv_ResultColumn = null, hv_RowXLD = null;
            HTuple hv_ColXLD = null, hv_Length = null, hv_Length2 = null;
            HTuple hv_DetectWidth = null, hv_i = null, hv_j = new HTuple();
            HTuple hv_RowE = new HTuple(), hv_ColE = new HTuple();
            HTuple hv_ATan = new HTuple(), hv_RowL2 = new HTuple();
            HTuple hv_RowL1 = new HTuple(), hv_ColL2 = new HTuple();
            HTuple hv_ColL1 = new HTuple(), hv_MsrHandle_Measure = new HTuple();
            HTuple hv_RowEdge = new HTuple(), hv_ColEdge = new HTuple();
            HTuple hv_Amplitude = new HTuple(), hv_Distance = new HTuple();
            HTuple hv_tRow = new HTuple(), hv_tCol = new HTuple();
            HTuple hv_t = new HTuple(), hv_Number = new HTuple(), hv_k = new HTuple();
            HTuple hv_StartPhi = new HTuple(), hv_EndPhi = new HTuple();
            HTuple hv_PointOrder = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_ArcType_COPY_INP_TMP = ArcType;
            HTuple hv_Elements_COPY_INP_TMP = Elements;
            HTuple hv_Select_COPY_INP_TMP = select.ToString();
            HTuple hv_Transition_COPY_INP_TMP = Transition.ToString();

            // Initialize local and output iconic variables
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_ContCircle);
            HOperatorSet.GenEmptyObj(out ho_Arrow1);
            HOperatorSet.GenEmptyObj(out ho_Circle);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HTuple hv_RowCenter = new HTuple();
            HTuple hv_ColCenter = new HTuple();
            HTuple hv_Radius = new HTuple();
            //Bing
            //20170705
            HOperatorSet.GetImageSize(Image, out hv_Width, out hv_Height);
            ho_Regions.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Regions);
            hv_ResultRow = new HTuple();
            hv_ResultColumn = new HTuple();

            ho_ContCircle.Dispose();
            HOperatorSet.GenCircleContourXld(out ho_ContCircle, CRow, CColumn, CRadius,
                0, (new HTuple(360)).TupleRad(), "positive", 3);
            HOperatorSet.GetContourXld(ho_ContCircle, out hv_RowXLD, out hv_ColXLD);
            HOperatorSet.LengthXld(ho_ContCircle, out hv_Length);
            HOperatorSet.TupleLength(hv_ColXLD, out hv_Length2);
            if ((int)(new HTuple(hv_Elements_COPY_INP_TMP.TupleLess(1))) != 0)
            {
                ho_Regions.Dispose();
                ho_ContCircle.Dispose();
                ho_Arrow1.Dispose();
                ho_Circle.Dispose();
                ho_Contour.Dispose();
            }
            hv_DetectWidth = (((new HTuple(360)).TupleRad()) * CRadius) / hv_Elements_COPY_INP_TMP;
            HTuple end_val15 = hv_Elements_COPY_INP_TMP - 1;
            HTuple step_val15 = 1;
            for (hv_i = 0; hv_i.Continue(end_val15, step_val15); hv_i = hv_i.TupleAdd(step_val15))
            {
                if ((int)(new HTuple(((hv_RowXLD.TupleSelect(0))).TupleEqual(hv_RowXLD.TupleSelect(
                    hv_Length2 - 1)))) != 0)
                {
                    HOperatorSet.TupleInt(((1.0 * hv_Length2) / (hv_Elements_COPY_INP_TMP - 1)) * hv_i,
                        out hv_j);
                    hv_ArcType_COPY_INP_TMP = "circle";
                }
                else
                {
                    HOperatorSet.TupleInt(((1.0 * hv_Length2) / (hv_Elements_COPY_INP_TMP - 1)) * hv_i,
                        out hv_j);
                    hv_ArcType_COPY_INP_TMP = "arc";
                }
                if ((int)(new HTuple(hv_j.TupleGreaterEqual(hv_Length2))) != 0)
                {
                    hv_j = hv_Length2 - 1;
                    //continue
                }
                hv_RowE = hv_RowXLD.TupleSelect(hv_j);
                hv_ColE = hv_ColXLD.TupleSelect(hv_j);

                //超出图像区域，不检测，否则容易报异常
                if ((int)((new HTuple((new HTuple((new HTuple(hv_RowE.TupleGreater(hv_Height - 1))).TupleOr(
                    new HTuple(hv_RowE.TupleLess(0))))).TupleOr(new HTuple(hv_ColE.TupleGreater(
                    hv_Width - 1))))).TupleOr(new HTuple(hv_ColE.TupleLess(0)))) != 0)
                {
                    continue;
                }
                if (Direct == "inner")
                {
                    HOperatorSet.TupleAtan2((-hv_RowE) + CRow, hv_ColE - CColumn, out hv_ATan);
                    hv_ATan = ((new HTuple(180)).TupleRad()) + hv_ATan;
                }
                else
                {
                    HOperatorSet.TupleAtan2((-hv_RowE) + CRow, hv_ColE - CColumn, out hv_ATan);
                }

                //gen_rectangle2 (Rectangle1, RowE, ColE, ATan, DetectHeight/2, DetectWidth/2)
                //concat_obj (Regions, Rectangle1, Regions)
                if ((int)(new HTuple(hv_i.TupleEqual(0))) != 0)
                {
                    hv_RowL2 = hv_RowE + ((DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                    hv_RowL1 = hv_RowE - ((DetectHeight / 2) * (((-hv_ATan)).TupleSin()));
                    hv_ColL2 = hv_ColE + ((DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
                    hv_ColL1 = hv_ColE - ((DetectHeight / 2) * (((-hv_ATan)).TupleCos()));
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
                HOperatorSet.GenMeasureRectangle2(hv_RowE, hv_ColE, hv_ATan, DetectHeight / 2,
                    hv_DetectWidth / 2, hv_Width, hv_Height, "nearest_neighbor", out hv_MsrHandle_Measure);

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

                HOperatorSet.MeasurePos(Image, hv_MsrHandle_Measure, Sigma, Threshold,
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
                HTuple end_val83 = hv_Number - 1;
                HTuple step_val83 = 1;
                for (hv_k = 0; hv_k.Continue(end_val83, step_val83); hv_k = hv_k.TupleAdd(step_val83))
                {
                    if ((int)(new HTuple(((((hv_Amplitude.TupleSelect(hv_k))).TupleAbs())).TupleGreater(
                        hv_t))) != 0)
                    {
                        hv_tRow = hv_RowEdge.TupleSelect(hv_k);
                        hv_tCol = hv_ColEdge.TupleSelect(hv_k);
                        hv_t = ((hv_Amplitude.TupleSelect(hv_k))).TupleAbs();
                    }
                }
                if ((int)(new HTuple(hv_t.TupleGreater(0))) != 0)
                {
                    hv_ResultRow = hv_ResultRow.TupleConcat(hv_tRow);
                    hv_ResultColumn = hv_ResultColumn.TupleConcat(hv_tCol);
                }
            }

            hv_RowCenter = 0;
            hv_ColCenter = 0;
            hv_Radius = 0;
            hv_Elements_COPY_INP_TMP = 15;
            ho_Circle.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Circle);
            HOperatorSet.TupleLength(hv_ResultColumn, out hv_Length);

            if ((int)((new HTuple(hv_Length.TupleGreaterEqual(hv_Elements_COPY_INP_TMP))).TupleAnd(
                new HTuple(hv_Elements_COPY_INP_TMP.TupleGreater(2)))) != 0)
            {
                ho_Contour.Dispose();
                HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_ResultRow, hv_ResultColumn);
                HOperatorSet.FitCircleContourXld(ho_Contour, "geotukey", -1, 0, 0, 3, 2, out hv_RowCenter,
                    out hv_ColCenter, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);

                HOperatorSet.TupleLength(hv_StartPhi, out hv_Length1);
                if ((int)(new HTuple(hv_Length1.TupleLess(1))) != 0)
                {
                    ho_Regions.Dispose();
                    ho_ContCircle.Dispose();
                    ho_Arrow1.Dispose();
                    ho_Circle.Dispose();
                    ho_Contour.Dispose();
                }
                if ((int)(new HTuple(hv_ArcType_COPY_INP_TMP.TupleEqual("arc"))) != 0)
                {
                    ho_Circle.Dispose();
                    HOperatorSet.GenCircleContourXld(out ho_Circle, hv_RowCenter, hv_ColCenter,
                        hv_Radius, hv_StartPhi, hv_EndPhi, hv_PointOrder, 1);
                }
                else
                {
                    ho_Circle.Dispose();
                    HOperatorSet.GenCircleContourXld(out ho_Circle, hv_RowCenter, hv_ColCenter,
                        hv_Radius, 0, (new HTuple(360)).TupleRad(), hv_PointOrder, 1);
                }
            }

            ho_Regions.Dispose();
            ho_ContCircle.Dispose();
            ho_Arrow1.Dispose();
            ho_Circle.Dispose();
            ho_Contour.Dispose();
            results[0] = hv_RowCenter.D;
            results[1] = hv_ColCenter.D;
            results[2] = hv_Radius.D;
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