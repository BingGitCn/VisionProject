using HalconDotNet;

namespace BingLibrary.Vision
{
    public class BingImageWindowData
    {
        /// <summary>
        /// 显示控件
        /// </summary>
        public WindowController WindowCtrl { set; get; }

        /// <summary>
        /// ROI相关
        /// </summary>
        public ROIController ROICtrl { set; get; }

        /// <summary>
        /// 显示消息相关
        /// </summary>
        public MessageController MessageCtrl { set; get; }

        /// <summary>
        /// 其余需要显示在窗口的
        /// </summary>
        public DispObjectController DispObjectCtrl { set; get; }

        /// <summary>
        /// 初始化窗口
        /// </summary>
        /// <param name="hWin"></param>
        internal void Init(HWindowControlWPF hwcw)
        {
            ROICtrl = new ROIController();
            MessageCtrl = new MessageController();
            DispObjectCtrl = new DispObjectController();
            WindowCtrl = new WindowController(hwcw);

            WindowCtrl.SetROIController(ROICtrl);
            WindowCtrl.SetMessageController(MessageCtrl);
            WindowCtrl.SetDispObjectController(DispObjectCtrl);
        }

        //public HWndCtrl HCtrl { set; get; }
        ///// <summary>
        ///// 是否进入绘画模式
        ///// </summary>
        ///// <param name="isUse"></param>
        //public void DrawMode(bool isUse)
        //{
        //    HCtrl.isDrawing = isUse;
        //    HCtrl.hWindowControlWPF.HalconWindow.SetColor("green");
        //    HalconAPI.CancelDraw();
        //}
    }
}