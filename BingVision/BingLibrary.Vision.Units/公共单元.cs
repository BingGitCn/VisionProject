namespace BingLibrary.Vision.Units
{
    #region 检测

    public enum ThresholdType { White, Black }

    public enum TransitionType { positive, negative, all }

    public enum SelectType { first, last, max }

    #endregion 检测

    #region 标定

    //点位
    public class CoordinatePoint
    {
        public double X { set; get; }
        public double Y { set; get; }
        public double U { set; get; }  // 角度预留
        public string Commit { set; get; }  // 点位注释预留
    }

    public enum CalibrateType { NicePoints, TwentyFivePoint }  // 九点标定，二十五点标定

    #endregion 标定

    #region
    public enum BarcodeType { 条形码, 二维码, DM码 }
    #endregion
}