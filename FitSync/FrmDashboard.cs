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
using System.Windows.Forms.DataVisualization.Charting;

namespace FitSync
{
    public partial class FrmDashboard : Form
    {
        private OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/FitSync/FitSync/bin/FitSync.accdb");

        public FrmDashboard()
        {
            InitializeComponent();
            LoadActivities();
            LoadUserActivity();
            LoadCalorieData();
            LoadActivityChart(); 
        }

        private void LoadActivityChart()
        {
            try
            {
                conn.Open();

                string query = "SELECT Activity, SUM(TotalCalories) AS TotalCalories FROM History WHERE Username = ? GROUP BY Activity";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", UserSession.CurrentUsername);
                    OleDbDataReader reader = cmd.ExecuteReader();

                    
                    chartActivity.Series.Clear();
                    chartActivity.Titles.Clear();
                    Title chartTitle = new Title
                    {
                        Text = "Calories Burned by Activity",
                        ForeColor = Color.White,
                        Font = new Font("Arial", 10, FontStyle.Bold) 
                    };

                    chartActivity.Titles.Add(chartTitle);


                   
                    Series series = new Series
                    {
                        Name = "Activities",
                        ChartType = SeriesChartType.Pie,
                        LabelForeColor = Color.White,
                        IsValueShownAsLabel = true
                    };

                    while (reader.Read())
                    {
                        string activity = reader["Activity"].ToString();
                        double calories = Convert.ToDouble(reader["TotalCalories"]);
                        series.Points.AddXY(activity, calories);
                    }

                    chartActivity.Series.Add(series);
                    chartActivity.ForeColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading activity chart: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public void LoadCalorieData()
            {
                try
                {
                    conn.Open();

               
                    string queryGoal = "SELECT [Calorie Goal] FROM UserInfo WHERE Username = ?";
                    int calorieGoal = 0;
                    using (OleDbCommand cmd = new OleDbCommand(queryGoal, conn))
                    {
                        cmd.Parameters.AddWithValue("?", UserSession.CurrentUsername);
                        object result = cmd.ExecuteScalar();

                        lblCalorieGoal.Text = (result != DBNull.Value && result != null)
                            ? result.ToString()
                            : "0";

                    
                        calorieGoal = (result != DBNull.Value && result != null)
                            ? Convert.ToInt32(result)
                            : 0;
                    }

               
                    string queryTotal = "SELECT SUM(TotalCalories) FROM History WHERE Username = ?";
                    int totalCalories = 0;
                    using (OleDbCommand cmd = new OleDbCommand(queryTotal, conn))
                    {
                        cmd.Parameters.AddWithValue("?", UserSession.CurrentUsername);
                        object totalCaloriesResult = cmd.ExecuteScalar();

                        lblTotalCalories.Text = (totalCaloriesResult != DBNull.Value && totalCaloriesResult != null)
                            ? totalCaloriesResult.ToString()
                            : "0";

                   
                        totalCalories = (totalCaloriesResult != DBNull.Value && totalCaloriesResult != null)
                            ? Convert.ToInt32(totalCaloriesResult)
                            : 0;
                    }

               
                    if (totalCalories >= calorieGoal && calorieGoal > 0)
                    {
                        MessageBox.Show("Congratulations! You have achieved your Calorie Goal!",
                            "Goal Achieved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (OleDbException ex)
                {
                    MessageBox.Show($"Database Error: {ex.Message}", "Database Error",
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

            private void LoadActivities()
            {
                try
                {
                    conn.Open();
                    string query = "SELECT DISTINCT Activity FROM History WHERE Username = ?";
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", UserSession.CurrentUsername);
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            cmbActivity.Items.Clear();
                            cmbActivity.Items.Add("All"); 
                            while (reader.Read())
                            {
                                cmbActivity.Items.Add(reader["Activity"].ToString());
                            }
                            cmbActivity.SelectedIndex = 0; 
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(" " + ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }

            private void LoadUserActivity()
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM History WHERE Username = ? ORDER BY Date DESC";
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", UserSession.CurrentUsername);
                        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(" " + ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }

            private void LoadSelectedActivity()
            {
                try
                {
                    conn.Open();
                    string query = cmbActivity.SelectedItem.ToString() == "All"
                        ? "SELECT * FROM History WHERE Username = ? ORDER BY Date DESC"
                        : "SELECT * FROM History WHERE Username = ? AND Activity = ? ORDER BY Date DESC";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", UserSession.CurrentUsername);
                        if (cmbActivity.SelectedItem.ToString() != "All")
                        {
                            cmd.Parameters.AddWithValue("?", cmbActivity.SelectedItem.ToString());
                        }

                        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(" " + ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }

            private void button5_Click(object sender, EventArgs e)
            {
                LoadSelectedActivity();
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
        }
    }
