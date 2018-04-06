using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Leadtools.Demos;
using Leadtools.Demos.Dialogs;
using Leadtools.Forms.Ocr;

namespace CamGenie
{
    public partial class ZoneProperties : Form
    {
        public string field = "";
        public string batchType = "";
        private IOcrZoneCollection selectedZones = null;

        public IOcrZoneCollection SelectedZones
        {
            get { return selectedZones; }
            set { selectedZones = value; }
        }

        public ZoneProperties()
        {
            InitializeComponent();
        }

        private void listBoxField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxField.SelectedIndex > -1) {
                field = listBoxField.SelectedItem.ToString();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void ZoneProperties_Load(object sender, EventArgs e)
        {
            if (batchType == "PhoneBill")
            {
                listBoxField.Items.Add("Calldate");
                listBoxField.Items.Add("Calltime");
                listBoxField.Items.Add("Callto");
                listBoxField.Items.Add("Callfrom");
                listBoxField.Items.Add("City");
                listBoxField.Items.Add("State");
                listBoxField.Items.Add("CityState");
                listBoxField.Items.Add("Duration");
                listBoxField.Items.Add("Calltype");
                listBoxField.Items.Add("Origination");
            }
            else
            {
                listBoxField.Items.Add("AccountNumber");
                listBoxField.Items.Add("CheckNumber");
                listBoxField.Items.Add("CardNumber");
                listBoxField.Items.Add("Amount");
                listBoxField.Items.Add("TransactionDate");
                listBoxField.Items.Add("TransactionType");
                listBoxField.Items.Add("RoutingNumber");
                listBoxField.Items.Add("AccountHolder1");
                listBoxField.Items.Add("AccountHolder2");
                listBoxField.Items.Add("Description");
                listBoxField.Items.Add("DepositAmount");
            }
                
            foreach (OcrZone zone in SelectedZones)
            {
                listBoxField.Items.Remove(zone.Name);
            }
        }
    }
}
