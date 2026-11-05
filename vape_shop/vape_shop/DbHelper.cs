using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace vape_shop
{
    static class DbHelper
    {
        public static bool Add(DataTable table, Control form)
        {
            var boxes = form.Controls.OfType<TextBox>().OrderBy(b => b.TabIndex).ToList();

            if (boxes.Any(b => string.IsNullOrWhiteSpace(b.Text)))
            {
                MessageBox.Show("Заполни все поля");
                return false;
            }

            try
            {
                table.Rows.Add(boxes.Select(b => (object)b.Text.Trim()).ToArray());
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
                return false;
            }
        }
    }
}