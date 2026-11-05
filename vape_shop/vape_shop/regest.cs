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
    public partial class regest : Form
    {
        public regest()
        {
            InitializeComponent();
        }

        private void regest_Load(object sender, EventArgs e)
        {

        }

        private void menuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vhod v = new vhod();
            if (v.ShowDialog(this) == DialogResult.OK)
            {
                Hide();
                if (Session.IsAdmin)
                    new Form1().ShowDialog();   // админ: все таблицы
                else
                    new pol().ShowDialog();     // пользователь: только товары
                Close();
            }
        }
        private void регестрацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new reg().ShowDialog(this);
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
