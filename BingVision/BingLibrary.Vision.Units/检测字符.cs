using BingLibrary.FileOpreate;
using HalconDotNet;
using System;

namespace BingLibrary.Vision.Units
{
    public class DetectPrint
    {
        private HWindow hWindow;
        public Data data { set; get; }

        public class Data
        {
            /// <summary>
            /// 检测区域的ID，请勿重复
            /// </summary>
            public int ID { set; get; }

            /// <summary>
            /// 模板图像
            /// </summary>
            public HImage Image { set; get; }

            /// <summary>
            /// 绘画的检测区域
            /// </summary>
            public HRegion InspectRegion { set; get; } = new HRegion();

            /// <summary>
            /// 自动生成的检测子区域
            /// </summary>
            public HRegion EachInspectRegions { set; get; } = new HRegion();

            /// <summary>
            /// 二值化区间最小值
            /// </summary>
            public double MinGray { set; get; } = 10;

            /// <summary>
            /// 二值化区间最大值
            /// </summary>
            public double MaxGray { set; get; } = 128;

            /// <summary>
            /// 过滤的最小面积
            /// </summary>
            public double Area { set; get; } = 5;

            /// <summary>
            /// 过滤的宽度
            /// </summary>
            public double Width { set; get; } = 5;

            /// <summary>
            /// 过滤的长度
            /// </summary>
            public double Height { set; get; } = 5;

            /// <summary>
            /// 字符连接的宽度
            /// </summary>
            public int WidthSpace { set; get; } = 11;

            /// <summary>
            /// 字符连接的高度
            /// </summary>
            public int HeightSpace { set; get; } = 7;

            /// <summary>
            /// 所有字符的模板
            /// </summary>
            public HShapeModel[] HShapeModels { set; get; }

            public double ShapeScore { set; get; } = 60;

            /// <summary>
            /// 检测结果的面积过滤
            /// </summary>
            public double ResultArea { set; get; } = 5;
        }

        /// <summary>
        /// 设置窗口句柄
        /// </summary>
        /// <param name="hw"></param>
        public void SetHwindow(HWindow hw)
        {
            hWindow = hw;
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="d"></param>
        public void SetData(Data d)
        {
            data = d;
        }

        /// <summary>
        /// 绘制检测区域
        /// </summary>
        /// <returns></returns>
        public void DrawInspectRegion()
        {
            data.InspectRegion = new HRegion();
            data.InspectRegion.DrawRegion(hWindow);
        }

        /// <summary>
        /// 绘制屏蔽检测的区域
        /// </summary>
        /// <returns></returns>
        public void DrawIgnoreRegion()
        {
            HRegion region = new HRegion();
            region.DrawRegion(hWindow);
            data.InspectRegion = data.InspectRegion.Difference(region);
        }

        /// <summary>
        /// 自动计算获取的每个检测区域
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void GetEachInspectRegions()
        {
            data.EachInspectRegions = data.Image.ReduceDomain(data.InspectRegion).Threshold(data.MinGray, data.MaxGray).Connection().SelectShape("area","and",data.Area,9999999).SelectShape("width", "and", data.Width, 9999999).SelectShape("height", "and", data.Area, 9999999).Union1()
                 .ClosingRectangle1(data.WidthSpace, data.HeightSpace)//闭运算连接相邻字符
                 .Connection().ShapeTrans("rectangle1")//转换成矩形
                 .DilationRectangle1(11, 11);//适当膨胀矩形区域
        }

        /// <summary>
        /// 学习各字符特征，保存模板
        /// </summary>
        /// <param name="inspectRegions"></param>
        public void LearnInspectRegion()
        {
            int number = data.EachInspectRegions.CountObj();
            data.HShapeModels = new HShapeModel[number + 1];//0为空
            for (int i = 1; i <= number; i++)
            {
                data.HShapeModels[i] = data.Image.ReduceDomain(data.EachInspectRegions.SelectObj(i))
                        .CreateShapeModel(3, (new HTuple(-20)).TupleRad(), (new HTuple(40)).TupleRad(), "auto", "no_pregeneration", "use_polarity", "auto", "auto");
            }
        }

        /// <summary>
        /// 读data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Data OpenData(int id)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "zifu" + id + ".data";
            return Serialize.ReadJson<Data>(path);
        }

        /// <summary>
        /// 写data
        /// </summary>
        /// <param name="data"></param>
        public void SaveData()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "zifu" + data.ID + ".data";
            Serialize.WriteJson(data, path);
        }

        /// <summary>
        /// 检测字符是否正确,可能耗时较长
        /// </summary>
        public HRegion Check(HImage image,int dilationSize =31)
        {
            HRegion hRegion = new HRegion();
            hRegion.GenEmptyRegion();
            int number = data.EachInspectRegions.CountObj();
            for (int i = 1; i <= number; i++)
            {
                HTuple row1, col1, angle1, row2, col2, angle2;
                HTuple score;
                HImage imagePart1 = data.Image.ReduceDomain(data.EachInspectRegions.SelectObj(i).DilationRectangle1(dilationSize, dilationSize)).CropDomain();
                imagePart1.FindShapeModel(
                    data.HShapeModels[i], new HTuple(-20).TupleRad(), new HTuple(40).TupleRad(),
                    0.3, 1, 0.5, "least_squares", 3, 0.5, out row1, out col1, out angle1, out score);
                HImage imagePart2 = image.ReduceDomain(data.EachInspectRegions.SelectObj(i).DilationRectangle1(dilationSize, dilationSize)).CropDomain();
                imagePart2.FindShapeModel(
            data.HShapeModels[i], new HTuple(-20).TupleRad(), new HTuple(40).TupleRad(),
            0.3, 1, 0.5, "least_squares", 3, 0.5, out row2, out col2, out angle2, out score);
                try
                {
                    if (score > data.ShapeScore / 100.0)
                    {
                        HHomMat2D hHomMat2D = new HHomMat2D();
                        hHomMat2D.VectorAngleToRigid(row1, col1, angle1, row2, col2, angle2);

                        HTuple w, h;
                        imagePart1.GetImageSize(out w, out h);
                        data.EachInspectRegions.SelectObj(i).AreaCenter(out row2, out col2);

                        HRegion mRegion = data.EachInspectRegions.SelectObj(i).MoveRegion((int)(h.D / 2 - row2.D), (int)(w.D / 2 - col2.D));
                        HRegion inspectRegion = mRegion.AffineTransRegion(hHomMat2D, "nearest_neighbor");
                        imagePart1 = imagePart1.AffineTransImage(hHomMat2D, "constant", "false");

                        HImage subImage = imagePart1.SubImage(imagePart2, 1.0, 128.0);
                        HRegion resultRegion = subImage.ReduceDomain(inspectRegion).Threshold((new HTuple(0)).TupleConcat(235), (new HTuple(30)).TupleConcat(255)).Union1().OpeningCircle(0.5);
                        if (resultRegion.Area > data.ResultArea)
                        {
                            resultRegion = resultRegion.MoveRegion((int)(-h.D / 2 + row2.D), (int)(-w.D / 2 + col2.D));
                            hRegion = hRegion.ConcatObj(resultRegion);
                        }

                    }
                    else
                    {
                        hRegion = hRegion.ConcatObj(data.EachInspectRegions.SelectObj(i));
                    }
                }
                catch (Exception ex){
                    hRegion = hRegion.ConcatObj(data.EachInspectRegions.SelectObj(i));
                }


            }
            return hRegion;
        }
    }
}