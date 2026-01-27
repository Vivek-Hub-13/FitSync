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

namespace FitSync
{
    public partial class FrmLogin : Form
    {
        private int loginCounter = 0; // Counter for failed login attempts
        private OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/FitSync/FitSync/bin/FitSync.accdb");

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void FrmLogin_Load_1(object sender, EventArgs e)
        {
            ResetUI();
        }

        private void ResetUI()
        {
            label4.Visible = true;
            label5.Visible = true;
            txtUsername.Visible = true;
            txtPassword.Visible = true;
            btnLogin.Visible = true;
            chkShowPass.Visible = true;
            cmdSQ.Visible = false;
            txtAnswer.Visible = false;
            btnSubmit.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open(); 
                string login = "SELECT * FROM UserInfo WHERE Username = ? AND Password = ?";

                using (OleDbCommand cmd = new OleDbCommand(login, con))
                {
                    cmd.Parameters.AddWithValue("?", txtUsername.Text);
                    cmd.Parameters.AddWithValue("?", txtPassword.Text);

                    using (OleDbDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            UserSession.Login(txtUsername.Text); 

                            MessageBox.Show("Login Successful!", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            new FrmDashboard().Show();
                            this.Hide();
                        }
                        else
                        {
                            loginCounter++;
                            if (loginCounter >= 3)
                            {
                                MessageBox.Show("Too many failed attempts.\nYour account is locked. Please answer your security question to unlock.", "Account Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                ShowSecurityQuestionUI();
                            }
                            else
                            {
                                MessageBox.Show("Invalid Username or Password, Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtPassword.Clear();
                                txtUsername.Focus();
                            }
                        }
                    }
                }
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

        private void ShowSecurityQuestionUI()
        {
            txtUsername.Visible = false;
            txtPassword.Visible = false;
            btnLogin.Visible = false;
            chkShowPass.Visible = false;

            cmdSQ.Visible = true;
            txtAnswer.Visible = true;
            btnSubmit.Visible = true;
            label7.Visible = true;
            label8.Visible = true;

            LoadSecurityQuestion();
        }

        private void LoadSecurityQuestion()
        {
            using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Database/FitSync.accdb"))
            {
                try
                {
                    con.Open();
                    string query = "SELECT SecurityQuestion FROM UserInfo WHERE Username = ?";

                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("?", txtUsername.Text);

                        using (OleDbDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                cmdSQ.Text = dr["SecurityQuestion"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("User not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAnswer.Text))
            {
                MessageBox.Show("Please enter your answer.", "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Database/FitSync.accdb"))
            {
                try
                {
                    con.Open();
                    string query = "SELECT SecurityAnswer FROM UserInfo WHERE Username = ?";

                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("?", txtUsername.Text);

                        using (OleDbDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                if (txtAnswer.Text == dr["SecurityAnswer"].ToString())
                                {
                                    MessageBox.Show("Account unlocked! Please try logging in again.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    ResetUI();
                                    loginCounter = 0;
                                }
                                else
                                {
                                    MessageBox.Show("Incorrect answer. Please try again.", "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    txtAnswer.Clear();
                                    txtAnswer.Focus();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void chkShowPass_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkShowPass.Checked ? '\0' : '•';
        }

        private void label6_Click(object sender, EventArgs e)
        {
            new FrmRegistartion().Show();
            this.Hide();
        }
    }
}
