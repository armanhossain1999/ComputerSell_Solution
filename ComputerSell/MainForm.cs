using ComputerSell.Customers;
using ComputerSell.Order;
using ComputerSell.Products;
using ComputerSell.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComputerSell
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new computerView { MdiParent= this }.Show();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new computerAdd { MdiParent=this}.Show();
        }

        private void editDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new computerEdit { MdiParent = this }.Show();
        }

        private void viewToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new CustomerView { MdiParent = this }.Show();
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new CustomerAdd { MdiParent = this }.Show();
        }

        private void editDeleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new CustomerEdit { MdiParent = this }.Show();
        }

        private void viewToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            new OrderView { MdiParent = this }.Show();
        }

        private void addToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            new OrderAdd { MdiParent = this }.Show();
        }

        private void editDeleteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            new OrderEdit { MdiParent = this }.Show();
        }

        private void report1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormRpt1 { MdiParent = this }.Show();
        }

        private void report2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormRpt2 { MdiParent = this }.Show();
        }

        private void report3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormRpt3 { MdiParent = this }.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void subReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FrmRpt4 { MdiParent = this }.Show();
        }
    }
}
