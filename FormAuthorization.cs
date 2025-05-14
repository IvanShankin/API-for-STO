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
using WindowsFormsAppSTO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsAppPolyclinic
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string login = "";
        string password = "";
        string connectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source = dataBase.accdb;";

        private void button1_Click(object sender, EventArgs e)
        {
            OleDbConnection dbConnection = new OleDbConnection(connectionString);

            dbConnection.Open();
            string query = "SELECT * FROM login_and_password";
            OleDbCommand dbCommand = new OleDbCommand(query, dbConnection);
            OleDbDataReader dbReader = dbCommand.ExecuteReader();

            if (dbReader.HasRows == false)
            {
                MessageBox.Show("Заполните данные для входа в БД!", "Внимаение!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                while (dbReader.Read())
                {
                    login = Convert.ToString(dbReader["login"]);
                    password = Convert.ToString(dbReader["password"]);
                }
            }
            dbReader.Close();
            dbConnection.Close();

            if (textBoxLogin.Text == login && textBoxPassword.Text == password)
            {
                FormServices formServices = new FormServices();
                FormOrders formOrders = new FormOrders(formServices);
                this.Hide();
                formOrders.Show();
            }
            else 
            {
                MessageBox.Show("Введён не верный логин или пароль!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
            }
        }


        private void buttonNoVisible_Click(object sender, EventArgs e)
        {
            textBoxPassword.UseSystemPasswordChar = false;
            buttonNoVisible.Visible = false;
            button2.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBoxPassword.UseSystemPasswordChar = true;
            button2.Visible = false;
            buttonNoVisible.Visible = true;
        }
        private void buttonUp_MouseUp(object sender, MouseEventArgs e)
        {
            // Приводим sender к типу Button 
            if (sender is System.Windows.Forms.Button button)
            {
                button.BackColor = Color.White; // меняем цвет при отжатии 
            }
        }

        private void buttonDown_MouseDown(object sender, MouseEventArgs e)
        {
            // Приводим sender к типу Button 
            if (sender is System.Windows.Forms.Button button)
            {
                button.BackColor = Color.FromArgb(210, 210, 213); // Цвет при нажатии
            }
        }
    }
}
