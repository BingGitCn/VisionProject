using HalconDotNet;
using System;
using System.Collections;

namespace BingLibrary.Vision
{
    public class HObjectEntry
    {
        public Hashtable gContext;

        public HObject HObj;

        public HObjectEntry(HObject obj, Hashtable gc)
        {
            gContext = gc;
            HObj = obj;
        }

        public void Clear()
        {
            gContext.Clear();
            HObj.Dispose();
        }

        public static explicit operator HObjectEntry(HObject v)
        {
            throw new NotImplementedException();
        }
    }
}