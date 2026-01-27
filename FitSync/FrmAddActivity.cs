using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace FitSync
{
    public partial class FrmAddActivity : Form
    {
        private double Weight;
        private OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/FitSync/FitSync/bin/FitSync.accdb");

        public FrmAddActivity()
        {
            InitializeComponent();
            Weight = GetUserWeight(UserSession.CurrentUsername);
        }

        private double GetUserWeight(string username)
        {
            double userweight = 0;
            try
            {
                conn.Open();
                string query = "SELECT Weight FROM UserInfo WHERE Username = ?";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.Add(new OleDbParameter("Username", OleDbType.VarChar)).Value = username;
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        userweight = Convert.ToDouble(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving user weight: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
            return userweight;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSteps.Text) ||
                string.IsNullOrWhiteSpace(txtDistance.Text) ||
                string.IsNullOrWhiteSpace(txtDuration.Text) ||
                string.IsNullOrWhiteSpace(txtTime.Text))
            {
                MessageBox.Show("All fields must be filled!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!UserSession.IsLoggedIn())
            {
                MessageBox.Show("User not logged in! Redirecting to login.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                new FrmLogin().Show();
                this.Hide();
                return;
            }

            int Steps = Convert.ToInt32(txtSteps.Text);
            double DistanceKm = Convert.ToDouble(txtDistance.Text);  // Assuming the input is in kilometers
            double DistanceMeters = DistanceKm * 1000;  // Convert km to meters
            int durationMinutes = Convert.ToInt32(txtDuration.Text);
            string Date = datepick.Text;
            string Time = txtTime.Text;

            double speedKmph = DistanceKm / (durationMinutes / 60.0);
            double metValue = (speedKmph < 2.0) ? 2.0 : (speedKmph < 4.0) ? 3.0 : 4.0;
            double CaloriesBurned = metValue * Weight * (durationMinutes / 60.0);

            try
            {
                conn.Open();

               
                string walkingQuery = "INSERT INTO Walking (Username, Steps, Distance, Duration, CaloriesBurned, [Date], [Time]) VALUES (?, ?, ?, ?, ?, ?, ?)";
                using (OleDbCommand cmd = new OleDbCommand(walkingQuery, conn))
                {
                    cmd.Parameters.Add(new OleDbParameter("Username", OleDbType.VarChar)).Value = UserSession.CurrentUsername;
                    cmd.Parameters.Add(new OleDbParameter("Steps", OleDbType.Integer)).Value = Steps;
                    cmd.Parameters.Add(new OleDbParameter("Distance", OleDbType.Double)).Value = DistanceMeters; // Store distance in meters
                    cmd.Parameters.Add(new OleDbParameter("Duration", OleDbType.Integer)).Value = durationMinutes;
                    cmd.Parameters.Add(new OleDbParameter("CaloriesBurned", OleDbType.Double)).Value = CaloriesBurned;
                    cmd.Parameters.Add(new OleDbParameter("Date", OleDbType.VarChar)).Value = Date;
                    cmd.Parameters.Add(new OleDbParameter("Time", OleDbType.VarChar)).Value = Time;

                    cmd.ExecuteNonQuery();
                }

                
                string historyQuery = "INSERT INTO History (Username, Activity, [Date], [Time], TotalCalories) VALUES (?, ?, ?, ?, ?)";
                using (OleDbCommand cmd = new OleDbCommand(historyQuery, conn))
                {
                    cmd.Parameters.Add(new OleDbParameter("Username", OleDbType.VarChar)).Value = UserSession.CurrentUsername;
                    cmd.Parameters.Add(new OleDbParameter("Activity", OleDbType.VarChar)).Value = "Walking";
                    cmd.Parameters.Add(new OleDbParameter("Date", OleDbType.VarChar)).Value = Date;
                    cmd.Parameters.Add(new OleDbParameter("Time", OleDbType.VarChar)).Value = Time;
                    cmd.Parameters.Add(new OleDbParameter("TotalCalories", OleDbType.Double)).Value = CaloriesBurned;

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show($"Activity saved successfully! Calories burned: {CaloriesBurned:F2}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSteps.Clear();
                txtDistance.Clear();
                txtTime.Clear();
                txtDuration.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new FrmDashboard().Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new FrmSettings().Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            UserSession.Logout();
            new FrmLogin().Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            new FrmSwim().Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            new FrmCycle().Show();
            this.Hide();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            new FrmST().Show();
            this.Hide();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            new FrmRunning().Show();
            this.Hide();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            new FrmCalisthenics().Show();
            this.Hide();
        }
    }
}
