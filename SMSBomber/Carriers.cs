using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMSBomber
{
    public partial class Carriers : Form
    {
        public Carriers()
        {
            InitializeComponent();
        }
        Settings settings = new Settings();
        private void Carriers_Load(object sender, EventArgs e)
        {
            LoadCarriers();
        }
        private void LoadCarriers()
        {
            listBox1.Items.Clear();
            foreach (string s in settings.LoadCarriers())
            {
                listBox1.Items.Add(s);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
           settings.AddCarrier(textBox1.Text, textBox2.Text);
           LoadCarriers();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            settings.RemoveCarrier(listBox1.SelectedIndex);
        }
    }
}
