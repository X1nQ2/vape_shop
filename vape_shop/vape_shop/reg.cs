using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace vape_shop
{
    public partial class reg : Form
    {
        public reg()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text.Trim();
            string pass = textBox2.Text;
            string pass2 = textBox3.Text;
            string fio = textBox4.Text.Trim();
            string phone = textBox5.Text.Trim();
            int age;

            if (login == "" || pass == "" || pass2 == "" || fio == "" || phone == "")
            {
                MessageBox.Show("Заполни все поля");
                return;
            }
            if (!int.TryParse(textBox6.Text, out age) || age <= 0)
            {
                MessageBox.Show("Возраст введи числом");
                return;
            }
            if (age < 18)
            {
                MessageBox.Show("Регистрация только с 18 лет");
                return;
            }
            if (login.ToLower() == "admin")
            {
                MessageBox.Show("Этот логин занят");
                return;
            }
            if (pass != pass2)
            {
                MessageBox.Show("Пароли не совпадают");
                textBox2.Clear();
                textBox3.Clear();
                return;
            }

            try
            {
                if (Users.Register(login, pass, fio, phone, age))
                {
                    MessageBox.Show("Регистрация прошла");
                    Close();
                }
                else
                {
                    MessageBox.Show("Такой логин уже есть");
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

        private void reg_Load(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}