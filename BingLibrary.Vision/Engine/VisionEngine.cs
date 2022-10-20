﻿using HalconDotNet;
using System.Collections.Generic;
using System.Linq;

/*************************************************************************************
 *
 * 文 件 名:   VisionEngine
 * 描    述:   step1: AddProcedure 添加需要使用的过程
 *             step2：Init 初始化主程序文件路径以及过程路径
 *
 * 版    本：  V1.0.0.0
 * 创 建 者：  Bing
 * 创建时间：  2022/2/9 14:37:38
 * ======================================
 * 历史更新记录
 * 版本：V          修改时间：         修改人：
 * 修改内容：
 * ======================================
*************************************************************************************/

namespace BingLibrary.Vision.Engine
{
    /// <summary>
    /// 推荐使用 VisionEngine2
    /// </summary>
    public class VisionEngine
    {
        /// <summary>
        /// 引擎
        /// </summary>
        private HDevEngine devEngine;

        /// <summary>
        ///
        /// </summary>
        private Dictionary<string, HDevProcedureCall> devProcedureCalls = new Dictionary<string, HDevProcedureCall>();

        private List<string> procedureNames = new List<string>();

        private HDevProgramCall devProgramCall;

        /// <summary>
        /// 清除
        /// </summary>
        /// <param name="name"></param>
        public void ClearProcedures(string name)
        {
            procedureNames.Clear();
            devProcedureCalls.Clear();
        }

        /// <summary>
        /// 添加过程
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddProcedure(string name)
        {
            if (procedureNames.Contains(name)) return false;
            else procedureNames.Add(name);
            return true;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="programFilePath"></param>
        /// <param name="procedurePath"></param>
        public void Init(string programFilePath, string procedurePath)
        {
            devEngine = new HDevEngine();
            devEngine.SetProcedurePath(procedurePath);
            HDevProgram devProgram = new HDevProgram(programFilePath);
            devProgramCall = new HDevProgramCall(devProgram);
            foreach (var p in procedureNames)
                devProcedureCalls.Add(p, new HDevProcedureCall(new HDevProcedure(devProgram, p)));
        }

        /// <summary>
        /// 设置图像参数
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="imageName"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool SetParamImage(string procedureName, string imageName, HImage image)
        {
            try
            {
                if (devProcedureCalls.Keys.Contains(procedureName))
                {
                    devProcedureCalls[procedureName].SetInputIconicParamObject(imageName, image);
                }
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 设置Tuple参数
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool SetParamTuple(string procedureName, string tupleName, HTuple tuple)
        {
            try
            {
                if (devProcedureCalls.Keys.Contains(procedureName))
                {
                    devProcedureCalls[procedureName].SetInputCtrlParamTuple(tupleName, tuple);
                }
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 设置Region参数
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool SetParamRegion(string procedureName, string regionName, HRegion region)
        {
            try
            {
                if (devProcedureCalls.Keys.Contains(procedureName))
                    devProcedureCalls[procedureName].SetInputIconicParamObject(regionName, region);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool InspectProcedure(string procedureName, HTuple param)
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

        /// <summary>
        /// 运行程序
        /// </summary>
        /// <returns></returns>
        public bool InspectProgram()
        {
            try
            {
                devEngine.UnloadAllProcedures();//可实时更新脚本
                devProgramCall.Execute();
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 获取程序结果
        /// </summary>
        /// <param name="tupleName"></param>
        /// <returns></returns>
        public HTuple GetResultProgramTuple(string tupleName)
        {
            HTuple tuple = new HTuple();
            try
            {
                tuple = devProgramCall.GetCtrlVarTuple(tupleName);
                return tuple;
            }
            catch { return tuple; }
        }

        /// <summary>
        /// 获取程序结果
        /// </summary>
        /// <param name="regionName"></param>
        /// <returns></returns>
        public HRegion GetResultProgramRegion(string regionName)
        {
            HRegion region = new HRegion();
            try
            {
                region = devProgramCall.GetIconicVarRegion(regionName);
                return region;
            }
            catch { return region; }
        }

        /// <summary>
        /// 获取程序结果
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public HImage GetResultProgramImage(string imageName)
        {
            HImage image = new HImage();
            try
            {
                image = devProgramCall.GetIconicVarImage(imageName);
                return image;
            }
            catch { return image; }
        }

        /// <summary>
        /// 获取Tuple结果
        /// </summary>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        public HTuple GetResultTuple(string procedureName, string tupleName)
        {
            HTuple tuple = new HTuple();
            try
            {
                if (devProcedureCalls.Keys.Contains(procedureName))
                    tuple = devProcedureCalls[procedureName].GetOutputCtrlParamTuple(tupleName);
                return tuple;
            }
            catch { return tuple; }
        }

        /// <summary>
        /// 获取Image结果
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public HImage GetResultImage(string procedureName, string imageName)
        {
            HImage image = new HImage();
            try
            {
                if (devProcedureCalls.Keys.Contains(procedureName))
                    image = devProcedureCalls[procedureName].GetOutputIconicParamImage(imageName);
                return image;
            }
            catch { return image; }
        }

        /// <summary>
        /// 获取Region结果
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="regionName"></param>
        /// <returns></returns>
        public HRegion GetResultRegion(string procedureName, string regionName)
        {
            HRegion region = new HRegion();
            try
            {
                if (devProcedureCalls.Keys.Contains(procedureName))
                    region = devProcedureCalls[procedureName].GetOutputIconicParamRegion(regionName);
                return region;
            }
            catch { return region; }
        }

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
    }
}