using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarCareMstD
{
    public partial class Form2 : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DbCon"].ConnectionString;
        public Form2()
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            RefreshDataGridView();
        }
        public void InitializeDatabaseConnection()
        {
            string connectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=CarCareDB;Integrated Security=True";

        }
        public void RefreshDataGridView()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT 
                                    c.Name,
                                    c.Phone,
                                    c.CarNumber,
                                    sd.ServiceDate,
                                    sd.ServiceName,
                                    sd.Price,
                                    sd.ImagePath
                            FROM 
                                    Customer c
                            INNER JOIN 
                                    ServiceDetails sd ON c.CustomerID = sd.CustomerID;";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable serviceDetails = new DataTable();
                    adapter.Fill(serviceDetails);
                    dataGridView1.DataSource = serviceDetails;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            Form1 obj = new Form1();
            obj.Show();
            this.Hide();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            RefreshDataGridView();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            ReportView report = new ReportView();
            report.Show();
        }
    }
}
