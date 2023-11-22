using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComputerSell.Reports
{
    public partial class FormRpt1 : Form
    {
        public FormRpt1()
        {
            InitializeComponent();
        }

        private void FormRpt1_Load(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM computers", con))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds, "Computers1");
                    DataTable dt = ds.Tables[0];
                    dt.Columns.Add(new DataColumn("image", typeof(System.Byte[])));
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), dt.Rows[i]["Picture"].ToString()));
                    }
                   CrystalReport1 rpt = new CrystalReport1();
                    rpt.SetDataSource(ds);
                    this.crystalReportViewer1.ReportSource= rpt;    
                    this.crystalReportViewer1.Refresh();
                }
            }
        }
    }
}
