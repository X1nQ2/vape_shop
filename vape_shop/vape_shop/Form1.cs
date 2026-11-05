using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vape_shop
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        public dtDataSet Data { get { return dtDataSet; } }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "dtDataSet.Товары". При необходимости она может быть перемещена или удалена.
            this.товарыTableAdapter.Fill(this.dtDataSet.Товары);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "dtDataSet.Сотрудники_на_Сменах". При необходимости она может быть перемещена или удалена.
            this.сотрудники_на_СменахTableAdapter.Fill(this.dtDataSet.Сотрудники_на_Сменах);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "dtDataSet.Сотрудники". При необходимости она может быть перемещена или удалена.
            this.сотрудникиTableAdapter.Fill(this.dtDataSet.Сотрудники);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "dtDataSet.Смены". При необходимости она может быть перемещена или удалена.
            this.сменыTableAdapter.Fill(this.dtDataSet.Смены);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "dtDataSet.Продажи_Чеки". При необходимости она может быть перемещена или удалена.
            this.продажи_ЧекиTableAdapter.Fill(this.dtDataSet.Продажи_Чеки);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "dtDataSet.Продажи_Диспетчер". При необходимости она может быть перемещена или удалена.
            this.продажи_ДиспетчерTableAdapter.Fill(this.dtDataSet.Продажи_Диспетчер);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "dtDataSet.Покупатели". При необходимости она может быть перемещена или удалена.
            this.покупателиTableAdapter.Fill(this.dtDataSet.Покупатели);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "dtDataSet.Позиции_Чека". При необходимости она может быть перемещена или удалена.
            this.позиции_ЧекаTableAdapter.Fill(this.dtDataSet.Позиции_Чека);
            if (!Session.IsAdmin)
            {
                позицииЧекаToolStripMenuItem.Visible = false;
                покупателиToolStripMenuItem.Visible = false;
                продажиДиспетчераToolStripMenuItem.Visible = false;
                продажиЧекиToolStripMenuItem.Visible = false;
                сменыToolStripMenuItem.Visible = false;
                сотрудникиToolStripMenuItem.Visible = false;
                сотрудникинаСменахToolStripMenuItem.Visible = false;

                товарыToolStripMenuItem_Click(sender, e); // сразу открыть «Товары»
            }
        }
        private DataTable LoadReceiptPositionsWithFio()
        {
            var result = new DataTable();
            using (var connection = new OleDbConnection(Properties.Settings.Default.dtConnectionString))
            {
                connection.Open();
                DataTable columns = connection.GetSchema("Columns", new string[] { null, null, "Позиции_Чека", "ФИО" });
                if (columns.Rows.Count == 0)
                {
                    using (var alter = new OleDbCommand("ALTER TABLE [Позиции_Чека] ADD COLUMN [ФИО] TEXT(255)", connection))
                        alter.ExecuteNonQuery();
                }
                // Заполняем ФИО и для чеков, созданных до обновления программы.
                using (var backfill = new OleDbCommand(
                    "UPDATE ([Позиции_Чека] INNER JOIN [Продажи_Диспетчер] ON [Позиции_Чека].[ID_Продажи] = [Продажи_Диспетчер].[ID_Продажи]) INNER JOIN [Покупатели] ON [Продажи_Диспетчер].[ID_Покупателя] = [Покупатели].[ID_Покупателя] SET [Позиции_Чека].[ФИО] = [Покупатели].[ФИО] WHERE [Позиции_Чека].[ФИО] IS NULL OR [Позиции_Чека].[ФИО] = ''", connection))
                    backfill.ExecuteNonQuery();
            }
            using (var connection = new OleDbConnection(Properties.Settings.Default.dtConnectionString))
            using (var adapter = new OleDbDataAdapter(
                "SELECT [ID_Позиции], [ID_Продажи], [ID_Товара], [Количество], [Цена_Продажи], [ФИО] FROM [Позиции_Чека] ORDER BY [ID_Позиции] DESC", connection))
            {
                adapter.Fill(result);
            }
            return result;
        }

        private void позицииЧекаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = LoadReceiptPositionsWithFio();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                if (dataGridView1.Columns["ФИО"] != null)
                    dataGridView1.Columns["ФИО"].HeaderText = "ФИО покупателя";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось обновить позиции чеков: " + ex.Message);
            }

            dataGridView1.Visible = true;
            dataGridView2.Visible = false;
            dataGridView3.Visible = false;
            dataGridView4.Visible = false;
            dataGridView5.Visible = false;
            dataGridView6.Visible = false;
            dataGridView7.Visible = false;
            dataGridView8.Visible = false;

        }

        private void покупателиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            dataGridView2.Visible = true;
            dataGridView3.Visible = false;
            dataGridView4.Visible = false;
            dataGridView5.Visible = false;
            dataGridView6.Visible = false;
            dataGridView7.Visible = false;
            dataGridView8.Visible = false;

        }

        private void продажиДиспетчераToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            dataGridView2.Visible = false;
            dataGridView3.Visible = true;
            dataGridView4.Visible = false;
            dataGridView5.Visible = false;
            dataGridView6.Visible = false;
            dataGridView7.Visible = false;
            dataGridView8.Visible = false;
        }

        // Загружаем чеки вместе с ФИО покупателя через связь Продажи_Диспетчер.
        // ФИО показывается в таблице «Продажи_Чеки», без добавления лишнего поля
        // в саму таблицу чеков Access.
        private DataTable LoadSalesReceiptsWithFio()
        {
            var result = new DataTable();
            using (var connection = new OleDbConnection(Properties.Settings.Default.dtConnectionString))
            using (var adapter = new OleDbDataAdapter(
                "SELECT [Продажи_Чеки].[ID_Продажи], [Продажи_Чеки].[Дата_Продажи], " +
                "[Покупатели].[ФИО] AS [ФИО покупателя], [Продажи_Чеки].[Сумма_Чека], " +
                "[Продажи_Чеки].[Способ_Оплаты], [Продажи_Чеки].[Статус_Терминала] " +
                "FROM ([Продажи_Чеки] LEFT JOIN [Продажи_Диспетчер] " +
                "ON [Продажи_Чеки].[ID_Продажи] = [Продажи_Диспетчер].[ID_Продажи]) " +
                "LEFT JOIN [Покупатели] " +
                "ON [Продажи_Диспетчер].[ID_Покупателя] = [Покупатели].[ID_Покупателя] " +
                "ORDER BY [Продажи_Чеки].[ID_Продажи] DESC", connection))
            {
                adapter.Fill(result);
            }
            return result;
        }

        private void продажиЧекиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView4.AutoGenerateColumns = true;
                dataGridView4.Columns.Clear();
                dataGridView4.DataSource = LoadSalesReceiptsWithFio();
                dataGridView4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось загрузить чеки с ФИО покупателей: " + ex.Message);
            }

            dataGridView1.Visible = false;
            dataGridView2.Visible = false;
            dataGridView3.Visible = false;
            dataGridView4.Visible = true;
            dataGridView5.Visible = false;
            dataGridView6.Visible = false;
            dataGridView7.Visible = false;
            dataGridView8.Visible = false;
        }

        private void сменыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            dataGridView2.Visible = false;
            dataGridView3.Visible = false;
            dataGridView4.Visible = false;
            dataGridView5.Visible = true;
            dataGridView6.Visible = false;
            dataGridView7.Visible = false;
            dataGridView8.Visible = false;
        }

        private void сотрудникиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            dataGridView2.Visible = false;
            dataGridView3.Visible = false;
            dataGridView4.Visible = false;
            dataGridView5.Visible = false;
            dataGridView6.Visible = true;
            dataGridView7.Visible = false;
            dataGridView8.Visible = false;
        }

        private void сотрудникинаСменахToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            dataGridView2.Visible = false;
            dataGridView3.Visible = false;
            dataGridView4.Visible = false;
            dataGridView5.Visible = false;
            dataGridView6.Visible = false;
            dataGridView7.Visible = true;
            dataGridView8.Visible = false;
        }

        private void товарыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            dataGridView2.Visible = false;
            dataGridView3.Visible = false;
            dataGridView4.Visible = false;
            dataGridView5.Visible = false;
            dataGridView6.Visible = false;
            dataGridView7.Visible = false;
            dataGridView8.Visible = true;
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Visible) new add1().ShowDialog(this);
            else if (dataGridView2.Visible) new add2().ShowDialog(this);
            else if (dataGridView3.Visible) new add3().ShowDialog(this);
            else if (dataGridView4.Visible) new add4().ShowDialog(this);
            else if (dataGridView5.Visible) new add5().ShowDialog(this);
            else if (dataGridView6.Visible) new add6().ShowDialog(this);
            else if (dataGridView7.Visible) new add7().ShowDialog(this);
            else if (dataGridView8.Visible) new add8().ShowDialog(this);
            else MessageBox.Show("Сначала выбери таблицу");
        }
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Validate();
                int n;

                if (dataGridView1.Visible) n = позиции_ЧекаTableAdapter.Update(dtDataSet.Позиции_Чека);
                else if (dataGridView2.Visible) n = покупателиTableAdapter.Update(dtDataSet.Покупатели);
                else if (dataGridView3.Visible) n = продажи_ДиспетчерTableAdapter.Update(dtDataSet.Продажи_Диспетчер);
                else if (dataGridView4.Visible) n = продажи_ЧекиTableAdapter.Update(dtDataSet.Продажи_Чеки);
                else if (dataGridView5.Visible) n = сменыTableAdapter.Update(dtDataSet.Смены);
                else if (dataGridView6.Visible) n = сотрудникиTableAdapter.Update(dtDataSet.Сотрудники);
                else if (dataGridView7.Visible) n = сотрудники_на_СменахTableAdapter.Update(dtDataSet.Сотрудники_на_Сменах);
                else if (dataGridView8.Visible) n = товарыTableAdapter.Update(dtDataSet.Товары);
                else
                {
                    MessageBox.Show("Сначала выбери таблицу");
                    return;
                }

                MessageBox.Show("Сохранено в БД. Записано строк: " + n);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridView[] grids = { dataGridView1, dataGridView2, dataGridView3, dataGridView4,
                             dataGridView5, dataGridView6, dataGridView7, dataGridView8 };
            DataGridView g = grids.FirstOrDefault(x => x.Visible);

            if (g == null)
            {
                MessageBox.Show("Сначала выбери таблицу");
                return;
            }
            if (g.CurrentRow == null || g.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Выбери строку");
                return;
            }
            if (MessageBox.Show("Удалить выбранную строку?", "Удаление",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            g.Rows.RemoveAt(g.CurrentRow.Index);
        }
    }
}
