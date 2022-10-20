using HalconDotNet;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BingLibrary.Vision
{
    /// <summary>
    /// BingImageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BingImageWindow : UserControl
    {
        public BingImageWindowData windowData = new BingImageWindowData();

        private Config config = new Config();

        public BingImageWindow()
        {
            InitializeComponent();
            BingLibrary.Tools.GlobalTools.EnableNonCommercial();
        }

        private string OpenImageDialog()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "所有文件|*.*|Tiff文件|*.tif|BMP文件|*.bmp|Jpeg文件|*.jpg";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            System.Windows.Forms.DialogResult result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return "";
            }
            string fileName = openFileDialog.FileName;
            return fileName;
        }

        private string SaveImageDialog()
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Title = "选择文件";
            saveFileDialog.Filter = "所有文件|*.*|Tiff文件|*.tif|BMP文件|*.bmp|Jpeg文件|*.jpg";
            saveFileDialog.FileName = string.Empty;
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            System.Windows.Forms.DialogResult result = saveFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return "";
            }
            string fileName = saveFileDialog.FileName;
            return fileName;
        }

        private void iwin_HInitWindow(object sender, EventArgs e)
        {
            try
            {
                config = Serialize.ReadJson<Config>(System.AppDomain.CurrentDomain.BaseDirectory + this.Name + ".Config");
            }
            catch { config = new Config(); }
            //HOperatorSet.SetWindowParam(iwin.HalconWindow, "background_color", WindowBackgroud.ToString());
            HOperatorSet.ClearWindow(iwin.HalconWindow);

            windowData.HCtrl = new HWndCtrl(iwin);
            iwin.HMouseUp += (sender0, e0) => { if (e0.Button == MouseButton.Right && windowData.HCtrl.isDrawing == false) CM.IsOpen = true; };
            windowData.InitHWindow(iwin.HalconWindow);

            windowData.HCtrl.isShowWaterString = config.IsShowWaterString;
            windowData.HCtrl.DrawMode = config.IsShowMargin ? HalconDrawing.margin : HalconDrawing.fill;
            windowData.HCtrl.canEdit = config.IsEdit;
            windowData.HCtrl.DrawFinishMode = config.IsDrawFinish ? HalconDrawMode.rightButton : HalconDrawMode.directly;

            HOperatorSet.SetWindowParam(iwin.HalconWindow, "background_color",
                config.ColorIndex == 0 ? "white"
               : config.ColorIndex == 1 ? "black"
               : config.ColorIndex == 2 ? "gray"
               : config.ColorIndex == 3 ? "orange"
               : config.ColorIndex == 4 ? "coral"
               : config.ColorIndex == 5 ? "red"
               : config.ColorIndex == 6 ? "spring green"
               : config.ColorIndex == 7 ? "cadet blue"
               : config.ColorIndex == 8 ? "indian red"
               : config.ColorIndex == 9 ? "dark olive green"

               : "black");

            c1.IsChecked = config.IsShowWaterString;
            c2.IsChecked = config.IsShowMargin;
            c4.IsChecked = config.IsEdit;
            c5.IsChecked = config.IsDrawFinish;
            colorSet.SelectedIndex = config.ColorIndex;

            HOperatorSet.ClearWindow(iwin.HalconWindow);
            windowData.Repaint();
            HOperatorSet.SetSystem("clip_region", "false");
        }

        //public BackGroundColor WindowBackgroud
        //{
        //    get { return (BackGroundColor)GetValue(WindowBackgroudProperty); }
        //    set { SetValue(WindowBackgroudProperty, value); }
        //}

        //public static readonly DependencyProperty WindowBackgroudProperty =
        // DependencyProperty.Register("WindowBackgroud", //属性名字
        // typeof(BackGroundColor), //属性类型
        // typeof(BingImageWindow));//属性所属，及属性所有者

        private void Read_Image(object sender, RoutedEventArgs e)
        {
            try
            {
                var rst = OpenImageDialog();
                if (rst != "")
                {
                    windowData.CurrentImage?.Dispose();
                    windowData.CurrentImage = new HImage(rst);
                    windowData.AddImageToWindow(windowData.CurrentImage);
                    windowData.FitWindow();
                    windowData.Repaint();
                }
            }
            catch { }
        }

        private void Write_Image(object sender, RoutedEventArgs e)
        {
            try
            {
                var rst = SaveImageDialog();
                if (rst != "")
                {
                    windowData.CurrentImage.WriteImage("tiff", new HTuple(0), new HTuple(rst));
                }
            }
            catch { }
        }

        private void Fit_Window(object sender, RoutedEventArgs e)
        {
            try
            {
                windowData.FitWindow();
                windowData.Repaint();
            }
            catch { }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetWindowParam(iwin.HalconWindow, "background_color", "white");
            HOperatorSet.ClearWindow(iwin.HalconWindow);
            windowData.Repaint();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetWindowParam(iwin.HalconWindow, "background_color", "black");
            HOperatorSet.ClearWindow(iwin.HalconWindow);
            windowData.Repaint();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetWindowParam(iwin.HalconWindow, "background_color", "gray");
            HOperatorSet.ClearWindow(iwin.HalconWindow);
            windowData.Repaint();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetWindowParam(iwin.HalconWindow, "background_color", "orange");
            HOperatorSet.ClearWindow(iwin.HalconWindow);
            windowData.Repaint();
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetWindowParam(iwin.HalconWindow, "background_color", "coral");
            HOperatorSet.ClearWindow(iwin.HalconWindow);
            windowData.Repaint();
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetWindowParam(iwin.HalconWindow, "background_color", "red");
            HOperatorSet.ClearWindow(iwin.HalconWindow);
            windowData.Repaint();
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetWindowParam(iwin.HalconWindow, "background_color", "spring green");
            HOperatorSet.ClearWindow(iwin.HalconWindow);
            windowData.Repaint();
        }

        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetWindowParam(iwin.HalconWindow, "background_color", "cadet blue");
            HOperatorSet.ClearWindow(iwin.HalconWindow);
            windowData.Repaint();
        }

        private void MenuItem_Click_8(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetWindowParam(iwin.HalconWindow, "background_color", "indian red");
            HOperatorSet.ClearWindow(iwin.HalconWindow);
            windowData.Repaint();
        }

        private void MenuItem_Click_9(object sender, RoutedEventArgs e)
        {
            windowData.HCtrl.DrawMode = (sender as MenuItem).IsChecked == true ? HalconDrawing.margin : HalconDrawing.fill;
            windowData.Repaint();
        }

        private void MenuItem_Click_10(object sender, RoutedEventArgs e)
        {
            windowData.HCtrl.isShowWaterString = (sender as MenuItem).IsChecked;
            windowData.Repaint();
        }

        private int mouse_X0_old = 0, mouse_Y0_old = 0;

        private void iwin_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                try
                {
                    int tempNum = 0;
                    int mouse_X0, mouse_Y0;//用来记录按下鼠标时的坐标位置
                    windowData.HCtrl.hWindowControlWPF.HalconWindow.GetMposition(out mouse_X0, out mouse_Y0, out tempNum);

                    if (mouse_X0_old == mouse_X0 && mouse_Y0_old == mouse_Y0)
                    {
                        return;
                    }
                    else
                    {
                        mouse_X0_old = mouse_X0; mouse_Y0_old = mouse_Y0;
                        var gray = windowData.CurrentImage.GetGrayval(mouse_X0, mouse_Y0);

                        zb.Text = "坐标:" + mouse_X0 + " , " + mouse_Y0;
                        try
                        {
                            hd.Text = "灰度: " + gray[0] + " , " + gray[1] + " , " + gray[2];
                        }
                        catch
                        {
                            hd.Text = "灰度: " + gray[0];
                        }
                        Pop.IsOpen = false;
                        Pop.IsOpen = true;
                    }
                }
                catch { Pop.IsOpen = false; }
            }
        }

        private void MenuItem_Click_11(object sender, RoutedEventArgs e)
        {
            windowData.Repaint();
        }

        private void MenuItem_Click_12(object sender, RoutedEventArgs e)
        {
            bool rst = (sender as MenuItem).IsChecked;
            if (rst)
                more.Width = 200;
            else
                more.Width = 0;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            windowData.HCtrl.isShowWaterString = (sender as CheckBox).IsChecked == true;
            windowData.Repaint();
            config.IsShowWaterString = windowData.HCtrl.isShowWaterString;
            Serialize.WriteJson(config, System.AppDomain.CurrentDomain.BaseDirectory + this.Name + ".Config");
        }

        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {
            windowData.HCtrl.DrawMode = (sender as CheckBox).IsChecked == true ? HalconDrawing.margin : HalconDrawing.fill;
            windowData.Repaint();
            config.IsShowMargin = (sender as CheckBox).IsChecked == true;
            Serialize.WriteJson(config, System.AppDomain.CurrentDomain.BaseDirectory + this.Name + ".Config");
        }

        private void CheckBox_Click_2(object sender, RoutedEventArgs e)
        {
        }

        private void colorSet_DropDownClosed(object sender, EventArgs e)
        {
            HOperatorSet.SetWindowParam(iwin.HalconWindow, "background_color",
                colorSet.SelectedIndex == 0 ? "white"
                : colorSet.SelectedIndex == 1 ? "black"
                : colorSet.SelectedIndex == 2 ? "gray"
                : colorSet.SelectedIndex == 3 ? "orange"
                : colorSet.SelectedIndex == 4 ? "coral"
                : colorSet.SelectedIndex == 5 ? "red"
                : colorSet.SelectedIndex == 6 ? "spring green"
                : colorSet.SelectedIndex == 7 ? "cadet blue"
                : colorSet.SelectedIndex == 8 ? "indian red"
                : colorSet.SelectedIndex == 9 ? "dark olive green"

                : "black");
            HOperatorSet.ClearWindow(iwin.HalconWindow);
            windowData.Repaint();
            config.ColorIndex = colorSet.SelectedIndex;
            Serialize.WriteJson(config, System.AppDomain.CurrentDomain.BaseDirectory + this.Name + ".Config");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            windowData.DrawMode(true);

            if (cb.SelectedIndex == 0)
            {
                ROIRegion rr0 = new ROIRegion(windowData.hWindow.DrawRegion());
                rr0.draw(iwin.HalconWindow);
                rr0.ROIName = getRoiName();
                windowData.HCtrl.roiManager.ROIList.Add(rr0);
            }
            else if (cb.SelectedIndex == 1)
            {
                ROILine rl1 = new ROILine();
                double r1, c1, r2, c2;
                windowData.hWindow.DrawLine(out r1, out c1, out r2, out c2);
                rl1.createROILine(r1, c1, r2, c2);
                rl1.draw(iwin.HalconWindow);
                rl1.ROIName = getRoiName();
                windowData.HCtrl.roiManager.ROIList.Add(rl1);
            }
            else if (cb.SelectedIndex == 2)
            {
                ROICircle rc2 = new ROICircle();
                double r1, c1, r;
                windowData.hWindow.DrawCircle(out r1, out c1, out r);
                rc2.createROICircle(c1, r1, r);
                rc2.draw(iwin.HalconWindow);
                rc2.ROIName = getRoiName();
                windowData.HCtrl.roiManager.ROIList.Add(rc2);
            }
            else if (cb.SelectedIndex == 3)
            {
                ROIRectangle1 rr3 = new ROIRectangle1();
                double r1, c1, r2, c2;
                windowData.hWindow.DrawRectangle1(out r1, out c1, out r2, out c2);
                rr3.createROIRect1(r1, c1, r2, c2);
                rr3.draw(iwin.HalconWindow);
                rr3.ROIName = getRoiName();
                windowData.HCtrl.roiManager.ROIList.Add(rr3);
            }
            else if (cb.SelectedIndex == 4)
            {
                ROIRectangle2 rr4 = new ROIRectangle2();
                double r1, c1, p, l1, l2;
                windowData.hWindow.DrawRectangle2(out c1, out r1, out p, out l1, out l2);
                rr4.createROIRect2(r1, c1, -p, l1, l2);
                rr4.draw(iwin.HalconWindow);
                rr4.ROIName = getRoiName();
                windowData.HCtrl.roiManager.ROIList.Add(rr4);
            }

            windowData.DrawMode(false);
            (sender as Button).IsEnabled = true;
        }

        private string getRoiName()
        {
            string name = "0";
            int k = 0;
            for (int j = 0; j < 1024; j++)
            {
                for (int i = 0; i < windowData.HCtrl.roiManager.ROIList.Count; i++)
                {
                    if (windowData.HCtrl.roiManager.ROIList[i].ROIName == k.ToString())
                    { k++; break; }
                }
            }
            return name = k.ToString();
        }

        private void c4_Click(object sender, RoutedEventArgs e)
        {
            windowData.HCtrl.canEdit = (sender as CheckBox).IsChecked == true;
            windowData.Repaint();
            config.IsEdit = (sender as CheckBox).IsChecked == true;
            Serialize.WriteJson(config, System.AppDomain.CurrentDomain.BaseDirectory + this.Name + ".Config");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            windowData.HCtrl.roiManager.RemoveActiveRoi();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            windowData.DrawMode(true);

            HRegion region = windowData.hWindow.DrawRegion();
            ROIRegion roi = new ROIRegion(region);

            roi.draw(iwin.HalconWindow);
            roi.ROIName = getRoiName();
            windowData.HCtrl.roiManager.ROIList.Add(roi);

            windowData.DrawMode(false);
            (sender as Button).IsEnabled = true;
        }

        private void c5_Click(object sender, RoutedEventArgs e)
        {
            windowData.HCtrl.DrawFinishMode = ((sender as CheckBox).IsChecked == true) ? HalconDrawMode.rightButton : HalconDrawMode.directly;
            config.IsDrawFinish = (sender as CheckBox).IsChecked == true;
            Serialize.WriteJson(config, System.AppDomain.CurrentDomain.BaseDirectory + this.Name + ".Config");
        }

        private void iwin_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            mouse_X0_old = 0; mouse_Y0_old = 0;
            Pop.IsOpen = false;
        }
    }

    //public enum BackGroundColor
    //{
    //    white,
    //    black,
    //    gray,
    //    orange,
    //    coral,
    //    red,
    //}

    public class Config
    {
        public bool IsDrawFinish { set; get; } = false;
        public bool IsShowMargin { set; get; } = true;
        public bool IsShowWaterString { set; get; } = false;
        public bool IsEdit { set; get; } = false;
        public int ColorIndex { set; get; } = 1;
    }

    public class Serialize
    {
        /// <summary>
        /// 序列化读取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonFileName"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static T ReadJson<T>(string jsonFileName)
        {
            FileInfo fileInfo = new FileInfo(jsonFileName);
            if (fileInfo.Exists)
            {
                using (var fs = new FileStream(jsonFileName, FileMode.Open, FileAccess.Read))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        var content = sr.ReadToEnd();
                        var rslt = JsonConvert.DeserializeObject<T>(content);
                        return rslt;
                    }
                }
            }
            else
                throw new FileNotFoundException();
        }

        /// <summary>
        /// 序列化保存
        /// </summary>
        /// <param name="objectToSerialize"></param>
        /// <param name="jsonFileName"></param>
        public static void WriteJson(object objectToSerialize, string jsonFileName)
        {
            using (var fs = new FileStream(jsonFileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.SetLength(0L);
                using (var sw = new StreamWriter(fs))
                {
                    var jsonStr = JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented);
                    sw.Write(jsonStr);
                }
            }
        }

        /// <summary>
        /// 当常规序列化失败时可以使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonFileName"></param>
        /// <returns></returns>
        public static T ReadJsonV2<T>(string jsonFileName)
        {
            var setting = new JsonSerializerSettings();
            setting.PreserveReferencesHandling = PreserveReferencesHandling.All;
            setting.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            setting.TypeNameHandling = TypeNameHandling.All;

            FileInfo fileInfo = new FileInfo(jsonFileName);
            if (fileInfo.Exists)
            {
                using (var fs = new FileStream(jsonFileName, FileMode.Open, FileAccess.Read))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        var content = sr.ReadToEnd();
                        var rslt = JsonConvert.DeserializeObject<T>(content, setting);
                        return rslt;
                    }
                }
            }
            else
                throw new FileNotFoundException();
        }

        /// <summary>
        /// 当常规序列化失败时可以使用
        /// </summary>
        /// <param name="objectToSerialize"></param>
        /// <param name="jsonFileName"></param>
        public static void WriteJsonV2(object objectToSerialize, string jsonFileName)
        {
            var setting = new JsonSerializerSettings();
            setting.PreserveReferencesHandling = PreserveReferencesHandling.All;
            setting.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            setting.TypeNameHandling = TypeNameHandling.All;
            setting.Formatting = Formatting.Indented;
            using (var fs = new FileStream(jsonFileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.SetLength(0L);
                using (var sw = new StreamWriter(fs))
                {
                    var jsonStr = JsonConvert.SerializeObject(objectToSerialize, setting);
                    sw.Write(jsonStr);
                }
            }
        }
    }
}