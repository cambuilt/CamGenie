using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CamGenie
{
    public partial class FinalQCInsertPhoneRow : Form
    {
        public string BatchName = "";

        public FinalQCInsertPhoneRow()
        {
            InitializeComponent();
        }

        private void buttonInsert_Click(object sender, EventArgs e)
        {
            if (textBoxPage.Text == "" || textBoxLine.Text == "")
            {
                MessageBox.Show("Page and Line are required.");
                return;
            }
            OleDbConnection oleDbConnection;
            OleDbCommand oleDbCommand;

            string cs = "Provider=SQLOLEDB;Data Source=ATR-LSB-LSS01;Initial Catalog=CamGenie;User ID=CMTS;Password=gErn8@f0Rm72";
            oleDbConnection = new OleDbConnection(cs);
            oleDbConnection.Open();

            string sql = "insert into Phone (Calldate, Calltime, Callfrom, Callto, City, State, Duration, Origination, Calltype, Acctno, Begdoc#, Company, Custodian, CmtsID, ATR_Load_Info, TimeZone, Page, Line, Batch) values (";
            sql += "'" + textBoxCalldate.Text + "', ";
            sql += "'" + textBoxCalltime.Text + "', ";
            sql += "'" + textBoxCallfrom.Text + "', ";
            sql += "'" + textBoxCallto.Text + "', ";
            sql += "'" + textBoxCity.Text.Replace("'", "''") + "', ";
            sql += "'" + textBoxState.Text + "', ";
            sql += "'" + textBoxDuration.Text + "', ";
            sql += "'" + textBoxOrigination.Text + "', ";
            sql += "'" + textBoxCalltype.Text + "', ";
            sql += "'" + textBoxAcctno.Text.Replace("'", "''") + "', ";
            sql += "'" + textBoxBegdoc.Text.Replace("'", "''") + "', ";
            sql += "'" + textBoxCompany.Text.Replace("'", "''") + "', ";
            sql += "'" + textBoxCustodian.Text.Replace("'", "''") + "', ";
            sql += "'" + textBoxCmtsID.Text + "', ";
            sql += "'" + textBoxATR_Load_Info.Text.Replace("'", "''") + "', ";
            sql += "'" + textBoxTimeZone.Text + "', ";
            sql += textBoxPage.Text + ", ";
            sql += textBoxLine.Text + ", ";
            sql += "'" + BatchName + "')";

            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            oleDbCommand.ExecuteNonQuery();

            MessageBox.Show("Record has been inserted.");
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }}
