using System;
using System.IO;
using System.Xml;

namespace GoDogCommon
{
    class Configuration
    {
        protected static Settings _Settings = new Settings();

        private static string _InputStreamURL = _Settings.GetSetting("InputStreamURL", "");
        public static string InputStreamURL
        {
            get { return _InputStreamURL; }
            set
            {
                _InputStreamURL = value;
                _Settings.PutSetting("InputStreamURL", value);
            }
        }

        private static string _OutputStreamURL = _Settings.GetSetting("OutputStreamURL", "");
        public static string OutputStreamURL
        {
            get { return _OutputStreamURL; }
            set
            {
                _OutputStreamURL = value;
                _Settings.PutSetting("OutputStreamURL", value);
            }
        }

        public class Settings
        {
            XmlDocument xmlDocument = new XmlDocument();
            string ConfigFile = AppDomain.CurrentDomain.BaseDirectory + "\\settings.xml";

            public Settings()
            {
                try
                {
                    using (var reader = new StreamReader(ConfigFile))
                    {
                        xmlDocument.Load(reader);
                    }
                }
                catch
                {
                    //xmlDocument.LoadXml(@"<settings><InputStreamURL>rtsp://root:password@192.168.1.43/axis-media/media.amp</InputStreamURL><OutputStreamURL>rtmp://104.248.182.51/live/1</OutputStreamURL></settings>");
                    xmlDocument.LoadXml(@"<settings><InputStreamURL>rtsp://admin:Foxhound1!@192.168.1.64/Streaming/Channels/1</InputStreamURL><OutputStreamURL>rtmp://104.248.182.51/live/2</OutputStreamURL></settings>");
                    SaveXmlDocument();
                }
            }

            public int GetSetting(string xPath, int defaultValue)
            { return Convert.ToInt16(GetSetting(xPath, Convert.ToString(defaultValue))); }

            public bool GetSetting(string xPath, bool defaultValue)
            { return Convert.ToBoolean(GetSetting(xPath, Convert.ToString(defaultValue))); }

            public string GetSetting(string xPath, string defaultValue)
            {
                XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
                if (xmlNode != null)
                {
                    return xmlNode.InnerText;
                }
                else
                {
                    return defaultValue;
                }
            }

            public void PutSetting(string xPath, int value)
            { PutSetting(xPath, Convert.ToString(value)); }

            public void PutSetting(string xPath, bool value)
            { PutSetting(xPath, Convert.ToString(value)); }

            public void PutSetting(string xPath, string value)
            {
                XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
                if (xmlNode == null) { xmlNode = createMissingNode("settings/" + xPath); }
                xmlNode.InnerText = value;

                SaveXmlDocument();
            }

            private void SaveXmlDocument()
            {
                try
                {
                    using (var writer = new StreamWriter(ConfigFile))
                    {
                        xmlDocument.Save(writer);
                    }
                }
                catch { }
            }

            private XmlNode createMissingNode(string xPath)
            {
                string[] xPathSections = xPath.Split('/');
                string currentXPath = "";
                XmlNode testNode = null;
                XmlNode currentNode = xmlDocument.SelectSingleNode("settings");
                foreach (string xPathSection in xPathSections)
                {
                    currentXPath += xPathSection;
                    testNode = xmlDocument.SelectSingleNode(currentXPath);
                    if (testNode == null)
                    {
                        currentNode.InnerXml += "<" +
                                    xPathSection + "></" +
                                    xPathSection + ">";
                    }
                    currentNode = xmlDocument.SelectSingleNode(currentXPath);
                    currentXPath += "/";
                }
                return currentNode;
            }
        }
    }
}
