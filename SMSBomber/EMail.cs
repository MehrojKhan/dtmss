using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSBomber
{
    class EMail
    {
        string Address = "";
        string Password = "";
        string SMTP = "";
        int port = 0;
        bool SSL = false;
        string ToAddress = "";
        string Message = "";
        public bool Stop = false;
        public int TimeOut = 0;
        public int Count = 0;
        public EMail(string Address, string Password, string SMTP, string Port, bool usessl,string To,string Message)
        {
            this.Address = Address;
            this.Password = Password;
            this.SMTP = SMTP;
            port = int.Parse(Port);
            SSL = usessl;
            ToAddress = To;
            this.Message = Message;
        }
        public void Send()
        {
            try
            {
                var smtp = new System.Net.Mail.SmtpClient
                {
                    Host = SMTP,
                    Port = port,
                    EnableSsl = SSL,
                    DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(Address, Password)
                };
                using (var message = new System.Net.Mail.MailMessage("TheBomb@Bomb.net", ToAddress)
                {
                    Subject = "",
                    Body = Message
                })
                {
                    smtp.Send(message);
                }
            }
            catch { }
        }
        public void Send(int count)
        {
            if (count == 0)
            {
                while (Stop == false)
                {
                    Send();
                    Count++;
                    System.Threading.Thread.Sleep(TimeOut);

                }
            }
            else
            {
                while (Count < count && Stop == false)
                {
                    Send();
                    Count++;
                    System.Threading.Thread.Sleep(TimeOut);
                }
            }
        }
    }
}
