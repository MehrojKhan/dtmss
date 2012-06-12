using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace SMSBomber
{
    class Settings : XMLWrapper
    {
        XmlElement SettingsNode = null;
        XmlElement EMailNode = null;
        XmlElement CarrierNode = null;
        public bool UseThreading = false;
        public string AmountToSend = "";
        public string LastNumber = "";
        public string LastCarrier = "";
        public string LastMessage = "";
        public Settings() : base("Config.xml")
        {
            Load();
        }
        public void Load()
        {
            SettingsNode = ElementExists("Settings") ? GetElementByName("Settings") : AppendElement("Settings");
            CarrierNode = ElementExists("Carriers") ? GetElementByName("Carriers") : AppendElement("Carriers");
            EMailNode = ElementExists("EMails") ? GetElementByName("EMails") : AppendElement("EMails");
            UseThreading = ElementExists(SettingsNode, "Threading") ? Convert.ToBoolean(GetText(GetElementByName(SettingsNode,"Threading"))) : false;
            AmountToSend = ElementExists(SettingsNode, "Send") ? GetText(GetElementByName(SettingsNode, "Send")) : "0";
            LastNumber = ElementExists(SettingsNode, "LastNumber") ? GetText(GetElementByName(SettingsNode, "LastNumber")) : "";
            LastCarrier = ElementExists(SettingsNode, "LastCarrier") ? GetText(GetElementByName(SettingsNode, "LastCarrier")) : "";
            LastMessage = ElementExists(SettingsNode, "LastMessage") ? GetText(GetElementByName(SettingsNode, "LastMessage")) : "";
        }
        public void Reload()
        {
            Reload("Config.xml");
            Load();
        }
        public void SaveSettings()
        {
            try
            {
                if (!ElementExists(SettingsNode, "Threading"))
                    AppendElement(SettingsNode, "Threading", Convert.ToString(UseThreading));
                if (!ElementExists(SettingsNode, "Send"))
                    AppendElement(SettingsNode, "Send", AmountToSend);
                if (!ElementExists(SettingsNode, "LastNumber"))
                    AppendElement(SettingsNode, "LastNumber", LastNumber);
                if (!ElementExists(SettingsNode, "LastCarrier"))
                    AppendElement(SettingsNode, "LastCarrier", LastCarrier);
                if (!ElementExists(SettingsNode, "LastMessage"))
                    AppendElement(SettingsNode, "LastMessage", LastMessage);

                SetText(GetElementByName(SettingsNode, "Threading"), Convert.ToString(UseThreading));
                SetText(GetElementByName(SettingsNode, "Send"), AmountToSend);
                SetText(GetElementByName(SettingsNode, "LastNumber"), LastNumber);
                SetText(GetElementByName(SettingsNode, "LastCarrier"), LastCarrier);
                SetText(GetElementByName(SettingsNode, "LastMessage"), LastMessage);


                Save();
            }
            catch { }
        }
        public string[] LoadEMails()
        {
            List<string> EMails = new List<string>();
            try
            {
                foreach (XmlElement ele in EMailNode.ChildNodes)
                {
                    EMails.Add(GetText(ele.ChildNodes[0]));
                }
            }
            catch { }
            return EMails.ToArray();
        }
        public string[] LoadCarriers()
        {
            List<string> Carriers = new List<string>();
            try
            {
                foreach (XmlElement ele in CarrierNode.ChildNodes)
                {
                  Carriers.Add(GetText(ele.ChildNodes[0]));
                }
            }
            catch { }
            return Carriers.ToArray();
        }
        public string CarrierHost(string name)
        {
           
            try
            {
                foreach (XmlElement ele in CarrierNode.ChildNodes)
                {
                    if (ele.ChildNodes[0].InnerText == name)
                        return ele.ChildNodes[1].InnerText;
                }
            }
            catch { }
            return "";
        }
        public IEmail[] GetClients()
        {
            List<IEmail> Clients = new List<IEmail>();
            try
            {
                foreach (XmlElement ele in EMailNode.ChildNodes)
                {
                    IEmail temp = new IEmail();
                    temp.Address = GetText(ele.ChildNodes[0]);
                    temp.Password = GetText(ele.ChildNodes[1]);
                    temp.SMTP = GetText(ele.ChildNodes[2]);
                    temp.Port = GetText(ele.ChildNodes[3]);
                    temp.SSL = Convert.ToBoolean(GetText(ele.ChildNodes[4]));
                    Clients.Add(temp);
                }
            }
            catch { }
            return Clients.ToArray();
        }
        public void RemoveEmail(int index)
        {
            if (index == -1) return;
            try
            {
                RemoveElement(EMailNode, EMailNode.ChildNodes[index]);
            }
            catch { }
            SaveSettings();
        }
        public void RemoveCarrier(int index)
        {
            if (index == -1) return;
            try
            {
                RemoveElement(CarrierNode, CarrierNode.ChildNodes[index]);
            }
            catch { }
            SaveSettings();
        }
        public void AddEmail(string Address, string password, string smtp, string port, bool usessl)
        {
            if (Address == "" || password == "" || smtp == "" || port == "")
                return;
            try
            {
                XmlElement NewEmail = AppendElement(EMailNode, "EMail");
                AppendElement(NewEmail, "Address", Address);
                AppendElement(NewEmail, "Password", password);
                AppendElement(NewEmail, "SMTP", smtp);
                AppendElement(NewEmail, "Port", port);
                AppendElement(NewEmail, "SSL", Convert.ToString(usessl));
                SaveSettings();
            }
            catch { }

        }
        public void AddCarrier(string Name, string Host)
        {
            if (Name == "" || Host == "")
                return;
            try
            {
                XmlElement NewCarrier = AppendElement(CarrierNode, "Carrier");
                AppendElement(NewCarrier, "Name", Name);
                AppendElement(NewCarrier, "Host", Host);
                SaveSettings();
            }
            catch { }
        }

      
    }
}
