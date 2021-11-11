using HalconDotNet;
using System;
using System.Collections.ObjectModel;

namespace BingLibrary.Vision
{
    public delegate void FuncROIDelegate();

    public class ROIController
    {
        public const int MODE_ROI_POS = 21;

        public const int MODE_ROI_NEG = 22;

        public const int MODE_ROI_NONE = 23;

        public const int EVENT_UPDATE_ROI = 50;

        public const int EVENT_CHANGED_ROI_SIGN = 51;

        public const int EVENT_MOVING_ROI = 52;

        public const int EVENT_DELETED_ACTROI = 53;

        public const int EVENT_DELETED_ALL_ROIS = 54;

        public const int EVENT_ACTIVATED_ROI = 55;

        public const int EVENT_CREATED_ROI = 56;

        public ROI roiMode;
        private int stateROI;
        private double currX, currY;

        public event EventHandler ActiveChanged;

        public event EventHandler ROIChanged;

        public int activeROIidx
        {
            get { return mactiveROIidx; }
            set
            {
                if (mactiveROIidx != value)
                {
                    mactiveROIidx = value;
                    if (ActiveChanged != null)
                        ActiveChanged(null, null);
                }
            }
        }

        public int mactiveROIidx = -1;
        public int deletedIdx;

        public ObservableCollection<ROI> ROIList;

        public HRegion ModelROI;

        private string activeCol = "green";//"green,cyan";
        private string activeHdlCol = "violet";
        private string inactiveCol = "medium slate blue";//"magenta";//"yellow";
        public HWndCtrl viewController;

        public IconicDelegate NotifyRCObserver;

        public ROIController()
        {
            stateROI = MODE_ROI_NONE;
            ROIList = new ObservableCollection<ROI>();
            activeROIidx = -1;
            ModelROI = new HRegion();
            NotifyRCObserver = new IconicDelegate(dummyI);
            deletedIdx = -1;
            currX = currY = -1;
        }

        public void setViewController(HWndCtrl view)
        {
            viewController = view;
        }

        public HRegion getModelRegion()
        {
            return ModelROI;
        }

        public ObservableCollection<ROI> getROIList()
        {
            return ROIList;
        }

        public ROI getActiveROI()
        {
            if (activeROIidx != -1)
                return ((ROI)ROIList[activeROIidx]);

            return null;
        }

        public void setActiveROIShape(ROI r)
        {
            setActiveROIIdx(ROIList.IndexOf(r));
        }

        public int getActiveROIIdx()
        {
            return activeROIidx;
        }

        public void setActiveROIIdx(int active)
        {
            activeROIidx = active;
        }

        public int getDelROIIdx()
        {
            return deletedIdx;
        }

        public void setROIShape(ROI r)
        {
            roiMode = r;
            roiMode.setOperatorFlag(stateROI);
        }

        public int GetROIShapeIndex(ROI r)
        {
            return ROIList.IndexOf(r);
        }

        public void showROIShape(ROI r, string roiColor)
        {
            r.ROIColor = roiColor;
            ROIList.Add(r);
            roiMode = null;
            // activeROIidx = ROIList.Count - 1;
            activeROIidx = -1;
            viewController.repaint();
        }

        public void removeROIShape(ROI r)
        {
            if (r == getActiveROI())
            {
                removeActive();
            }
            else
            {
                ROIList.Remove(r);
                //ROIList.RemoveAt(activeROIidx);
                //deletedIdx = activeROIidx;
                activeROIidx = -1;
                viewController.repaint();
                NotifyRCObserver(EVENT_DELETED_ACTROI);
            }
            GC.Collect();//垃圾回收
        }

        public void setROISign(int mode)
        {
            stateROI = mode;

            if (activeROIidx != -1)
            {
                ((ROI)ROIList[activeROIidx]).setOperatorFlag(stateROI);
                viewController.repaint();
                NotifyRCObserver(ROIController.EVENT_CHANGED_ROI_SIGN);
            }
        }

        public void removeActive()
        {
            if (activeROIidx != -1)
            {
                ROIList.RemoveAt(activeROIidx);
                deletedIdx = activeROIidx;
                activeROIidx = -1;
                viewController.repaint();
                NotifyRCObserver(EVENT_DELETED_ACTROI);
            }
        }

        public bool defineModelROI()
        {
            HRegion tmpAdd, tmpDiff, tmp;
            double row, col;

            if (stateROI == MODE_ROI_NONE)
                return true;

            tmpAdd = new HRegion();
            tmpDiff = new HRegion();
            tmpAdd.GenEmptyRegion();
            tmpDiff.GenEmptyRegion();

            for (int i = 0; i < ROIList.Count; i++)
            {
                switch (((ROI)ROIList[i]).getOperatorFlag())
                {
                    case ROI.POSITIVE_FLAG:
                        tmp = ((ROI)ROIList[i]).getRegion();
                        tmpAdd = tmp.Union2(tmpAdd);
                        break;

                    case ROI.NEGATIVE_FLAG:
                        tmp = ((ROI)ROIList[i]).getRegion();
                        tmpDiff = tmp.Union2(tmpDiff);
                        break;

                    default:
                        break;
                }//end of switch
            }//end of for

            ModelROI = null;

            if (tmpAdd.AreaCenter(out row, out col) > 0)
            {
                tmp = tmpAdd.Difference(tmpDiff);
                if (tmp.AreaCenter(out row, out col) > 0)
                    ModelROI = tmp;
            }

            //in case the set of positiv and negative ROIs dissolve
            if (ModelROI == null || ROIList.Count == 0)
                return false;

            return true;
        }

        public void reset()
        {
            ROIList.Clear();
            activeROIidx = -1;
            ModelROI = null;
            roiMode = null;
            viewController.repaint();
            NotifyRCObserver(EVENT_DELETED_ALL_ROIS);
        }

        public void resetROI()
        {
            activeROIidx = -1;
            roiMode = null;
        }

        public void setDrawColor(string aColor,
                                  string aHdlColor,
                                  string inaColor)
        {
            if (aColor != "")
                activeCol = aColor;
            if (aHdlColor != "")
                activeHdlCol = aHdlColor;
            if (inaColor != "")
                inactiveCol = inaColor;
        }

        public void paintData(HalconDotNet.HWindow window, string drawMode)
        {
            //window.SetDraw("margin");
            window.SetDraw(drawMode);
            window.SetLineWidth(2);

            if (ROIList.Count > 0)
            {
                window.SetColor(inactiveCol);
                // window.SetDraw("margin");

                for (int i = 0; i < ROIList.Count; i++)
                {
                    if (string.IsNullOrEmpty(ROIList[i].ROIColor))
                        window.SetColor(inactiveCol);
                    else
                        window.SetColor(ROIList[i].ROIColor);
                    window.SetLineStyle(((ROI)ROIList[i]).flagLineStyle);
                    ((ROI)ROIList[i]).draw(window);
                }

                if (activeROIidx != -1)
                {
                    window.SetColor(activeCol);
                    window.SetLineStyle(((ROI)ROIList[activeROIidx]).flagLineStyle);

                    ((ROI)ROIList[activeROIidx]).draw(window);

                    window.SetColor(activeHdlCol);
                    ((ROI)ROIList[activeROIidx]).displayActive(window);
                }
            }
        }

        public int mouseMoveROI(double imgX, double imgY)
        {
            int idxROI = -1;
            double dist = -1.0;
            if (ROIList.Count > 0)		// ... or an existing one is manipulated
            {
                for (int i = 0; i < ROIList.Count; i++)
                {
                    if (ROIList[i] is ROIRegion || ROIList[i].SizeEnable == false)//ROIRegion例外,SizeEnable flase例外
                        continue;
                    dist = ((ROI)ROIList[i]).distToClosestROI(imgX, imgY);
                    ((ROI)ROIList[i]).ShowRect = false;
                    if (dist == 0.0)
                    {
                        idxROI = i;
                        break;
                    }
                }//end of for

                if (idxROI >= 0)
                {
                    ((ROI)ROIList[idxROI]).ShowRect = true;
                    //System.Diagnostics.Debug.Print(idxROI.ToString());
                }
            }

            viewController.repaint();
            return idxROI;
        }

        public int mouseDownAction(double imgX, double imgY)
        {
            int idxROI = -1;
            int idxSizeEnableROI = -1;
            double max = 10000, dist = 0;
            double epsilon = 20.0;  //矩形移动点击生效的区域		//maximal shortest distance to one of
                                    //the handles

            if (roiMode != null)             //either a new ROI object is created
            {
                roiMode.createROI(imgX, imgY);
                ROIList.Add(roiMode);
                roiMode = null;
                activeROIidx = ROIList.Count - 1;
                viewController.repaint();

                NotifyRCObserver(ROIController.EVENT_CREATED_ROI);
            }
            else if (ROIList.Count > 0)     // ... or an existing one is manipulated
            {
                activeROIidx = -1;

                for (int i = 0; i < ROIList.Count; i++)
                {
                    if (ROIList[i] is ROIRegion)//ROIRegion例外
                    {
                        HRegion tempRegion = new HRegion();//创建一个局部变量并实例化，用来保存鼠标点击的区域
                        tempRegion.GenRegionPoints(imgX, imgY);//根据鼠标点击的位置创建一个Point区域
                        var r = ((ROIRegion)ROIList[i]).mCurHRegion.SelectShapeProto(tempRegion, "overlaps_abs", 1.0, 5.0);//选择与该点存在重合面积在1到5个像素之间的区域

                        HTuple a, b;
                        ((ROIRegion)ROIList[i]).mCurHRegion.AreaCenter(out a, out b);

                        int temp = r.CountObj();//判断是否选择到了区域
                        if (r.CountObj() > 0)
                        {
                            idxSizeEnableROI = i;
                            break;
                        }
                    }
                    else
                    {
                        dist = ((ROI)ROIList[i]).distToClosestHandle(imgX, imgY);
                        if ((dist <= max) && (dist < epsilon))
                        {
                            max = dist;
                            idxROI = i;
                            if (((ROI)ROIList[i]).SizeEnable == true)
                                idxSizeEnableROI = i;
                        }
                    }
                }//end of for
                if (idxROI != idxSizeEnableROI && idxSizeEnableROI >= 0)//优先SizeEnable的
                    idxROI = idxSizeEnableROI;
                if (idxROI >= 0)
                {
                    activeROIidx = idxROI;
                    NotifyRCObserver(ROIController.EVENT_ACTIVATED_ROI);
                }

                viewController.repaint();
            }
            return activeROIidx;
        }

        public void mouseMoveAction(double newX, double newY)
        {
            if ((newX == currX) && (newY == currY))
                return;
            try
            {
                ((ROI)ROIList[activeROIidx]).moveByHandle(newX, newY);
                viewController.repaint();
                currX = newX;
                currY = newY;
                NotifyRCObserver(ROIController.EVENT_MOVING_ROI);
            }
            catch { }
        }

        /***********************************************************/
        private bool haschanged = false;

        public void dummyI(int v)
        {
            if (v == EVENT_MOVING_ROI)
            {
                haschanged = true;
            }
            else if (v == EVENT_UPDATE_ROI && haschanged)
            {
                haschanged = false;
                if (ROIChanged != null)
                    ROIChanged(null, null);
            }
        }
    }
}