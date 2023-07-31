using BingLibrary.Extension;
using HalconDotNet;
using System;
using System.Threading.Tasks;

namespace BingLibrary.Vision
{
    /// <summary>
    /// ������ʾ������
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

        //�Ƿ����ڻ���
        public bool IsDrawing = false;

        //��ʾˮӡ
        public bool IsShowWaterString = false;

        //�Ƿ�༭���ƶ���roi��
        public bool CanEdit = false;

        //��ʾģʽ
        public HalconDrawing DrawMode = HalconDrawing.margin;

        //���ƽ�����ʽ
        public HalconDrawMode DrawFinishMode = HalconDrawMode.rightButton;

        //�϶�ģʽ
        private HalconMouseMode viewMode = HalconMouseMode.View_Move;

        //��ʾģʽ
        private HalconMouseMode dispROI = HalconMouseMode.Include_ROI;

        //����ϵ��
        private double zoomWndFactor = 1.0;

        //��갴��ʱ������
        private double startX, startY;

        //����
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
                //��ʼ������
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
        /// ����ROIController
        /// </summary>
        /// <param name="rc"></param>
        public void SetROIController(ROIController rc)
        {
            roiController = rc;
        }

        /// <summary>
        /// ����DispObjectController
        /// </summary>
        /// <param name="dc"></param>
        public void SetDispObjectController(DispObjectController dc)
        {
            dispObjectController = dc;
        }

        /// <summary>
        /// ����MessageController
        /// </summary>
        /// <param name="mc"></param>
        public void SetMessageController(MessageController mc)
        {
            messageController = mc;
        }

        private void ViewPort_HMouseDown(object sender, HMouseEventArgsWPF e)
        {
            //�Ҽ��������ڻ滭����������
            if (e.Button == System.Windows.Input.MouseButton.Right || IsDrawing)
                return;

            startX = e.Column;
            startY = e.Row;

            if (roiController != null && dispROI == HalconMouseMode.Include_ROI && CanEdit)
            {
                int mouse_X0, mouse_Y0;//������¼�������ʱ������λ��
                int tempNum = 0;
                this.hWindowControlWPF.HalconWindow.GetMposition(out mouse_X0, out mouse_Y0, out tempNum);
                //�ж��Ƿ��ڶ�Ӧ��ROI������
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
            //�Ҽ��������ڻ滭����������
            if (e.Button == System.Windows.Input.MouseButton.Right || IsDrawing)
                return;
            //
            if (roiController == null || dispROI != HalconMouseMode.Include_ROI)
                return;

            //�����������ʱ��������
            if (e.Button != System.Windows.Input.MouseButton.Left)
            {
                return;
            }

            double motionX, motionY;

            roiController.MouseMoveROI(e.Column, e.Row);//��꾭����index
            //���Ա༭roi
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
            //�����ƶ�ͼ��
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

        //���ڴ�С�仯��ͼ����Ӧ����
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
        /// ����ͼ����ʾ����
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
        /// ��Ӧͼ��
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
        /// ��Ӧ����
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
        /// �ƶ�ͼ��
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
        /// ˢ��
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
                window.SetColor(HalconColors.��ɫ.ToDescription());
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
        /// ��ʾˮӡ
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
        /// ͼ����ʾ������
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
        /// ͼ����ʾ��Ӧ����
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