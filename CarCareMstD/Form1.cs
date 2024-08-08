using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CarCareMstD
{
    public partial class Form1 : Form
    {

        readonly string cs = ConfigurationManager.ConnectionStrings["DbCon"].ConnectionString;
        SqlConnection Con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        public Form1()
        {
            InitializeComponent();
            InitializeTable();
        }

        private DataTable dt = new DataTable();
        private void InitializeTable()
        {
            dt.Columns.Add("ServiceName", typeof(string));
            dt.Columns.Add("ServiceDate", typeof(DateTime));
            dt.Columns.Add("Price", typeof(decimal));
            dt.Columns.Add("ImagePath", typeof(string)); // Add new column for image path
        }

        public int InsertCustomer(string Name,  string Phone, string CarNumber)
        {
            int customerID = 0;
            string query = "INSERT INTO Customer (Name, Phone, CarNumber) VALUES (@Name , @Phone, @CarNumber); SELECT SCOPE_IDENTITY();";

            Con = new SqlConnection(cs);
            cmd = new SqlCommand(query, Con);
            cmd.Parameters.AddWithValue("@Name", Name);
            cmd.Parameters.AddWithValue("@CarNumber", CarNumber);
            cmd.Parameters.AddWithValue("@Phone", Phone);
            try
            {
                Con.Open();
                customerID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inserting order: " + ex.Message);
            }
            return customerID;
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox4.Text) || string.IsNullOrWhiteSpace(textBox5.Text))
                {
                    MessageBox.Show("Missing Information");
                }
                else
                {
                    // Validate and parse the price
                    if (decimal.TryParse(textBox5.Text, out decimal price))
                    {
                        // If parsing is successful, add the row to the DataTable
                        dt.Rows.Add(comboBox2.SelectedItem.ToString(), dateTimePicker1.Value.Date, price, textBoxImagePath.Text); // Add image path to the row
                        dataGridView1.DataSource = dt;
                    }
                    else
                    {
                        MessageBox.Show("Invalid price format. Please enter a valid decimal value for the price.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime testDate = dateTimePicker1.Value.Date;
                string name = textBox2.Text.Trim();
                string phone = textBox3.Text.Trim();
                string carnumber = textBox4.Text.Trim();
                int customerID = InsertCustomer(name, phone, carnumber);

                // Example: Dictionary to store images associated with service names
                Dictionary<string, Image> serviceImages = new Dictionary<string, Image>();
                // Populate serviceImages with service names and associated images as needed

                foreach (DataRow row in dt.Rows)
                {
                    string sname = (string)row["ServiceName"];
                    DateTime date = (DateTime)row["ServiceDate"];
                    decimal price = (decimal)row["Price"];
                    string imagePath = (string)row["ImagePath"];
                    // Retrieve image associated with the service name
                    Image serviceImage = serviceImages.ContainsKey(sname) ? serviceImages[sname] : null;

                    // Convert the Image object to a byte array
                    byte[] imageData = ImageToByteArray(serviceImage);

                    // Pass the byte array to InsertServiceDetail
                    InsertServiceDetail(customerID, sname, date, price,  imagePath);
                }

                MessageBox.Show("Confirmed successfully.");
                // Clear all textboxes after confirmation
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                textBoxImagePath.Clear();
                pictureBox1.Image = null; // Clear the PictureBox
                comboBox1.SelectedIndex = -1; // Clear the selected item in comboBox1
                comboBox2.SelectedIndex=-1; 
                dt.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Method to convert Image to byte array
        private byte[] ImageToByteArray(Image image)
        {
            if (image == null)
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }
        private void InsertServiceDetail(int customerID, string sname, DateTime date, decimal price, string imagePath)
        {
            string query = "INSERT INTO ServiceDetails (CustomerID, ServiceName, ServiceDate, Price, ImagePath) " +
                   "VALUES (@CustomerID, @ServiceName, @ServiceDate, @Price, @ImagePath);";

            Con = new SqlConnection(cs);
            cmd = new SqlCommand(query, Con);

            cmd.Parameters.AddWithValue("@CustomerID", customerID);
            cmd.Parameters.AddWithValue("@ServiceName", sname);
            cmd.Parameters.AddWithValue("@ServiceDate", date);
            cmd.Parameters.AddWithValue("@Price", price);
            cmd.Parameters.AddWithValue("@ImagePath", imagePath); // Add image path parameter

            try
            {
                Con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inserting order detail: " + ex.Message);
            }
        }


        private void but_view_Click(object sender, EventArgs e)
        {
            Form2 obj = new Form2();
            obj.Show();
            this.Hide();
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                }
                else
                {
                    MessageBox.Show("Please select a row to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set the filter to only allow image files
            openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.gif;*.png)|*.BMP;*.JPG;*.JPEG;*.GIF;*.PNG";
            openFileDialog.FilterIndex = 1; // Set the default filter index
            openFileDialog.RestoreDirectory = true; // Restore the directory to the previous one when closing

            // If the user selects a file and clicks OK
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Load the selected image into a PictureBox or any other control as needed
                    pictureBox1.Image = new Bitmap(openFileDialog.FileName);

                    // Set the image path to the textBoxImagePath
                    textBoxImagePath.Text = openFileDialog.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not open file. Original error: " + ex.Message);
                }
            }

        }
    }
}
