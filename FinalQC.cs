using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Leadtools;
using Leadtools.Controls;
using Leadtools.Codecs;
using Leadtools.Forms;
using Leadtools.Forms.Ocr;
using Leadtools.Forms.DocumentWriters;
using Leadtools.Drawing;
using Leadtools.Demos;
using Leadtools.Demos.Dialogs;

namespace CamGenie
{
    public partial class FinalQC : Form
    {
        public string BatchName = "";
        public string BatchType = "";
        public string BatchTableName = "Phone"; 
        public MainForm mainForm;
        private DataTable dataTableFinalQC = new DataTable();
        private string editSelText = "";
        private ArrayList lineTops; 
        OleDbConnection oleDbConnection;
        OleDbCommand oleDbCommand;
        OleDbDataAdapter oleDbDataAdapter;

        public FinalQC()
        {
            InitializeComponent();
        }

        private void FinalQC_Load(object sender, EventArgs e)
        {
            string cs = "Provider=SQLOLEDB;Data Source=ATR-LSB-LSS01;Initial Catalog=CamGenie;User ID=CMTS;Password=gErn8@f0Rm72";
            oleDbConnection = new OleDbConnection(cs);
            oleDbConnection.Open();

            if (BatchType == "PhoneBill")
            {
                comboBoxGlobalReplaceField.Items.Add("Calldate");
                comboBoxGlobalReplaceField.Items.Add("Calltime");
                comboBoxGlobalReplaceField.Items.Add("Callto");
                comboBoxGlobalReplaceField.Items.Add("City");
                comboBoxGlobalReplaceField.Items.Add("State");
                comboBoxGlobalReplaceField.Items.Add("Duration");
                comboBoxGlobalReplaceField.Items.Add("Callfrom");
                comboBoxGlobalReplaceField.Items.Add("Calltype");
                comboBoxGlobalReplaceField.Items.Add("Origination");
            }
            else
            {
                comboBoxGlobalReplaceField.Items.Add("AccountNumber");
                comboBoxGlobalReplaceField.Items.Add("CheckNumber");
                comboBoxGlobalReplaceField.Items.Add("CardNumber");
                comboBoxGlobalReplaceField.Items.Add("Amount");
                comboBoxGlobalReplaceField.Items.Add("TransactionDate");
                comboBoxGlobalReplaceField.Items.Add("TransactionType");
                comboBoxGlobalReplaceField.Items.Add("RoutingNumber");
                comboBoxGlobalReplaceField.Items.Add("AccountHolder1");
                comboBoxGlobalReplaceField.Items.Add("AccountHolder2");
                comboBoxGlobalReplaceField.Items.Add("Description");
                comboBoxGlobalReplaceField.Items.Add("DepositAmount");
                BatchTableName = "BankStatement";
                buttonUpload.Text = "Upload to CamBank";
            }

            LoadData();
        }

        private void LoadData()
        {
            string sql = "select ";

            if (BatchType == "PhoneBill")
                sql += "CONVERT(varchar(10), Calldate, 101) Calldate, Calltime, Callfrom, Callto, City, State, Duration, Origination, Calltype, Acctno, Begdoc#, Company, Custodian, CmtsID, ATR_Load_Info, TimeZone, Page, Line from Phone where Batch = '" + BatchName + "'";
            else if (BatchType == "BankStatement")
                sql += "AccountNumber, CardNumber, RoutingNumber, AccountHolder1, AccountHolder2, TransactionType, CONVERT(varchar(12), TransactionDate, 101) TransactionDate, " +
                       "CheckNumber, Amount, DepositAmount, Description, Begdoc#, Company, Custodian, VolInfo, ATRLoadInfo, Page, Line from BankStatement where Batch = '" + BatchName + "'";

            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
            dataTableFinalQC = new DataTable();
            oleDbDataAdapter.Fill(dataTableFinalQC);

            dataGridViewFinalQC.DataSource = dataTableFinalQC;

            if (BatchType == "PhoneBill")
            {
                dataGridViewFinalQC.Columns[0].Width = 70;
                dataGridViewFinalQC.Columns[1].Width = 60;
                dataGridViewFinalQC.Columns[5].Width = 60;
                dataGridViewFinalQC.Columns[6].Width = 60;
                dataGridViewFinalQC.Columns[7].Width = 120;
                dataGridViewFinalQC.Columns[8].Width = 140;
                dataGridViewFinalQC.Columns[9].Width = 140;
                dataGridViewFinalQC.Columns[10].Width = 140;
                dataGridViewFinalQC.Columns[11].Width = 80;
                dataGridViewFinalQC.Columns[12].Width = 140;
                dataGridViewFinalQC.Columns[13].Width = 80;
            }
        }

        private void BuildZoneLineArray()
        {
            lineTops = new ArrayList();
            Hashtable dupCheck = new Hashtable();
            double top, lastTop = 0;

            foreach (OcrZone zone in mainForm.ocrPageProfessional.Zones)
            {
                top = zone.Bounds.Top;
                if (!dupCheck.ContainsKey(top.ToString()) && (lastTop == 0 || Math.Abs(top - lastTop) > 5))
                {
                    lineTops.Add(top);
                    dupCheck.Add(top.ToString(), top);
                    lastTop = top;
                }
            }

            lineTops.Sort();
        }

        private void dataGridViewFinalQC_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            string page = dataGridViewFinalQC.Rows[e.RowIndex].Cells[dataTableFinalQC.Columns.Count - 2].Value.ToString();
            string line = dataGridViewFinalQC.Rows[e.RowIndex].Cells[dataTableFinalQC.Columns.Count - 1].Value.ToString();
            toolStripStatusLabel.Text = "Page " + page + ", Line " + line;

            if (mainForm.toolStripTextBoxPageNumber.Text != page || lineTops == null || lineTops.Count == 0)
            {
                mainForm.toolStripTextBoxPageNumber.Text = page;
                mainForm.LoadImage();
                BuildZoneLineArray();
            }

            if (e.ColumnIndex < 9 && e.ColumnIndex != 2)
            {
                string fieldName = dataTableFinalQC.Columns[e.ColumnIndex].ColumnName;
                int fieldID = GetFieldID(fieldName);

                if (int.Parse(line) < lineTops.Count)
                {
                    double lineTop = double.Parse(lineTops[int.Parse(line) - 1].ToString());
                    mainForm.FocusZone(fieldName, lineTop);
                }

                //string sql = "select ZoneID from Field where Batch = '" + BatchName + "' and Page = " + page +
                //             " and Line = " + line + " and "; 
                
                //if (fieldID == 5 || fieldID == 6)
                //    sql += "FieldID in (" + fieldID.ToString() + ", 7)";
                //else if (fieldID == 7)
                //    sql += "FieldID = 8";
                //else
                //    sql += "FieldID = " + fieldID.ToString();
                
                //oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                //oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
                //DataTable dataTable = new DataTable();
                //oleDbDataAdapter.Fill(dataTable);

                //if (dataTable.Rows.Count > 0)
                //    mainForm.FocusZone(int.Parse(dataTable.Rows[0][0].ToString()));
            }
        }

        private void FinalQC_FormClosing(object sender, FormClosingEventArgs e)
        {
            oleDbConnection.Close();
        }

        private void dataGridViewFinalQC_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string page = dataGridViewFinalQC.Rows[e.RowIndex].Cells[dataTableFinalQC.Columns.Count - 2].Value.ToString();
            string line = dataGridViewFinalQC.Rows[e.RowIndex].Cells[dataTableFinalQC.Columns.Count - 1].Value.ToString();
                
            int fieldID = e.ColumnIndex > 5 ? e.ColumnIndex + 2 : e.ColumnIndex + 1;
            string newValue = dataGridViewFinalQC.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            string fieldName = GetFieldName(fieldID);

            string sql = "update " + BatchTableName + " set " + fieldName + " = '" + newValue.Replace("'", "''") + "' where Batch = '" + 
                         BatchName + "' and Page = " + page + " and Line = " + line;
            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            oleDbCommand.ExecuteNonQuery();
        }

        private string GetFieldName(int index)
        {
            string fieldName = "";

            switch (index)
            {
                case 1:
                    fieldName = "Calldate";
                    break;
                case 2:
                    fieldName = "Calltime";
                    break;
                case 3:
                    fieldName = "Callfrom";
                    break;
                case 4:
                    fieldName = "Callto";
                    break;
                case 5:
                    fieldName = "City";
                    break;
                case 6:
                    fieldName = "State";
                    break;
                case 8:
                    fieldName = "Duration";
                    break;
                case 9:
                    fieldName = "Origination";
                    break;
                case 10:
                    fieldName = "Calltype";
                    break;
                case 11:
                    if (BatchType == "PhoneBill")
                        fieldName = "Acctno";
                    else
                        fieldName = "AccountNumber";

                    break;
                case 12:
                    if (BatchType == "PhoneBill")
                        fieldName = "Begdoc#";
                    else
                        fieldName = "CheckNumber";

                    break;
                case 13:
                    if (BatchType == "PhoneBill")
                        fieldName = "Company";
                    else
                        fieldName = "CardNumber";
                    break;
                case 14:
                    if (BatchType == "PhoneBill")
                        fieldName = "Custodian";
                    else
                        fieldName = "Amount";
                    break;
                case 15:
                    if (BatchType == "PhoneBill")
                        fieldName = "CmtsID";
                    else
                        fieldName = "TransactionDate";
                    break;
                case 16:
                    if (BatchType == "PhoneBill")
                        fieldName = "ATR_Load_Info";
                    else
                        fieldName = "TransactionType";
                    break;
                case 17:
                    if (BatchType == "PhoneBill")
                        fieldName = "TimeZone";
                    else
                        fieldName = "RoutingNumber";
                    break;
                case 18:
                    if (BatchType == "PhoneBill")
                        fieldName = "Page";
                    else
                        fieldName = "AccountHolder1";
                    break;
                case 19:
                    if (BatchType == "PhoneBill")
                        fieldName = "Line";
                    else
                        fieldName = "AccountHolder2";
                    break;
                case 20:
                    fieldName = "Description";
                    break;
                case 21:
                    fieldName = "DepositAmount";
                    break;
            }
                        
            return fieldName;
        }

        private int GetFieldID(string fieldName)
        {
            int fieldID = 1;

            if (fieldName.StartsWith("Calldate"))
                fieldID = 1;
            else if (fieldName.StartsWith("Calltime"))
                fieldID = 2;
            else if (fieldName.StartsWith("Callto"))
                fieldID = 3;
            else if (fieldName.StartsWith("Callfrom"))
                fieldID = 4;
            else if (fieldName.StartsWith("City"))
                fieldID = 5;
            else if (fieldName.StartsWith("State"))
                fieldID = 6;
            else if (fieldName.StartsWith("CityState"))
                fieldID = 7;
            else if (fieldName.StartsWith("Duration"))
                fieldID = 8;
            else if (fieldName.StartsWith("Origination"))
                fieldID = 9;
            else if (fieldName.StartsWith("Calltype"))
                fieldID = 10;
            else if (fieldName.StartsWith("AccountNumber"))
                fieldID = 11;
            else if (fieldName.StartsWith("CheckNumber"))
                fieldID = 12;
            else if (fieldName.StartsWith("CardNumber"))
                fieldID = 13;
            else if (fieldName.StartsWith("Amount"))
                fieldID = 14;
            else if (fieldName.StartsWith("TransactionDate"))
                fieldID = 15;
            else if (fieldName.StartsWith("TransactionType"))
                fieldID = 16;
            else if (fieldName.StartsWith("RoutingNumber"))
                fieldID = 17;
            else if (fieldName.StartsWith("AccountHolder1"))
                fieldID = 18;
            else if (fieldName.StartsWith("AccountHolder2"))
                fieldID = 19;
            else if (fieldName.StartsWith("Description"))
                fieldID = 20;
            else if (fieldName.StartsWith("DepositAmount"))
                fieldID = 21;

            return fieldID;
        }

        private void buttonUpload_Click(object sender, EventArgs e)
        {
            string cs = "Provider=SQLOLEDB;Data Source=ATR-LSB-LSS01;Initial Catalog=CamGenie;User ID=CMTS;Password=gErn8@f0Rm72", sql;
            OleDbConnection oleDbConnection = new OleDbConnection(cs);
            oleDbConnection.Open();
            OleDbCommand oleDbCommand;            

            if (BatchType == "BankStatement")
            {
                string db = "CG_" + BatchName.Split(' ')[0];
                sql = "delete from " + db + ".dbo.BankStatement where Batch = '" + BatchName + "'";
                oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                oleDbCommand.ExecuteNonQuery();

                sql = "insert into " + db + ".dbo.BankStatement (Batch, Page, Line, AccountNumber, CardNumber, RoutingNumber, AccountHolder1, AccountHolder2, TransactionType, TransactionDate, " +
                      "CheckNumber, Amount, DepositAmount, Description, Begdoc#, Company, Custodian, CmtsIDWithPrefix, VolInfo, ATRLoadInfo, SYS_Type, SourceFiles) " +
                      "select Batch, Page, Line, AccountNumber, CardNumber, RoutingNumber, AccountHolder1, AccountHolder2, TransactionType, TransactionDate, " +
                      "CheckNumber, Amount, DepositAmount, Description, Begdoc#, Company, Custodian, CmtsIDWithPrefix, VolInfo, ATRLoadInfo, 'Q', Begdoc# + '.tif' from BankStatement where Batch = '" + BatchName + "'";
            }
            else
            {
                sql = "delete from DocuGenie.dbo.Phone where Batch = '" + BatchName + "'";
                oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                oleDbCommand.ExecuteNonQuery();

                sql = "insert into DocuGenie.dbo.Phone (Batch, Page, Line, TempCalldate, Calldate, Calltime, Callfrom, Callto, City, State, Duration, Acctno, " +
                      "Srcfile, Begdoc#, Company, Custodian, CmtsIDWithPrefix, ATR_Load_Info, TimeZone, Calltype, Origination, VolInfo) " +
                      "select * from Phone where Batch = '" + BatchName + "'";
            }

            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            int recs = oleDbCommand.ExecuteNonQuery();
            oleDbConnection.Close();
            Application.DoEvents();
            MessageBox.Show(this, recs.ToString("#,0") + " records of " + BatchName + " have been uploaded.", "Upload Complete");
        }

        private void comboBoxGlobalReplaceField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxGlobalReplaceField.SelectedIndex > -1)
            {
                labelFieldName.Text = comboBoxGlobalReplaceField.SelectedItem.ToString();
                buttonReplace.Enabled = textBoxReplacementTarget.Text.Trim() != "" && textBoxReplacementValue.Text.Trim() != "";
                buttonClear.Enabled = textBoxReplacementTarget.Text.Trim() != "" && textBoxReplacementValue.Text.Trim() != "";
            }
        }

        private void textBoxReplacementTarget_TextChanged(object sender, EventArgs e)
        {
            buttonReplace.Enabled = textBoxReplacementTarget.Text.Trim() != "" && textBoxReplacementValue.Text.Trim() != "" && comboBoxGlobalReplaceField.SelectedIndex > -1;
            buttonClear.Enabled = textBoxReplacementTarget.Text.Trim() != "" && textBoxReplacementValue.Text.Trim() != "" && comboBoxGlobalReplaceField.SelectedIndex > -1;
        }

        private void textBoxReplacementValue_TextChanged(object sender, EventArgs e)
        {
            buttonReplace.Enabled = textBoxReplacementTarget.Text.Trim() != "" && textBoxReplacementValue.Text.Trim() != "" && comboBoxGlobalReplaceField.SelectedIndex > -1;
            buttonClear.Enabled = textBoxReplacementTarget.Text.Trim() != "" && textBoxReplacementValue.Text.Trim() != "" && comboBoxGlobalReplaceField.SelectedIndex > -1;
        }

        private void comboBoxPosition_TextChanged(object sender, EventArgs e)
        {
            int numberTest;

            if (comboBoxPosition.Text != "")
            {
                if (int.TryParse(comboBoxPosition.Text, out numberTest) == false)
                    comboBoxPosition.Text = "";
            }
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.AppStarting;
            string replaceField = comboBoxGlobalReplaceField.SelectedItem.ToString();
            string sql = "update " + BatchTableName + " set " + replaceField + " = ";

            if (textBoxReplacementTarget.Text == "*")
            {
                sql += "'" + textBoxReplacementValue.Text.Replace("'", "''") + "'";
            }
            else if (comboBoxPosition.Text.Trim() == "")
            {
                sql += "replace(" + replaceField + ", '" +
                       textBoxReplacementTarget.Text.Replace("'", "''") + "', '" +
                       textBoxReplacementValue.Text.Replace("'", "''") + "')";
            }
            else if (comboBoxPosition.Text == "end")
            {
                int replacementLength = textBoxReplacementTarget.Text.Length;
                sql += "left(" + replaceField + ", len(" + replaceField + ") - " + replacementLength.ToString() + ") + '" +
                        textBoxReplacementValue.Text.Replace("'", "''") + "'";
            }
            else if (textBoxLength.Text.Trim() != "")
            {
                string pos = comboBoxPosition.Text.Trim();
                int leftLength = int.Parse(pos);
                int length = int.Parse(textBoxLength.Text);

                if (leftLength > 1)
                    leftLength -= 1;

                sql += "left(" + replaceField + ", " + leftLength.ToString() + ") + '" + textBoxReplacementValue.Text.Replace("'", "''") + "'";

                if (leftLength + 1 < length)
                {
                    int rightLength = length - leftLength - 1;
                    sql += " + right(" + replaceField + ", " + rightLength.ToString() + ")";
                }
            }
            else
            {
                Messager.ShowInformation(this, "Length is required when character position is entered.");
                this.Cursor = Cursors.Default;
                return;
            }

            sql += " where Batch = '" + BatchName + "'";

            if (textBoxWhereValue.Text.Trim() != "")
            {
                sql += " and " + replaceField;

                if (comboBoxWhereComparison.SelectedIndex == -1)
                {
                    Messager.ShowInformation(this, "Select a comparison operator.");
                    comboBoxWhereComparison.Focus();
                    this.Cursor = Cursors.Default;
                    return;
                }
                else if (comboBoxWhereComparison.SelectedIndex == 0)
                {
                    sql += " = '";
                }
                else if (comboBoxWhereComparison.SelectedIndex == 1)
                {
                    sql += " like '" + textBoxWhereValue.Text.Replace("'", "''") + "%'";
                }
                else if (comboBoxWhereComparison.SelectedIndex == 2)
                {
                    sql += " like '%" + textBoxWhereValue.Text.Replace("'", "''") + "'";
                }
                else if (comboBoxWhereComparison.SelectedIndex == 3)
                {
                    sql += " like '%" + textBoxWhereValue.Text.Replace("'", "''") + "%'";
                }
            }
            else
            {
                sql += " and (" + replaceField + " like '%" + textBoxReplacementTarget.Text.Replace("'", "''") + "%')";
            }

            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            int recs = oleDbCommand.ExecuteNonQuery();
            this.Cursor = Cursors.Default;
            buttonClear.PerformClick();
            Messager.ShowInformation(this, recs.ToString() + " records were updated.");
            LoadData();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxReplacementTarget.Text = "";
            textBoxReplacementValue.Text = "";
            comboBoxPosition.Text = "";
            textBoxLength.Text = "";
            textBoxWhereValue.Text = "";
        }

        private void buttonInsertRow_Click(object sender, EventArgs e)
        {
            FinalQCInsertPhoneRow finalQCInsertPhoneRow = new FinalQCInsertPhoneRow();
            finalQCInsertPhoneRow.BatchName = BatchName;
            finalQCInsertPhoneRow.ShowDialog(this);
            LoadData();
        }

        private void buttonDeleteRow_Click(object sender, EventArgs e)
        {
            if (dataGridViewFinalQC.SelectedRows.Count == 0)
            {
                MessageBox.Show("Highlight a row to delete by clicking the left most cell of the row.");
                return;
            }

            string page = dataGridViewFinalQC.SelectedRows[0].Cells[dataGridViewFinalQC.Columns.Count - 2].Value.ToString();
            string line = dataGridViewFinalQC.SelectedRows[0].Cells[dataGridViewFinalQC.Columns.Count - 1].Value.ToString();
            oleDbCommand = new OleDbCommand("delete from Phone where Batch = '" + BatchName + "' and Page = " + page + " and Line = " + line, oleDbConnection);
            oleDbCommand.ExecuteNonQuery();
            LoadData();
        }
    }
}
