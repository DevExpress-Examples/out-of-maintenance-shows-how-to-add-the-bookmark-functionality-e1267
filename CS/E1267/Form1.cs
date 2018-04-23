using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraGrid.Bookmarks;

namespace E1267 {
    public partial class FormMain : Form {
        public FormMain() {
            InitializeComponent();
            new GridViewBookmarks(this.gridView1, "OrderID");
        }

        private void FormMain_Load(object sender, EventArgs e) {
            // TODO: This line of code loads data into the 'nwindDataSet.Orders' table. You can move, or remove it, as needed.
            this.ordersTableAdapter.Fill(this.nwindDataSet.Orders);

        }
    }
}