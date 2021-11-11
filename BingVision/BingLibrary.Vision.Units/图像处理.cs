using HalconDotNet;

namespace BingLibrary.Vision.Units
{
    public class ImageProcess
    {
        /// <summary>
        /// 将彩色图像分解为R.G.B.独立的灰度图像
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static HImage[] RGBToGray(HImage image)
        {
            HImage RImage, GImage, BImage;
            HImage[] images = new HImage[3];
            try
            {
                HTuple channels = image.CountChannels();
                if (channels == 3)
                {
                    RImage = image.Decompose3(out GImage, out BImage);

                    images[0] = RImage;
                    images[1] = GImage;
                    images[2] = BImage;
                }
            }
            catch { }
            return images;
        }

        /// <summary>
        /// 将HObject类型的图像转换程HImage类型
        /// </summary>
        /// <param name="hObject"></param>
        /// <param name="hImage"></param>
        public static void HObjectToHImage(HObject hObject, out HImage hImage)
        {
            HTuple channels;
            HOperatorSet.CountChannels(hObject, out channels);
            if (channels.I == 1)
            {
                HTuple pointer, htype, width, height;
                HOperatorSet.GetImagePointer1(hObject, out pointer, out htype, out width, out height);
                hImage = new HImage();
                hImage.GenImage1(htype.S, width.I, height.I, pointer.IP);
            }
            else if (channels.I == 3)
            {
                HTuple pointerR, pointerG, pointerB, htype, width, height;
                HOperatorSet.GetImagePointer3(hObject, out pointerR, out pointerG, out pointerB, out htype, out width, out height);
                hImage = new HImage();
                hImage.GenImage3(htype.S, width.I, height.I, pointerR.IP, pointerG.IP, pointerB.IP);
            }
            else
                hImage = new HImage();
        }
    }
}