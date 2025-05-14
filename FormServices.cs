using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsAppSTO
{
    public partial class FormServices : Form
    {
        FormOrders formOrders;
        public FormServices()
        {
            InitializeComponent();
        }
        public void SetFormOrders(FormOrders formOrders)
        {
            this.formOrders = formOrders;
        }


        string connectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source = dataBase.accdb;";
        string query = "SELECT * FROM orders";
        public void FormServices_Load(object sender, EventArgs e)
        {
            OleDbConnection dbConnection = new OleDbConnection(connectionString);

            dbConnection.Open();
            query = "SELECT * FROM service";
            OleDbCommand dbCommand = new OleDbCommand(query, dbConnection);
            OleDbDataReader dbReader = dbCommand.ExecuteReader();

            if (dbReader.HasRows == false)
            {
                MessageBox.Show("Услуг нет!", "Внимаение!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                dataGridView1.Rows.Clear();

                while (dbReader.Read())
                {
                    dataGridView1.Rows.Add(dbReader["ID"], dbReader["name_service"], dbReader["prise"], dbReader["description"]); // добавляем новые строки
                }
                dataGridView1.Sort(dataGridView1.Columns["ID"], ListSortDirection.Ascending); // применяем сортировку
            }

            dbReader.Close();
            dbConnection.Close();

        }

        private void buttonOrders_Click(object sender, EventArgs e)
        {
            formOrders.Show();
            this.Hide();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FormServices_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 1)
            { MessageBox.Show("Пожалуйста, Выберите только одну строку", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            // Получаем первую выбранную строку
            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value.ToString());

            OleDbConnection dbConnection = new OleDbConnection(connectionString);//создаём новое соеденение 
            dbConnection.Open();//открытие соеденения
            string query = "DELETE FROM service WHERE ID = " + id;//сам запрос
            OleDbCommand dbCommand = new OleDbCommand(query, dbConnection);// команда которую надо выполнить

            //выполнение запроса 
            if (dbCommand.ExecuteNonQuery() != 1)//это выполнение запроса, а так же он возвращает кол-во добавленных строк 
            { MessageBox.Show("Ошибка выполнения запроса!", "Внимание!"); return; }
            else
            {
                dataGridView1.Rows.Remove(selectedRow);//удаляем выбранную строку
                MessageBox.Show("Данные Удалены", "Готово!");
            }

            //закрытие соединения с БД
            dbConnection.Close();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            FormServiceChange formServiceChange = new FormServiceChange(this,true);
            formServiceChange.ShowDialog();
        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 1)
            { MessageBox.Show("Пожалуйста, Выберите только одну строку", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            string service = dataGridView1.CurrentRow.Cells["service"].Value.ToString();
            string prise = dataGridView1.CurrentRow.Cells["prise"].Value.ToString();
            string description = dataGridView1.CurrentRow.Cells["description"].Value.ToString();
            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value.ToString());

            FormServiceChange formServiceChange = new FormServiceChange(this, false, service, prise, description, id);
            formServiceChange.ShowDialog();
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (textBoxSearchService.Text == "")
            {
                MessageBox.Show("Заполнете поле с услугой по которому хотете искать!", "Готово!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string name_service = textBoxSearchService.Text.ToString();
            OleDbConnection dbConnection = new OleDbConnection(connectionString);

            dbConnection.Open();
            query = "SELECT * FROM service WHERE name_service = '" + name_service +"'";
            OleDbCommand dbCommand = new OleDbCommand(query, dbConnection);
            OleDbDataReader dbReader = dbCommand.ExecuteReader();

            if (dbReader.HasRows == false)
            {
                MessageBox.Show("Такой услуги нет!", "Внимаение!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                dataGridView1.Rows.Clear();
                while (dbReader.Read())
                {
                    dataGridView1.Rows.Add(dbReader["ID"], dbReader["name_service"], dbReader["prise"], dbReader["description"]); // добавляем новые строки
                }
            }
        }

        private void textBoxSearchID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Отменяем ввод, если это цифра
            }
        }

        private void buttonUp_MouseUp(object sender, MouseEventArgs e)
        {
            // Приводим sender к типу Button 
            if (sender is Button button)
            {
                button.BackColor = Color.White; // меняем цвет при отжатии 
            }
        }

        private void buttonDown_MouseDown(object sender, MouseEventArgs e)
        {
            // Приводим sender к типу Button 
            if (sender is Button button)
            {
                button.BackColor = Color.FromArgb(210, 210, 213); // Цвет при нажатии
            }
        }
    }
}
