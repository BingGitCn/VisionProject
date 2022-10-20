using HalconDotNet;
using System.Collections.ObjectModel;

namespace BingLibrary.Vision
{
    public class BingImageWindowData
    {
        public HWindow hWindow { set; get; }
        public HWndCtrl HCtrl { set; get; }

        //  public ROIController RCtrl { set; get; }
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
            HCtrl.isShowWaterString = isShow;
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
        /// 是否进入绘画模式
        /// </summary>
        /// <param name="isUse"></param>
        public void DrawMode(bool isUse)
        {
            HCtrl.isDrawing = isUse;
            HalconAPI.CancelDraw();
        }

        /// <summary>
        /// 是否启用编辑ROI
        /// </summary>
        /// <param name="isEdit"></param>
        public void EditMode(bool isEdit)
        {
            HCtrl.canEdit = isEdit;
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
        /// 将图像显示到窗口
        /// </summary>
        /// <param name="hObject"></param>
        /// <param name="roiColor"></param>
        public void AddImageToWindow(HImage himage)
        {
            try
            {
                HCtrl.addImageVar(himage);
            }
            catch { }
        }

        /// <summary>
        /// 将区域添加到ROIList
        /// </summary>
        /// <param name="hRegion"></param>
        /// <param name="roiColor"></param>
        public void AddRegionToROIList(HRegion hRegion, HalconColors roiColor = HalconColors.红色)
        {
            try
            {
                HCtrl.roiManager.AddROI(new ROIRegion(hRegion) { ROIColor = roiColor });
            }
            catch { }
        }

        /// <summary>
        /// 获取Roi List
        /// </summary>
        /// <param name="hRegion"></param>
        /// <param name="roiColor"></param>
        /// <returns></returns>
        public ObservableCollection<ROI> GetROIList(HRegion hRegion, HalconColors roiColor = HalconColors.红色)
        {
            return HCtrl.roiManager.ROIList;
        }

        /// <summary>
        /// 设置ROI当前索引
        /// </summary>
        /// <param name="index"></param>
        /// <param name="color"></param>
        public void SetActive(int index)
        {
            try
            {
                HCtrl.roiManager.setActiveROIIdx(index);
                if (HCtrl.roiManager.ROIList.Count > 0)
                    Repaint();
            }
            catch { }
        }

        /// <summary>
        /// 获取活动的ROI
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ROI GetActive(int index)
        {
            return HCtrl.roiManager.getActiveROI();
        }

        /// <summary>
        /// 将消息显示到窗口
        /// </summary>
        /// <param name="message"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="fontSize"></param>
        public void AddMessageToWindow(string message, int row, int column, int fontSize = 12, HalconColors color = HalconColors.黑色, HalconCoordinateSystem mode = HalconCoordinateSystem.window)
        {
            try
            {
                HCtrl.addMessageVar(message, row, column, fontSize, color, mode);
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
                HCtrl.roiManager.ROIList.Clear();
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

        /// <summary>
        /// 刷新界面显示
        /// </summary>
        public void FitWindow()
        {
            try
            {
                HCtrl.fitWindow();
            }
            catch { }
        }
    }
}