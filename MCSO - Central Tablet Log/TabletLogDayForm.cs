using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace MCSO___Central_Tablet_Log
{
    public partial class TabletLogDayForm : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["MCSOTabletLog"].ConnectionString;
        public TabletLogDayForm()
        {
            InitializeComponent();
            DTPDayShift.Value = DateTime.Today;
            comboBoxShift.DataSource = new List<string> { " Day Shift ", "Night Shift " };
            cmbTimeFrames.DataSource = new List<string> { "0800 - 1100 ", "1300 - 1600", " 2000-2200" };

            // Run a test when the form loads (optional)
            // TestDatabaseConnection();
        }
       /* private void TestDatabaseConnection()
        {
            // Retrieve the connection string from the App.config file
            string connectionString = ConfigurationManager.ConnectionStrings["MCSOTabletLog"].ConnectionString;

            try
            {
                // Establish a connection to the database
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();  // Open the connection

                    // If successful, show a message box with the success message
                    MessageBox.Show("Connection successful!", "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // You can run a simple query to confirm
                    SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM TabletLogEntries", conn);
                    int rowCount = (int)command.ExecuteScalar();

                    // Display the number of rows found
                    MessageBox.Show($"The TabletLogEntries table contains {rowCount} rows.", "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // If an error occurs, display the error message
                MessageBox.Show($"Connection failed: {ex.Message}", "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
       */
            private void btnSave_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MCSOTabletLog"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                //Define the SQL query for inserting  into the single table
                string query = " INSERT INTO  TabletAssignment ( Date, Shift, Pod, PodSupervisor, TabletStartCount, TabletEndCount, TimeFrame, TabletSerialNumber, ResidentName, ResidentCellNumber) " +
                                "VALUES  ( @Date, @Shift, @Pod, @PodSupervisor, @TabletStartCount, @TabletEndCount, @TimeFrame, @TabletSerialNumber, @ResidentName, @ResidentCellNumber)";
                SqlCommand cmd = new SqlCommand(query, conn);

                //Caputre values from the form
                cmd.Parameters.AddWithValue("@Date",DTPDayShift.Value);
                cmd.Parameters.AddWithValue("@Shift",comboBoxShift.Text);
                cmd.Parameters.AddWithValue("@Pod", ComboBoxPod.Text);
                cmd.Parameters.AddWithValue("@PodSupervisor", tbPodSupervisor.Text);
                cmd.Parameters.AddWithValue("@TabletStartCount", int.Parse(tbTabletCountStart.Text));
                cmd.Parameters.AddWithValue("@TabletEndCount", int.Parse(tbTabletCountEnd.Text));
                cmd.Parameters.AddWithValue("@TimeFrame", cmbTimeFrames.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@TabletSerialNumber", tbSerialNumber.Text);
                cmd.Parameters.AddWithValue("@ResidentCellNumber", tbResidentCell.Text);

                //Execute the query and insert the data 
                cmd.ExecuteNonQuery();

                MessageBox.Show("Data Saved Successfully!");
            }





        }

        private void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                //Define your SQL query to select all data from TabletAssignment table
                var selectQuery = "SELECT * FROM  TabletAssignment";

                // Retrieve the connection string from App.Config
                string connectionString = ConfigurationManager.ConnectionStrings["MCSOTabletLog"].ConnectionString;

                // Set up SQL connection using the connection string
                using (var conn = new SqlConnection(connectionString))
                {
                    // Set up a DataAdapter to run the select command 
                    var dataAdapter = new SqlDataAdapter(selectQuery, conn);

                    // Use SQL CommandBuilder to auto-generate the commands needed for updates 
                    var commandBuilder = new SqlCommandBuilder(dataAdapter);


                    //Fill the dataset with the results from the DataAdapter

                    var dataSet = new DataSet();
                    dataAdapter.Fill(dataSet);

                    //Set up the DataGridView to display the results 
                    dvgTabletLog.ReadOnly = true;
                    dvgTabletLog.DataSource = dataSet.Tables[0];

                }
            }
            catch (Exception ex) 
            {
                // Display any errors that occure during the process 
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
