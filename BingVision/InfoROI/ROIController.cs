using BingLibrary.Extension;
using HalconDotNet;
using System;
using System.Collections.ObjectModel;

namespace BingLibrary.Vision
{
    public enum ModeROI
    {
        Roi_None,

        //��ͷ������ֱ�ߵ�
        Roi_Positive,

        Roi_Negative,
    }

    /// <summary>
    /// ROI������
    /// </summary>
    public class ROIController
    {
        public ObservableCollection<ROIBase> ROIList;

        private HalconColors activeCol = HalconColors.��ɫ;//"green,cyan";
        private HalconColors activeHdlCol = HalconColors.��ɫ;
        private HalconColors inactiveCol = HalconColors.��ɫ;//"magenta";//"yellow";

        public ROIBase roi;

        /// <summary>
        /// ���캯��
        /// </summary>
        public ROIController()
        {
            ROIList = new ObservableCollection<ROIBase>();
            ActiveROIidx = -1;
            currX = currY = -1;
        }

        private double currX, currY;

        /// <summary>
        /// ���ROI index
        /// </summary>
        public int ActiveROIidx { set; get; }

        /// <summary>
        /// ��ȡROI�б�
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<ROIBase> GetROIList()
        {
            return ROIList;
        }

        /// <summary>
        /// ��ȡ��ǰ���ROI
        /// </summary>
        /// <returns></returns>
        public ROIBase GetActiveROI()
        {
            if (ActiveROIidx != -1)
                return ((ROIBase)ROIList[ActiveROIidx]);

            return null;
        }

        /// <summary>
        /// ��ȡѡ�е�ROI����
        /// </summary>
        /// <returns></returns>
        public int GetActiveROIIdx()
        {
            return ActiveROIidx;
        }

        /// <summary>
        /// ���û��ROI����
        /// </summary>
        /// <param name="active"></param>
        public void SetActiveROIIdx(int active)
        {
            ActiveROIidx = active;
        }

        /// <summary>
        /// ����ROI
        /// </summary>
        /// <param name="r"></param>
        public void AddROI(ROIBase r)
        {
            ROIList.Add(r);
            ActiveROIidx = -1;
        }

        /// <summary>
        /// �Ƴ�roi
        /// </summary>
        /// <param name="idx"></param>
        public void RemoveROI(int idx)
        {
            try
            {
                ROIList.RemoveAt(idx);
                ActiveROIidx = -1;
                GC.Collect();//��������
            }
            catch { }
        }

        /// <summary>
        /// �Ƴ����ROI
        /// </summary>
        public void RemoveActiveRoi()
        {
            try
            {
                if (ActiveROIidx != -1)
                {
                    ROIList.RemoveAt(ActiveROIidx);
                    ActiveROIidx = -1;
                }
            }
            catch { }
        }

        public void Clear()
        {
            ROIList.Clear();
            ActiveROIidx = -1;
            roi = null;
            GC.Collect();
        }

        public void ResetROI()
        {
            ActiveROIidx = -1;
            roi = null;
        }

        /// <summary>
        /// ��ʾROI
        /// </summary>
        /// <param name="window"></param>
        /// <param name="drawMode"></param>
        public void PaintData(HalconDotNet.HWindow window, HalconDrawing drawMode)
        {
            window.SetDraw(drawMode.ToDescription());
            window.SetLineWidth(2);

            if (ROIList.Count > 0)
            {
                window.SetColor(inactiveCol.ToDescription());

                for (int i = 0; i < ROIList.Count; i++)
                {
                    if (string.IsNullOrEmpty(ROIList[i].ROIColor.ToDescription()))
                        window.SetColor(inactiveCol.ToDescription());
                    else
                        window.SetColor(ROIList[i].ROIColor.ToDescription());
                    window.SetLineStyle(((ROIBase)ROIList[i]).flagLineStyle);
                    ROIList[i].draw(window);
                }

                if (ActiveROIidx != -1)
                {
                    window.SetColor(activeCol.ToDescription());
                    window.SetLineStyle(((ROIBase)ROIList[ActiveROIidx]).flagLineStyle);

                    ROIList[ActiveROIidx].draw(window);

                    window.SetColor(activeHdlCol.ToDescription());
                    ROIList[ActiveROIidx].displayActive(window);
                }
            }
        }

        //���������ʾ���϶�������
        public int MouseMoveROI(double imgX, double imgY)
        {
            int idxROI = -1;
            double dist = -1.0;
            if (ROIList.Count > 0)
            {
                for (int i = 0; i < ROIList.Count; i++)
                {
                    if (ROIList[i].SizeEnable == false)//SizeEnable flase����
                        continue;
                    dist = ROIList[i].distToClosestROI(imgX, imgY);
                    ROIList[i].ShowRect = true;
                    if (dist == 0.0)
                    {
                        idxROI = i;
                        break;
                    }
                }//end of for

                if (idxROI >= 0)
                {
                    ROIList[idxROI].ShowRect = true;
                    //System.Diagnostics.Debug.Print(idxROI.ToString());
                }
            }

            return idxROI;
        }

        /// <summary>
        /// �ж��Ƿ���������
        /// </summary>
        /// <param name="imgX"></param>
        /// <param name="imgY"></param>
        /// <returns></returns>
        public int MouseDownAction(double imgX, double imgY)
        {
            int idxROI = -1;
            int idxSizeEnableROI = -1;
            double max = 10000, dist = 0;
            //maximal shortest distance to one of
            //the handles

            if (roi != null)             //either a new ROI object is created
            {
                roi.createROI(imgX, imgY);
                ROIList.Add(roi);
                roi = null;
                ActiveROIidx = ROIList.Count - 1;
            }
            else if (ROIList.Count > 0)     // ... or an existing one is manipulated
            {
                ActiveROIidx = -1;

                for (int i = 0; i < ROIList.Count; i++)
                {
                    double epsilon = ROIList[i].smallregionwidth;  //�����ƶ������Ч������
                    if (ROIList[i] is ROIRegion)//ROIRegion����
                    {
                        HRegion tempRegion = new HRegion();//����һ���ֲ�������ʵ�������������������������
                        tempRegion.GenRegionPoints(imgY, imgX);//�����������λ�ô���һ��Point����
                        var r = ((ROIRegion)ROIList[i]).mCurHRegion.Intersection(tempRegion);
                        if (r.Area.TupleMax() >= 1)
                        {
                            idxSizeEnableROI = i;
                            break;
                        }
                    }
                    else
                    {
                        dist = ((ROIBase)ROIList[i]).distToClosestHandle(imgX, imgY);
                        if ((dist <= max) && (dist < epsilon))
                        {
                            max = dist;
                            idxROI = i;
                            if (((ROIBase)ROIList[i]).SizeEnable == true)
                                idxSizeEnableROI = i;
                        }
                    }
                }//end of for
                if (idxROI != idxSizeEnableROI && idxSizeEnableROI >= 0)//����SizeEnable��
                    idxROI = idxSizeEnableROI;
                if (idxROI >= 0)
                {
                    ActiveROIidx = idxROI;
                }
            }
            return ActiveROIidx;
        }

        /// <summary>
        /// ������Ա��϶�������ʾ������Hregionֱ���϶�
        /// </summary>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        /// <param name="motionX"></param>
        /// <param name="motionY"></param>
        public void MouseMoveAction(double newX, double newY, double motionX = 0, double motionY = 0)
        {
            if ((newX == currX) && (newY == currY))
                return;
            try
            {
                if (ROIList[ActiveROIidx] is ROIRegion)//ROIRegion
                {
                    ((ROIRegion)ROIList[ActiveROIidx]).mCurHRegion = ((ROIRegion)ROIList[ActiveROIidx]).mCurHRegion.MoveRegion((int)motionY, (int)motionX);
                }
                else
                {
                    ROIList[ActiveROIidx].moveByHandle(newX, newY);
                }

                currX = newX;
                currY = newY;
            }
            catch { }
        }
    }
}