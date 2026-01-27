using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Windows.Media.Media3D;
using System.Data.OleDb;
using System.Windows.Controls;

namespace FitSync
{
    public partial class FrmCycle : Form
    {

        private double Weight;
        private OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/FitSync/FitSync/bin/FitSync.accdb");

        public FrmCycle()
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

            int duration = Convert.ToInt32(txtDuration.Text);
            double distance = Convert.ToDouble(txtDistance.Text);
            double speed = Convert.ToDouble(txtSpeed.Text);
            string intensity = cmbIntensity.SelectedItem.ToString();
            string date = datepick.Text;
            string time = txtTime.Text;

            // Set MET value based on intensity level
            double metValue;
            if (intensity == "Low")
            {
                metValue = 6.0;
            }
            else if (intensity == "Moderate")
            {
                metValue = 8.0;
            }
            else if (intensity == "High")
            {
                metValue = 12.0;
            }
            else
            {
                metValue = 6.0;
            }

            double caloriesBurned = metValue * Weight * (duration / 60.0);

            try
            {
                conn.Open();

                // Insert into Cycling table
                string cyclingQuery = "INSERT INTO Cycling (Username, Duration, Distance, Speed, Intensity, CaloriesBurned, [Date], [Time]) VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
                using (OleDbCommand cmd = new OleDbCommand(cyclingQuery, conn))
                {
                    cmd.Parameters.Add(new OleDbParameter("Username", OleDbType.VarChar)).Value = UserSession.CurrentUsername;
                    cmd.Parameters.Add(new OleDbParameter("Duration", OleDbType.Integer)).Value = duration;
                    cmd.Parameters.Add(new OleDbParameter("Distance", OleDbType.Double)).Value = distance;
                    cmd.Parameters.Add(new OleDbParameter("Speed", OleDbType.Double)).Value = speed;
                    cmd.Parameters.Add(new OleDbParameter("Intensity", OleDbType.VarChar)).Value = intensity;
                    cmd.Parameters.Add(new OleDbParameter("CaloriesBurned", OleDbType.Double)).Value = caloriesBurned;
                    cmd.Parameters.Add(new OleDbParameter("Date", OleDbType.VarChar)).Value = date;
                    cmd.Parameters.Add(new OleDbParameter("Time", OleDbType.VarChar)).Value = time;

                    cmd.ExecuteNonQuery();
                }

                // Insert into History table
                string historyQuery = "INSERT INTO History (Username, Activity, [Date], [Time], TotalCalories) VALUES (?, ?, ?, ?, ?)";
                using (OleDbCommand cmd = new OleDbCommand(historyQuery, conn))
                {
                    cmd.Parameters.Add(new OleDbParameter("Username", OleDbType.VarChar)).Value = UserSession.CurrentUsername;
                    cmd.Parameters.Add(new OleDbParameter("Activity", OleDbType.VarChar)).Value = "Cycling";
                    cmd.Parameters.Add(new OleDbParameter("Date", OleDbType.VarChar)).Value = date;
                    cmd.Parameters.Add(new OleDbParameter("Time", OleDbType.VarChar)).Value = time;
                    cmd.Parameters.Add(new OleDbParameter("TotalCalories", OleDbType.Double)).Value = caloriesBurned;

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show($"Activity saved successfully! Calories burned: {caloriesBurned:F2}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

