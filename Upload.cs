using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.OleDb;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CamGenie
{
    public partial class Upload : Form
    {
        public string Custodian;
        public string Company;
        public string CMTSID;
        public string ATR_Load_Info;
        public string TimeZone;
        public string BatchName = "";
        public string BatchPath = "";
        public string BatchType = "";
        public string BatchTableName = "Phone";
        public int PageTotal = 0;
        public MainForm mainForm;

        public Upload()
        {
            InitializeComponent();
            comboBoxTimeZone.SelectedIndex = 0;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Custodian = textBoxCustodian.Text;
            Company = textBoxCompany.Text;
            CMTSID = textBoxCMTSID.Text;
            ATR_Load_Info = textBoxATR_Load_Info.Text;

            string cs = "Provider=SQLOLEDB;Data Source=ATR-LSB-LSS01;Initial Catalog=CamGenie;User ID=CMTS;Password=gErn8@f0Rm72", sql;
            OleDbConnection oleDbConnection = new OleDbConnection(cs);
            oleDbConnection.Open();
            OleDbCommand oleDbCommand;
            OleDbDataAdapter oleDbDataAdapter;
            DataTable dataTable;

            if (BatchType == "PhoneBill")
            {
                TimeZone = comboBoxTimeZone.Text;
                sql = "delete from DocuGenie.dbo.PhoneBackup where Batch = '" + BatchName + "'";
                oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                oleDbCommand.ExecuteNonQuery();
            }

            toolStripStatusLabel.Text = "Uploading...";
            Application.DoEvents();

            string batchConfigFile = BatchPath + "\\batch.config", fieldValue;
            sql = "";

            if (File.Exists(batchConfigFile) && BatchType == "PhoneBill")
            {
                string line;

                using (StreamReader sr = new StreamReader(batchConfigFile))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        fieldValue = line.Split('=')[1];

                        if (line.StartsWith("Acctno"))
                        {
                            sql = "update " + BatchTableName + " set Acctno = '" + fieldValue.Replace("'", "''") + "', ";
                        }
                        else if (line.StartsWith("Year") && fieldValue != "")
                        {
                            sql += "Callyear = '" + fieldValue + "', ";
                        }
                        else if (line.StartsWith("Callfrom") && fieldValue != "")
                        {
                            sql += "Callfrom = '" + fieldValue.Replace("'", "''") + "' ";
                        }
                        else if (line.StartsWith("PageStart"))
                        {
                            if (fieldValue == "")
                                sql += "where Batch = '" + BatchName + "' ";
                            else
                                sql += "where Batch = '" + BatchName + "' and Page between " + fieldValue + " and ";
                        }
                        else if (line.StartsWith("PageEnd"))
                        {
                            if (fieldValue != "")
                                sql += fieldValue;

                            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                            oleDbCommand.ExecuteNonQuery();
                        }
                    }
                }
            }

            sql = "select ValueAdvantage, Page, Line, FieldID from Field where Batch = '" + BatchName + "' order by Page, Line, FieldID";
            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
            dataTable = new DataTable();
            oleDbDataAdapter.Fill(dataTable);
            string callDate = "", callTime = "<None>", lineHold = "", callTo = "<None>", callFrom = "<None>", cityState = "<None>", duration = "<None>", callType = "", fieldID = "", state = "", origination = "";
            string accountNumber = "", checkNumber = "", cardNumber = "", amount = "null", depositAmount = "null", description = "", transactionDate = "", transactionType = "", routingNumber = "", accountHolder1 = "", accountHolder2 = "";
            string pageHold = "1";
            toolStripStatusLabel.Text = "Uploading page 1 of " + PageTotal.ToString() + "...";
            Application.DoEvents();
                
            foreach (DataRow dataRow in dataTable.Rows)
            {
                fieldID = dataRow["FieldID"].ToString();

                if (lineHold != dataRow["Line"].ToString())
                {
                    if (lineHold != "")
                    {
                        if (BatchType == "PhoneBill")
                        {
                            if (callDate != "")
                            {
                                if (callDate.Split('/').Length == 3)
                                    sql = "update Phone set Calldate = '" + callDate + "', Calltime = '" + callTime.Replace("'", "''") + "', Callto = '" + callTo.Replace("'", "''") + "'";
                                else
                                    sql = "update Phone set Calldate = '" + callDate + "/' + Callyear, Calltime = '" + callTime.Replace("'", "''") + "', Callto = '" + callTo.Replace("'", "''") + "'";
                            }
                            else
                            {
                                sql = "update Phone set Calltime = '" + callTime.Replace("'", "''") + "', Callto = '" + callTo.Replace("'", "''") + "'";
                            }

                            if (callFrom != "<None>") sql += ", Callfrom = '" + callFrom.Replace("'", "''") + "'";

                            if (cityState != "")
                            {
                                if (cityState.Length > 3 && cityState.Substring(cityState.Length - 3, 1) == " ")
                                {
                                    sql += ", City = '" + cityState.Substring(0, cityState.Length - 3).Replace("'", "''") + "', " +
                                            "State = '" + cityState.Substring(cityState.Length - 2, 2) + "'";
                                }
                                else
                                {
                                    int numCheck;

                                    if (!cityState.Contains(" ") && cityState.Length > 3 && cityState.Substring(cityState.Length - 2, 2) == cityState.Substring(cityState.Length - 2, 2).ToUpper() && int.TryParse(cityState.Substring(cityState.Length - 2, 2), out numCheck) == false)
                                    {
                                        string city = cityState.Substring(0, cityState.Length - 2).Replace("'", "''");
                                        string stateInits = cityState.Substring(cityState.Length - 2, 2);
                                        string sqlCityState = "update Phone set City = '" + city + "', State = '" + stateInits + "' where Batch = '" + BatchName + "' and Page = " + pageHold + " and Line = " + lineHold;
                                        oleDbCommand = new OleDbCommand(sqlCityState, oleDbConnection);
                                        oleDbCommand.ExecuteNonQuery();

                                    }
                                    else
                                    {
                                        string sqlCityState = "update Phone set City = '" + cityState.Replace("'", "''") + "' where Batch = '" + BatchName + "' and Page = " + pageHold + " and Line = " + lineHold;
                                        oleDbCommand = new OleDbCommand(sqlCityState, oleDbConnection);
                                        oleDbCommand.ExecuteNonQuery();
                                    }
                                }
                            }

                            sql += ", Duration = '" + duration.Replace("'", "''") + "', Origination = '" + origination.Replace("'", "''") + "' ";
                            sql += "where Batch = '" + BatchName + "' and Page = " + pageHold + " and Line = " + lineHold;

                            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                            oleDbCommand.ExecuteNonQuery();
                            lineHold = dataRow["Line"].ToString();

                            if (dataRow["Page"].ToString() != pageHold)
                            {
                                toolStripStatusLabel.Text = "Uploading page " + pageHold + " of " + PageTotal.ToString() + "...";
                                pageHold = dataRow["Page"].ToString();
                                Application.DoEvents();
                            }

                            callTime = "<None>";
                            callTo = "<None>";
                            callFrom = "<None>";
                            cityState = "<None>";
                            duration = "<None>";
                            callType = "";
                        }
                        else if (BatchType == "BankStatement")
                        {
                            sql = "update BankStatement set AccountNumber = '" + accountNumber + "', CheckNumber = '" + checkNumber.Replace("'", "''") + "', CardNumber = '" + cardNumber.Replace("'", "''") + "', ";
                            sql += "Amount = " + amount + ", TransactionDate = '" + transactionDate + "', TransactionType = 'Statement', ";
                            sql += "RoutingNumber = '" + routingNumber.Replace("'", "''") + "', AccountHolder1 = '" + accountHolder1.Replace("'", "''") + "', ";
                            sql += "AccountHolder2 = '" + accountHolder2.Replace("'", "''") + "', Description = '" + description.Replace("'", "''") + "', ";
                            sql += "DepositAmount = " + depositAmount + " ";
                            sql += "where Batch = '" + BatchName + "' and Page = " + pageHold + " and Line = " + lineHold;

                            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                            oleDbCommand.ExecuteNonQuery();
                            lineHold = dataRow["Line"].ToString();

                            if (dataRow["Page"].ToString() != pageHold)
                            {
                                toolStripStatusLabel.Text = "Uploading page " + pageHold + " of " + PageTotal.ToString() + "...";
                                pageHold = dataRow["Page"].ToString();
                                Application.DoEvents();
                            }

                            accountNumber = "";
                            checkNumber = "";
                            cardNumber = "";
                            amount = "null";
                            depositAmount = "null";
                            description = "";
                            transactionType = "";
                            routingNumber = "";
                            accountHolder1 = "";
                            accountHolder2 = "";
                        }
                    }

                    lineHold = dataRow["Line"].ToString();
                }

                if (dataRow["ValueAdvantage"].ToString().Trim() != "")
                {
                    switch (fieldID)
                    {
                        case "1":
                            callDate = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "2":
                            callTime = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "3":
                            callTo = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "4":
                            callFrom = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "5":
                            cityState = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "6":
                            state = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "7":
                            cityState = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "8":
                            duration = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "9":
                            origination = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "10":
                            callType = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "11":
                            accountNumber = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "12":
                            checkNumber = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "13":
                            cardNumber = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "14":
                            amount = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "15":
                            transactionDate = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "16":
                            transactionType = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "17":
                            routingNumber = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "18":
                            accountHolder1 = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "19":
                            accountHolder2 = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "20":
                            description = dataRow["ValueAdvantage"].ToString();
                            break;
                        case "21":
                            depositAmount = dataRow["ValueAdvantage"].ToString();
                            break;
                    }
                }
            }

            if (lineHold != "")
            {
                if (BatchType == "PhoneBill")
                {
                    sql = "update Phone set Calldate = '" + callDate + "/' + Callyear, Calltime = '" + callTime.Replace("'", "''") + "', Callto = '" + callTo.Replace("'", "''") + "'";
                    if (callFrom != "<None>") sql += ", Callfrom = '" + callFrom.Replace("'", "''") + "'";

                    if (cityState != "")
                    {
                        if (cityState.Length > 3 && cityState.Substring(cityState.Length - 3, 1) == " ")
                        {
                            sql += ", City = '" + cityState.Substring(0, cityState.Length - 3).Replace("'", "''") + "', " +
                                    "State = '" + cityState.Substring(cityState.Length - 2, 2) + "'";
                        }
                        else
                        {
                            sql += ", City = '" + cityState.Replace("'", "''") + "', State = '<None>'";
                        }
                    }

                    sql += ", Duration = '" + duration.Replace("'", "''") + "' ";
                    sql += "where Batch = '" + BatchName + "' and Page = " + pageHold + " and Line = " + lineHold;
                }
                else if (BatchType == "BankStatement")
                {
                    sql = "update BankStatement set AccountNumber = '" + accountNumber + "', CheckNumber = '" + checkNumber.Replace("'", "''") + "', CardNumber = '" + cardNumber.Replace("'", "''") + "', ";
                    sql += "Amount = " + amount + ", TransactionDate = '" + transactionDate + "', TransactionType = '" + transactionType + "', ";
                    sql += "RoutingNumber = '" + routingNumber.Replace("'", "''") + "', AccountHolder1 = '" + accountHolder1.Replace("'", "''") + "', ";
                    sql += "AccountHolder2 = '" + accountHolder2.Replace("'", "''") + "', Description = '" + description.Replace("'", "''") + "', ";
                    sql += "DepositAmount = " + depositAmount + " ";
                    sql += "where Batch = '" + BatchName + "' and Page = " + pageHold + " and Line = " + lineHold;
                }

                oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                oleDbCommand.ExecuteNonQuery();
                toolStripStatusLabel.Text = "Uploading page " + pageHold + " of " + PageTotal.ToString() + "...";
                Application.DoEvents();
            }

            sql = "update " + BatchTableName + " set Custodian = '" + textBoxCustodian.Text.Replace("'", "''") + "', Company = '" + textBoxCompany.Text.Replace("'", "''") + "', ";
            sql += "CmtsID = '" + textBoxCMTSID.Text + "', ATR_Load_Info = '" + textBoxATR_Load_Info.Text + "'";
            if (comboBoxTimeZone.SelectedIndex > -1 && BatchType == "PhoneBill") sql += ", TimeZone = '" + comboBoxTimeZone.Text + "'";
            sql += ", VolInfo = '" + textBoxVolInfo.Text.Replace("'", "''") + "'";
            sql += " where Batch = '" + BatchName + "'";
            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            oleDbCommand.ExecuteNonQuery();

            oleDbConnection.Close();
            toolStripStatusLabel.Text = "Ready";
            Application.DoEvents();
            MessageBox.Show(this, BatchName + " has been uploaded.", "Upload Complete");
            mainForm.finalQCToolStripMenuItem.Enabled = true;
            this.Close();
        }

        private void Upload_Load(object sender, EventArgs e)
        {
            if (BatchType != "PhoneBill")
            {
                labelTimeZone.Visible = false;
                comboBoxTimeZone.Visible = false;
            }
        }
    }
}
