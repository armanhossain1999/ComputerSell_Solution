using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ComputerSell.Order
{
    public partial class OrderEdit : Form
    {
        DataTable dtoi = new DataTable();
        public OrderEdit()
        {
            InitializeComponent();
            this.dataGridView1.AutoGenerateColumns = false;
        }

        private void OrderEdit_Load(object sender, EventArgs e)
        {
            LoadCombo();

        }

        private void LoadCombo()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Orders", con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    this.comboBox3.DataSource = dt;
                    DataTable dt2 = new DataTable();
                    da.SelectCommand.CommandText = "Select * from Customers";
                    da.Fill(dt2);
                    comboBox1.DataSource = dt2;
                    DataTable dt3 = new DataTable();
                    da.SelectCommand.CommandText = "Select * from Computers";
                    da.Fill(dt3);
                    comboBox2.DataSource = dt3;
                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand($"SELECT * FROM Orders WHERE OrderId={comboBox3.SelectedValue}", con))
                {
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        dateTimePicker1.Value = dr.GetDateTime(dr.GetOrdinal("OrderDate"));
                        int n = dr.GetInt32(dr.GetOrdinal("customerid"));
                        string x = GetCustomerName(n);
                        comboBox2.SelectedIndex = comboBox2.FindStringExact(x);
                        GetOrderItems(int.Parse(comboBox3.SelectedValue.ToString()));
                    }
                    con.Close();
                }
            }
        }

        private void GetOrderItems(int id)
        {
            string sql = $@"SELECT OrderItems.*, Computers.ComputerName
            FROM            OrderItems INNER JOIN
                         Computers ON OrderItems.ComputerId = Computers.ComputerId

                         WHERE OrderItems.OrderId={id}";
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(sql, con))
                {

                    da.Fill(dtoi);
                    this.dataGridView1.DataSource = dtoi;

                }
            }
        }

        private string GetCustomerName(int n)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand($"SELECT * FROM Customers WHERE customerid={n}", con))
                {
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {

                        return dr["customername"].ToString();

                    }
                    else
                    {

                        return "";
                    }

                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataRow dr = dtoi.NewRow();
            dr["computerid"] = comboBox2.SelectedValue;
            dr["computername"] = comboBox2.Text;
            dr["quantity"] = textBox2.Text;
            dtoi.Rows.Add(dr);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                dtoi.Rows[e.RowIndex].Delete();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.EndEdit();
            DataTable dt = dtoi.GetChanges();
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConnectionString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {
                    try
                    {
                        string sqlU = @"UPDATE Orders SET OrderDate=@od, customerid=@ci WHERE orderid=@oi";
                        SqlCommand cmdLoan = new SqlCommand(sqlU, con, trx);
                        cmdLoan.Parameters.AddWithValue("@od", dateTimePicker1.Value.Date);
                        cmdLoan.Parameters.AddWithValue("@ci", comboBox1.SelectedValue);
                        cmdLoan.Parameters.AddWithValue("@oi", comboBox3.SelectedValue);


                        cmdLoan.ExecuteNonQuery();
                        if (dt != null)
                        {
                           
                            foreach (DataRow dr in dt.Rows)
                            {
                                //var x = dr["computerid"].ToString();
                                //int pid = dr["PaymentId"];
                                if (dr.RowState == System.Data.DataRowState.Added)
                                {
                                    string sql = @"INSERT INTO OrderItems  VALUES (@oid, @cid, @q)";
                                    SqlCommand cmd = new SqlCommand(sql, con, trx);
                                    cmd.Parameters.AddWithValue("@oid", comboBox3.SelectedValue);

                                    cmd.Parameters.AddWithValue("@cid", dr["computerid"]);
                                    cmd.Parameters.AddWithValue("@q", dr["quantity"]);
                                    cmd.ExecuteNonQuery();

                                }
                                if (dr.RowState == System.Data.DataRowState.Modified)
                                {
                                    string sql = @"UPDATE OrderItems SET quantity=@q WHERE OrderId=@oid and computerid=@cid";
                                    SqlCommand cmd = new SqlCommand(sql, con, trx);
                                    cmd.Parameters.AddWithValue("@oid", comboBox1.SelectedValue);
                                    cmd.Parameters.AddWithValue("@q", dr["quantity"]);
                                    cmd.Parameters.AddWithValue("@cid", dr["computerid"]);
                                    cmd.ExecuteNonQuery();

                                }
                                if (dr.RowState == DataRowState.Deleted)
                                {
                                    string sql = @"Delete OrderItems  WHERE OrderId=@oid and computerid=@cid";
                                    SqlCommand cmd = new SqlCommand(sql, con, trx);
                                    cmd.Parameters.AddWithValue("@oid", comboBox1.SelectedValue);

                                    cmd.Parameters.AddWithValue("@cid", dr["computerid", DataRowVersion.Original]);
                                    cmd.ExecuteNonQuery();

                                }
                            }
                        }
                        trx.Commit();
                        dtoi.AcceptChanges();
                        MessageBox.Show("Data saved", "Success");

                    }
                    catch (Exception ex)
                    {
                        trx.Rollback();
                        MessageBox.Show(ex.Message, "Error");
                    }

                }
                con.Close();
            }
        }
    }
}
