using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace SMSBomber
{
    public partial class Main : Form
    {
        XMLWrapper xml = new XMLWrapper("Config.xml");
        EMailForm emf = new EMailForm();
        Settings settings = new Settings();
        Carriers carriers = new Carriers();
        List<Thread> ThreadPool = new List<Thread>();
        List<EMail> ClientPool = new List<EMail>();
        Thread MonitorThread = null;
        int TotalSent = 0;
        bool Stop = false;
        public Main()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
            LoadEmails();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            LoadEmails();
            textBox1.Text = settings.LastNumber;
            comboBox1.Text = settings.LastCarrier;
            textBox2.Text = settings.LastMessage;
            checkBox2.Checked = settings.UseThreading;
            textBox5.Text = settings.AmountToSend;
           
        }
        private void LoadEmails()
        {
            settings.Reload();
            listBox1.Items.Clear();
            foreach (string s in settings.LoadEMails())
            {
                listBox1.Items.Add(s);
            }
            try
            {
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
            catch { }
        }
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            settings.Reload();
            settings.UseThreading = checkBox2.Checked;
            settings.AmountToSend = textBox5.Text;
            settings.LastNumber = textBox1.Text;
            settings.LastCarrier = comboBox1.Text;
            settings.LastMessage = textBox2.Text;
            settings.SaveSettings();
            StopAll();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (carriers.IsDisposed)
                carriers = new Carriers();
            if (carriers ==null)
                carriers = new Carriers();
           if(carriers.Visible == false)
               carriers.Visible = true;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (emf.IsDisposed)
                emf = new EMailForm();
            if (emf == null)
                emf = new EMailForm();
            if (emf.Visible == false)
                emf.Visible = true;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            settings.RemoveEmail(listBox1.SelectedIndex);
            settings.Reload();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            foreach (string s in settings.LoadCarriers())
            {
                comboBox1.Items.Add(s);
            }
         
        }
        private bool CharIsNumber(char c)
        {
            if ((int)c >= 48 && (int)c <= 57)
                return true;
            return false;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            foreach (char c in textBox1.Text)
            {
                if (!CharIsNumber(c))
                {
                    textBox1.Text = textBox1.Text.Remove(textBox1.Text.IndexOf(c));
                    MessageBox.Show("Number cannot contain non-numeric values!", "Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            foreach (char c in textBox5.Text)
            {
                if (!CharIsNumber(c))
                {
                    textBox5.Text = textBox5.Text.Remove(textBox5.Text.IndexOf(c));
                    MessageBox.Show("Send value cannot contain non-numeric values!", "Send Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Start();
        }
        private void Start()
        {
            button1.Enabled = false;
            button2.Enabled = true;
            if (checkBox2.Checked)
            {
                if (settings.LoadEMails().Count() > 1)
                {
                    if (textBox5.Text == "0")
                    {
                        IEmail[] iemails = settings.GetClients();
                        foreach (IEmail iemail in iemails)
                        {
                            EMail temp = new EMail(iemail.Address, iemail.Password, iemail.SMTP, iemail.Port, iemail.SSL, textBox1.Text + settings.CarrierHost(comboBox1.Text), textBox2.Text);
                            Thread t = new Thread(new ThreadStart(delegate { temp.Send(0); }));
                            ThreadPool.Add(t);
                            ClientPool.Add(temp);
                            t.Start();
                        }
                    }
                    else
                    {
                        int i;
                        if (!int.TryParse(textBox5.Text, out i))
                            return;
                        int cpt = i / settings.LoadEMails().Count();
                        IEmail[] iemails = settings.GetClients();
                        foreach (IEmail iemail in iemails)
                        {
                            EMail temp = new EMail(iemail.Address, iemail.Password, iemail.SMTP, iemail.Port, iemail.SSL, textBox1.Text + settings.CarrierHost(comboBox1.Text), textBox2.Text);
                            Thread t = new Thread(new ThreadStart(delegate { temp.Send(cpt); }));
                            ThreadPool.Add(t);
                            ClientPool.Add(temp);
                            t.Start();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Multi-threading only works when you have more than one Emails!", "Multi-Threading", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {

                IEmail iemail = settings.GetClients()[listBox1.SelectedIndex];
                EMail temp = new EMail(iemail.Address, iemail.Password, iemail.SMTP, iemail.Port, iemail.SSL, textBox1.Text + settings.CarrierHost(comboBox1.Text), textBox2.Text);
                if (textBox5.Text == "0")
                {
                    Thread t = new Thread(new ThreadStart(delegate { temp.Send(0); }));
                    ThreadPool.Add(t);
                    ClientPool.Add(temp);
                    t.Start();
                }
                else
                {
                    int i;
                    if (!int.TryParse(textBox5.Text, out i))
                        return;
                    Thread t = new Thread(new ThreadStart(delegate { temp.Send(i); }));
                    ThreadPool.Add(t);
                    ClientPool.Add(temp);
                    t.Start();

                }
            }
            MonitorThread = new Thread(Monitor);
            MonitorThread.Start();
        }
        private void StopAll()
        {
            try
            {
                button1.Enabled = true;
                button2.Enabled = false;
                foreach (EMail client in ClientPool)
                {
                    client.Stop = true;
                }
                foreach (Thread t in ThreadPool)
                {
                    try { t.Abort(); }
                    catch { }
                }
                Stop = true;
                MonitorThread.Abort();
            }
            catch { }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            StopAll();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopAll();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopAll();
            Close();
        }
        private void Monitor()
        {
            while (!Stop)
            {
                int temp = 0;
                foreach (EMail es in ClientPool)
                {
                    temp += es.Count;
                }
                TotalSent = temp;
                this.Text = "SMS Bomber : Sent " + TotalSent.ToString();
            }
        }
    }
}
