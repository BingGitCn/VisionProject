using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VisionProject.GlobalVars;
using BingLibrary.Communication.Net;
using BingLibrary.Extension;
using BingLibrary.Vision;
using HandyControl.Controls;
using MySqlX.XDevAPI.Relational;
using static VisionProject.ViewModels.Function_CameraViewModel;
using VisionProject.ViewModels;
using System.Collections.ObjectModel;
using Google.Protobuf.WellKnownTypes;
using MySqlX.XDevAPI.Common;
using System.IO;

namespace VisionProject.RunTools
{
    public static class Function_CameraTool
    {
        //programData.Parameters
        public static RunResult Run(HImage image, ProgramData programData)
        {
            HImage resultImage = new HImage();
            try
            {
                var CameraIndex = programData.Parameters.BingGetOrAdd("CameraIndex", 0).ToString().BingToInt();
                var CameraTypeIndex = programData.Parameters.BingGetOrAdd(CameraIndex + ".CameraTypeIndex", 0).ToString().BingToInt();
                var ExpouseTime = programData.Parameters.BingGetOrAdd(CameraIndex + ".ExpouseTime", 200).ToString().BingToDouble();


                Variables.Cameras[CameraIndex].SetExpouseTime(ExpouseTime);
                resultImage = Variables.Cameras[CameraIndex].GrabOne();

            }
            catch { }
            return new RunResult() { ResultImage = resultImage};
        }
    }
}
