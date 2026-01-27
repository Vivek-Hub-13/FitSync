using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Text.RegularExpressions;

namespace FitSync
{
    public partial class FrmRegistartion : Form
    {
        public FrmRegistartion()
        {
            InitializeComponent();
        }

        OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/FitSync/FitSync/bin/FitSync.accdb");

        private void btnRegister_Click(object sender, EventArgs e)
        {
           
            if (string.IsNullOrWhiteSpace(txtFirstname.Text) ||
                string.IsNullOrWhiteSpace(txtLastname.Text) ||
                string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtComPassword.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtHeight.Text) ||
                string.IsNullOrWhiteSpace(txtWeight.Text) ||
                string.IsNullOrWhiteSpace(cmdGender.Text) ||
                string.IsNullOrWhiteSpace(cmdSQ.Text) ||
                string.IsNullOrWhiteSpace(txtAnswer.Text))
            {
                MessageBox.Show("Fields are empty", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            string password = txtPassword.Text;
            if (!ValidatePassword(password))
            {
                MessageBox.Show("Password must be at least 12 characters long and contain at least one uppercase and one lowercase letter.", "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtComPassword.Clear();
                txtPassword.Focus();
                return;
            }

            if (password != txtComPassword.Text)
            {
                MessageBox.Show("Passwords do not match. Please Re-enter.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtComPassword.Clear();
                txtPassword.Focus();
                return;
            }

            try
            {
                con.Open();

                string register = "INSERT INTO UserInfo ([Username], [Password], [FirstName], [LastName], [Email], [PhoneNumber], [Height], [Weight], [Gender], [SecurityQuestion], [SecurityAnswer]) " +
                                  "VALUES (@Username, @Password, @FirstName, @LastName, @Email, @PhoneNumber, @Height, @Weight, @Gender, @SecurityQuestion, @SecurityAnswer)";

                using (OleDbCommand cmd = new OleDbCommand(register, con))
                {
                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@FirstName", txtFirstname.Text);
                    cmd.Parameters.AddWithValue("@LastName", txtLastname.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@PhoneNumber", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@Height", txtHeight.Text);
                    cmd.Parameters.AddWithValue("@Weight", txtWeight.Text);
                    cmd.Parameters.AddWithValue("@Gender", cmdGender.Text);
                    cmd.Parameters.AddWithValue("@SecurityQuestion", cmdSQ.Text);
                    cmd.Parameters.AddWithValue("@SecurityAnswer", txtAnswer.Text);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Your account has been successfully created!", "Registration Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private bool ValidatePassword(string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z]).{12,}$";
            return Regex.IsMatch(password, pattern);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = checkBox1.Checked ? '\0' : '•';
            txtComPassword.PasswordChar = checkBox1.Checked ? '\0' : '•';
        }

        private void label16_Click(object sender, EventArgs e)
        {
            new FrmLogin().Show();
            this.Hide();
        }
    }
}
