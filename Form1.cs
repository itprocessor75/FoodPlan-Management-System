using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FoodPlan
{
    public partial class Form1 : Form
    {
        SQLiteConnection sqlite_conn;
        List<UserItem> UserList { get; set; } = new List<UserItem>();
        int selectedUserId = 0;
        public Form1()
        {
            sqlite_conn = new SQLiteConnection("Data Source=foodplan.db");
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Create_Table();
            LoadUserData();
        }
        private void LoadUserData()
        {
            UserList = new List<UserItem>();
            /*tgv_Users.Rows.Clear();*/
            SQLiteCommand query = sqlite_conn.CreateCommand();
            try
            {
                sqlite_conn.Open();
                query.CommandText = $"SELECT * FROM users";
                using (var reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        var name = reader.GetString(1);
                        var surname = reader.GetString(2);
                        var address = reader.GetString(3);
                        var phone = reader.GetString(4);
                        var email = reader.GetString(5);
                        var height = reader.GetDecimal(6);
                        var weight = reader.GetDecimal(7);
                        var bodyfat = reader.GetString(8);
                        var typeofjob = reader.GetString(9);
                        UserList.Add(new UserItem
                        {
                            id = id,
                            name = name,
                            surname = surname,
                            address = address,
                            phone = phone,
                            email = email,
                            height = height,
                            weight = weight,
                            bodyfat = bodyfat,
                            typeofjob = typeofjob
                        });
                    }
                }
                sqlite_conn.Close();
                tgv_Users.Rows.Clear();
                foreach(var item in UserList)
                {
                    var index = tgv_Users.Rows.Add();
                    tgv_Users.Rows[index].Cells["ID"].Value = item.id;
                    tgv_Users.Rows[index].Cells["tName"].Value = item.name;
                    tgv_Users.Rows[index].Cells["SurName"].Value = item.surname;
                    tgv_Users.Rows[index].Cells["Address"].Value = item.address;
                    tgv_Users.Rows[index].Cells["Phone"].Value = item.phone;
                    tgv_Users.Rows[index].Cells["Email"].Value = item.email;
                    tgv_Users.Rows[index].Cells["Height"].Value = item.height;
                    tgv_Users.Rows[index].Cells["Weight"].Value = item.weight;
                    tgv_Users.Rows[index].Cells["TypeOfJob"].Value = item.typeofjob;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void Create_Table()
        {
            try
            {
                SQLiteCommand command = sqlite_conn.CreateCommand();
                sqlite_conn.Open();
                command.CommandText = "SELECT name FROM sqlite_master WHERE name='users'";
                var name = command.ExecuteScalar();
                // check account table exist or not 
                // if exist do nothing 
                if (name != null && name.ToString() == "users")
                {
                    sqlite_conn.Close();
                    return;
                }
                // acount table not exist, create table and insert 
                command.CommandText = "CREATE TABLE users (id integer primary key, name VARCHAR(50), sur_name VARCHAR(50), address VARCHAR(100), phone VARCHAR(50), email VARCHAR(50), height real, weight real, body_fat real, type_of_job VARCHAR(100))";
                command.ExecuteNonQuery();
                sqlite_conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            selectedUserId = 0;
            txt_Name.Text = "";
            txt_SurName.Text = "";
            txt_Address.Text = "";
            txt_Phone.Text = "";
            txt_Email.Text = "";
            num_Height.Value = 0;
            num_Weight.Value = 0;
            txt_BodyFat.Text = "";
            txt_TypeOfJob.Text = "";
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if(txt_Name.Text == null || txt_Name.Text == "")
            {
                MessageBox.Show("Please input Name.");
                return;
            }
            if(txt_Email.Text == null || txt_Email.Text == "")
            {
                MessageBox.Show("Please input Email.");
                return;
            }
            SQLiteCommand command = sqlite_conn.CreateCommand();
            if(selectedUserId == 0)
            {
                command.CommandText = $"INSERT INTO users (id, name, sur_name, address, phone, email, height, weight, body_fat, type_of_job) VALUES " +
                    $"(null, '{txt_Name.Text}', '{txt_SurName.Text}', '{txt_Address.Text}', '{txt_Phone.Text}', '{txt_Email.Text}', {num_Height.Value}, {num_Weight.Value}, '{txt_BodyFat.Text}', '{txt_TypeOfJob.Text}')";
            }
            else
            {
                command.CommandText = $"UPDATE users SET name='{txt_Name.Text}', sur_name='{txt_SurName.Text}', address='{txt_Address.Text}', phone='{txt_Phone.Text}', " +
                    $"email='{txt_Email.Text}', height={num_Height.Value}, weight={num_Weight.Value}, body_fat='{txt_BodyFat.Text}', type_of_job='{txt_TypeOfJob.Text}' WHERE id={selectedUserId}";
            }
            try
            {
                sqlite_conn.Open();
                command.ExecuteNonQuery();
                sqlite_conn.Close();
                btn_Clear_Click(new object(), new EventArgs());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
    public class UserItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public decimal height { get; set; }
        public decimal weight { get; set; }
        public string bodyfat { get; set; }
        public string typeofjob { get; set; }
    }
}
