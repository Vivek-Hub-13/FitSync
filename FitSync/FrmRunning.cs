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

namespace FitSync
{
    public partial class FrmRunning : Form
    {
        private double Weight;
        private OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/FitSync/FitSync/bin/FitSync.accdb");


        public FrmRunning()
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
            if (string.IsNullOrWhiteSpace(txtDuration.Text) ||
                string.IsNullOrWhiteSpace(txtDistance.Text) ||
                string.IsNullOrWhiteSpace(txtSpeed.Text) ||
                cmbIntensity.SelectedIndex == -1)
            {
                MessageBox.Show("All fields must be filled!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int durationMinutes = Convert.ToInt32(txtDuration.Text);
            double distance = Convert.ToDouble(txtDistance.Text);
            double speed = Convert.ToDouble(txtSpeed.Text);
            string intensity = cmbIntensity.SelectedItem.ToString();
            string Date = datepick.Text;
            string Time = txtTime.Text;

            // Set MET value based on intensity level
            double metValue;
            if (intensity == "Low")
            {
                metValue = 6.0;
            }
            else if (intensity == "Moderate")
            {
                metValue = 9.8;
            }
            else if (intensity == "High")
            {
                metValue = 12.0;
            }
            else
            {
                metValue = 6.0;
            }

            double CaloriesBurned = metValue * Weight * (durationMinutes / 60.0);

            try
            {
                conn.Open();

                // Insert into Running table
                string runningQuery = "INSERT INTO Running (Username, Duration, Distance, Speed, Intensity, CaloriesBurned, [Date], [Time]) VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
                using (OleDbCommand cmd = new OleDbCommand(runningQuery, conn))
                {
                    cmd.Parameters.Add(new OleDbParameter("Username", OleDbType.VarChar)).Value = UserSession.CurrentUsername;
                    cmd.Parameters.Add(new OleDbParameter("Duration", OleDbType.Integer)).Value = durationMinutes;
                    cmd.Parameters.Add(new OleDbParameter("Distance", OleDbType.Double)).Value = distance;
                    cmd.Parameters.Add(new OleDbParameter("Speed", OleDbType.Double)).Value = speed;
                    cmd.Parameters.Add(new OleDbParameter("Intensity", OleDbType.VarChar)).Value = intensity;
                    cmd.Parameters.Add(new OleDbParameter("CaloriesBurned", OleDbType.Double)).Value = CaloriesBurned;
                    cmd.Parameters.Add(new OleDbParameter("Date", OleDbType.VarChar)).Value = Date;
                    cmd.Parameters.Add(new OleDbParameter("Time", OleDbType.VarChar)).Value = Time;

                    cmd.ExecuteNonQuery();
                }

                // Insert into History table
                string historyQuery = "INSERT INTO History (Username, Activity, [Date], [Time], TotalCalories) VALUES (?, ?, ?, ?, ?)";
                using (OleDbCommand cmd = new OleDbCommand(historyQuery, conn))
                {
                    cmd.Parameters.Add(new OleDbParameter("Username", OleDbType.VarChar)).Value = UserSession.CurrentUsername;
                    cmd.Parameters.Add(new OleDbParameter("Activity", OleDbType.VarChar)).Value = "Running";
                    cmd.Parameters.Add(new OleDbParameter("Date", OleDbType.VarChar)).Value = Date;
                    cmd.Parameters.Add(new OleDbParameter("Time", OleDbType.VarChar)).Value = Time;
                    cmd.Parameters.Add(new OleDbParameter("TotalCalories", OleDbType.Double)).Value = CaloriesBurned;

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show($"Activity saved successfully! Calories burned: {CaloriesBurned:F2}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear fields after saving
                txtDuration.Clear();
                txtDistance.Clear();
                txtSpeed.Clear();
                cmbIntensity.SelectedIndex = -1;
                txtTime.Clear();
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

        private void button2_Click(object sender, EventArgs e)
        {
            new FrmAddActivity().Show();
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

        private void button5_Click(object sender, EventArgs e)
        {
            new FrmAddActivity().Show();
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

        private void button10_Click(object sender, EventArgs e)
        {
            new FrmCalisthenics().Show();
            this.Hide();
        }
    }
}
