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

namespace WindowsFormsAppSTO
{
    public partial class FormServiceChange : Form
    {
        private FormServices formServices;
        private bool add; //если True значит необходимо добавить новый заказ
        string service = "";
        string prise = "";
        string description = "";
        int id = 0;
        public FormServiceChange(FormServices form1, bool add = true, string service = "", string prise = "", string description = "", int id = 0)
        {
            InitializeComponent();
            formServices = form1;
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

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            if (textBoxService.Text == "" || textBoxPrise.Text == ""|| richTextBoxDescription.Text == "")
            {
                MessageBox.Show("Не все данные заполнены!", "Внимаение!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // записываем данные которые ввёл пользователь
            string service = textBoxService.Text.ToString();
            int prise = Convert.ToInt32(textBoxPrise.Text);
            string description = richTextBoxDescription.Text.ToString();

            OleDbConnection dbConnection = new OleDbConnection(connectionString);//создаём новое соеденение 

            //выполнение запроса к БД
            dbConnection.Open();//открытие соеденения
            if (add == true)
            {
                string queryAdd = "INSERT INTO service (name_service, prise, description) " +
                                  "VALUES ( ?, ?, ?)"; // Знак вопроса - это параметры
                using (OleDbCommand dbCommandAdd = new OleDbCommand(queryAdd, dbConnection))
                {
                    // Добавление параметров
                    dbCommandAdd.Parameters.AddWithValue("?", service);
                    dbCommandAdd.Parameters.AddWithValue("?", prise);
                    dbCommandAdd.Parameters.AddWithValue("?", description);
                    //выполнение запроса 
                    if (dbCommandAdd.ExecuteNonQuery() != 1)//это выполнение команды,а так же этот метот возвращает кол-во добавленных строк 
                    { 
                        MessageBox.Show("Ошибка выполнения запроса!", "Внимание!");
                        dbConnection.Close();                                                                     
                        return; 
                    }
                    else
                    {
                        MessageBox.Show("Данные добавленны", "Готово!");
                        formServices.FormServices_Load(sender, e);
                        dbConnection.Close();
                        this.Close();
                    }
                }
            }
            else
            {
                query = "UPDATE service SET name_service = ?, prise = ?, description = ? WHERE ID = ?";//сам запрос

                using (OleDbCommand dbCommandChange = new OleDbCommand(query, dbConnection))
                {
                    // Заменяем параметры на настоящие значения
                    dbCommandChange.Parameters.AddWithValue("?", service);
                    dbCommandChange.Parameters.AddWithValue("?", prise);
                    dbCommandChange.Parameters.AddWithValue("?", description);
                    dbCommandChange.Parameters.AddWithValue("?", id);

                    //выполнение запроса 
                    if (dbCommandChange.ExecuteNonQuery() != 1)//это выполнение запроса, а так же метот возвращает кол-во добавленных строк 
                    { 
                        MessageBox.Show("Ошибка выполнения запроса!", "Внимание!");
                        dbConnection.Close(); //закрытие соединения с БД
                        return; 
                    }
                    else
                    {
                        MessageBox.Show("Данные изменены", "Готово!");
                        formServices.FormServices_Load(sender, e);
                        dbConnection.Close(); //закрытие соединения с БД
                        this.Close();
                    }
                }
            }
        }

        private void FormServiceChange_Load(object sender, EventArgs e)
        {
            if (add == false) // если изменяем данные 
            {
                buttonAccept.Text = "Сохранить";
                textBoxInfo.Text = "Изменение заказа";
            }

            textBoxService.Text = service;
            textBoxPrise.Text = prise;
            richTextBoxDescription.Text = description;
        }
        private void textBoxPrise_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }// запрещаем вписывать любые значения кроме цифр
        }

        private void textBoxService_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Отменяем ввод, если это цифра
            }
        }
    }
}
