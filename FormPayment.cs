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
    public partial class FormPayment : Form
    {
        private FormOrders formOrders;
        int prise = 0;
        int id = 0;
        public FormPayment(FormOrders formOrders, int prise, int id)
        {
            InitializeComponent();
            this.formOrders = formOrders;
            this.prise = prise;
            this.id = id;
        }

        private void FormPayment_Load(object sender, EventArgs e)
        {
            labelPrise.Text = prise.ToString() + " рублей";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source = dataBase.accdb;";
            string query = "UPDATE orders SET status = ? WHERE ID = ?";//сам запрос

            OleDbConnection dbConnection = new OleDbConnection(connectionString);//создаём новое соеденение 
            dbConnection.Open();//открытие соеденения

            using (OleDbCommand dbCommandChange = new OleDbCommand(query, dbConnection))
            {
                //подстанавливаем вместо знаков вопроса 
                dbCommandChange.Parameters.AddWithValue("?", "completed");
                dbCommandChange.Parameters.AddWithValue("?", id);

                //выполнение запроса 
                if (dbCommandChange.ExecuteNonQuery() != 1)//это выполнение запроса, а так же метот возвращает кол-во добавленных строк 
                { MessageBox.Show("Ошибка выполнения запроса!", "Внимание!"); return; }
                else
                {
                    MessageBox.Show("Заказ выполнен!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    formOrders.FormOrders_Load(sender, e);
                    this.Close();
                }

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
