using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vape_shop
{
    public partial class add4 : Form
    {
        public add4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DbHelper.Add(((Form1)Owner).Data.Продажи_Чеки, this)) Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
