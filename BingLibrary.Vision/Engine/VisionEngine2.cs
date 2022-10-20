using HalconDotNet;
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
    /// 脱离 program，直接调用 procedure，推荐使用。
    /// </summary>
    public class VisionEngine2
    {
        /// <summary>
        /// 引擎
        /// </summary>
        private HDevEngine devEngine;

        /// <summary>
        ///
        /// </summary>
        private Dictionary<string, HDevProcedureCall> devProcedureCalls = new Dictionary<string, HDevProcedureCall>();

        public List<string> ProcedureNames = new List<string>();

        /// <summary>
        /// 清除
        /// </summary>
        /// <param name="name"></param>
        public void ClearProcedures(string name)
        {
            ProcedureNames.Clear();
            devProcedureCalls.Clear();
        }

        /// <summary>
        /// 添加过程
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddProcedure(string name)
        {
            if (ProcedureNames.Contains(name)) return false;
            else ProcedureNames.Add(name);
            return true;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="procedurePath"></param>
        public void Init(string procedurePath)
        {
            devEngine = new HDevEngine();
            devEngine.SetProcedurePath(procedurePath);
            foreach (var p in ProcedureNames)
                devProcedureCalls.Add(p, new HDevProcedureCall(new HDevProcedure(p)));
        }

        /// <summary>
        /// 脚本更新后需重新加载，否则不会实时更新。
        /// </summary>
        public void Reload()
        {
            foreach (var p in ProcedureNames)
            {
                if (devProcedureCalls.Keys.Contains(p))
                {
                    devProcedureCalls[p] = new HDevProcedureCall(new HDevProcedure(p));
                }
                else
                    devProcedureCalls.Add(p, new HDevProcedureCall(new HDevProcedure(p)));

                devEngine.UnloadAllProcedures();
            }
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
                    devEngine.UnloadAllProcedures();//可实时更新脚本
                    devProcedureCalls[procedureName].Execute();
                    return true;
                }
                else
                    return false;
            }
            catch { return false; }
        }

        #region

        /// <summary>
        /// 创建一个新的字典
        /// </summary>
        /// <returns></returns>
        public HTuple GetNewEmptyHDict()
        {
            HTuple hDict;
            HOperatorSet.CreateDict(out hDict);
            return hDict;
        }

        /// <summary>
        /// 设置字典参数
        /// </summary>
        /// <param name="hDict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetHDictObject(ref HTuple hDict, string key, HObject value)
        {
            try
            {
                HOperatorSet.SetDictObject(value, hDict, key);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 设置字典参数
        /// </summary>
        /// <param name="hDict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetHDictTuple(ref HTuple hDict, string key, HTuple value)
        {
            try
            {
                HOperatorSet.SetDictTuple(hDict, key, value);
                return true;
            }
            catch { return false; }
        }

        public HObject GetHDictObject(HTuple hDict, string key)
        {
            HObject hObject = new HObject();
            try
            {
                HOperatorSet.GetDictObject(out hObject, hDict, key);
            }
            catch { }
            return hObject;
        }

        public HTuple GetHDictTuple(HTuple hDict, string key)
        {
            HTuple hTuple = new HTuple();
            try
            {
                HOperatorSet.GetDictTuple(hDict, key, out hTuple);
            }
            catch { }
            return hTuple;
        }

        #endregion
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