using System;
using System.Text;
using System.Xml;

/*************************************************************************************
 *
 * 文 件 名:   SWBase
 * 描    述:
 *
 * 版    本：  V1.0.0.0
 * 创 建 者：  Bing
 * 创建时间：  2022/4/25 9:54:53
 * ======================================
 * 历史更新记录
 * 版本：V          修改时间：         修改人：
 * 修改内容：
 * ======================================
*************************************************************************************/

namespace BingLibrary.Vision
{
    public class ScriptEdit
    {
        //读写Halcon脚本
        private XmlDocument m_xmlDoc = new XmlDocument();

        private StringBuilder m_strBuilderProcedure = new StringBuilder();

        private string procedurePath = "";

        private string procedureName = "";

        /// <summary>
        /// 设置过程路径
        /// </summary>
        /// <param name="path"></param>
        public void SetProcedurePath(string path)
        {
            procedurePath = path;
        }

        /// <summary>
        /// 读过程
        /// </summary>
        public string ReadProcedure(string fileName)
        {
            try
            {
                procedureName = fileName;
                m_strBuilderProcedure.Clear();

                m_xmlDoc.Load(procedurePath + "\\" + procedureName + ".hdvp");

                XmlNodeList xnl = m_xmlDoc.SelectNodes("/hdevelop/procedure");

                foreach (XmlNode xn in xnl)
                {
                    XmlElement xe = (XmlElement)xn;

                    if (xe.GetAttribute("name") == procedureName)
                    {
                        //获取body
                        XmlNode body = xe.SelectSingleNode("body");

                        if (body != null)
                        {
                            XmlNodeList codes = body.ChildNodes;

                            foreach (XmlNode code in codes)
                            {
                                m_strBuilderProcedure.AppendLine(code.InnerText);
                            }
                        }
                    }
                }
                return m_strBuilderProcedure.ToString();
            }
            catch { return "error"; }
        }

        /// <summary>
        /// 保存过程
        /// </summary>
        public void SaveProcedure(string code)
        {
            try
            {
                XmlNodeList xnl = m_xmlDoc.SelectNodes("/hdevelop/procedure");

                foreach (XmlNode xn in xnl)
                {
                    XmlElement xe = (XmlElement)xn;

                    if (xe.GetAttribute("name") == procedureName)
                    {
                        //获取body
                        XmlNode body = xe.SelectSingleNode("body");

                        if (body != null)
                        {
                            body.RemoveAll();

                            string[] codes = code.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var c in codes)
                            {
                                XmlElement item = m_xmlDoc.CreateElement("l");
                                item.InnerText = c;

                                body.AppendChild(item);
                            }
                        }
                    }
                }

                string strTempFile = procedurePath + "\\" + procedureName + ".hdvp";

                m_xmlDoc.Save(strTempFile);
            }
            catch (Exception ex)
            {
            }
        }
    }
}