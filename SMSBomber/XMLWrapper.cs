using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace SMSBomber
{
    class XMLWrapper
    {
        public XmlDocument _Doc = null;
        public XmlElement _Root = null;
        public XMLWrapper(string FilePath)
        {
            if (!File.Exists(FilePath))
            {
               string s = Path.GetFileNameWithoutExtension(FilePath);
                CreateXMLDoc(FilePath, s);
                LoadXMLDoc(FilePath);
            }
            else
            {
                try
                {
                    LoadXMLDoc(FilePath);
                }
                catch
                {
                    string s = Path.GetFileNameWithoutExtension(FilePath);
                    CreateXMLDoc(FilePath, s);
                }
            }
        }
        private void LoadXMLDoc(string FilePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FilePath);
            _Root = doc.DocumentElement;
            _Doc = doc;
        }
        public void CreateXMLDoc(string FileName,string RootName)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", null, null);
            doc.AppendChild(dec);
            XmlElement Root = doc.CreateElement(RootName);
            doc.AppendChild(Root);
            _Doc = doc;
            _Root = Root;
            Save();
        }
        
        public void Save()
        {
            string name = _Doc.DocumentElement.Name +".xml";
            XmlTextWriter xtw = new XmlTextWriter(name,null);
            xtw.Formatting = Formatting.Indented;
            _Doc.Save(xtw);
            xtw.Dispose();
            LoadXMLDoc(name);
        }
        public bool ElementExists(string ElementName)
        {
            return GetElementByName(ElementName) != null;

        }
        public bool ElementExists(XmlElement ele,string ElementName)
        {
            return GetElementByName(ele,ElementName) != null;
        }
        public XmlElement AppendElement(XmlElement ele, string newEle)
        {
            XmlElement n = _Doc.CreateElement(newEle);
            ele.AppendChild(n);
          
            return n;
        }
        public XmlElement AppendElement(string NewEle)
        {
            XmlElement temp = _Doc.CreateElement(NewEle);
            _Root.AppendChild(temp);
            return temp;
        }
        public XmlElement AppendElement(string NewEle,string value)
        {
            XmlElement temp = _Doc.CreateElement(NewEle);
            temp.InnerText = value;
            _Root.AppendChild(temp);
            return temp;
        }
        public XmlElement AppendElement(XmlElement ele, string newEle,string value)
        {
            XmlElement n = _Doc.CreateElement(newEle);
            n.InnerText = value;
            ele.AppendChild(n);
          
            return n;
        }
        public void Reload(string name)
        {
            LoadXMLDoc(name);
        }
        public void RemoveElement(XmlElement ele)
        {
            ele.RemoveAll();
        }
        public void RemoveElement(XmlNode ele1,XmlNode ele2)
        {
            ele1.RemoveChild(ele2);
          
        }
        public XmlElement GetElementByName(string name)
        {
            foreach (XmlElement ele in _Root.ChildNodes)
                if (ele.Name == name) return ele;
            return null;
         
        }
        public XmlElement GetElementByName(XmlElement xele,string name)
        {
            foreach (XmlElement ele in xele.ChildNodes)
                if (ele.Name == name) return ele; 
            return null;

        }
        public XmlElement GetElementByText(string text)
        {
            foreach (XmlElement ele in _Root.ChildNodes)
                if (ele.InnerText == text) return ele; 
            return null;
          
        }
        public void SetText(XmlElement ele, string text)
        {
            ele.InnerText = text;
           
        }
        public string GetText(XmlNode ele)
        {
            try
            {
                return ele.InnerText;
            }
            catch { return ""; }
        }
        public void SetAttribute(XmlElement ele, string name, string value)
        {
            XmlAttribute att = _Doc.CreateAttribute(name);
            att.Value = value;
            ele.AppendChild(att);
           
        }
        public string GetAttribute(XmlElement ele, string name)
        {
            return ele.GetAttribute(name);
        }
        public string GetAttribute(XmlNode ele, string name)
        {
            XmlElement temp = (XmlElement)ele;
            return temp.GetAttribute(name);
        }
        public int ElementCount(XmlElement ele)
        {
            return ele.ChildNodes.Count;
        }
    }
}
