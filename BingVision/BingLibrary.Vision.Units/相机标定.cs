using HalconDotNet;

namespace BingLibrary.Vision.Units
{
    public class CameraCalibrate
    {
        //九点标定像素和世界坐标定义,提前设定好
        public CoordinatePoint[] CalibPixelPoints = new CoordinatePoint[9];

        public CoordinatePoint[] CalibWorldPoints = new CoordinatePoint[9];

        //三点旋转中心像素和世界坐标定义，提前设定好
        public CoordinatePoint[] RotatePixelPoints = new CoordinatePoint[3];

        public CoordinatePoint[] RotateWorldPoints = new CoordinatePoint[3];

        //  [5]****[4]****[3]
        //  *****************
        //  *****************
        //  [6]****[1]****[2]
        //  *****************
        //  *****************
        //  [7]****[8]****[9]
        //  起始点1，终点9，CalibPixelPoints[0]中存放起始点1。

        //  [17]****[16]****[15]****[14]****[13]
        //  ************************************
        //  ************************************
        //  [18]****[05]****[04]****[03]****[12]
        //  ************************************
        //  ************************************
        //  [19]****[06]****[01]****[02]****[11]
        //  ************************************
        //  ************************************
        //  [20]****[07]****[08]****[09]****[10]
        //  ************************************
        //  ************************************
        //  [21]****[22]****[23]****[24]****[25]
        //  起始点1，终点25，CalibPixelPoints[0]中存放起始点1。

        public CameraCalibrate(CalibrateType calibrateType)
        {
            if (calibrateType == CalibrateType.NicePoints)
            {
                CoordinatePoint[] CalibPixelPoints = new CoordinatePoint[9];
                CoordinatePoint[] CalibWorldPoints = new CoordinatePoint[9];
            }
            else
            {
                CoordinatePoint[] CalibPixelPoints = new CoordinatePoint[25];
                CoordinatePoint[] CalibWorldPoints = new CoordinatePoint[25];
            }
        }

        /// <summary>
        /// 多点标定，保存结果
        /// </summary>
        /// <param name="filePath">保存文件路径.tup</param>
        public void CreateCalibrateFile(string filePath)
        {
            HTuple calibHomMat2D1, calibHomMat2D2;

            HTuple px = new HTuple();
            HTuple py = new HTuple();
            HTuple qx = new HTuple();
            HTuple qy = new HTuple();
            for (int i = 0; i < CalibPixelPoints.Length; i++)
            {
                px = px.TupleConcat(CalibPixelPoints[i].X);
                py = py.TupleConcat(CalibPixelPoints[i].Y);
                qx = qx.TupleConcat(CalibWorldPoints[i].X);
                qx = qx.TupleConcat(CalibWorldPoints[i].Y);
            }

            HOperatorSet.VectorToHomMat2d(px, py, qx, qy, out calibHomMat2D1);

            var roxy = rotateCenter(RotatePixelPoints[0].X, RotatePixelPoints[0].Y, RotatePixelPoints[1].X, RotatePixelPoints[1].Y, RotatePixelPoints[2].X, RotatePixelPoints[2].Y);
            HTuple hv_Qx, hv_Qy;
            HOperatorSet.AffineTransPoint2d(calibHomMat2D1, roxy[0], roxy[1], out hv_Qx, out hv_Qy);
            double bx = CalibPixelPoints[0].X - hv_Qx.D;
            double by = CalibPixelPoints[0].Y - hv_Qy.D;

            for (int i = 0; i < CalibPixelPoints.Length; i++)
            {
                px = px.TupleConcat(CalibPixelPoints[i].X);
                py = py.TupleConcat(CalibPixelPoints[i].Y);
                qx = qx.TupleConcat(CalibWorldPoints[i].X) + bx;
                qx = qx.TupleConcat(CalibWorldPoints[i].Y) + by;
            }

            HOperatorSet.VectorToHomMat2d(px, py, qx, qy, out calibHomMat2D2);
            HOperatorSet.WriteTuple(calibHomMat2D2, filePath);
        }

        /// <summary>
        /// 读取标定文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public HTuple ReadCalibrateFile(string path)
        {
            HTuple calibHomMat2D;
            HOperatorSet.ReadTuple(path, out calibHomMat2D);
            return calibHomMat2D;
        }

        /// <summary>
        /// 将像素坐标转换成世界坐标
        /// </summary>
        /// <param name="pixelPoint">像素坐标</param>
        /// <param name="Path">标定文件路径.tup</param>
        /// <returns></returns>
        public CoordinatePoint PixelToWorld(CoordinatePoint pixelPoint, HTuple calibHomMat2D)
        {
            HTuple qx, qy;
            HOperatorSet.AffineTransPoint2d(calibHomMat2D, pixelPoint.X, pixelPoint.Y, out qx, out qy);
            CoordinatePoint worldPoint = new CoordinatePoint();
            worldPoint.X = qx.D;
            worldPoint.Y = qy.D;
            return worldPoint;
        }

        /// <summary>
        /// 计算旋转中心
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <returns></returns>
        private double[] rotateCenter(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            double a, b, c, d, e, f;
            a = 2 * (x2 - x1);
            b = 2 * (y2 - y1);
            c = x2 * x2 + y2 * y2 - x1 * x1 - y1 * y1;
            d = 2 * (x3 - x2);
            e = 2 * (y3 - y2);
            f = x3 * x3 + y3 * y3 - x2 * x2 - y2 * y2;

            double x = (b * f - e * c) / (b * d - e * a);
            double y = (d * c - a * f) / (b * d - e * a);
            double[] xy = new double[2];
            xy[0] = x;
            xy[1] = y;
            return xy;
        }
    }

    public class CameraCalibrateSimilarity
    {
        //标定像素和世界坐标定义,提前设定好
        public CoordinatePoint[] CalibPixelPoints = new CoordinatePoint[2];

        public CoordinatePoint[] CalibWorldPoints = new CoordinatePoint[2];

        public CameraCalibrateSimilarity(int pointCount)
        {
            CoordinatePoint[] CalibPixelPoints = new CoordinatePoint[pointCount];
            CoordinatePoint[] CalibWorldPoints = new CoordinatePoint[pointCount];
        }

        /// <summary>
        /// 多点近似标定，适用于XY分离，无需计算旋转的情况，相机需标准安装
        /// </summary>
        /// <param name="filePath">.tup</param>
        public void CreateCalibrateFile(string filePath)
        {
            HTuple px = new HTuple();
            HTuple py = new HTuple();
            HTuple qx = new HTuple();
            HTuple qy = new HTuple();
            for (int i = 0; i < CalibPixelPoints.Length; i++)
            {
                px = px.TupleConcat(CalibPixelPoints[i].X);
                py = py.TupleConcat(CalibPixelPoints[i].Y);
                qx = qx.TupleConcat(CalibWorldPoints[i].X);
                qx = qx.TupleConcat(CalibWorldPoints[i].Y);
            }
            HTuple calibHomMat2D;
            HOperatorSet.VectorToSimilarity(px, py, qx, qy, out calibHomMat2D);

            HOperatorSet.WriteTuple(calibHomMat2D, filePath);
        }

        /// <summary>
        /// 读取标定文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public HTuple ReadCalibrateFile(string path)
        {
            HTuple calibHomMat2D;
            HOperatorSet.ReadTuple(path, out calibHomMat2D);
            return calibHomMat2D;
        }

        /// <summary>
        /// 将像素距离转换世界距离
        /// </summary>
        /// <param name="pixelDistance">偏离的像素距离</param>
        /// <param name="Path"></param>
        /// <returns></returns>
        public CoordinatePoint PixelToWorld2(CoordinatePoint pixelPoint, HTuple calibHomMat2D)
        {
            HTuple qx, qy;
            HOperatorSet.AffineTransPoint2d(calibHomMat2D, pixelPoint.X, pixelPoint.Y, out qx, out qy);
            CoordinatePoint worldPoint = new CoordinatePoint();
            worldPoint.X = qx.D;
            worldPoint.Y = qy.D;
            return worldPoint;
        }
    }
}