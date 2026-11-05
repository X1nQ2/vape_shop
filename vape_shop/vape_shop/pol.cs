using System;
using System.Data;
using System.Windows.Forms;

namespace vape_shop
{
    public partial class pol : Form
    {
        public pol()
        {
            InitializeComponent();
            товарыToolStripMenuItem.Text = "Корзина (0)";
            купитьToolStripMenuItem.Text = "В корзину";
            Cart.Items.ListChanged += CartItems_ListChanged;
            FormClosed += (s, e) => Cart.Items.ListChanged -= CartItems_ListChanged;
            UpdateCartCaption();
        }

        private void CartItems_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            UpdateCartCaption();
        }

        private void UpdateCartCaption()
        {
            товарыToolStripMenuItem.Text = "Корзина (" + Cart.Count + ")";
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void pol_Load(object sender, EventArgs e)
        {
            try
            {
                товарыTableAdapter.Fill(dtDataSet.Товары);

                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    if (col.DataPropertyName == "Цена_закупки")
                        col.Visible = false;
                }

                товарыBindingSource.Filter = "Количество > 0";
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.ReadOnly = true;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.MultiSelect = false;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось загрузить товары: " + ex.Message);
            }
        }

        private void товарыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var cartForm = new kor(dtDataSet.Товары,
                () => товарыTableAdapter.Update(dtDataSet.Товары)))
            {
                cartForm.ShowDialog(this);
            }
            UpdateCartCaption();
            товарыBindingSource.ResetBindings(false);
        }

        private void купитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Сначала выбери товар");
                return;
            }

            DataRow row = ((DataRowView)dataGridView1.CurrentRow.DataBoundItem).Row;
            using (var addForm = new bue(row))
                addForm.ShowDialog(this);

            UpdateCartCaption();
        }
    }
}
