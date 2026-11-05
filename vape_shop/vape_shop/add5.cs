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
    public partial class add5 : Form
    {
        public add5()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DbHelper.Add(((Form1)Owner).Data.Смены, this)) Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
