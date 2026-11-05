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
    public partial class vhod : Form
    {
        public vhod()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string login = textBox1.Text.Trim();
                string pass = textBox2.Text;

                if (login == "admin" && pass == "123123")
                {
                    Session.IsAdmin = true;
                    Session.Login = login;
                    Session.CustomerId = 0;
                    DialogResult = DialogResult.OK;
                }
                else if (Users.Check(login, pass))
                {
                    Session.IsAdmin = false;
                    Session.Login = login;
                    Session.CustomerId = Users.GetCustomerId(login);
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                    textBox2.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
