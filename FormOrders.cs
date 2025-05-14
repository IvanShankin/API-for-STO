using System;
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
    public partial class FormOrders : Form
    {
        FormServices formServices;
        public FormOrders(FormServices formServices)
        {
            InitializeComponent();
            this.formServices = formServices;
        }
        string connectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source = dataBase.accdb;";
        string query = "SELECT * FROM orders";
        bool showCompleted = true; // если необходимо показать только выполненые заказы
        public void FormOrders_Load(object sender, EventArgs e)
        {

            OleDbConnection dbConnection = new OleDbConnection(connectionString);

            dbConnection.Open();
            query = "SELECT * FROM orders";
            OleDbCommand dbCommand = new OleDbCommand(query, dbConnection);
            OleDbDataReader dbReader = dbCommand.ExecuteReader();

            if (dbReader.HasRows == false)
            {
                MessageBox.Show("Все заказы выполнены!", "Внимаение!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                dataGridView1.Rows.Clear();
                if (showCompleted == true)
                {
                    while (dbReader.Read())
                    {
                        if (Convert.ToString(dbReader["status"]) == "not_completed")
                        {
                            dataGridView1.Rows.Add(dbReader["ID"], dbReader["service"], dbReader["prise"], dbReader["start_date"], dbReader["end_date"], dbReader["description"], "Не выполнен");
                        }
                    }
                    dataGridView1.Sort(dataGridView1.Columns["ID"], ListSortDirection.Ascending);
                }
                else
                {
                    while (dbReader.Read())
                    {
                        if (Convert.ToString(dbReader["status"]) == "not_completed")
                        {
                            dataGridView1.Rows.Add(dbReader["ID"], dbReader["service"], dbReader["prise"], dbReader["start_date"], dbReader["end_date"], dbReader["description"], "Не выполнен");
                        }
                        else
                        {
                            dataGridView1.Rows.Add(dbReader["ID"], dbReader["service"], dbReader["prise"], dbReader["start_date"], dbReader["end_date"], dbReader["description"], "Выполнен");
                        }

                    }
                    dataGridView1.Sort(dataGridView1.Columns["status"], ListSortDirection.Descending);
                }    
            }

            dbReader.Close();
            dbConnection.Close();
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

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            FormOrdersChange formOrdersChange = new FormOrdersChange(this,DateTime.Now, DateTime.Now,true);
            formOrdersChange.ShowDialog();
        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 1)
            { MessageBox.Show("Пожалуйста, Выберите только одну строку", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            
            string start_date_str = dataGridView1.CurrentRow.Cells["start_date"].Value.ToString();
            DateTime start_date = DateTime.ParseExact(start_date_str, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            string end_date_str = dataGridView1.CurrentRow.Cells["end_date"].Value.ToString();
            DateTime end_date = DateTime.ParseExact(end_date_str, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            string service = dataGridView1.CurrentRow.Cells["service"].Value.ToString();
            string prise = dataGridView1.CurrentRow.Cells["prise"].Value.ToString();
            string description = dataGridView1.CurrentRow.Cells["description"].Value.ToString();
            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value.ToString());

            FormOrdersChange formOrdersChange = new FormOrdersChange(this, start_date, end_date, false, service, prise, description, id);
            formOrdersChange.ShowDialog();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

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
            string query = "DELETE FROM orders WHERE ID = " + id;//сам запрос
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
        private void buttonShowAll_Click(object sender, EventArgs e)
        {
            if (showCompleted == true)
            {  
                buttonShowAll.Text = "Показать не выполненые";
                showCompleted = false;
                FormOrders_Load(sender, e);
            }
            else
            {
                buttonShowAll.Text = "Показать всё";
                showCompleted = true;
                FormOrders_Load(sender, e);
            }

            
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (textBoxSearchID.Text == "")
            {
                MessageBox.Show("Заполнете поле с ID по которому хотете искать!", "Готово!",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int id = Convert.ToInt32(textBoxSearchID.Text.ToString());
            OleDbConnection dbConnection = new OleDbConnection(connectionString);

            dbConnection.Open();
            query = "SELECT * FROM orders WHERE ID = "+ id;
            OleDbCommand dbCommand = new OleDbCommand(query, dbConnection);
            OleDbDataReader dbReader = dbCommand.ExecuteReader();

            if (dbReader.HasRows == false)
            {
                MessageBox.Show("По такому ID нет данных!", "Внимаение!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                dataGridView1.Rows.Clear();
                while (dbReader.Read())
                {
                    if (Convert.ToString(dbReader["status"]) == "not_completed")
                    {
                        dataGridView1.Rows.Add(dbReader["ID"], dbReader["service"], dbReader["prise"], dbReader["start_date"], dbReader["end_date"], dbReader["description"], "Не выполнен");
                    }
                    else
                    {
                        dataGridView1.Rows.Add(dbReader["ID"], dbReader["service"], dbReader["prise"], dbReader["start_date"], dbReader["end_date"], dbReader["description"], "Выполнен");
                    }
                }
            }
        }

        private void textBoxSearchID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }// запрещаем вписывать любые значения кроме цифр
        }

        private void buttonCompleted_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 1)
            { MessageBox.Show("Пожалуйста, Выберите только одну строку", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            int prise = Convert.ToInt32(dataGridView1.CurrentRow.Cells["prise"].Value.ToString());
            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value.ToString());

            FormPayment formPayment = new FormPayment(this, prise, id);
            formPayment.ShowDialog();
        }

        private void buttonServices_Click(object sender, EventArgs e)
        {
            formServices.Show();
            formServices.SetFormOrders(this);
            formServices.Location = this.Location; // Сохраняет координаты текущей формы
            formServices.Size = this.Size;
            this.Hide();
            
        }

        private void FormOrders_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
