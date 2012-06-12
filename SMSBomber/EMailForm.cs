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
    public partial class EMailForm : Form
    {
        public EMailForm()
        {
            InitializeComponent();
        }
        Settings settings = new Settings();
        private void button1_Click(object sender, EventArgs e)
        {
            settings.AddEmail(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, checkBox1.Checked);
            this.Visible = false;
        }

        private void EMailForm_Load(object sender, EventArgs e)
        {

        }
    }
}
