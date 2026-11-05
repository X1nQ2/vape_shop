using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.OleDb;

namespace vape_shop
{
    public partial class kor : Form
    {
        private readonly DataTable table;
        private readonly Func<int> update;

        public kor(DataTable products, Func<int> saveProducts)
        {
            InitializeComponent();
            ClientSize = new Size(700, 390);
            dataGridView1.Location = new Point(12, 42);
            dataGridView1.Size = new Size(676, 250);
            labelTotal.Location = new Point(12, 300);
            labelTotal.Size = new Size(400, 24);
            button1.Location = new Point(12, 335);
            button1.Size = new Size(90, 30);
            button2.Location = new Point(110, 335);
            button2.Size = new Size(90, 30);
            button3.Location = new Point(208, 335);
            button3.Size = new Size(150, 30);
            button4.Location = new Point(366, 335);
            button4.Size = new Size(90, 30);
            table = products;
            update = saveProducts;

            Text = "Корзина";
            StartPosition = FormStartPosition.CenterParent;
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.DataSource = Cart.Items;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dataGridView1.Columns["Id"] != null)
                dataGridView1.Columns["Id"].Visible = false;
            if (dataGridView1.Columns["Name"] != null)
                dataGridView1.Columns["Name"].HeaderText = "Товар";
            if (dataGridView1.Columns["Price"] != null)
                dataGridView1.Columns["Price"].HeaderText = "Цена";
            if (dataGridView1.Columns["Qty"] != null)
                dataGridView1.Columns["Qty"].HeaderText = "Количество";
            if (dataGridView1.Columns["Sum"] != null)
                dataGridView1.Columns["Sum"].HeaderText = "Сумма";

            button1.Text = "Удалить";
            button2.Text = "Очистить";
            button3.Text = "Оформить заказ";
            button4.Text = "Закрыть";
            button1.Click += Remove_Click;
            button2.Click += Clear_Click;
            button3.Click += Pay_Click;
            button4.Click += (s, e) => Close();

            // Кнопки изменения количества
            Button minus = new Button { Name = "buttonMinus", Text = "− 1", Width = 65, Height = 28, Left = 8, Top = 8 };
            Button plus = new Button { Name = "buttonPlus", Text = "+ 1", Width = 65, Height = 28, Left = 78, Top = 8 };
            Controls.Add(minus);
            Controls.Add(plus);
            minus.Click += (s, e) => ChangeSelectedQty(-1);
            plus.Click += (s, e) => ChangeSelectedQty(1);
            minus.BringToFront();
            plus.BringToFront();


            Cart.Items.ListChanged += Items_ListChanged;
            FormClosed += (s, e) => Cart.Items.ListChanged -= Items_ListChanged;
            ShowTotal();
        }

        private CartItem SelectedItem()
        {
            return dataGridView1.CurrentRow == null ? null :
                dataGridView1.CurrentRow.DataBoundItem as CartItem;
        }

        private void ChangeSelectedQty(int delta)
        {
            CartItem item = SelectedItem();
            if (item == null)
            {
                MessageBox.Show("Выбери товар в корзине");
                return;
            }

            if (delta > 0)
            {
                DataRow[] rows = table.Select("ID_товара = " + item.Id);
                if (rows.Length == 0 || item.Qty >= Convert.ToInt32(rows[0]["Количество"]))
                {
                    MessageBox.Show("Больше товара нет на складе");
                    return;
                }
            }

            Cart.ChangeQty(item, delta);
            ShowTotal();
        }

        private void Items_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            ShowTotal();
        }

        private void ShowTotal()
        {
            labelTotal.Text = "Итого: " + Cart.Total.ToString("0.00") +
                " ₽    Товаров: " + Cart.Count;
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            CartItem item = SelectedItem();
            if (item == null)
            {
                MessageBox.Show("Выбери товар в корзине");
                return;
            }
            Cart.Items.Remove(item);
            ShowTotal();
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            if (Cart.Items.Count == 0) return;
            if (MessageBox.Show("Очистить корзину?", "Корзина",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Cart.Items.Clear();
            ShowTotal();
        }

        private static void EnsureReceiptFioColumn(OleDbConnection connection)
        {
            DataTable columns = connection.GetSchema("Columns", new string[] { null, null, "Позиции_Чека", "ФИО" });
            if (columns.Rows.Count == 0)
            {
                using (var alter = new OleDbCommand("ALTER TABLE [Позиции_Чека] ADD COLUMN [ФИО] TEXT(255)", connection))
                    alter.ExecuteNonQuery();
            }
        }

        private static string GetCustomerFio(OleDbConnection connection, OleDbTransaction transaction)
        {
            using (var command = new OleDbCommand(
                "SELECT [ФИО] FROM [Покупатели] WHERE [ID_Покупателя] = ?", connection, transaction))
            {
                command.Parameters.Add("pCustomer", OleDbType.Integer).Value = Session.CustomerId;
                object value = command.ExecuteScalar();
                if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(Convert.ToString(value)))
                    throw new Exception("Для текущего аккаунта не заполнено ФИО покупателя.");
                return Convert.ToString(value).Trim();
            }
        }

        private void Pay_Click(object sender, EventArgs e)
        {
            if (Cart.Items.Count == 0)
            {
                MessageBox.Show("Корзина пуста");
                return;
            }

            if (Session.CustomerId <= 0)
            {
                MessageBox.Show("Не удалось определить покупателя. Выйдите и войдите в аккаунт заново.");
                return;
            }

            foreach (CartItem item in Cart.Items)
            {
                DataRow[] rows = table.Select("ID_товара = " + item.Id);
                if (rows.Length == 0 || Convert.ToInt32(rows[0]["Количество"]) < item.Qty)
                {
                    MessageBox.Show("Товара «" + item.Name + "» не хватает на складе");
                    return;
                }
            }

            decimal total = Cart.Total;
            if (MessageBox.Show("К оплате: " + total.ToString("0.00") +
                " ₽. Подтвердить покупку?", "Оформление заказа",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            int createdSaleId = 0;
            string stage = "подключение к базе данных";
            try
            {
                string cs = Properties.Settings.Default.dtConnectionString;
                using (var connection = new OleDbConnection(cs))
                {
                    connection.Open();
                    EnsureReceiptFioColumn(connection);
                    using (var transaction = connection.BeginTransaction())
                    {
                        string customerFio = GetCustomerFio(connection, transaction);

                        // 1. Создаём заголовок чека. Типы параметров задаём явно:
                        // OleDb использует позиционные параметры и не должен угадывать типы.
                        stage = "создание чека";
                        using (var receipt = new OleDbCommand(
                            "INSERT INTO [Продажи_Чеки] ([Дата_Продажи], [Сумма_Чека], [Способ_Оплаты], [Статус_Терминала]) VALUES (?, ?, ?, ?)",
                            connection, transaction))
                        {
                            receipt.Parameters.Add("pDate", OleDbType.Date).Value = DateTime.Now;
                            receipt.Parameters.Add("pTotal", OleDbType.Currency).Value = total;
                            receipt.Parameters.Add("pPayment", OleDbType.VarWChar, 255).Value = "Оплата при оформлении";
                            receipt.Parameters.Add("pStatus", OleDbType.VarWChar, 255).Value = "Оплачено";
                            receipt.ExecuteNonQuery();
                        }

                        stage = "получение номера чека";
                        int saleId;
                        using (var identity = new OleDbCommand("SELECT @@IDENTITY", connection, transaction))
                            saleId = Convert.ToInt32(identity.ExecuteScalar());
                        createdSaleId = saleId;

                        // 2. Привязываем чек к текущему покупателю.
                        stage = "привязка чека к покупателю";
                        using (var dispatcher = new OleDbCommand(
                            "INSERT INTO [Продажи_Диспетчер] ([ID_Продажи], [ID_Покупателя]) VALUES (?, ?)",
                            connection, transaction))
                        {
                            dispatcher.Parameters.Add("pSale", OleDbType.Integer).Value = saleId;
                            dispatcher.Parameters.Add("pCustomer", OleDbType.Integer).Value = Session.CustomerId;
                            dispatcher.ExecuteNonQuery();
                        }

                        // 3. Сохраняем товары в чеке и уменьшаем остаток.
                        foreach (CartItem item in Cart.Items)
                        {
                            stage = "проверка остатка товара «" + item.Name + "»";
                            int stockQty;
                            using (var check = new OleDbCommand(
                                "SELECT [Количество] FROM [Товары] WHERE [ID_товара] = ?",
                                connection, transaction))
                            {
                                check.Parameters.Add("pProduct", OleDbType.Integer).Value = item.Id;
                                object value = check.ExecuteScalar();
                                if (value == null || value == DBNull.Value)
                                    throw new Exception("Товар не найден в базе данных.");
                                stockQty = Convert.ToInt32(value);
                            }

                            if (stockQty < item.Qty)
                                throw new Exception("Недостаточно товара на складе.");

                            stage = "сохранение позиции чека «" + item.Name + "»";
                            using (var line = new OleDbCommand(
                                "INSERT INTO [Позиции_Чека] ([ID_Продажи], [ID_Товара], [Количество], [Цена_Продажи], [ФИО]) VALUES (?, ?, ?, ?, ?)",
                                connection, transaction))
                            {
                                line.Parameters.Add("pSale", OleDbType.Integer).Value = saleId;
                                line.Parameters.Add("pProduct", OleDbType.Integer).Value = item.Id;
                                line.Parameters.Add("pQty", OleDbType.Integer).Value = item.Qty;
                                line.Parameters.Add("pPrice", OleDbType.Currency).Value = item.Price;
                                line.Parameters.Add("pFio", OleDbType.VarWChar, 255).Value = customerFio;
                                line.ExecuteNonQuery();
                            }

                            stage = "списание товара «" + item.Name + "» со склада";
                            using (var stock = new OleDbCommand(
                                "UPDATE [Товары] SET [Количество] = [Количество] - ? WHERE [ID_товара] = ?",
                                connection, transaction))
                            {
                                stock.Parameters.Add("pQty", OleDbType.Integer).Value = item.Qty;
                                stock.Parameters.Add("pProduct", OleDbType.Integer).Value = item.Id;
                                if (stock.ExecuteNonQuery() != 1)
                                    throw new Exception("Не удалось обновить остаток товара.");
                            }
                        }

                        stage = "подтверждение покупки";
                        transaction.Commit();
                    }
                }

                // Обновляем остатки в таблице, отображаемой пользователю.
                foreach (CartItem item in Cart.Items)
                {
                    DataRow[] rows = table.Select("ID_товара = " + item.Id);
                    if (rows.Length > 0)
                        rows[0]["Количество"] = Convert.ToInt32(rows[0]["Количество"]) - item.Qty;
                }

                Cart.Items.Clear();
                ShowTotal();
                MessageBox.Show("Покупка успешно оформлена! Чек №" +
                    createdSaleId + " сохранён.");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось оформить покупку (" + stage + "): " + ex.Message,
                    "Ошибка оформления", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
