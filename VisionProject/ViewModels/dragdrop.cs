using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VisionProject.GlobalVars;

namespace VisionProject.ViewModels
{
    public partial class MainWindowViewModel : IDropTarget
    {
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            SubProgram sourceItem = dropInfo.Data as SubProgram;
            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            dropInfo.Effects = DragDropEffects.Move;
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            SubProgram sourceItem = dropInfo.Data as SubProgram;

            Program1.Remove(sourceItem);
            if (Program1.Count <= dropInfo.InsertIndex)
                Program1.Add(sourceItem);
            else
                Program1.Insert(dropInfo.InsertIndex, sourceItem);
            //将界面的program更新到项目的程序集合中。
            try
            {
                Programs[ProgramsName[ProgramsIndex]].Clear();
                for (int i = 0; i < Program1.Count; i++)
                    Programs[ProgramsName[ProgramsIndex]].Add(Program1[i]);
            }
            catch (Exception ex) { }
        }
    }
}