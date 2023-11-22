using ComputerSell.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComputerSell.Order
{
    public partial class OrderAdd : Form
    {
        DataTable dtOrderItems = new DataTable();
        public OrderAdd()
        {
            InitializeComponent();
        }
        public OrderView MainOrderForm { get; set; }

        private void OrderAdd_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = GetOrderId().ToString();
            LoadComboBox();
            BuildOrderItemTable();
        }

        private void BuildOrderItemTable()
        {
            dtOrderItems.Columns.Add(new DataColumn("computerid", typeof(int)));
            dtOrderItems.Columns.Add(new DataColumn("computername", typeof(string)) { MaxLength=40});
            dtOrderItems.Columns.Add(new DataColumn("quantity", typeof(int)));
            this.dataGridView1.DataSource = dtOrderItems;
        }

        private int GetOrderId()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(OrderId), 0) FROM Orders", con))
                {
                    con.Open();
                    int id = (int)cmd.ExecuteScalar();
                    con.Close();
                    return id + 1;
                }
            }
        }
        private void LoadComboBox()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Customers", con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    this.comboBox1.DataSource = dt;
                    DataTable dt2 = new DataTable();
                    da.SelectCommand.CommandText = "Select * from Computers";
                    da.Fill(dt2);
                    comboBox2.DataSource = dt2;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dtOrderItems.EndInit();
            
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConnectionString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Orders 
                                            (OrderId, OrderDate, CustomerId ) VALUES
                                            (@i, @d, @c)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@d", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@c", comboBox1.SelectedValue);





                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                              
                                
                                //con.Close();
                                
                                foreach(DataRow r in dtOrderItems.Rows)
                                {
                                    string sql = "INSERT INTO OrderItems VALUES(@oi, @ci, @q)";
                                    SqlCommand cmd2 = new SqlCommand(sql, con, tran);
                                    cmd2.Parameters.AddWithValue("@oi", textBox1.Text);
                                    cmd2.Parameters.AddWithValue("@ci", r["computerid"]);
                                    cmd2.Parameters.AddWithValue("@q", r["quantity"]);
                                    cmd2.ExecuteNonQuery();
                                }
                                tran.Commit();
                                MessageBox.Show("Data Saved", "Success");
                                this.textBox1.Text = GetOrderId().ToString();
                                this.textBox1.Clear();
                                dateTimePicker1.Value = DateTime.Now;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            tran.Rollback();
                        }
                        finally
                        {
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }

                    }
                }

            }
        }

        private void OrderAdd_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.MainOrderForm.LoadData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
           DataRow dr= dtOrderItems.NewRow();
            dr["computerid"] = comboBox1.SelectedValue;
            dr["computername"] = comboBox1.Text;
            dr["quantity"] = textBox2.Text;
            dtOrderItems.Rows.Add(dr);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex==3)
            {
                dtOrderItems.Rows[e.RowIndex].Delete();
            }
        }
    }
}
