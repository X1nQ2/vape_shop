// ФАЙЛ: bue.cs  (замени всё содержимое)
// В конструкторе у кнопки button1 поставь Text = "в корзину".
// В bue.Designer.cs должны быть привязки:
//   button1.Click -> button1_Click
//   button2.Click -> button2_Click
//   textBox2.TextChanged -> textBox2_TextChanged
// Строку this.Load += ...bue_Load в Designer.cs нужно удалить (метода bue_Load тут нет).

using System;
using System.Data;
using System.Windows.Forms;

namespace vape_shop
{
    public partial class bue : Form
    {
        private DataRow row;

        public bue(DataRow r)
        {
            InitializeComponent();
            row = r;

            textBox1.Text = r["Наименование_товара"].ToString();
            textBox1.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox2.Text = "1";
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            int qty;
            if (int.TryParse(textBox2.Text, out qty) && qty > 0)
            {
                decimal price = Convert.ToDecimal(row["Цена_продажи"]);
                textBox3.Text = (qty * price).ToString("0.00") + " ₽";
            }
            else
            {
                textBox3.Text = "";
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            UpdateTotal();
        }

        private void button1_Click(object sender, EventArgs e) // в корзину
        {
            int qty;
            if (!int.TryParse(textBox2.Text, out qty) || qty <= 0)
            {
                MessageBox.Show("Введи количество числом, больше 0");
                return;
            }

            int id = Convert.ToInt32(row["ID_товара"]);
            int stock = Convert.ToInt32(row["Количество"]);
            int inCart = Cart.QtyOf(id);
            int available = stock - inCart;

            if (qty > available)
            {
                MessageBox.Show("Доступно: " + available + " шт. (в корзине уже " + inCart + ")");
                return;
            }

            Cart.Add(id, row["Наименование_товара"].ToString(), Convert.ToDecimal(row["Цена_продажи"]), qty);
            MessageBox.Show("Добавлено в корзину");
            DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e) // отмена
        {
            Close();
        }
    }
}