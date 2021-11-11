using HalconDotNet;

namespace BingLibrary.Vision
{
    public class BingImageWindowData
    {
        public HWindow hWindow { set; get; }
        public HWndCtrl HCtrl { set; get; }
        public ROIController RCtrl { set; get; }
        public HImage CurrentImage { set; get; } = new HImage();

        /// <summary>
        /// 设置水印
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="fontSize"></param>
        public void SetWaterString(string msg, int fontSize = 36)
        {
            HCtrl.WaterString = msg;
            HCtrl.WaterFontSize = fontSize;
        }

        /// <summary>
        /// 显示水印
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowWaterString(bool isShow = true)
        {
            HCtrl.ShowWaterString = isShow;
        }

        /// <summary>
        /// 初始化窗口
        /// </summary>
        /// <param name="hWin"></param>
        internal void InitHWindow(HWindow hWin)
        {
            hWindow = hWin;
        }

        /// <summary>
        /// 是否启用鼠标右键
        /// </summary>
        /// <param name="isUse"></param>
        public void DrawMode(bool isUse)
        {
            HCtrl.isDrawing = isUse;
        }

        /// <summary>
        /// 是否启用编辑ROI
        /// </summary>
        /// <param name="isEdit"></param>
        public void EditMode(bool isEdit)
        {
            HCtrl.isEdit = isEdit;
        }

        /// <summary>
        /// 绘制区域
        /// </summary>
        /// <returns></returns>
        public HRegion DrawRegion()
        {
            HRegion region = new HRegion();
            try
            {
                region.DrawRegion(hWindow);
            }
            catch { }
            return region;
        }

        /// <summary>
        /// 读取一张图像
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public bool ReadImage(string imagePath)
        {
            try
            {
                CurrentImage?.Dispose();
                CurrentImage.ReadImage(imagePath);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 保存图像
        /// </summary>
        /// <param name="imageType"></param>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public bool SaveImage(string imageType, string imagePath)
        {
            try
            {
                CurrentImage.WriteImage(imageType, 0, imagePath);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 将对象显示到窗口
        /// </summary>
        /// <param name="hObject"></param>
        /// <param name="roiColor"></param>
        public void AddObjectToWindow(HObject hObject, ROIColors roiColor = ROIColors.red)
        {
            try
            {
                HCtrl.addIconicVar(hObject, roiColor.ToString());
            }
            catch { }
        }

        /// <summary>
        /// 将对象显示到ROIList
        /// </summary>
        /// <param name="hRegion"></param>
        /// <param name="roiColor"></param>
        public void AddObjectToROIList(HObject hRegion, ROIColors roiColor = ROIColors.red)
        {
            try
            {
                RCtrl.showROIShape(new ROIObjectRegion(hRegion), roiColor.ToString());
            }
            catch { }
        }

        /// <summary>
        /// 将区域添加到ROIList
        /// </summary>
        /// <param name="hRegion"></param>
        /// <param name="roiColor"></param>
        public void AddRegionToROIList(HRegion hRegion, ROIColors roiColor = ROIColors.red)
        {
            try
            {
                RCtrl.showROIShape(new ROIRegion(hRegion), roiColor.ToString());
            }
            catch { }
        }

        /// <summary>
        /// 设置ROI当前索引
        /// </summary>
        /// <param name="index"></param>
        /// <param name="color"></param>
        public void SetActive(int index, string color = "red")
        {
            try
            {
                RCtrl.setActiveROIIdx(index);
                if (RCtrl.ROIList.Count > 0)
                    Repaint();
            }
            catch { }
        }

      

        /// <summary>
        /// 将消息显示到窗口
        /// </summary>
        /// <param name="message"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="fontSize"></param>
        public void AddMessageToWindow(string message, int row, int column, int fontSize = 12,string color="black",string mode="window")
        {
            try
            {
                HCtrl.addMessageVar(message, row, column, fontSize,color,mode);
            }
            catch { }
        }

        /// <summary>
        /// 清除所有消息
        /// </summary>
        public void ClearMessages()
        {
            try
            {
                HCtrl.clearMessages();
            }
            catch { }
        }

        /// <summary>
        /// 清除所有显示
        /// </summary>
        public void ClearHObjects()
        {
            try
            {
                AddObjectToWindow(CurrentImage);
            }
            catch { }
        }

        /// <summary>
        /// 刷新界面显示
        /// </summary>
        public void Repaint()
        {
            try
            {
                HCtrl.repaint();
            }
            catch { }
        }
    }
}