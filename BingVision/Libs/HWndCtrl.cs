using HalconDotNet;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BingLibrary.Vision
{
    public delegate void IconicDelegate(int val);

    public delegate void FuncDelegate();



    public class ShowObject
    {
        public HObjectEntry ShowHObject { get; set; }

        public string ShowColor { get; set; } = "red";

        public string DrawMode { get; set; } = "margin";

        public ShowObject(HObjectEntry hObject, string color = "red", string drawMode = "margin")
        {
            ShowHObject = hObject;
            ShowColor = color;
            DrawMode = drawMode;
        }

    }

    public class ShowText
    {
        public double PositionX { get; set; } = 0;

        public double PositionY { get; set; } = 0;

        public string ShowColor { get; set; } = "green";

        public string ShowContent { get; set; } = string.Empty;

        public int ShowFontSize { set; get; }

        public string ShowMode { set; get; }

        public ShowText(double posX, double posY, string text,int fontsize=12, string color = "green",string mode="window")
        {
            PositionX = posX;
            PositionY = posY;
            ShowColor = color;
            ShowContent = text;
            ShowFontSize = fontsize;
            ShowMode = mode;
        }
    }
    public class HWndCtrl
    {
        public const int MODE_INCLUDE_ROI = 1;

        public const int MODE_EXCLUDE_ROI = 2;

        public const int MODE_VIEW_NONE = 10;

        public const int MODE_VIEW_ZOOM = 11;

        public const int MODE_VIEW_MOVE = 12;

        public const int MODE_VIEW_ZOOMWINDOW = 13;



        public const int EVENT_UPDATE_IMAGE = 31;

        public const int ERR_READING_IMG = 32;

        public const int ERR_DEFINING_GC = 33;

        private const int MAXNUMOBJLIST = 50;

        private int stateView;
        private bool mousePressed = false;
        private double startX, startY;

        public HWindowControlWPF viewPort;

        private ROIController roiManager;

        private int dispROI;

        private double windowWidth;
        private double windowHeight;
        private int imageWidth;
        private int imageHeight;

        private int[] CompRangeX;
        private int[] CompRangeY;

        private int prevCompX, prevCompY;
        private double stepSizeX, stepSizeY;

        private double ImgRow1, ImgCol1, ImgRow2, ImgCol2;

        public string exceptionText = "";
        public FuncDelegate addInfoDelegate;

        public IconicDelegate NotifyIconObserver;

        private HWindow ZoomWindow;
        private double zoomWndFactor;
        private double zoomAddOn;
        private int zoomWndSize;

        public List<ShowObject> HObjList = new List<ShowObject>();



        private GraphicsContext mGC;
        //是否正在绘制
        public bool isDrawing = false;

        //是否可变几roi框
        public bool isEdit = false;

        public HWndCtrl(HWindowControlWPF view)
        {
            viewPort = view;
            stateView = MODE_VIEW_NONE;
            windowWidth = viewPort.ActualWidth;
            windowHeight = viewPort.ActualHeight;

            zoomWndFactor = (double)imageWidth / viewPort.ActualWidth;
            zoomAddOn = Math.Pow(0.9, 5);
            zoomWndSize = 150;

            /*default*/
            CompRangeX = new int[] { 0, 100 };
            CompRangeY = new int[] { 0, 100 };

            prevCompX = prevCompY = 0;

            dispROI = MODE_INCLUDE_ROI;//1;

            viewPort.HMouseUp += ViewPort_HMouseUp;
            viewPort.HMouseMove += ViewPort_HMouseMove;

            viewPort.HMouseDown += ViewPort_HMouseDown;

            viewPort.HMouseWheel += ViewPort_HMouseWheel;

            viewPort.SizeChanged += ViewPort_SizeChanged;

            addInfoDelegate = new FuncDelegate(dummyV);
            NotifyIconObserver = new IconicDelegate(dummy);
            HObjList = new List<ShowObject>();

            mGC = new GraphicsContext();
            mGC.gcNotification = new GCDelegate(exceptionGC);
        }

        private void ViewPort_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            DelayRepaint();
        }

        private async void DelayRepaint()
        {
            await System.Threading.Tasks.Task.Delay(1);
            FitImage();
            repaint();
        }

        private void ViewPort_HMouseWheel(object sender, HMouseEventArgsWPF e)
        {
            if (e.Delta > 0)
                zoomImage(e.Column, e.Row, 0.9);
            else
                zoomImage(e.Column, e.Row, 1 / 0.9);
        }

        private void ViewPort_HMouseMove(object sender, HMouseEventArgsWPF e)
        {
            if (e.Button == System.Windows.Input.MouseButton.Right)
                return;
            if (isDrawing)
                return;
            double motionX, motionY;
            double posX, posY;
            double zoomZone;
            if (roiManager == null)
                return;
            int a = roiManager.mouseMoveROI(e.Column, e.Row);//鼠标经过的index
            if (!mousePressed)
                return;

            if (roiManager != null && (roiManager.activeROIidx != -1) && (dispROI == MODE_INCLUDE_ROI) && isEdit)
            {
                roiManager.mouseMoveAction(e.Column, e.Row);
            }
            else if (stateView == MODE_VIEW_MOVE)
            {
                motionX = e.Column - startX;
                motionY = e.Row - startY;

                if (((int)motionX != 0) || ((int)motionY != 0))
                {
                    moveImage(motionX, motionY);
                    startX = e.Column - motionX;
                    startY = e.Row - motionY;
                }
            }
            else if (stateView == MODE_VIEW_ZOOMWINDOW)
            {
                HSystem.SetSystem("flush_graphic", "false");
                ZoomWindow.ClearWindow();

                posX = (e.Column - ImgCol1) / (ImgCol2 - ImgCol1) * viewPort.ActualWidth;
                posY = (e.Row - ImgRow1) / (ImgRow2 - ImgRow1) * viewPort.ActualHeight;
                zoomZone = zoomWndSize / 2 * zoomWndFactor * zoomAddOn;

                ZoomWindow.SetWindowExtents((int)posY - (zoomWndSize / 2),
                                            (int)posX - (zoomWndSize / 2),
                                            zoomWndSize, zoomWndSize);
                ZoomWindow.SetPart((int)(e.Row - zoomZone), (int)(e.Column - zoomZone),
                                   (int)(e.Row + zoomZone), (int)(e.Column + zoomZone));
                repaint(ZoomWindow);

                HSystem.SetSystem("flush_graphic", "true");
                ZoomWindow.DispLine(-100.0, -100.0, -100.0, -100.0);
            }
        }

        private void ViewPort_HMouseDown(object sender, HMouseEventArgsWPF e)
        {
            if (e.Button == System.Windows.Input.MouseButton.Right)
                return;

            if (isDrawing)
                return;

            mousePressed = true;
            int activeROIidx = -1;
            double scale;

            if (roiManager != null && (dispROI == MODE_INCLUDE_ROI) && isEdit)
            {
                int tempNum = 0;
                int mouse_X0, mouse_Y0;//用来记录按下鼠标时的坐标位置
                this.viewPort.HalconWindow.GetMposition(out mouse_X0, out mouse_Y0, out tempNum);
                activeROIidx = roiManager.mouseDownAction(mouse_Y0, mouse_X0);
            }
            else
                activeROIidx = -1;

            if (activeROIidx == -1)
            {
                switch (stateView)
                {
                    case MODE_VIEW_MOVE:
                        startX = e.Column;
                        startY = e.Row;
                        break;

                    case MODE_VIEW_ZOOM:
                        if (e.Button == System.Windows.Input.MouseButton.Left)
                            scale = 0.9;
                        else
                            scale = 1 / 0.9;
                        zoomImage(e.Column, e.Row, scale);
                        break;

                    case MODE_VIEW_NONE:
                        break;

                    case MODE_VIEW_ZOOMWINDOW:
                        activateZoomWindow((int)e.Column, (int)e.Row);
                        break;

                    default:
                        break;
                }
            }
        }

        private void ViewPort_HMouseUp(object sender, HMouseEventArgsWPF e)
        {
            if (e.Button == System.Windows.Input.MouseButton.Right)
                return;
            if (isDrawing)
                return;
            mousePressed = false;

            if (roiManager != null
                && (roiManager.activeROIidx != -1)
                && (dispROI == MODE_INCLUDE_ROI) && isEdit)
            {
                roiManager.NotifyRCObserver(ROIController.EVENT_UPDATE_ROI);
            }
            else if (stateView == MODE_VIEW_ZOOMWINDOW)
            {
                ZoomWindow.Dispose();
            }
            if (e.Button.ToString() == "Right")
            {
                fitWindow();
            }
        }

        public void fitWindow()
        {
            int h = imgHeight, w = imgWidth;
            int _beginRow, _begin_Col, _endRow, _endCol;
            double ratio_win = (double)viewPort.ActualWidth / (double)viewPort.ActualHeight;
            double ratio_img = (double)w / (double)h;
            imageHeight = h;
            imageWidth = w;
            if (ratio_win >= ratio_img)
            {
                _beginRow = 0;
                _endRow = h - 1;
                _begin_Col = (int)(-w * (ratio_win / ratio_img - 1d) / 2d);
                _endCol = (int)(w + w * (ratio_win / ratio_img - 1d) / 2d);
                zoomWndFactor = (double)h / viewPort.ActualHeight;
            }
            else
            {
                _begin_Col = 0;
                _endCol = w - 1;
                _beginRow = (int)(-h * (ratio_img / ratio_win - 1d) / 2d);
                _endRow = (int)(h + h * (ratio_img / ratio_win - 1d) / 2d);
                zoomWndFactor = (double)w / viewPort.ActualWidth;
            }
            //viewPort.HalconWindow.SetPart(_beginRow, _begin_Col, _endRow, _endCol);
            setImagePart(_beginRow, _begin_Col, (int)viewPort.ActualHeight, (int)viewPort.ActualWidth);
            //setImagePart(_beginRow, _begin_Col, _endRow-_beginRow, _endCol-_begin_Col);
            zoomImage(zoomWndFactor);
        }

      
        public void useROIController(ROIController rC)
        {
            roiManager = rC;
            rC.setViewController(this);
        }

        private void setImagePart(HImage image)
        {
            string s;
            int w, h;

            image.GetImagePointer1(out s, out w, out h);
            setImagePart(0, 0, h, w);
        }

        private void setImagePart(int r1, int c1, int r2, int c2)
        {
            ImgRow1 = r1;
            ImgCol1 = c1;
            ImgRow2 = imageHeight = r2;
            ImgCol2 = imageWidth = c2;

            System.Windows.Rect rect = viewPort.ImagePart;
            rect.X = (int)ImgCol1;
            rect.Y = (int)ImgRow1;
            rect.Height = (int)imageHeight;
            rect.Width = (int)imageWidth;
            viewPort.ImagePart = rect;
        }

        public void setViewState(int mode)
        {
            stateView = mode;

            if (roiManager != null)
                roiManager.resetROI();
        }

        /********************************************************************/

        private void dummy(int val)
        {
        }

        private void dummyV()
        {
        }

        /*******************************************************************/

        private void exceptionGC(string message)
        {
            exceptionText = message;
            NotifyIconObserver(ERR_DEFINING_GC);
        }

        public void setDispLevel(int mode)
        {
            dispROI = mode;
        }

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

            System.Windows.Rect rect = viewPort.ImagePart;
            rect.X = (int)Math.Round(ImgCol1);
            rect.Y = (int)Math.Round(ImgRow1);
            rect.Width = (lenC > 0) ? lenC : 1;
            rect.Height = (lenR > 0) ? lenR : 1;
            viewPort.ImagePart = rect;

            zoomWndFactor *= scale;
            repaint();
        }

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

            zoomWndFactor = (double)imageWidth / viewPort.ActualWidth;
            zoomImage(midPointX, midPointY, scaleFactor);
        }

        public void scaleWindow(double scale)
        {
            ImgRow1 = 0;
            ImgCol1 = 0;

            ImgRow2 = imageHeight;
            ImgCol2 = imageWidth;

            viewPort.Width = (int)(ImgCol2 * scale);
            viewPort.Height = (int)(ImgRow2 * scale);

            zoomWndFactor = ((double)imageWidth / viewPort.ActualWidth);
        }

        public void setZoomWndFactor()
        {
            zoomWndFactor = ((double)imageWidth / viewPort.ActualWidth);
        }

        public void setZoomWndFactor(double zoomF)
        {
            zoomWndFactor = zoomF;
        }

        private void moveImage(double motionX, double motionY)
        {
            ImgRow1 += -motionY;
            ImgRow2 += -motionY;

            ImgCol1 += -motionX;
            ImgCol2 += -motionX;

            System.Windows.Rect rect = viewPort.ImagePart;
            rect.X = (int)Math.Round(ImgCol1);
            rect.Y = (int)Math.Round(ImgRow1);
            viewPort.ImagePart = rect;

            repaint();
        }

        public void resetAll()
        {
            ImgRow1 = 0;
            ImgCol1 = 0;
            ImgRow2 = imageHeight;
            ImgCol2 = imageWidth;

            zoomWndFactor = (double)imageWidth / viewPort.ActualWidth;

            System.Windows.Rect rect = viewPort.ImagePart;
            rect.X = (int)ImgCol1;
            rect.Y = (int)ImgRow1;
            rect.Width = (int)imageWidth;
            rect.Height = (int)imageHeight;
            viewPort.ImagePart = rect;

            if (roiManager != null)
                roiManager.reset();
        }

        public void resetWindow()
        {
            ImgRow1 = 0;
            ImgCol1 = 0;
            ImgRow2 = imageHeight;
            ImgCol2 = imageWidth;

            zoomWndFactor = (double)imageWidth / viewPort.ActualWidth;

            System.Windows.Rect rect = viewPort.ImagePart;
            rect.X = (int)ImgCol1;
            rect.Y = (int)ImgRow1;
            rect.Width = (int)imageWidth;
            rect.Height = (int)imageHeight;
            viewPort.ImagePart = rect;
        }

        private void activateZoomWindow(int X, int Y)
        {
            double posX, posY;
            int zoomZone;

            if (ZoomWindow != null)
                ZoomWindow.Dispose();

            HOperatorSet.SetSystem("border_width", 10);
            ZoomWindow = new HWindow();

            posX = ((X - ImgCol1) / (ImgCol2 - ImgCol1)) * viewPort.ActualWidth;
            posY = ((Y - ImgRow1) / (ImgRow2 - ImgRow1)) * viewPort.ActualHeight;

            zoomZone = (int)((zoomWndSize / 2) * zoomWndFactor * zoomAddOn);
            ZoomWindow.OpenWindow((int)posY - (zoomWndSize / 2), (int)posX - (zoomWndSize / 2),
                                   zoomWndSize, zoomWndSize,
                                   viewPort.HalconID, "visible", "");
            ZoomWindow.SetPart(Y - zoomZone, X - zoomZone, Y + zoomZone, X + zoomZone);
            repaint(ZoomWindow);
            ZoomWindow.SetColor("black");
        }

        public void setGUICompRangeX(int[] xRange, int Init)
        {
            int cRangeX;

            CompRangeX = xRange;
            cRangeX = xRange[1] - xRange[0];
            prevCompX = Init;
            stepSizeX = ((double)imageWidth / cRangeX) * (imageWidth / windowWidth);
        }

        public void setGUICompRangeY(int[] yRange, int Init)
        {
            int cRangeY;

            CompRangeY = yRange;
            cRangeY = yRange[1] - yRange[0];
            prevCompY = Init;
            stepSizeY = ((double)imageHeight / cRangeY) * (imageHeight / windowHeight);
        }

        public void resetGUIInitValues(int xVal, int yVal)
        {
            prevCompX = xVal;
            prevCompY = yVal;
        }

        public void moveXByGUIHandle(int valX)
        {
            double motionX;

            motionX = (valX - prevCompX) * stepSizeX;

            if (motionX == 0)
                return;

            moveImage(motionX, 0.0);
            prevCompX = valX;
        }

        public void moveYByGUIHandle(int valY)
        {
            double motionY;

            motionY = (valY - prevCompY) * stepSizeY;

            if (motionY == 0)
                return;

            moveImage(0.0, motionY);
            prevCompY = valY;
        }

        public void zoomByGUIHandle(double valF)
        {
            double x, y, scale;
            double prevScaleC;

            x = (ImgCol1 + (ImgCol2 - ImgCol1) / 2);
            y = (ImgRow1 + (ImgRow2 - ImgRow1) / 2);

            prevScaleC = (double)((ImgCol2 - ImgCol1) / imageWidth);
            scale = ((double)1.0 / prevScaleC * (100.0 / valF));

            zoomImage(x, y, scale);
        }

        public void repaint()
        {
            repaint(viewPort.HalconWindow);
        }

        public string DrawMode = "margin";
        public bool ShowWaterString = false;

        public void repaint(HalconDotNet.HWindow window)
        {
            try
            {
                int h = imageHeight;
                if (window.IsInitialized() == false || viewPort.HalconID.ToInt64() == -1 || viewPort.ImagePart.Width <= 1 || viewPort.ImagePart.Height <= 1)
                    return;
                int count = HObjList.Count;
                HObjectEntry entry;

                HSystem.SetSystem("flush_graphic", "false");
                window.ClearWindow();
                mGC.stateOfSettings.Clear();

                for (int i = 0; i < count; i++)
                {
                    entry = (HObjectEntry)HObjList[i].ShowHObject;
                    mGC.applyContext(window, entry.gContext);
                    window.SetColor(HObjList[i].ShowColor);
                    window.SetDraw(HObjList[i].DrawMode);
                    window.DispObj(entry.HObj);
                }

                addInfoDelegate();

                if (roiManager != null && (dispROI == MODE_INCLUDE_ROI))
                    roiManager.paintData(window, DrawMode);

                HSystem.SetSystem("flush_graphic", "true");
                window.SetDraw(DrawMode);
                window.SetColor("black");
                window.DispLine(-100.0, -100.0, -101.0, -101.0);

                HTuple hv_Font = new HTuple();
                HTuple hv_OS = new HTuple();

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

                if (ShowWaterString)
                {
                    try
                    {
                        HOperatorSet.DispText(window, WaterString, "window", "bottom",
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
                        HOperatorSet.DispText(window, messages[i].ShowContent, messages[i].ShowMode, new HTuple(messages[i].PositionX),
                            new HTuple(messages[i].PositionY), messages[i].ShowColor, "box_color", "#ffffff77");
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
            showHat(window);
        }

        public bool ShowHat = false;

        private void showHat(HWindow window)
        {
            try
            {
                // HSystem.SetSystem("flush_graphic", "true");
                if (ShowHat)
                {
                    //获取当前显示信息
                    HTuple hv_Red = null, hv_Green = null, hv_Blue = null;
                    int hv_lineWidth;

                    window.GetRgb(out hv_Red, out hv_Green, out hv_Blue);

                    hv_lineWidth = (int)window.GetLineWidth();
                    string hv_Draw = window.GetDraw();
                    window.SetLineWidth(2);//设置线宽
                    window.SetLineStyle(new HTuple());
                    window.SetColor("green");//十字架显示颜色

                    var w = (double)imgWidth;
                    var h = (double)imgHeight;

                    double CrossCol = w / 2.0, CrossRow = h / 2.0;
                    double borderWidth = w / 50.0;
                    CrossCol = w / 2.0;
                    CrossRow = h / 2.0;
                    //竖线1
                    window.DispPolygon(new HTuple(0, CrossRow - 30), new HTuple(CrossCol, CrossCol));
                    window.DispPolygon(new HTuple(CrossRow + 30, h), new HTuple(CrossCol, CrossCol));

                    //中心点
                    window.DispPolygon(new HTuple(CrossRow - 5, CrossRow + 5), new HTuple(CrossCol, CrossCol));
                    window.DispPolygon(new HTuple(CrossRow, CrossRow), new HTuple(CrossCol - 5, CrossCol + 5));

                    //横线
                    window.DispPolygon(new HTuple(CrossRow, CrossRow), new HTuple(0, CrossCol - 30));
                    window.DispPolygon(new HTuple(CrossRow, CrossRow), new HTuple(CrossCol + 30, w));

                    //恢复窗口显示信息
                    window.SetRgb(hv_Red, hv_Green, hv_Blue);
                    window.SetLineWidth(hv_lineWidth);
                    window.SetDraw(hv_Draw);
                }
                else
                {
                    window.DispLine(-100.0, -100.0, -101.0, -101.0);
                }
                // window.FlushBuffer();
            }
            catch { }
        }

        private List<ShowText> messages = new List<ShowText>();

        public void addMessageVar(string message, int row, int column, int fontsize = 12, string color="green",string mode ="window")
        {
            messages.Add(new ShowText(row,column,message,fontsize,color,mode));
        }

        public void clearMessages()
        {
            messages.Clear();
        }

        public string WaterString = "Powered by Leader";
        public int WaterFontSize = 36;
        private int imgWidth, imgHeight;

        public void addIconicVar(HObject obj, string color = "red",string drawMode="margin")
        {
            HObjectEntry entry;

            if (obj == null)
                return;

            if (obj is HImage)
            {
                double r, c;
                int h, w, area;
                string s;

                area = ((HImage)obj).GetDomain().AreaCenter(out r, out c);
                ((HImage)obj).GetImagePointer1(out s, out w, out h);
                imgHeight = h;
                imgWidth = w;
                if (true)//area == (w * h))//大小不同也清除原来的
                {
                    ClearList();
                    clearMessages();
                    if ((h != imageHeight) || (w != imageWidth))
                    {
                        int _beginRow, _begin_Col, _endRow, _endCol;
                        double ratio_win = (double)viewPort.ActualWidth / (double)viewPort.ActualHeight;
                        double ratio_img = (double)w / (double)h;
                        imageHeight = h;
                        imageWidth = w;
                        if (ratio_win >= ratio_img)
                        {
                            _beginRow = 0;
                            _endRow = h - 1;
                            _begin_Col = (int)(-w * (ratio_win / ratio_img - 1d) / 2d);
                            _endCol = (int)(w + w * (ratio_win / ratio_img - 1d) / 2d);
                            zoomWndFactor = (double)h / viewPort.ActualHeight;
                        }
                        else
                        {
                            _begin_Col = 0;
                            _endCol = w - 1;
                            _beginRow = (int)(-h * (ratio_img / ratio_win - 1d) / 2d);
                            _endRow = (int)(h + h * (ratio_img / ratio_win - 1d) / 2d);
                            zoomWndFactor = (double)w / viewPort.ActualWidth;
                        }
                        //viewPort.HalconWindow.SetPart(_beginRow, _begin_Col, _endRow, _endCol);
                        setImagePart(_beginRow, _begin_Col, (int)viewPort.ActualHeight, (int)viewPort.ActualWidth);
                        //setImagePart(_beginRow, _begin_Col, _endRow-_beginRow, _endCol-_begin_Col);
                        ClearList();

                        zoomImage(zoomWndFactor);
                    }
                }//if
            }//if

            entry = new HObjectEntry(obj, mGC.copyContextList());

            HObjList.Add(new ShowObject(entry,color,drawMode) );
           
            if (HObjList.Count > MAXNUMOBJLIST)
            {
                HObjList.RemoveAt(1);
            }
        }

     
        public void FitImage()
        {
            if (HObjList.Count == 0)
                return;

            HObjectEntry entry = HObjList[0].ShowHObject as HObjectEntry;
            if (entry.HObj is HImage)
            {
                double r, c;
                int h, w, area;
                string s;

                area = ((HImage)entry.HObj).GetDomain().AreaCenter(out r, out c);
                ((HImage)entry.HObj).GetImagePointer1(out s, out w, out h);

                if ((h != imageHeight) || (w != imageWidth))
                {
                    int _beginRow, _begin_Col, _endRow, _endCol;
                    double ratio_win = (double)viewPort.ActualWidth / (double)viewPort.ActualHeight;
                    double ratio_img = (double)w / (double)h;
                    imageHeight = h;
                    imageWidth = w;
                    if (ratio_win >= ratio_img)
                    {
                        _beginRow = 0;
                        _endRow = h - 1;
                        _begin_Col = (int)(-w * (ratio_win / ratio_img - 1d) / 2d);
                        _endCol = (int)(w + w * (ratio_win / ratio_img - 1d) / 2d);
                        zoomWndFactor = (double)h / viewPort.ActualHeight;
                    }
                    else
                    {
                        _begin_Col = 0;
                        _endCol = w - 1;
                        _beginRow = (int)(-h * (ratio_img / ratio_win - 1d) / 2d);
                        _endRow = (int)(h + h * (ratio_img / ratio_win - 1d) / 2d);
                        zoomWndFactor = (double)w / viewPort.ActualWidth;
                    }
                    //viewPort.HalconWindow.SetPart(_beginRow, _begin_Col, _endRow, _endCol);
                    setImagePart(_beginRow, _begin_Col, (int)viewPort.ActualHeight, (int)viewPort.ActualWidth);
                    //setImagePart(_beginRow, _begin_Col, _endRow-_beginRow, _endCol-_begin_Col);

                    zoomImage(zoomWndFactor);
                }
            }
        }

        public void ClearList()
        {
            HObjList.Clear();
        }

        public int GetListCount()
        {
            return HObjList.Count;
        }

        public void changeGraphicSettings(string mode, string val)
        {
            switch (mode)
            {
                case GraphicsContext.GC_COLOR:
                    mGC.setColorAttribute(val);
                    break;

                case GraphicsContext.GC_DRAWMODE:
                    mGC.setDrawModeAttribute(val);
                    break;

                case GraphicsContext.GC_LUT:
                    mGC.setLutAttribute(val);
                    break;

                case GraphicsContext.GC_PAINT:
                    mGC.setPaintAttribute(val);
                    break;

                case GraphicsContext.GC_SHAPE:
                    mGC.setShapeAttribute(val);
                    break;

                default:
                    break;
            }
        }

        public void changeGraphicSettings(string mode, int val)
        {
            switch (mode)
            {
                case GraphicsContext.GC_COLORED:
                    mGC.setColoredAttribute(val);
                    break;

                case GraphicsContext.GC_LINEWIDTH:
                    mGC.setLineWidthAttribute(val);
                    break;

                default:
                    break;
            }
        }

        public void changeGraphicSettings(string mode, HTuple val)
        {
            switch (mode)
            {
                case GraphicsContext.GC_LINESTYLE:
                    mGC.setLineStyleAttribute(val);
                    break;

                default:
                    break;
            }
        }

        public void clearGraphicContext()
        {
            mGC.clear();
        }

        public Hashtable getGraphicContext()
        {
            return mGC.copyContextList();
        }
    }
}