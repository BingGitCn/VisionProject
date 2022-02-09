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

    public class ROIController
    {
        public HWndCtrl viewController;
        public ObservableCollection<ROI> ROIList;

        private HalconColors activeCol =  HalconColors.��ɫ;//"green,cyan";
        private HalconColors activeHdlCol =  HalconColors.��ɫ;
        private HalconColors inactiveCol =  HalconColors.��ɫ;//"magenta";//"yellow";

        public ROI roi;

        /// <summary>
        /// ���캯��
        /// </summary>
        public ROIController()
        {
            ROIList = new ObservableCollection<ROI>();
            ActiveROIidx = -1;
            currX = currY = -1;
        }

        /// <summary>
        /// ����view
        /// </summary>
        /// <param name="view"></param>
        public void setViewController(HWndCtrl view)
        {
            viewController = view;
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
        public ObservableCollection<ROI> getROIList()
        {
            return ROIList;
        }

        /// <summary>
        /// ��ȡ��ǰ���ROI
        /// </summary>
        /// <returns></returns>
        public ROI getActiveROI()
        {
            if (ActiveROIidx != -1)
                return ((ROI)ROIList[ActiveROIidx]);

            return null;
        }

        /// <summary>
        /// ��ȡѡ�е�ROI����
        /// </summary>
        /// <returns></returns>
        public int getActiveROIIdx()
        {
            return ActiveROIidx;
        }

        /// <summary>
        /// ���û��ROI����
        /// </summary>
        /// <param name="active"></param>
        public void setActiveROIIdx(int active)
        {
            ActiveROIidx = active;
        }

        /// <summary>
        /// ����ROI
        /// </summary>
        /// <param name="r"></param>
        public void AddROI(ROI r)
        {
            ROIList.Add(r);
            ActiveROIidx = -1;
            viewController.repaint();
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
                viewController.repaint();
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
                    viewController.repaint();
                }
            }
            catch { }
        }

        public void Reset()
        {
            ROIList.Clear();
            ActiveROIidx = -1;
            roi = null;
            viewController.repaint();
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
        public void paintData(HalconDotNet.HWindow window, HalconDrawing drawMode)
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
                    window.SetLineStyle(((ROI)ROIList[i]).flagLineStyle);
                    ROIList[i].draw(window);
                }

                if (ActiveROIidx != -1)
                {
                    window.SetColor(activeCol.ToDescription());
                    window.SetLineStyle(((ROI)ROIList[ActiveROIidx]).flagLineStyle);

                    ROIList[ActiveROIidx].draw(window);

                    window.SetColor(activeHdlCol.ToDescription());
                    ROIList[ActiveROIidx].displayActive(window);
                }
            }
        }

        //���������ʾ���϶�������
        public int mouseMoveROI(double imgX, double imgY)
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

            viewController.repaint();
            return idxROI;
        }

        /// <summary>
        /// �ж��Ƿ���������
        /// </summary>
        /// <param name="imgX"></param>
        /// <param name="imgY"></param>
        /// <returns></returns>
        public int mouseDownAction(double imgX, double imgY)
        {
            int idxROI = -1;
            int idxSizeEnableROI = -1;
            double max = 10000, dist = 0;
            double epsilon = 20.0;  //�����ƶ������Ч������		//maximal shortest distance to one of
                                    //the handles

            if (roi != null)             //either a new ROI object is created
            {
                roi.createROI(imgX, imgY);
                ROIList.Add(roi);
                roi = null;
                ActiveROIidx = ROIList.Count - 1;
                viewController.repaint();
            }
            else if (ROIList.Count > 0)     // ... or an existing one is manipulated
            {
                ActiveROIidx = -1;

                for (int i = 0; i < ROIList.Count; i++)
                {
                    if (ROIList[i] is ROIRegion)//ROIRegion����
                    {
                        HRegion tempRegion = new HRegion();//����һ���ֲ�������ʵ�������������������������
                        tempRegion.GenRegionPoints(imgY, imgX);//�����������λ�ô���һ��Point����
                        var r = ((ROIRegion)ROIList[i]).mCurHRegion.Intersection(tempRegion);
                        if (r.Area > 0)
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
                if (idxROI != idxSizeEnableROI && idxSizeEnableROI >= 0)//����SizeEnable��
                    idxROI = idxSizeEnableROI;
                if (idxROI >= 0)
                {
                    ActiveROIidx = idxROI;
                }

                viewController.repaint();
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
        public void mouseMoveAction(double newX, double newY, double motionX = 0, double motionY = 0)
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

                viewController.repaint();
                currX = newX;
                currY = newY;
            }
            catch { }
        }
    }
}