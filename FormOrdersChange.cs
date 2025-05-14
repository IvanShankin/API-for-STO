using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsAppSTO
{
    public partial class FormOrdersChange : Form
    {
        private FormOrders formOrders;
        DateTime start_data;
        DateTime end_data;
        private bool add; //если True значит необходимо добавить новый заказ
        string service = "";
        string prise = "";
        string description = "";
        int id = 0;
        public FormOrdersChange(FormOrders form1,DateTime start_data, DateTime end_data,
            bool add = true,string service = "", string prise = "", string description = "", int id = 0)
        {
            InitializeComponent();
            formOrders = form1;
            this.start_data = start_data; 
            this.end_data = end_data;
            this.add = add;
            this.service = service;
            this.prise = prise;
            this.description = description;
            this.id = id;
        }
        string connectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source = dataBase.accdb;";
        string query = "SELECT * FROM service";
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
        private void FormOrdersChange_Load(object sender, EventArgs e)
        {
            OleDbConnection dbConnection = new OleDbConnection(connectionString);

            dbConnection.Open();
            query = "SELECT * FROM service";
            OleDbCommand dbCommand = new OleDbCommand(query, dbConnection);
            OleDbDataReader dbReader = dbCommand.ExecuteReader();

            if (dbReader.HasRows == false)
            {
                MessageBox.Show("У вас нету ни одной услуги!", "Внимаение!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                while (dbReader.Read())
                {
                    comboBoxService.Items.Add(dbReader["name_service"]);
                }
            }

            dbReader.Close();
            dbConnection.Close();

            if (add == false) // если изменяем данные 
            {
                buttonAccept.Text = "Сохранить"; 
                textBoxInfo.Text = "Изменение заказа"; 
            }

            comboBoxService.Text = service;
            textBoxPrise.Text = prise;
            richTextBoxDescription.Text = description;
            
            dateTimePickerStart.Value = start_data;
            dateTimePickerEnd.Value = end_data;
            dateTimePickerStart.MinDate = start_data;
            dateTimePickerEnd.MinDate = end_data; // устанавливаем даты на сейчас 
        }
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            if (comboBoxService.Text == "" || textBoxPrise.Text == "" || richTextBoxDescription.Text == "")
            {
                MessageBox.Show("Не все данные заполнены!", "Внимаение!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dateTimePickerStart.Value >= dateTimePickerEnd.Value)
            {
                MessageBox.Show("Дата начала не может быть\nбольше даты окончания!", "Внимаение!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // записываем данные которые ввёл пользователь
            string service = comboBoxService.SelectedItem.ToString();
            int prise = Convert.ToInt32(textBoxPrise.Text);
            string start_date = dateTimePickerStart.Value.ToString();
            string end_date = dateTimePickerEnd.Value.ToString();
            string description = richTextBoxDescription.Text.ToString();

            OleDbConnection dbConnection = new OleDbConnection(connectionString);//создаём новое соеденение 

            //выполнение запроса к БД
            dbConnection.Open();//открытие соеденения
            if (add == true)
            {
                string queryAdd = "INSERT INTO orders (service, prise, start_date, end_date, description, status) " +
                                  "VALUES ( ?, ?, ?, ?, ?, ?)"; // Знак вопроса - это параметры
                using (OleDbCommand dbCommandAdd = new OleDbCommand(queryAdd, dbConnection))
                {
                    // Добавление параметров
                    dbCommandAdd.Parameters.AddWithValue("?", service);
                    dbCommandAdd.Parameters.AddWithValue("?", prise);
                    dbCommandAdd.Parameters.AddWithValue("?", start_date);
                    dbCommandAdd.Parameters.AddWithValue("?", end_date);
                    dbCommandAdd.Parameters.AddWithValue("?", description);
                    dbCommandAdd.Parameters.AddWithValue("?", "not_completed");
                    //выполнение запроса 
                    if (dbCommandAdd.ExecuteNonQuery() != 1)//это выполнение команды,а так же этот метот возвращает кол-во добавленных строк 
                    { MessageBox.Show("Ошибка выполнения запроса!", "Внимание!"); return; }
                    else
                    {

                        MessageBox.Show("Данные добавленны", "Готово!");
                        formOrders.FormOrders_Load(sender, e);
                        this.Close();
                    }
                }
            }
            else 
            {
                query = "UPDATE orders SET service = ?, prise = ?, start_date = ?, end_date = ?, description = ? WHERE ID = ?";//сам запрос
                                                                                                                                                                                       
                using (OleDbCommand dbCommandChange = new OleDbCommand(query, dbConnection))
                {
                    // Заменяем параметры на настоящие значения
                    dbCommandChange.Parameters.AddWithValue("?", service);
                    dbCommandChange.Parameters.AddWithValue("?", prise);
                    dbCommandChange.Parameters.AddWithValue("?", start_date);
                    dbCommandChange.Parameters.AddWithValue("?", end_date);
                    dbCommandChange.Parameters.AddWithValue("?", description);
                    dbCommandChange.Parameters.AddWithValue("?", id);

                    //выполнение запроса 
                    if (dbCommandChange.ExecuteNonQuery() != 1)//это выполнение запроса, а так же метот возвращает кол-во добавленных строк 
                    { MessageBox.Show("Ошибка выполнения запроса!", "Внимание!"); return; }
                    else
                    { 
                        MessageBox.Show("Данные изменены", "Готово!");
                        formOrders.FormOrders_Load(sender, e);
                        this.Close();
                    }
                }
            }
            dbConnection.Close(); //закрытие соединения с БД
        }

        private void comboBoxService_SelectedIndexChanged(object sender, EventArgs e)
        {
            OleDbConnection dbConnection = new OleDbConnection(connectionString);

            dbConnection.Open();
            query = "SELECT prise FROM service WHERE name_service = '" + comboBoxService.Text + "'";
            OleDbCommand dbCommand = new OleDbCommand(query, dbConnection);
            OleDbDataReader dbReader = dbCommand.ExecuteReader();

            while (dbReader.Read())
            {
                string prise = Convert.ToString(dbReader["prise"]);
                textBoxPrise.Text = prise;
            }

            dbReader.Close();
            dbConnection.Close();
        }

        private void textBoxPrise_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }// запрещаем вписывать любые значения кроме цифр
        }

        private void textBoxInfo_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
