using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace VisionProject.GlobalVars
{
    public class MESClass
    {
        public MESClass()
        {
        }

        private string IP = "";
        private WebRequest request;
        private string strHttpURL = "";

        public void setMESConfig(string ip, string port, string url, string xieyi)
        {
            IP = ip;
            strHttpURL = xieyi + "://" + ip + ":" + port + url;
        }

        /// <summary>
        /// 是否ping成功
        /// </summary>
        /// <returns></returns>
        public bool isConnect()
        {
            Ping ping = new Ping();
            try
            {
                PingReply pingReply = ping.Send(IP);
                if (pingReply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public void SendMESTest(string strBarcode, ref bool bSendStatu, ref string strLogMes, ref string sendLogMes)
        {
            List<MesDatatest> sendData1 = new List<MesDatatest>();
            MesDatatest mesdata1 = new MesDatatest();
            //mesdata1.SN = strBarcode;
            StrInfoXML strInfoXML = new StrInfoXML();
            strInfoXML.SN = strBarcode;
            strInfoXML.PDLINE_NAME = "1";
            strInfoXML.MACHINE_CODE = "1";
            strInfoXML.PIC_PATH = "1";
            strInfoXML.EMP_CODE = "1";
            strInfoXML.Side = "1";
            strInfoXML.RESULT = "1";
            strInfoXML.ProveTime = "1";
            strInfoXML.TotalBlock = "1";
            strInfoXML.NormalBlock = "1";
            strInfoXML.BadBlock = "1";
            mesdata1.TESTDATA = "1";
            mesdata1.strInfoXML = strInfoXML;

            //sendData1.Add(mesdata1);
            string jsonString = "strInfoXML =% 3CInfo % 3E+ % 3CSN % 3E55065100877AA923102800001 % 3C % 2FSN % 3E+ % 3CPDLINE_NAME % 3ENE209 % 3C % 2FPDLINE_NAME % 3E+ % 3CMACHINE_CODE % 3EDA - CS - AVI1 - 001 % 3C % 2FMACHINE_CODE % 3E+ % 3CPIC_PATH % 3EE % 3A % 2FImages % 2F20230310 % 2F55065100877AA923102800001 % 3C % 2FPIC_PATH % 3E+ % 3CEMP_CODE % 3ET07661 % 3C % 2FEMP_CODE % 3E+ % 3CSide % 3ET % 3C % 2FSide % 3E+ % 3CRESULT % 3EY % 3C % 2FRESULT % 3E+ % 3CProveTime % 3E0.1 % 3C % 2FProveTime % 3E+ % 3CTotalBlock % 3E2 % 3C % 2FTotalBlock % 3E+ % 3CNormalBlock % 3E2 % 3C % 2FNormalBlock % 3E+ % 3CBadBlock % 3E0 % 3C % 2FBadBlock % 3E+ % 3C % 2FInfo % 3E & TESTDATA = 1";
            //string jsonString = JsonConvert.SerializeObject(mesdata1);
            sendLogMes = jsonString;
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);

            request = WebRequest.Create("http://prod-webiis-02.tonytech.com:8030/FPCUploadInfo.asmx?op=UP_AOIDATA");
            request.Method = "POST";
            request.ContentLength = byteArray.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            //request.ContentType = "application/json";

            request.Timeout = 5000;

            //写入POST
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();

            using (dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string readdata = reader.ReadToEnd();
                try
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(readdata);
                    string sResult = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                    byte[] bomBuffer = new byte[] { 0xef, 0xbb, 0xbf };
                    if (buffer[0] == bomBuffer[0] && buffer[1] == bomBuffer[1] && buffer[2] == bomBuffer[2])
                    {
                        int copyLength = buffer.Length - 3;
                        byte[] dataNew = new byte[copyLength];
                        Buffer.BlockCopy(buffer, 3, dataNew, 0, copyLength);
                        sResult = System.Text.Encoding.UTF8.GetString(dataNew);
                    }
                    recData recdata = JsonConvert.DeserializeObject<recData>(sResult);
                    if (recdata.status == 200 || recdata.status == 100)
                    {
                        bSendStatu = true;
                    }
                    strLogMes = "data:" + recdata.data + ".status:" + recdata.status;
                }
                catch (Exception ex)
                {
                    bSendStatu = false;
                    strLogMes = ex.Message;
                }
            }

            response.Close();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sn">主码</param>
        /// <param name="mc">机台编号MachineCode</param>
        /// <param name="pc">制程编号ProcessCode</param>
        /// <param name="SN1">子板码1</param>
        /// <param name="SN2">子板码2</param>
        /// <returns></returns>
        public string Upload(string sn, string mc, string pc, string SN1, string SN2)
        {
            string value = "";

            StringBuilder soap = new StringBuilder();
            soap.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" >");

            soap.Append("<soap:Body>");
            soap.Append("<MachineData xmlns=\"http://tempuri.org/\">");
            soap.Append("<SN>" + sn + "</SN>");

            soap.Append("<MachineCode>" + mc + "</MachineCode>");
            soap.Append("<ProcessCode>" + pc + "</ProcessCode>");
            soap.Append("<ParameterAll>" + "1!" + SN1 + "|2!" + SN2 + "</ParameterAll>");

            soap.Append("</MachineData>");
            soap.Append("</soap:Body>");
            soap.Append("</soap:Envelope>");

            try
            {
                //发起请求
                string address = "http://prod-webiis-02.tonytech.com:8070/MachineData.asmx";
                Uri uri = new Uri(address);
                WebRequest webRequest = WebRequest.Create(uri);
                webRequest.ContentType = "text/xml;charset=UTF-8";
                webRequest.Method = "POST";
                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    byte[] param = Encoding.UTF8.GetBytes(soap.ToString());
                    requestStream.Write(param, 0, param.Length);
                }
                //响应
                WebResponse res = webRequest.GetResponse();
                using (StreamReader read = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                {
                    System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                    string str = read.ReadToEnd();
                    xml.LoadXml(str);
                    value = xml.DocumentElement.InnerText;
                }
                return value;
            }
            catch (Exception EX)
            {
                return "FAIL";
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sn">主码</param>
        /// <param name="pd">线别编号</param>
        /// <param name="mc">机台编号</param>
        /// <param name="ImagePath">存图路径</param>
        /// <param name="PNum">员工号</param>
        /// <param name="Re">检测结果</param>
        /// <param name="mesad">MES地址</param>
        /// <param name="aoidata">AOI接口</param>
        /// <returns></returns>
        public string UploadAOI(string sn, string pd, string mc, string ImagePath, string PNum, string Re, string mesad, string aoidata, string msDetail)
        {
            string value = "";
            sn = sn.Replace("条码：", "");
            //客户要求开放出来的值
            //mesad = "http://prod-webiis-02.tonytech.com:8030/FPCUploadInfo.asmx ";
            //aoidata = "UP_AOIDATA";
            //下面两行好像没啥用
            List<MesData> sendData = new List<MesData>();
            MesData mesdata = new MesData();

            StringBuilder soap = new StringBuilder();
            soap.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            soap.Append("<soap:Body>");
            //soap.Append("<UP_AOIDATA xmlns=\"http://tempuri.org/\">");
            soap.Append("<" + aoidata + " xmlns=\"http://tempuri.org/\">");
            soap.Append("<strInfoXML><![CDATA[");
            soap.Append("<Info>");
            soap.Append("<SN>" + sn + "</SN>");
            soap.Append("<PDLINE_NAME>" + pd + " </PDLINE_NAME>");
            soap.Append("<MACHINE_CODE>" + mc + "</MACHINE_CODE>");
            soap.Append("<PIC_PATH>" + ImagePath + "</PIC_PATH>");
            soap.Append("<EMP_CODE>" + PNum + "</EMP_CODE>");
            soap.Append("<Side>" + "T" + "</Side>");
            soap.Append("<RESULT>" + Re + "</RESULT>");
            soap.Append("<ProveTime>" + "1" + "</ProveTime>");
            soap.Append("<TotalBlock>" + "2" + "</TotalBlock>");
            soap.Append("<NormalBlock>2</NormalBlock>");
            soap.Append("<BadBlock>0</BadBlock>");
            soap.Append("</Info>");

            soap.Append("]]></strInfoXML>");
            soap.Append("<TESTDATA>" + msDetail + "</TESTDATA>");
            soap.Append("</UP_AOIDATA>");
            soap.Append("</soap:Body>");
            soap.Append("</soap:Envelope>");

            try
            {
                //发起请求http://prod-webiis-02.tonytech.com:8030/FPCUploadInfo.asmx/UP_AOIDATA
                //string address = "http://prod-webiis-02.tonytech.com:8030/FPCUploadInfo.asmx ";

                Uri uri = new Uri(mesad);
                WebRequest webRequest = WebRequest.Create(uri);
                webRequest.ContentType = "text/xml;charset=UTF-8";
                webRequest.Method = "POST";
                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    byte[] param = Encoding.UTF8.GetBytes(soap.ToString());
                    requestStream.Write(param, 0, param.Length);
                }
                //响应
                BingLibrary.Logs.LogOpreate.Info("MES上传: " + soap.ToString());
                WebResponse res = webRequest.GetResponse();
                using (StreamReader read = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                {
                    System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                    string str = read.ReadToEnd();
                    BingLibrary.Logs.LogOpreate.Info("MES返回: " + soap.ToString());
                    xml.LoadXml(str);
                    value = xml.DocumentElement.InnerText + soap.ToString();
                }

                return value;
            }
            catch (Exception EX)
            {
                BingLibrary.Logs.LogOpreate.Info("MES异常: " + EX.Message);
                return "FAIL";
            }
        }

        /// <summary>
        /// 传入条码，返回料号
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public string CheckLotSoap(string sn)
        {
            string value = "";

            StringBuilder soap = new StringBuilder();
            soap.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" >");
            //soap.Append("<soapenv:Header/>");
            soap.Append("<soap:Body>");
            soap.Append("<GetSN_PartLot xmlns=\"http://tempuri.org/\">");
            soap.Append("<SN>" + sn + "</SN>");

            soap.Append("</GetSN_PartLot>");
            soap.Append("</soap:Body>");
            soap.Append("</soap:Envelope>");

            try
            {
                //发起请求
                string address = "http://prod-webiis-02.tonytech.com:8012/GetSN_Det.asmx";
                Uri uri = new Uri(address);
                WebRequest webRequest = WebRequest.Create(uri);
                webRequest.ContentType = "text/xml;charset=UTF-8";
                webRequest.Method = "POST";
                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    byte[] param = Encoding.UTF8.GetBytes(soap.ToString());
                    requestStream.Write(param, 0, param.Length);
                }
                //响应
                WebResponse res = webRequest.GetResponse();
                using (StreamReader read = new StreamReader(res.GetResponseStream(), Encoding.Default))
                {
                    System.Xml.XmlDocument xml = new System.Xml.XmlDocument();

                    string str = read.ReadToEnd();

                    value = str;
                }
                return value;
            }
            catch (Exception EX)
            {
                return "FAIL";
            }
        }

        /// <summary>
        /// 传入LOT号，返回料号
        /// </summary>
        /// <param name="rcno"></param>
        /// <returns></returns>
        public string GetRCNOSoap(string rcno)
        {
            string value = "";

            StringBuilder soap = new StringBuilder();
            soap.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" >");

            soap.Append("<soap:Body>");
            soap.Append("<GetRCNO_Part xmlns=\"http://tempuri.org/\">");
            soap.Append("<RCNO>" + rcno + "</RCNO>");

            soap.Append("</GetRCNO_Part>");
            soap.Append("</soap:Body>");
            soap.Append("</soap:Envelope>");

            try
            {
                //发起请求
                string address = "http://prod-webiis-02.tonytech.com:8012/GetSN_Det.asmx";
                Uri uri = new Uri(address);
                WebRequest webRequest = WebRequest.Create(uri);
                webRequest.ContentType = "text/xml;charset=UTF-8";
                webRequest.Method = "POST";
                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    byte[] param = Encoding.UTF8.GetBytes(soap.ToString());
                    requestStream.Write(param, 0, param.Length);
                }
                //响应
                WebResponse res = webRequest.GetResponse();
                using (StreamReader read = new StreamReader(res.GetResponseStream(), Encoding.Default))
                {
                    System.Xml.XmlDocument xml = new System.Xml.XmlDocument();

                    string str = read.ReadToEnd();

                    value = str;
                }

                return value;
            }
            catch (Exception EX)
            {
                return "FAIL";
            }
        }

        /// <summary>
        /// 传入主码，没被检测过则返回值包含0
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public string CheckSoap(string sn)
        {
            string value = "";

            StringBuilder soap = new StringBuilder();
            soap.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" >");

            soap.Append("<soap:Body>");
            soap.Append("<LotQualification xmlns=\"http://tempuri.org/\">");
            soap.Append("<SN>" + sn + "</SN>");

            soap.Append("<MachineCode>DA-CS-AVI1-001</MachineCode>");
            soap.Append("<ProcessCode>NE209</ProcessCode>");
            soap.Append("<LineCode>22</LineCode>");
            soap.Append("<OperatorCode>T07661</OperatorCode>");
            soap.Append("<ToolCode>22</ToolCode>");

            soap.Append("</LotQualification>");
            soap.Append("</soap:Body>");
            soap.Append("</soap:Envelope>");

            try
            {
                //发起请求
                string address = "http://prod-webiis-02.tonytech.com:8040/FPCCirculation.asmx";
                Uri uri = new Uri(address);
                WebRequest webRequest = WebRequest.Create(uri);
                webRequest.ContentType = "text/xml;charset=UTF-8";
                webRequest.Method = "POST";
                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    byte[] param = Encoding.UTF8.GetBytes(soap.ToString());
                    requestStream.Write(param, 0, param.Length);
                }
                //响应
                WebResponse res = webRequest.GetResponse();
                using (StreamReader read = new StreamReader(res.GetResponseStream(), Encoding.Default))
                {
                    System.Xml.XmlDocument xml = new System.Xml.XmlDocument();

                    string str = read.ReadToEnd();

                    value = str;
                }

                return value;
            }
            catch (Exception EX)
            {
                return "FAIL";
            }
        }

        public class MesDatatest
        {
            public StrInfoXML strInfoXML { get; set; }

            public string TESTDATA { get; set; }
        }

        public class StrInfoXML
        {
            public string SN { get; set; }
            public string PDLINE_NAME { get; set; }
            public string MACHINE_CODE { get; set; }
            public string PIC_PATH { get; set; }
            public string EMP_CODE { get; set; }
            public string Side { get; set; }
            public string RESULT { get; set; }
            public string ProveTime { get; set; }
            public string TotalBlock { get; set; }
            public string NormalBlock { get; set; }
            public string BadBlock { get; set; }
        }

        public string GetHttpUrl()
        {
            return strHttpURL;
        }
    }

    public class MesData
    {
        public string SN { get; set; }
        public string PDLINE_NAME { get; set; }
        public string MACHINE_CODE { get; set; }
        public string PIC_PATH { get; set; }
        public string EMP_CODE { get; set; }
        public string Side { get; set; }
        public string RESULT { get; set; }
        public string ProveTime { get; set; }
        public string TotalBlock { get; set; }
        public string NormalBlock { get; set; }
        public string BadBlock { get; set; }
    }

    public class recData
    {
        public string message { get; set; }
        public string data { get; set; }
        public int status { get; set; }
        public int code { get; set; }
    }
}