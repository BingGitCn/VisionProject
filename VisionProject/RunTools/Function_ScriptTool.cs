using BingLibrary.Extension;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using VisionProject.GlobalVars;
using VisionProject.ViewModels;

namespace VisionProject.RunTools
{
    public static class Function_ScriptTool
    {
        //programData.Parameters
        public static RunResult Run(HImage image, ProgramData programData)
        {
            #region 参数获得

            var row1 = (double)programData.Parameters.BingGetOrAdd("ROIRow1", 0.0);
            var row2 = (double)programData.Parameters.BingGetOrAdd("ROIRow2", 0.0);
            var col1 = (double)programData.Parameters.BingGetOrAdd("ROIColumn1", 0.0);
            var col2 = (double)programData.Parameters.BingGetOrAdd("ROIColumn2", 0.0);
            var IsSaveNG = (bool)programData.Parameters.BingGetOrAdd("IsSaveNG", false);

            var paramDict = (Dictionary<string, ParamSetVar>)Variables.CurrentProgramData.Parameters.BingGetOrAdd("ParamDict", new Dictionary<string, ParamSetVar>());

            var EngineIndex = Variables.CurrentProgramData.Parameters.BingGetOrAdd("EngineIndex", 0).ToString().BingToInt();

            var ScriptName = Variables.CurrentProgramData.Parameters.BingGetOrAdd("ScriptName", "").ToString();

            #endregion 参数获得

            // 照片传进来
            bool resultBool = true;
            try
            {
                image = ((HImage)programData.Parameters.BingGetOrAdd("ROIImage", new HImage())).CopyImage();
                if (row1 + row2 != 0)
                    image = image.CropRectangle1(row1, col1, row2, col2);
            }
            catch { }

            RunResult runResult = new RunResult();

            try
            {
                HDict dict = new HDict();
                dict.CreateDict();

                foreach (var kv in paramDict)
                {
                    if (kv.Value.SelectedIndex == 0)
                        dict.SetDictTuple(kv.Value.Name, new HTuple(bool.Parse(kv.Value.Value)));
                    else if (kv.Value.SelectedIndex == 1)
                        dict.SetDictTuple(kv.Value.Name, new HTuple(kv.Value.Value));
                    else if (kv.Value.SelectedIndex == 2)
                        dict.SetDictTuple(kv.Value.Name, new HTuple(double.Parse(kv.Value.Value)));
                    else if (kv.Value.SelectedIndex == 3)
                        dict.SetDictTuple(kv.Value.Name, new HTuple(int.Parse(kv.Value.Value)));
                }

                //设置图像
                //HImage hImage = new HImage("C:\\Users\\N294\\Pictures\\Camera Roll\\1186626.jpg");
                //dict.SetDictObject(hImage, "Image");

                Variables.WorkEngines[EngineIndex].SetParam(
                     ScriptName, "InputDict", dict);
                bool rst = Variables.WorkEngines[EngineIndex].InspectProcedure(ScriptName);
                //获取结果
                HDict resultDict = Variables.WorkEngines[EngineIndex].GetParam<HalconDotNet.HDict>(ScriptName, "OutputDict");
                //这里约定好对应的输出结果
                runResult.MessageResult = resultDict.GetDictTuple("Result").D.ToString();//检查一下。。0619
            }
            catch { }

            runResult.BoolResult = resultBool;
            runResult.NGImagePath = "";
            return runResult;
        }
    }
}