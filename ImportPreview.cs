using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CamGenie
{
    public partial class ImportPreview : Form
    {
        private DataTable dataTable;

        public DataTable DataTable
        {
            get { return dataTable; }
            set { dataTable = value; }
        }

        public ImportPreview()
        {
            InitializeComponent();
        }

        private void ImportPreview_Load(object sender, EventArgs e)
        {
            dataGridViewImport.DataSource = dataTable;
            dataGridViewImport.Refresh();
        }
    }
}
