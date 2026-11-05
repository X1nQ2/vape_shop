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
    public partial class add3 : Form
    {
        public add3()
        {
            InitializeComponent();
        }

        private void add3_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DbHelper.Add(((Form1)Owner).Data.Продажи_Диспетчер, this)) Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
