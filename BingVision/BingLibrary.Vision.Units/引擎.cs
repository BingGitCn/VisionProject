using HalconDotNet;
using System;
using System.Collections.Generic;

namespace BingLibrary.Vision.Units
{
    public class BingEngine
    {
        private HDevEngine myEngine = new HDevEngine();
        private HDevProgram myProgram = new HDevProgram();
        public Dictionary<string, HDevProcedureCall> myProcedureCalls = new Dictionary<string, HDevProcedureCall>();

        /// <summary>
        /// 初始化引擎
        /// </summary>
        /// <param name="programFilePath">主程序文件路径带后缀</param>
        /// <param name="procedureDirectory">过程路径</param>
        public BingEngine Init(string programFilePath, string procedureDirectory)
        {
            try
            {
                myEngine.SetProcedurePath(procedureDirectory);
                myEngine.SetHDevOperators(null);

                myProgram = new HDevProgram(programFilePath);
            }
            catch (Exception ex) { }
            return this;
        }

        /// <summary>
        /// 添加过程
        /// </summary>
        /// <param name="procedureName">过程名字不带后缀</param>
        public BingEngine AddProcedure(string procedureName)
        {
            try
            {
                myProcedureCalls.Add(procedureName, new HDevProcedureCall(new HDevProcedure(myProgram, procedureName)));
            }
            catch { }
            return this;
        }

        /// <summary>
        /// 实时应用更改
        /// </summary>
        public BingEngine ApplyChanges()
        {
            try { myEngine.UnloadAllProcedures(); } catch { }
            return this;
        }

        public HTuple GetTuple(string procedureName, string variableName)
        {
            try { return myProcedureCalls[procedureName].GetOutputCtrlParamTuple(variableName); }
            catch { return ""; }
        }

        public HImage GetImage(string procedureName, string variableName)
        {
            try { return myProcedureCalls[procedureName].GetOutputIconicParamImage(variableName); }
            catch { return null; }
        }

        public HRegion GetRegion(string procedureName, string variableName)
        {
            try { return myProcedureCalls[procedureName].GetOutputIconicParamRegion(variableName); }
            catch { return null; }
        }

        public HObject GetObject(string procedureName, string variableName)
        {
            try { return myProcedureCalls[procedureName].GetOutputIconicParamObject(variableName); }
            catch { return null; }
        }

        public BingEngine SetTuple(string procedureName, string variableName, HTuple variableValue)
        {
            try { myProcedureCalls[procedureName].SetInputCtrlParamTuple(variableName, variableValue); }
            catch { }
            return this;
        }

        public BingEngine SetObject(string procedureName, string variableName, HObject variableValue)
        {
            try { myProcedureCalls[procedureName].SetInputIconicParamObject(variableName, variableValue); }
            catch { }
            return this;
        }

        public BingEngine Run(string procedureName)
        {
            try
            {
                myProcedureCalls[procedureName].Execute();
            }
            catch { }
            return this;
        }
    }
}