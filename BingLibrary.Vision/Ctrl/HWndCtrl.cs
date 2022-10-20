using BingLibrary.Extension;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingLibrary.Vision
{
    public class HWndCtrl
    {
        #region ����

        public HWindowControlWPF hWindowControlWPF;

        public ROIController roiManager;

        //�Ƿ����ڻ���
        public bool isDrawing = false;

        //�Ƿ�༭���ƶ���roi��
        public bool canEdit = false;

        //��ʾģʽ
        public HalconDrawing DrawMode = HalconDrawing.margin;

        //���ƽ�����ʽ
        public HalconDrawMode DrawFinishMode = HalconDrawMode.rightButton;

        //��ʾˮӡ
        public bool isShowWaterString = false;

        //�϶�ģʽ
        private ModeCtrl modelCtrl = ModeCtrl.View_move;

        //��ʾģʽ
        private ModeCtrl dispROI = ModeCtrl.Include_ROI;

        //���ڳ���
        private double windowWidth;

        private double windowHeight;

        //����ϵ��
        private double zoomWndFactor;

        private double zoomAddOn;

        //��갴�±�־λ
        private bool mousePressed = false;

        //��갴��ʱ������
        private double startX, startY;

        //����
        private HTuple hv_Font = new HTuple();

        private HTuple hv_OS = new HTuple();

        #endregion ����

        public HWndCtrl(HWindowControlWPF view)
        {
            hWindowControlWPF = view;
            windowWidth = hWindowControlWPF.ActualWidth;
            windowHeight = hWindowControlWPF.ActualHeight;

            zoomWndFactor = (double)imageWidth / hWindowControlWPF.ActualWidth;
            zoomAddOn = Math.Pow(0.9, 5);

            /*default*/
            CompRangeX = new int[] { 0, 100 };
            CompRangeY = new int[] { 0, 100 };

            hWindowControlWPF.HMouseDown += HWndCtrl_HMouseDown;
            hWindowControlWPF.HMouseUp += HWndCtrl_HMouseUp;
            hWindowControlWPF.HMouseMove += HWndCtrl_HMouseMove;
            hWindowControlWPF.HMouseWheel += HWndCtrl_HMouseWheel;
            hWindowControlWPF.SizeChanged += HWndCtrl_SizeChanged;

            initFont(hWindowControlWPF.HalconWindow);
            roiManager = new ROIController();
        }

        /// <summary>
        ///   //��ʼ������
        /// </summary>
        /// <param name="window"></param>
        private void initFont(HTuple window)
        {
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

        private int activeROIidx = -1;

        private void HWndCtrl_HMouseDown(object sender, HMouseEventArgsWPF e)
        {
            //�Ҽ��������ڻ滭����������
            if (isDrawing)
                return;

            if (e.Button == System.Windows.Input.MouseButton.Right)
            {
                if (activeROIidx < 0)
                    return;
            }

            mousePressed = true;

            startX = e.Column;
            startY = e.Row;

            if (roiManager != null && dispROI == ModeCtrl.Include_ROI && canEdit)
            {
                int mouse_X0, mouse_Y0;//������¼�������ʱ������λ��
                int tempNum = 0;
                this.hWindowControlWPF.HalconWindow.GetMposition(out mouse_X0, out mouse_Y0, out tempNum);
                //�ж��Ƿ��ڶ�Ӧ��ROI������
                activeROIidx = roiManager.mouseDownAction(mouse_Y0, mouse_X0);
                repaint();
            }
        }

        private void HWndCtrl_HMouseUp(object sender, HMouseEventArgsWPF e)
        {
            mousePressed = false;

            if (DrawFinishMode == HalconDrawMode.directly)
                if (e.Button == System.Windows.Input.MouseButton.Left)
                    if (isDrawing)
                        HalconMicroSoft.FinishDraw();
            //�϶�����
            startX = 0;
            startY = 0;
        }

        private void HWndCtrl_HMouseWheel(object sender, HMouseEventArgsWPF e)
        {
            if (e.Delta > 0)
                zoomImage(e.Column, e.Row, 0.9);
            else
                zoomImage(e.Column, e.Row, 1 / 0.9);
        }

        private void HWndCtrl_HMouseMove(object sender, HMouseEventArgsWPF e)
        {
            if (isOpenImage)
                return;
            //�Ҽ��������ڻ滭����������
            if (e.Button == System.Windows.Input.MouseButton.Right || isDrawing)
                return;
            //
            if (roiManager == null || dispROI != ModeCtrl.Include_ROI)
                return;

            //�����������ʱ��������
            if (e.Button != System.Windows.Input.MouseButton.Left)
            {
                //������ע�⣬��갴�º��ƶ��Ⱥ�˳��
                mousePressed = false;
                return;
            }

            double motionX, motionY;

            roiManager.mouseMoveROI(e.Column, e.Row);//��꾭����index
            //���Ա༭roi
            if (roiManager != null && (roiManager.ActiveROIidx != -1) && dispROI == ModeCtrl.Include_ROI && canEdit)
            {
                motionX = e.Column - startX;
                motionY = e.Row - startY;

                if (((int)motionX != 0) || ((int)motionY != 0))
                {
                    roiManager.mouseMoveAction(e.Column, e.Row, motionX, motionY);
                    repaint();
                    startX = e.Column;
                    startY = e.Row;
                }
            }
            //�����ƶ�ͼ��
            else if (modelCtrl == ModeCtrl.View_move)
            {
                //��ֹ��ͼ��ʱ�϶�
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
        private async void HWndCtrl_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            await Task.Delay(1);
            fitWindow();
            repaint();
        }

        private int imageWidth;
        private int imageHeight;

        private int[] CompRangeX;
        private int[] CompRangeY;

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
            repaint();
        }

        /// <summary>
        /// ��Ӧ����
        /// </summary>
        /// <param name="scaleFactor"></param>
        public void zoomImage(double scaleFactor)
        {
            double midPointX, midPointY;

            if (((ImgRow2 - ImgRow1) == scaleFactor * imageHeight) &&
                ((ImgCol2 - ImgCol1) == scaleFactor * imageWidth))
            {
                repaint();
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

            repaint();
        }

        /// <summary>
        /// ˢ��
        /// </summary>
        public void repaint()
        {
            repaint(hWindowControlWPF.HalconWindow);
        }

        private void repaint(HalconDotNet.HWindow window)
        {
            try
            {
                int h = imageHeight;
                if (window.IsInitialized() == false || hWindowControlWPF.HalconID.ToInt64() == -1 || hWindowControlWPF.ImagePart.Width <= 1 || hWindowControlWPF.ImagePart.Height <= 1)
                    return;

                HSystem.SetSystem("flush_graphic", "false");
                window.ClearWindow();

                window.DispObj(image);

                if (roiManager != null && (dispROI == ModeCtrl.Include_ROI))
                    roiManager.paintData(window, DrawMode);

                HSystem.SetSystem("flush_graphic", "true");
                window.SetDraw(DrawMode.ToDescription());
                window.SetColor(HalconColors.��ɫ.ToDescription());
                window.DispLine(-100.0, -100.0, -101.0, -101.0);

                if (isShowWaterString)
                {
                    try
                    {
                        HOperatorSet.DispText(window, WaterString, HalconCoordinateSystem.window.ToDescription(), "bottom",
                            "left", "black", "box_color", "#ffffff77");
                    }
                    catch { }
                }

                for (int i = 0; i < messages.Count; i++)
                {
                    try
                    {
                        if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetFont(window, (hv_Font.TupleSelect(0)) + "-" + messages[i].ShowFontSize.ToString());
                            }
                        }
                        else
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetFont(window, (hv_Font.TupleSelect(0)) + "-" + messages[i].ShowFontSize.ToString());
                            }
                        }
                        HOperatorSet.DispText(window, messages[i].ShowContent, messages[i].ShowMode.ToDescription(), new HTuple(messages[i].PositionX),
                            new HTuple(messages[i].PositionY), messages[i].ShowColor.ToDescription(), "box_color", "#ffffff77");
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
        }

        private List<MessageBase> messages = new List<MessageBase>();

        public void addMessageVar(string message, int row, int column, int fontsize = 12, HalconColors color = HalconColors.��ɫ, HalconCoordinateSystem mode = HalconCoordinateSystem.window)
        {
            messages.Add(new MessageBase(row, column, message, fontsize, color, mode));
        }

        public void clearMessages()
        {
            messages.Clear();
        }

        public string WaterString = "Powered by Leader";
        public int WaterFontSize = 36;

        public HImage image = new HImage();
        public bool isOpenImage = false;

        public void addImageVar(HImage image)
        {
            isOpenImage = true;
            this.image?.Dispose();
            this.image = image;
            isOpenImage = false;
        }

        /// <summary>
        /// ��Ӧ����
        /// </summary>
        public void fitWindow()
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