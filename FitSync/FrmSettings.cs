using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FitSync
{
    public partial class FrmSettings : Form
    {
        private OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/FitSync/FitSync/bin/FitSync.accdb");

        public FrmSettings()
        {
            InitializeComponent();
            LoadUserProfile();
            LoadCalorieGoal(); 
        }

        private void LoadUserProfile()
        {
            try
            {
                conn.Open();
                string query = "SELECT * FROM UserInfo WHERE Username = ?";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", UserSession.CurrentUsername);
                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        LblUsername.Text = reader["Username"].ToString().Trim();
                        LblPassword.Text = reader["Password"].ToString().Trim();
                        LblFirstName.Text = reader["FirstName"].ToString().Trim();
                        LblLastName.Text = reader["LastName"].ToString().Trim();
                        LblGender.Text = reader["Gender"].ToString().Trim();
                        LblPhone.Text = reader["PhoneNumber"].ToString().Trim();
                        LblEmail.Text = reader["Email"].ToString().Trim();
                        LblHeight.Text = reader["Height"].ToString().Trim();
                        LblWeight.Text = reader["Weight"].ToString().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(" " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void LoadCalorieGoal()
        {
            try
            {
                conn.Open();
                string query = "SELECT [Calorie Goal] FROM UserInfo WHERE Username = ?";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", UserSession.CurrentUsername);
                    object result = cmd.ExecuteScalar();

                    if (result != DBNull.Value && result != null)
                    {
                        txtCalorieGoal.Text = result.ToString();
                    }
                    else
                    {
                        txtCalorieGoal.Text = string.Empty; // Clear if no data
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading calorie data: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }
        private void btnSubmit_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtCalorieGoal.Text.Trim(), out int calorieGoal) || calorieGoal <= 0)
                {
                    MessageBox.Show("Please enter a valid calorie goal!", "Invalid Input",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                conn.Open();
                string query = "UPDATE UserInfo SET [Calorie Goal] = ? WHERE Username = ?";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", calorieGoal);
                    cmd.Parameters.AddWithValue("?", UserSession.CurrentUsername);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Calorie Goal Updated.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        
                        txtCalorieGoal.Text = calorieGoal.ToString();

                        
                        if (Application.OpenForms["FrmDashboard"] is FrmDashboard dashboard)
                        {
                            dashboard.LoadCalorieData();
                        }
                    }
                    else
                    {
                        MessageBox.Show("No records updated. Please check if the username exists.",
                            "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating calorie goal: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new FrmDashboard().Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new FrmAddActivity().Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            UserSession.Logout();
            new FrmLogin().Show();
            this.Hide();
        }

        
    }
}