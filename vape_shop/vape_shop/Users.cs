using System;
using System.Data.OleDb;

namespace vape_shop
{
    static class Users
    {
        static string Cs { get { return Properties.Settings.Default.dtConnectionString; } }

        static void Exec(OleDbConnection c, string sql)
        {
            new OleDbCommand(sql, c).ExecuteNonQuery();
        }

        static void EnsureTable(OleDbConnection c)
        {
            var t = c.GetSchema("Tables", new string[] { null, null, "Пользователи", "TABLE" });
            if (t.Rows.Count == 0)
            {
                Exec(c, "CREATE TABLE Пользователи (Логин TEXT(50) PRIMARY KEY, Пароль TEXT(50), ID_Покупателя LONG)");
            }
            else
            {
                var cols = c.GetSchema("Columns", new string[] { null, null, "Пользователи", "ID_Покупателя" });
                if (cols.Rows.Count == 0)
                    Exec(c, "ALTER TABLE Пользователи ADD COLUMN ID_Покупателя LONG");
            }
        }

        // false, если такой логин уже есть
        public static bool Register(string login, string password, string fio, string phone, int age)
        {
            using (var c = new OleDbConnection(Cs))
            {
                c.Open();
                EnsureTable(c);

                var check = new OleDbCommand("SELECT COUNT(*) FROM Пользователи WHERE Логин = ?", c);
                check.Parameters.AddWithValue("?", login);
                if ((int)check.ExecuteScalar() > 0) return false;

                // новый ID покупателя
                object max = new OleDbCommand("SELECT MAX(ID_Покупателя) FROM Покупатели", c).ExecuteScalar();
                int id = (max == null || max == DBNull.Value) ? 1 : Convert.ToInt32(max) + 1;

                using (var tr = c.BeginTransaction())
                {
                    var insBuyer = new OleDbCommand(
                        "INSERT INTO Покупатели (ID_Покупателя, ФИО, Телефон, Возраст) VALUES (?, ?, ?, ?)", c, tr);
                    insBuyer.Parameters.AddWithValue("?", id);
                    insBuyer.Parameters.AddWithValue("?", fio);
                    insBuyer.Parameters.AddWithValue("?", phone);
                    insBuyer.Parameters.AddWithValue("?", age);
                    insBuyer.ExecuteNonQuery();

                    var insUser = new OleDbCommand(
                        "INSERT INTO Пользователи (Логин, Пароль, ID_Покупателя) VALUES (?, ?, ?)", c, tr);
                    insUser.Parameters.AddWithValue("?", login);
                    insUser.Parameters.AddWithValue("?", password);
                    insUser.Parameters.AddWithValue("?", id);
                    insUser.ExecuteNonQuery();

                    tr.Commit();
                }
                return true;
            }
        }

        public static bool Check(string login, string password)
        {
            using (var c = new OleDbConnection(Cs))
            {
                c.Open();
                EnsureTable(c);

                var cmd = new OleDbCommand("SELECT COUNT(*) FROM Пользователи WHERE Логин = ? AND Пароль = ?", c);
                cmd.Parameters.AddWithValue("?", login);
                cmd.Parameters.AddWithValue("?", password);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        // ID покупателя этого пользователя (0, если нет)
        public static int GetCustomerId(string login)
        {
            using (var c = new OleDbConnection(Cs))
            {
                c.Open();
                EnsureTable(c);

                var cmd = new OleDbCommand("SELECT ID_Покупателя FROM Пользователи WHERE Логин = ?", c);
                cmd.Parameters.AddWithValue("?", login);
                object r = cmd.ExecuteScalar();
                return (r == null || r == DBNull.Value) ? 0 : Convert.ToInt32(r);
            }
        }
    }
}