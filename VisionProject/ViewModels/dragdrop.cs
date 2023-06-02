using GongSolutions.Wpf.DragDrop;
using System;
using System.Windows;

namespace VisionProject.ViewModels
{
    public partial class MainWindowViewModel : IDropTarget
    {
        public void DragEnter(IDropInfo dropInfo)
        {
        }

        public void DragLeave(IDropInfo dropInfo)
        {
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            SubProgram sourceItem = dropInfo.Data as SubProgram;
            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            dropInfo.Effects = DragDropEffects.Move;
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            SubProgram sourceItem = dropInfo.Data as SubProgram;

            CurrentProgram.Remove(sourceItem);
            if (CurrentProgram.Count <= dropInfo.InsertIndex)
                CurrentProgram.Add(sourceItem);
            else
                CurrentProgram.Insert(dropInfo.InsertIndex, sourceItem);
            //将界面的program更新到项目的程序集合中。
            try
            {
                Programs[ProgramsName[ProgramsIndex]].Clear();
                for (int i = 0; i < CurrentProgram.Count; i++)
                    Programs[ProgramsName[ProgramsIndex]].Add(CurrentProgram[i]);
            }
            catch (Exception ex) { }
        }
    }
}