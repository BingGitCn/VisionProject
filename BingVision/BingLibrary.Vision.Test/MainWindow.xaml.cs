using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BingLibrary.Vision.Units;
using BingLibrary.Vision;
using HalconDotNet;

namespace BingLibrary.Vision.Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        BingImageWindowData windowData = new BingImageWindowData();
        public MainWindow()
        {
            InitializeComponent();
            windowData = win.windowData;
            HTuple aaa = new HTuple();
          
        }

        DetectPrint zfjc = new DetectPrint();

        DetectPrint.Data data1 = new DetectPrint.Data() { ID=1};
        DetectPrint.Data data2 = new DetectPrint.Data() { ID=2};
        DetectPrint.Data data3 = new DetectPrint.Data() { ID=3};

        HRegion smr=new HRegion();
        HShapeModel hShapeModel = new HShapeModel();
        HTuple or, oc,oa, os;
        private void Button_Click0(object sender, RoutedEventArgs e)
        {
            try
            {



                smr = windowData.DrawRegion();
              
                hShapeModel.CreateShapeModel(windowData.CurrentImage.ReduceDomain(smr), new HTuple(7), -0.3, 0.3, new HTuple("auto"), new HTuple("no_pregeneration"), new HTuple("use_polarity"), new HTuple("auto"), new HTuple("auto"));
                windowData.CurrentImage.ReduceDomain(smr.DilationCircle(10.0)).FindShapeModel(hShapeModel, -0.3, 0.3, 0.5, 1, 0.5, "least_squares", 3, 0.5, out or, out oc, out oa, out os);
            }
            catch { }
        }

        private void Button_Click00(object sender, RoutedEventArgs e)
        {
            try
            {
                HTuple cr, cc, ca, cs;
                windowData.CurrentImage.ReduceDomain(smr.DilationCircle(500.0)).FindShapeModel(hShapeModel, -0.3, 0.3, 0.5, 1, 0.5, "least_squares", 3, 0.5, out cr, out cc, out ca, out cs);
                HHomMat2D hHomMat2D = new HHomMat2D();
                hHomMat2D.VectorAngleToRigid(cr, cc, ca, or, oc, oa);
                windowData.CurrentImage = windowData.CurrentImage.AffineTransImage(hHomMat2D, "constant", "false");
                windowData.AddObjectToWindow(windowData.CurrentImage);
                windowData.Repaint();
            }
            catch { }

        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            zfjc.SetHwindow(windowData.hWindow);
            if(DataID.SelectedIndex==0)
                zfjc.SetData(data1);
            if (DataID.SelectedIndex == 1)
                zfjc.SetData(data2);
            if (DataID.SelectedIndex == 2)
                zfjc.SetData(data3);
            windowData.DrawMode(false);
            zfjc.DrawInspectRegion();
            windowData.DrawMode(true);
            windowData.AddObjectToWindow(zfjc.data.InspectRegion);
            windowData.Repaint();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (DataID.SelectedIndex == 0)
                zfjc.SetData(data1);
            if (DataID.SelectedIndex == 1)
                zfjc.SetData(data2);
            if (DataID.SelectedIndex == 2)
                zfjc.SetData(data3);

            try
            {

                zfjc.data.MinGray = min.Value;
                zfjc.data.MaxGray = max.Value;
                zfjc.data.Area = double.Parse(area.Text);
                zfjc.data.Width = double.Parse(width.Text);
                zfjc.data.Height = double.Parse(height.Text);
                zfjc.data.WidthSpace = int.Parse(widthSpace.Text);
                zfjc.data.HeightSpace = int.Parse(heightSpace.Text);
                zfjc.data.ResultArea = double.Parse(resultArea.Text);
                zfjc.data.ShapeScore = double.Parse(shapeScore.Text);

                zfjc.data.Image = windowData.CurrentImage.CopyImage();
                zfjc.GetEachInspectRegions();

                windowData.ClearHObjects();
                windowData.AddObjectToWindow(zfjc.data.EachInspectRegions);
                windowData.Repaint();
            }
            catch { }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            zfjc.LearnInspectRegion();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
           HRegion hRegion = zfjc.Check(windowData.CurrentImage,71);
            windowData.ClearHObjects();
            windowData.AddObjectToWindow(hRegion);
            windowData.Repaint();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            zfjc.data.MinGray = min.Value;
            zfjc.data.MaxGray = max.Value;
            zfjc.data.Area = double.Parse(area.Text);
            zfjc.data.Width = double.Parse(width.Text);
            zfjc.data.Height = double.Parse(height.Text);
            zfjc.data.WidthSpace = int.Parse(widthSpace.Text);
            zfjc.data.HeightSpace = int.Parse(heightSpace.Text);
            zfjc.data.ResultArea = double.Parse(resultArea.Text);
            zfjc.data.ShapeScore = double.Parse(shapeScore.Text);

            zfjc.SaveData();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (DataID.SelectedIndex == 0)
                data1 = zfjc.OpenData(1);
            if (DataID.SelectedIndex == 1)
               data2 = zfjc.OpenData(2);
            if (DataID.SelectedIndex == 2)
                data3=zfjc.OpenData(3);

            if (DataID.SelectedIndex == 0)
                zfjc.SetData(data1);
            if (DataID.SelectedIndex == 1)
                zfjc.SetData(data2);
            if (DataID.SelectedIndex == 2)
                zfjc.SetData(data3);

            min.Value = zfjc.data.MinGray;
            max.Value = zfjc.data.MaxGray;
            area.Text = zfjc.data.Area.ToString();
            width.Text = zfjc.data.Width.ToString();
            height.Text = zfjc.data.Height.ToString();
            widthSpace.Text = zfjc.data.WidthSpace.ToString();
            heightSpace.Text = zfjc.data.HeightSpace.ToString();
            resultArea.Text = zfjc.data.ResultArea.ToString();
            shapeScore.Text = zfjc.data.ShapeScore.ToString();


        }


        double r11, c11, r12, c12;
        double r21, c21, r22, c22;


        SurfaceCheck surfaceCheck = new SurfaceCheck();
        private void Button_Click_s(object sender, RoutedEventArgs e)
        {
            try
            {
                windowData.AddMessageToWindow("nihk",100,100,64);
                windowData.Repaint();
                //windowData.DrawMode(false);
                //HRegion hRegion = windowData.DrawRegion();
                //windowData.AddObjectToWindow(surfaceCheck.DefectsDetect(windowData.CurrentImage.Rgb1ToGray(),hRegion));
                //windowData.Repaint();
                //windowData.DrawMode(true);
            }
            catch { }
        }
        public int MyProperty { get; set; }
        private void Button_Click_b(object sender, RoutedEventArgs e)
        {
            try
            {
                if (barcodeType.SelectedIndex == 0)
                {
                    HBarCode hBarCode = new HBarCode(new HTuple(),new HTuple());
                   // hBarCode.SetBarCodeParam("persistence", 1);
                    HTuple bstring;
                    windowData.CurrentImage.ReduceDomain(new HRegion(rb1, cb1, rb2, cb2)).CropDomain().FindBarCode(hBarCode,new HTuple( "auto"), out bstring);
                    b_result.Text = bstring.S;
                    hBarCode.ClearBarCodeModel();




                }
                else
                {
                    HDataCode2D hDataCode2D = new HDataCode2D("QR Code",new HTuple(),new HTuple());

                    HTuple bstring, bhandle;
                    windowData.CurrentImage.ReduceDomain(new HRegion(rb1, cb1, rb2, cb2)).CropDomain().FindDataCode2d(hDataCode2D,new HTuple(),new HTuple(),out bhandle, out bstring);
                    b_result.Text = bstring.S;
                    hDataCode2D.ClearDataCode2dModel();

                }
            }
            catch(Exception ex) { }
        }

        double rb1, rb2, cb1, cb2;
        private void Button_Click_d3(object sender, RoutedEventArgs e)
        {
            windowData.DrawMode(false);
            windowData.hWindow.DrawRectangle1(out rb1,out cb1,out rb2,out cb2);
            windowData.DrawMode(true);
        }

        private void Button_Click_d1(object sender, RoutedEventArgs e)
        {
            windowData.DrawMode(false);

            windowData.hWindow.DrawLine(out r11,out c11,out r12,out c12);
            windowData.DrawMode(true);
        }

        private void Button_Click_d2(object sender, RoutedEventArgs e)
        {
            windowData.DrawMode(false);
            windowData.hWindow.DrawLine(out r21, out c21, out r22, out c22);
            windowData.DrawMode(true);
        }

        private void Button_Click_m(object sender, RoutedEventArgs e)
        {
            try {
                double[] l1 =  DetectLine.FindLine(windowData.CurrentImage,r11,c11,r12,c12, line1.SelectedIndex==0? TransitionType.negative:   TransitionType.positive,100,60,30,line1_c.SelectedIndex==0? SelectType.max:line1_c.SelectedIndex==1? SelectType.first:  SelectType.last,1,30);
            double[] l2 = DetectLine.FindLine(windowData.CurrentImage, r21, c21, r22, c22, line2.SelectedIndex == 0 ? TransitionType.negative : TransitionType.positive, 100, 60, 30, line2_c.SelectedIndex == 0 ? SelectType.max : line2_c.SelectedIndex == 1 ? SelectType.first : SelectType.last, 1, 30);

          
                windowData.hWindow.SetColor("green");
                windowData.hWindow.DispLine(l1[0], l1[1], l1[2], l1[3]);
                windowData.hWindow.DispLine(l2[0], l2[1], l2[2], l2[3]);

                HTuple distance1, distance2;

                HOperatorSet.DistancePl((l2[0] + l2[2]) / 2, (l2[1] + l2[3]) / 2, l1[0], l1[1], l1[2], l1[3], out distance1);
                HOperatorSet.DistancePl((l1[0] + l1[2]) / 2, (l1[1] + l1[3]) / 2, l2[0], l2[1], l2[2], l2[3], out distance2);

                HTuple distance3, distance4;

                HOperatorSet.DistancePl(l2[0] , l2[1], l1[0], l1[1], l1[2], l1[3], out distance3);
                HOperatorSet.DistancePl(l2[2], l2[3], l1[0], l1[1], l1[2], l1[3], out distance4);

                HTuple distance5, distance6;

                HOperatorSet.DistancePl(l1[0], l1[1], l2[0], l2[1], l2[2], l2[3], out distance5);
                HOperatorSet.DistancePl(l1[2], l1[3], l2[0], l2[1], l2[2], l2[3], out distance6);



                m_result.Text = ((distance1+distance2+distance3+distance4+distance5+distance6)/6).D.ToString("0.00");

               var rr= BingLibrary.Vision.Units.Measure.GetLineDistance(l1[0], l1[1], l1[2], l1[3], l2[0], l2[1], l2[2], l2[3],50);
                m_result.Text += "    " + rr.ToString("0.00");
            }
            catch(Exception ex){ }


        }
    }
}
