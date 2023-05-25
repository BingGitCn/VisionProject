﻿using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;

/*************************************************************************************
 *
 * 文 件 名:   VisionEngine2
 * 描    述:
 *
 * 版    本：  V1.0.0.0
 * 创 建 者：  Bing
 * 创建时间：  2022/4/2 10:10:02
 * ======================================
 * 历史更新记录
 * 版本：V          修改时间：         修改人：
 * 修改内容：
 * ======================================
*************************************************************************************/

namespace BingLibrary.Vision.Engine
{
    /// <summary>
    /// 全局引擎
    /// </summary>
    internal static class HalEngine
    {
        public static HDevEngine myEngine = new HDevEngine();
    }

    /// <summary>
    /// 脱离 program，直接调用 procedure，推荐使用。
    /// </summary>
    public class WorkerEngine
    {
        private Dictionary<string, HDevProcedureCall> devProcedureCalls = new Dictionary<string, HDevProcedureCall>();
        private List<string> procedureNames = new List<string>();

        public List<string> GetProcedureNames()
        {
            return procedureNames;
        }

        /// <summary>
        /// 移除所有脚本
        /// </summary>
        /// <param name="name"></param>
        public void RemoveProcedures()
        {
            foreach (string name in procedureNames)
                devProcedureCalls[name].Dispose();
            procedureNames.Clear();
            devProcedureCalls.Clear();
        }

        /// <summary>
        /// 移除指定脚本
        /// </summary>
        /// <param name="name"></param>
        public void RemoveProcedure(string name)
        {
            procedureNames.Remove(name);
            devProcedureCalls[name].Dispose();
            devProcedureCalls.Remove(name);
        }

        /// <summary>
        /// 添加过程
        /// </summary>
        /// <param name="name">脚本名字，不包含后缀</param>
        /// <param name="path">脚本所在路径</param>
        /// <returns></returns>
        public bool AddProcedure(string name, string path)
        {
            try
            {
                if (procedureNames.Contains(name)) return false;
                else procedureNames.Add(name);

                HalEngine.myEngine.SetProcedurePath(path);
                devProcedureCalls.Add(name, new HDevProcedureCall(new HDevProcedure(name)));
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 获取 procedur 参数信息
        /// </summary>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        public ProcedureInfo GetProcedureInfo(string procedureName)
        {
            try
            {
                ProcedureInfo procedureInfo = new ProcedureInfo();
                procedureInfo.InputCtrlParamCount = devProcedureCalls[procedureName].GetProcedure().GetInputCtrlParamCount();
                procedureInfo.IntputIconicParamCount = devProcedureCalls[procedureName].GetProcedure().GetInputIconicParamCount();

                procedureInfo.OutputCtrlParamCount = devProcedureCalls[procedureName].GetProcedure().GetOutputCtrlParamCount();
                procedureInfo.OutputIconicParamCount = devProcedureCalls[procedureName].GetProcedure().GetOutputIconicParamCount();

                for (int i = 1; i <= procedureInfo.InputCtrlParamCount; i++)
                {
                    procedureInfo.InputCtrlParamNames.Add(devProcedureCalls[procedureName].GetProcedure().GetInputCtrlParamName(i));
                }

                for (int i = 1; i <= procedureInfo.IntputIconicParamCount; i++)
                {
                    procedureInfo.InputIconicParamNames.Add(devProcedureCalls[procedureName].GetProcedure().GetInputIconicParamName(i));
                }

                for (int i = 1; i <= procedureInfo.OutputCtrlParamCount; i++)
                {
                    procedureInfo.OutputCtrlParamNames.Add(devProcedureCalls[procedureName].GetProcedure().GetOutputCtrlParamName(i));
                }

                for (int i = 1; i <= procedureInfo.OutputIconicParamCount; i++)
                {
                    procedureInfo.OutputIconicParamNames.Add(devProcedureCalls[procedureName].GetProcedure().GetOutputIconicParamName(i));
                }

                return procedureInfo;
            }
            catch { return new ProcedureInfo(); }
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public bool SetParam<T>(string procedureName, string paramName, T paramValue)
        {
            try
            {
                if (paramValue is HImage || paramValue is HRegion)
                {
                    if (devProcedureCalls.Keys.Contains(procedureName))
                        devProcedureCalls[procedureName].SetInputIconicParamObject(paramName, paramValue as HObject);
                }
                else if (paramValue is HTuple)
                {
                    if (devProcedureCalls.Keys.Contains(procedureName))
                        devProcedureCalls[procedureName].SetInputCtrlParamTuple(paramName, paramValue as HTuple);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public T GetParam<T>(string procedureName, string paramName) where T : class
        {
            try
            {
                if (typeof(T) == typeof(HImage))
                {
                    if (devProcedureCalls.Keys.Contains(procedureName))
                        return devProcedureCalls[procedureName].GetOutputIconicParamImage(paramName) as T;
                }
                else if (typeof(T) == typeof(HRegion))
                {
                    if (devProcedureCalls.Keys.Contains(procedureName))
                        return devProcedureCalls[procedureName].GetOutputIconicParamImage(paramName) as T;
                }
                else if (typeof(T) == typeof(HTuple))
                {
                    if (devProcedureCalls.Keys.Contains(procedureName))
                        return devProcedureCalls[procedureName].GetOutputCtrlParamTuple(paramName) as T;
                }
                return default(T);
            }
            catch { return default(T); }
        }

        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        public bool InspectProcedure(string procedureName)
        {
            try
            {
                if (devProcedureCalls.Keys.Contains(procedureName))
                {
                    devProcedureCalls[procedureName].Execute();
                    return true;
                }
                else
                    return false;
            }
            catch { return false; }
        }
    }

    public class ProcedureInfo
    {
        public int InputCtrlParamCount { set; get; }
        public int IntputIconicParamCount { set; get; }
        public int OutputCtrlParamCount { set; get; }
        public int OutputIconicParamCount { set; get; }

        public List<string> InputCtrlParamNames { set; get; } = new List<string>();
        public List<string> InputIconicParamNames { set; get; } = new List<string>();
        public List<string> OutputCtrlParamNames { set; get; } = new List<string>();
        public List<string> OutputIconicParamNames { set; get; } = new List<string>();
    }
}