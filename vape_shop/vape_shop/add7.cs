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
    public partial class add7 : Form
    {
        public add7()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DbHelper.Add(((Form1)Owner).Data.Сотрудники_на_Сменах, this)) Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
