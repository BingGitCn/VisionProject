using BingLibrary.Extension;
using HalconDotNet;
using System;
using System.Threading.Tasks;

namespace BingLibrary.Vision
{
    /// <summary>
    /// 窗口显示控制器
    /// </summary>
    public class WindowController
    {
        public HWindowControlWPF hWindowControlWPF;

        public HImage image = new HImage();

        private string WaterString = "Powered by Leader";
        private int WaterFontSize = 36;

        private ROIController roiController;
        private MessageController messageController;
        private DispObjectController dispObjectController;

        //是否正在绘制
        public bool IsDrawing = false;

        //显示水印
        public bool IsShowWaterString = false;

        //是否编辑（移动）roi框
        public bool CanEdit = false;

        //显示模式
        public HalconDrawing DrawMode = HalconDrawing.margin;

        //绘制结束方式
        public HalconDrawMode DrawFinishMode = HalconDrawMode.rightButton;

        //拖动模式
        private HalconMouseMode viewMode = HalconMouseMode.View_Move;

        //显示模式
        private HalconMouseMode dispROI = HalconMouseMode.Include_ROI;

        //缩放系数
        private double zoomWndFactor = 1.0;

        //鼠标按下时的坐标
        private double startX, startY;

        //字体
        private HTuple hv_Font = new HTuple();

        private HTuple hv_OS = new HTuple();

        public WindowController(HWindowControlWPF windowControlWPF)
        {
            hWindowControlWPF = windowControlWPF;
            zoomWndFactor = (double)imageWidth / hWindowControlWPF.ActualWidth;

            hWindowControlWPF.HMouseDown += ViewPort_HMouseDown;
            hWindowControlWPF.HMouseUp += ViewPort_HMouseUp;
            hWindowControlWPF.HMouseMove += ViewPort_HMouseMove;
            hWindowControlWPF.HMouseWheel += ViewPort_HMouseWheel;
            hWindowControlWPF.SizeChanged += ViewPort_SizeChanged;

            initFont(hWindowControlWPF.HalconWindow);
        }

        private void initFont(HTuple window)
        {
            try
            {
                //初始化字体
                hv_Font.Dispose();
                HOperatorSet.QueryFont(window, out hv_Font);
                hv_OS.Dispose();
                HOperatorSet.GetSystem("operating_system", out hv_OS);
                if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetFont(window, (hv_Font.TupleSelect(0)) + "-" + WaterFontSize.ToString());
                        HOperatorSet.SetFont(window, "Microsoft JhengHei" + "-" + WaterFontSize.ToString());
                    }
                }
                else
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetFont(window, (hv_Font.TupleSelect(0)) + "-" + WaterFontSize.ToString());
                        HOperatorSet.SetFont(window, "Microsoft JhengHei" + "-" + WaterFontSize.ToString());
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 设置ROIController
        /// </summary>
        /// <param name="rc"></param>
        public void SetROIController(ROIController rc)
        {
            roiController = rc;
        }

        /// <summary>
        /// 设置DispObjectController
        /// </summary>
        /// <param name="dc"></param>
        public void SetDispObjectController(DispObjectController dc)
        {
            dispObjectController = dc;
        }

        /// <summary>
        /// 设置MessageController
        /// </summary>
        /// <param name="mc"></param>
        public void SetMessageController(MessageController mc)
        {
            messageController = mc;
        }

        private void ViewPort_HMouseDown(object sender, HMouseEventArgsWPF e)
        {
            //右键或者正在绘画，则不做处理
            if (e.Button == System.Windows.Input.MouseButton.Right || IsDrawing)
                return;

            startX = e.Column;
            startY = e.Row;

            if (roiController != null && dispROI == HalconMouseMode.Include_ROI && CanEdit)
            {
                int mouse_X0, mouse_Y0;//用来记录按下鼠标时的坐标位置
                int tempNum = 0;
                this.hWindowControlWPF.HalconWindow.GetMposition(out mouse_X0, out mouse_Y0, out tempNum);
                //判断是否在对应的ROI区域内
                roiController.ActiveROIidx = roiController.MouseDownAction(mouse_Y0, mouse_X0);
            }
        }

        private void ViewPort_HMouseUp(object sender, HMouseEventArgsWPF e)
        {
            if (DrawFinishMode == HalconDrawMode.directly)
                if (e.Button == System.Windows.Input.MouseButton.Left)
                    if (IsDrawing)
                        HalconMicroSoft.FinishDraw();
            Repaint();
        }

        private void ViewPort_HMouseWheel(object sender, HMouseEventArgsWPF e)
        {
            zoomImage(e.Column, e.Row, e.Delta > 0 ? 0.9 : 1 / 0.9);
        }

        private void ViewPort_HMouseMove(object sender, HMouseEventArgsWPF e)
        {
            //右键或者正在绘画，则不做处理
            if (e.Button == System.Windows.Input.MouseButton.Right || IsDrawing)
                return;
            //
            if (roiController == null || dispROI != HalconMouseMode.Include_ROI)
                return;

            //仅在左键按下时候起作用
            if (e.Button != System.Windows.Input.MouseButton.Left)
            {
                return;
            }

            double motionX, motionY;

            roiController.MouseMoveROI(e.Column, e.Row);//鼠标经过的index
            //可以编辑roi
            if (roiController != null && (roiController.ActiveROIidx != -1) && dispROI == HalconMouseMode.Include_ROI && CanEdit)
            {
                motionX = e.Column - startX;
                motionY = e.Row - startY;

                if (((int)motionX != 0) || ((int)motionY != 0))
                {
                    roiController.MouseMoveAction(e.Column, e.Row, motionX, motionY);

                    startX = e.Column;
                    startY = e.Row;
                }
                Repaint();
            }
            //否则移动图像
            else if (viewMode == HalconMouseMode.View_Move)
            {
                if (startX == 0 && startY == 0)
                    return;

                motionX = e.Column - startX;
                motionY = e.Row - startY;

                if (((int)motionX != 0) || ((int)motionY != 0))
                {
                    moveImage(motionX, motionY);
                    startX = e.Column - motionX;
                    startY = e.Row - motionY;
                }
            }
        }

        //窗口大小变化是图像适应窗口
        private async void ViewPort_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            await Task.Delay(1);
            FitImageToWindow();
            Repaint();
        }

        private int imageWidth;
        private int imageHeight;

        private double ImgRow1, ImgCol1, ImgRow2, ImgCol2;

        /// <summary>
        /// 设置图像显示区域
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="c1"></param>
        /// <param name="r2"></param>
        /// <param name="c2"></param>
        private void setImagePart(int r1, int c1, int r2, int c2)
        {
            ImgRow1 = r1;
            ImgCol1 = c1;
            ImgRow2 = imageHeight = r2;
            ImgCol2 = imageWidth = c2;

            System.Windows.Rect rect = hWindowControlWPF.ImagePart;
            rect.X = (int)ImgCol1;
            rect.Y = (int)ImgRow1;
            rect.Height = (int)imageHeight;
            rect.Width = (int)imageWidth;
            hWindowControlWPF.ImagePart = rect;
        }

        /// <summary>
        /// 适应图像
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="scale"></param>
        private void zoomImage(double x, double y, double scale)
        {
            double lengthC, lengthR;
            double percentC, percentR;
            int lenC, lenR;

            percentC = (x - ImgCol1) / (ImgCol2 - ImgCol1);
            percentR = (y - ImgRow1) / (ImgRow2 - ImgRow1);

            lengthC = (ImgCol2 - ImgCol1) * scale;
            lengthR = (ImgRow2 - ImgRow1) * scale;

            ImgCol1 = x - lengthC * percentC;
            ImgCol2 = x + lengthC * (1 - percentC);

            ImgRow1 = y - lengthR * percentR;
            ImgRow2 = y + lengthR * (1 - percentR);

            lenC = (int)Math.Round(lengthC);
            lenR = (int)Math.Round(lengthR);

            System.Windows.Rect rect = hWindowControlWPF.ImagePart;
            rect.X = (int)Math.Round(ImgCol1);
            rect.Y = (int)Math.Round(ImgRow1);
            rect.Width = (lenC > 0) ? lenC : 1;
            rect.Height = (lenR > 0) ? lenR : 1;
            hWindowControlWPF.ImagePart = rect;

            zoomWndFactor *= scale;
            Repaint();
        }

        /// <summary>
        /// 适应窗口
        /// </summary>
        /// <param name="scaleFactor"></param>
        private void zoomImage(double scaleFactor)
        {
            double midPointX, midPointY;

            if (((ImgRow2 - ImgRow1) == scaleFactor * imageHeight) &&
                ((ImgCol2 - ImgCol1) == scaleFactor * imageWidth))
            {
                Repaint();
                return;
            }

            ImgRow2 = ImgRow1 + imageHeight;
            ImgCol2 = ImgCol1 + imageWidth;

            midPointX = ImgCol1;
            midPointY = ImgRow1;

            zoomWndFactor = (double)imageWidth / hWindowControlWPF.ActualWidth;
            zoomImage(midPointX, midPointY, scaleFactor);
        }

        /// <summary>
        /// 移动图像
        /// </summary>
        /// <param name="motionX"></param>
        /// <param name="motionY"></param>
        private void moveImage(double motionX, double motionY)
        {
            ImgRow1 += -motionY;
            ImgRow2 += -motionY;

            ImgCol1 += -motionX;
            ImgCol2 += -motionX;

            System.Windows.Rect rect = hWindowControlWPF.ImagePart;
            rect.X = (int)Math.Round(ImgCol1);
            rect.Y = (int)Math.Round(ImgRow1);
            hWindowControlWPF.ImagePart = rect;

            Repaint();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Repaint()
        {
            repaint(hWindowControlWPF.HalconWindow);
        }

        private void repaint(HalconDotNet.HWindow window)
        {
            HSystem.SetSystem("flush_graphic", "false");
            try
            {
                int h = imageHeight;
                if (window.IsInitialized() == false || hWindowControlWPF.HalconID.ToInt64() == -1 || hWindowControlWPF.ImagePart.Width <= 1 || hWindowControlWPF.ImagePart.Height <= 1)
                    return;

                window.ClearWindow();
                window.DispObj(image);

                if (roiController != null && (dispROI == HalconMouseMode.Include_ROI))
                    roiController.PaintData(window, DrawMode);

                window.SetDraw(DrawMode.ToDescription());
                window.SetColor(HalconColors.黑色.ToDescription());
                window.DispLine(-100.0, -100.0, -101.0, -101.0);

                if (IsShowWaterString)
                {
                    try
                    {
                        HOperatorSet.DispText(window, WaterString, HalconCoordinateSystem.window.ToDescription(), "bottom",
                            "left", "black", "box_color", "#ffffff77");
                    }
                    catch { }
                }

                var dispObjectList = dispObjectController.GetDispObjectList();
                for (int i = 0; i < dispObjectList.Count; i++)
                {
                    window.SetColor(dispObjectList[i].ShowColor.ToDescription());
                    window.DispObj(dispObjectList[i].ShowObject);
                }

                var messageList = messageController.GetMessageList();

                for (int i = 0; i < messageList.Count; i++)
                {
                    try
                    {
                        if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetFont(window, (hv_Font.TupleSelect(0)) + "-" + messageList[i].ShowFontSize.ToString());
                            }
                        }
                        else
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetFont(window, (hv_Font.TupleSelect(0)) + "-" + messageList[i].ShowFontSize.ToString());
                            }
                        }
                        HOperatorSet.DispText(window, messageList[i].ShowContent, messageList[i].ShowMode.ToDescription(), new HTuple(messageList[i].PositionX),
                            new HTuple(messageList[i].PositionY), messageList[i].ShowColor.ToDescription(), "box_color", "#ffffff77");
                    }
                    catch { }
                }
                try
                {
                    hv_Font.Dispose();
                    hv_OS.Dispose();
                }
                catch { }
                HOperatorSet.SetFont(window, "Microsoft JhengHei" + "-" + WaterFontSize.ToString());
            }
            catch { }
            HSystem.SetSystem("flush_graphic", "true");
        }

        /// <summary>
        /// 显示水印
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="fontSize"></param>
        /// <param name="isShow"></param>
        public void ShowWaterStringToWindow(string msg, int fontSize = 36, bool isShow = true)
        {
            WaterString = msg;
            WaterFontSize = fontSize;
            IsShowWaterString = isShow;
        }

        private bool isFirstShowImage = true;

        /// <summary>
        /// 图像显示到窗口
        /// </summary>
        /// <param name="image"></param>
        public void ShowImageToWindow(HImage image)
        {
            this.image?.Dispose();
            this.image = image;
            Repaint();
            if (isFirstShowImage)
            {
                isFirstShowImage = false;
                FitImageToWindow();
            }
        }

        /// <summary>
        /// 图像显示适应窗口
        /// </summary>
        public void FitImageToWindow()
        {
            try
            {
                if (image != null)
                {
                    int h, w;
                    string s;

                    image.GetImagePointer1(out s, out w, out h);

                    if ((h != imageHeight) || (w != imageWidth))
                    {
                        int _beginRow, _begin_Col, _endRow, _endCol;
                        double ratio_win = (double)hWindowControlWPF.ActualWidth / (double)hWindowControlWPF.ActualHeight;
                        double ratio_img = (double)w / (double)h;
                        imageHeight = h;
                        imageWidth = w;
                        if (ratio_win >= ratio_img)
                        {
                            _beginRow = 0;
                            _endRow = h - 1;
                            _begin_Col = (int)(-w * (ratio_win / ratio_img - 1d) / 2d);
                            _endCol = (int)(w + w * (ratio_win / ratio_img - 1d) / 2d);
                            zoomWndFactor = (double)h / hWindowControlWPF.ActualHeight;
                        }
                        else
                        {
                            _begin_Col = 0;
                            _endCol = w - 1;
                            _beginRow = (int)(-h * (ratio_img / ratio_win - 1d) / 2d);
                            _endRow = (int)(h + h * (ratio_img / ratio_win - 1d) / 2d);
                            zoomWndFactor = (double)w / hWindowControlWPF.ActualWidth;
                        }
                        setImagePart(_beginRow, _begin_Col, (int)hWindowControlWPF.ActualHeight, (int)hWindowControlWPF.ActualWidth);
                        zoomImage(zoomWndFactor);
                    }
                }
            }
            catch { }
        }
    }
}