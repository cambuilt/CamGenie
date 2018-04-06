using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CamGenie
{
    /// <summary>
    /// Dialog to accept system generated load file to transfer to SQL database for presentation.
    /// </summary>
    public partial class ImportBatch : Form
    {
        private bool openingTemplate = false;
        private string lastFieldSelected = "";
        private string printText = "";
        private int sourceFileLineCount = 0;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ImportBatch()
        {
            InitializeComponent();
            comboBoxFieldDelim.SelectedIndex = 0;
            comboBoxFieldDelim.Items.Add(Convert.ToChar(20).ToString());
            comboBoxFieldDelim.SelectedIndex = 0;
            comboBoxQuoteDelim.Items.Add(Convert.ToChar(254).ToString());
            comboBoxQuoteDelim.SelectedIndex = 0;
        }

        private void openFileDialogLoadDGWeb_FileOk(object sender, CancelEventArgs e)
        {
            string delim = "", line;
            textBoxLoadFile.Text = "";

            foreach (string fileName in openFileDialogLoadDGWeb.FileNames)
            {
                textBoxLoadFile.Text += delim + fileName;
                using (StreamReader sr = new StreamReader(fileName))
                {
                    line = sr.ReadLine();

                    if (!line.Contains(comboBoxFieldDelim.SelectedItem.ToString()))
                    {
                        MessageBox.Show("Cannot find field delimiter " + comboBoxFieldDelim.SelectedItem.ToString() + " in the load file.");
                        return;
                    }

                    sourceFileLineCount++;
                }            
            }

            // loadMappingControls();
        }

        private void loadMappingControls()
        {
            this.Cursor = Cursors.AppStarting;
            string fileName = textBoxLoadFile.Text, line, fieldDelim, quoteDelim, quoteString = "";
            groupBoxFieldMapping.Controls.Clear();

            using (StreamReader sr = new StreamReader(fileName))
            {
                line = sr.ReadLine();
                fieldDelim = comboBoxFieldDelim.SelectedItem.ToString();
                quoteDelim = comboBoxQuoteDelim.SelectedItem.ToString();
                string[] fields = line.Split(fieldDelim.ToCharArray()[0]);                
                int top = 25, left = 25, fieldCounter = 0;
                int height = 420 + (20 * fields.Length);
                if (height > 1400) height = 1000;                
                int fieldMappingHeight = 90 + (20 * fields.Length);
                if (fieldMappingHeight > 580) fieldMappingHeight = 580;
                groupBoxFieldMapping.Height = fieldMappingHeight;
                this.Height = height;
                Application.DoEvents();

                foreach (string field in fields)
                {
                    if (field != "")
                    {
                        Label sourceFieldLabel = new Label();
                        sourceFieldLabel.Text = quoteDelim.Length > 0 ? field.Replace(quoteDelim, "") : field;
                        sourceFieldLabel.Top = top;
                        sourceFieldLabel.Left = left;
                        sourceFieldLabel.AutoSize = true;
                        groupBoxFieldMapping.Controls.Add(sourceFieldLabel);
                        Label arrowLabel = new Label();
                        arrowLabel.Top = top - 2;
                        arrowLabel.Left = left + 120;
                        arrowLabel.AutoSize = true;
                        arrowLabel.Text = "->";
                        groupBoxFieldMapping.Controls.Add(arrowLabel);
                        ComboBox destFieldList = new ComboBox();
                        destFieldList.Width = 130;
                        destFieldList.DropDownStyle = ComboBoxStyle.DropDownList;
                        destFieldList.SelectedIndexChanged += new EventHandler(this.destFieldList_SelectedIndexChanged);
                        destFieldList.MouseDown += new MouseEventHandler(this.destFieldList_MouseDown);
                        destFieldList.Name = "destFieldList" + fieldCounter.ToString();

                        if (radioButtonPhoneBill.Checked)
                        {
                            destFieldList.Items.Add("SYS_Exclude");
                            destFieldList.Items.Add("Acctno");
                            destFieldList.Items.Add("Calldate");
                            destFieldList.Items.Add("Calltime");
                            destFieldList.Items.Add("Calldatetime");
                            destFieldList.Items.Add("Callto");
                            destFieldList.Items.Add("CalltoAlternate");
                            destFieldList.Items.Add("Callfrom");
                            destFieldList.Items.Add("City");
                            destFieldList.Items.Add("State");
                            destFieldList.Items.Add("Duration");
                            destFieldList.Items.Add("Calltype");
                        }
                        else
                        {
                            destFieldList.Items.Add("SYS_Exclude");
                            destFieldList.Items.Add("AccountNumber");
                            destFieldList.Items.Add("AcctHolder1");
                            destFieldList.Items.Add("AcctHolder2");
                            destFieldList.Items.Add("BankCDVolume");
                            destFieldList.Items.Add("BankCDVolume2");
                            destFieldList.Items.Add("BankName");
                            destFieldList.Items.Add("CheckNum");
                            destFieldList.Items.Add("Date");
                            destFieldList.Items.Add("Amount");
                            destFieldList.Items.Add("DepositAmount");
                            destFieldList.Items.Add("Description");
                            destFieldList.Items.Add("ItemAccount");
                            destFieldList.Items.Add("ItemAcctHolder");
                            destFieldList.Items.Add("ItemAmount");
                            destFieldList.Items.Add("ItemDate");
                            destFieldList.Items.Add("ItemImageKey1");
                            destFieldList.Items.Add("ItemImageKey2");
                            destFieldList.Items.Add("ItemRoutingTransit");
                            destFieldList.Items.Add("ItemSequence");
                            destFieldList.Items.Add("ItemSerial");
                            destFieldList.Items.Add("ItemTransactionType");
                            destFieldList.Items.Add("TransactionType");
                            destFieldList.Items.Add("OtherDocID");
                            destFieldList.Items.Add("PageNumber");
                            destFieldList.Items.Add("Recipient");
                            destFieldList.Items.Add("ReferenceFile");
                            destFieldList.Items.Add("SequenceNumber");
                            destFieldList.Items.Add("SourceFiles");
                            destFieldList.Items.Add("StatementDate");
                            destFieldList.Items.Add("TotalItems");
                            destFieldList.Items.Add("TotalPages");
                            destFieldList.Items.Add("Size");
                            destFieldList.Items.Add("SYS_Group_Order");
                            destFieldList.Items.Add("SYS_Err");
                            destFieldList.Items.Add("SYS_Type");
                            destFieldList.Items.Add("VolumeName");                            
                        }

                        destFieldList.Top = top - 5;
                        destFieldList.Left = left + 150;
                        groupBoxFieldMapping.Controls.Add(destFieldList);
                        top += 23;
                        fieldCounter++;

                        if (fieldCounter == 22)
                        {
                            this.Top = 50;
                            top = 25;
                            left += 340;
                        }
                    }
                }
            }

            this.Cursor = Cursors.Default;
        }

        protected void destFieldList_MouseDown(object sender, EventArgs e)
        {
            ComboBox comboBoxSelected = (ComboBox)sender;
            if (comboBoxSelected.SelectedIndex == -1)
                lastFieldSelected = "";
            else
                lastFieldSelected = comboBoxSelected.SelectedItem.ToString();
        }

        protected void destFieldList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBoxSelected = (ComboBox)sender;

            if (comboBoxSelected.SelectedIndex > -1 && comboBoxSelected.SelectedItem.ToString() != "SYS_Exclude" && openingTemplate == false)
            {
                string fieldName = comboBoxSelected.SelectedItem.ToString();
                string name = comboBoxSelected.Name;

                foreach (Control control in groupBoxFieldMapping.Controls)
                {
                    if (control.GetType().Equals(new ComboBox().GetType()))
                    {
                        ComboBox comboBox = control as ComboBox;

                        if (comboBox.Name != name)
                        {
                            comboBox.Items.Remove(fieldName);

                            if (lastFieldSelected != "")
                            {
                                comboBox.Items.Add(lastFieldSelected);
                            }
                        }
                    }
                }
            }

            lastFieldSelected = "";
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            openFileDialogLoadDGWeb.ShowDialog(this);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (textBoxBatch.Text.Trim() == "")
            {
                MessageBox.Show(this, "Batch name is required.", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (textBoxTemplateFile.Text.Trim() == "")
            {
                MessageBox.Show(this, "Template file is required.", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            string destinationTable = radioButtonPhoneBill.Checked ? "Phone" : "BankStatementClaude";
            
            if (IsBatchDuplicate(textBoxBatch.Text))
            {
                if (MessageBox.Show(this, "Batch already exists. Replace batch?", "Duplicate Batch", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
                else
                {
                    DeletePhone(textBoxBatch.Text);
                }
            }

            toolStripStatusLabelImportBatch.Text = "Preparing for load...";
            this.Cursor = Cursors.WaitCursor;
            TextFieldParser textFieldParser;
            Application.DoEvents();
            DataTable dataTableLoadFile;
            DataRow currentRow;
            string quoteString = comboBoxQuoteDelim.SelectedItem.ToString(), line, header = "";
            string value, fieldName, sql, target = "", callfromSave = "", calltoSave = "";
            char quoteChar = '0';
            DateTime dateTest;
            if (comboBoxQuoteDelim.SelectedIndex > -1 && comboBoxQuoteDelim.SelectedItem.ToString() != "") quoteChar = comboBoxQuoteDelim.SelectedItem.ToString().ToCharArray()[0];
            char fieldDelim = comboBoxFieldDelim.SelectedItem.ToString().ToCharArray()[0];
            string[] loadFiles = textBoxLoadFile.Text.Split(';');
            int lineCounter, fieldCounter;
            string userID = Environment.UserName.Replace("ATR\\", "");
            string cs = "Provider=SQLOLEDB;Data Source=ATR-LSB-LSS01;Initial Catalog=DocuGenie;User ID=CMTS;Password=gErn8@f0Rm72";
            OleDbConnection oleDbConnection = new OleDbConnection(cs);
            oleDbConnection.Open();
            OleDbCommand oleDbCommand;
            
            foreach (string loadFile in loadFiles)
            {
                using (StreamReader sr = new StreamReader(loadFile))
                {
                    line = sr.ReadLine();

                    if (!line.Contains(comboBoxFieldDelim.SelectedItem.ToString()))
                    {
                        MessageBox.Show("Cannot find field delimiter " + comboBoxFieldDelim.SelectedItem.ToString() + " in the load file.");
                        return;
                    }
                    else
                    {
                        header = line;
                    }
                }

                textFieldParser = new TextFieldParser(loadFile);
                textFieldParser.FieldDelimiter = fieldDelim;
                if (quoteChar != '0') textFieldParser.QuoteDelimiter = quoteChar;
                string[] colHeaders = header.Split(fieldDelim);
                TextFieldCollection textFieldCollection = new TextFieldCollection();                
                ArrayList mappedFieldList = new ArrayList();

                foreach (Control control in groupBoxFieldMapping.Controls)
                {
                    if (control.GetType().Equals(new ComboBox().GetType()))
                    {
                        ComboBox comboBox = control as ComboBox;

                        if (comboBox.Name.StartsWith("destFieldList"))
                        {
                            mappedFieldList.Add(comboBox.SelectedItem.ToString());
                            textFieldCollection.Add(new TextField(comboBox.SelectedItem.ToString(), TypeCode.String, true));
                        }
                    }
                }

                textFieldParser.TextFields = textFieldCollection;
                textFieldParser.RecordFailed += new RecordFailedHandler(textFieldParser_RecordFailed);
                dataTableLoadFile = textFieldParser.ParseToDataTable(0, 0);
                UpdateBatchTemplateFile(textBoxBatch.Text, textBoxTemplateFile.Text);
                lineCounter = 1;

                for (int x = 1; x < dataTableLoadFile.Rows.Count; x++)
                {
                    currentRow = dataTableLoadFile.Rows[x];
                    sql = "insert into " + destinationTable + " (Batch, Page, Line, LoadDate, " +
                          "Company, Custodian, ATR_Load_Info, VolInfo, CmtsIDWithPrefix";
                    
                    if (radioButtonBankStatement.Checked)
                        sql += ", CaseID";
                    else
                        sql += ", [Begdoc#], Srcfile";

                    for (int y = 0; y < textFieldCollection.Count; y++)
                    {
                        fieldName = textFieldCollection[y].Name;

                        if (fieldName == "Calldatetime")
                            sql += ", Calldate, Calltime";
                        else if (fieldName == "CalltoAlternate")
                            target = textBoxTargetNumber.Text;
                        else if (fieldName != "SYS_Exclude")
                            sql += ", " + fieldName;
                    }

                    sql += ") values ('" + textBoxBatch.Text + "', 1, " + lineCounter.ToString() +
                           ", '" + DateTime.Now.ToShortDateString() + "', '" +
                           textBoxCompany.Text.Replace("'", "''") + "', '" + textBoxCustodian.Text.Replace("'", "''") + "', '" +
                           textBoxATRLoadInfo.Text + "', '" + textBoxVolume_Info.Text.Replace("'", "''") + "', '" +
                           textBoxCmtsIDWithPrefix.Text + "'";

                    if (radioButtonBankStatement.Checked)
                        sql += ", 1";
                    else
                        sql += ", '" + textBoxBegdoc.Text + "', '" + textBoxSrcfile.Text + "'";

                    fieldCounter = 0;

                    foreach (string fldName in mappedFieldList)
                    {
                        value = currentRow.ItemArray[fieldCounter].ToString().Replace("Ã", "").Replace("¾", "");
                        if (quoteString != "") value = value.Replace(quoteString, "");

                        if (fldName == "Calldatetime")
                        {
                            if (value.Trim().Length > 0)
                            {
                                string dateVal = value.Split(' ')[0];
                                string timeVal = value.Split(' ')[1];

                                if (DateTime.TryParse(dateVal, out dateTest) == true)
                                    dateVal = dateTest.ToShortDateString();
                                else
                                {
                                    MessageBox.Show("The date " + dateVal + " is an invalid date.");
                                    this.Cursor = Cursors.Default;
                                    toolStripStatusLabelImportBatch.Text = "Ready";
                                    return;
                                }

                                if (timeVal.Split(':').Length == 2)
                                {
                                    DateTime callTime = new DateTime(2000, 1, 1, int.Parse(timeVal.Split(':')[0]), int.Parse(timeVal.Split(':')[1]), 0);
                                    sql += ", '" + dateVal + "', '" + callTime.ToShortTimeString() + "'";
                                }
                                else
                                {
                                    DateTime callTime = new DateTime(2000, 1, 1, int.Parse(timeVal.Split(':')[0]), int.Parse(timeVal.Split(':')[1]), int.Parse(timeVal.Split(':')[2]));
                                    sql += ", '" + dateVal + "', '" + callTime.ToShortTimeString() + "'";
                                }
                            }
                            else
                            {
                                sql += ", null, null";
                            }
                        }
                        else if (fldName == "Callto" || fldName == "Callfrom")
                        {
                            double numTest;
                            value = value.Replace("(1", "").Replace("(", "").Replace(") ", "").Replace(")", "").Replace("-", "");
                            if (double.TryParse(value, out numTest))
                            {
                                if (value.Length == 10)
                                    value = value.Substring(0, 3) + "-" + value.Substring(3, 3) + "-" + value.Substring(6, 4);
                                else if (value.Length == 11)
                                    value = value.Substring(1, 3) + "-" + value.Substring(4, 3) + "-" + value.Substring(7, 4);
                            }
                            sql += ", '" + value + "'";

                            if (target != "")
                            {
                                if (fldName == "Callto")
                                    calltoSave = value;
                                else
                                    callfromSave = value;
                            }
                        }
                        else if (fldName == "CalltoAlternate")
                        {
                            if (calltoSave != target && callfromSave != target)
                                sql = sql.Replace("'" + callfromSave + "', '" + calltoSave + "'", "'" + callfromSave + "', '" + value + "'");
                        }
                        else if (fldName == "Calldate")
                        {
                            sql += ", '" + value.Replace("-", " ").Replace("Sunday,", "").Replace("Monday,", "").Replace("Tuesday,", "").Replace("Wednesday,", "").Replace("Thursday,", "").Replace("Friday,", "").Replace("Saturday,", "").Trim() + "'";
                        }
                        else if (fldName == "Calltype")
                        {
                            sql += ", '" + value.Replace("'", "''") + "'";
                        }
                        else if (fldName != "SYS_Exclude")
                        {
                            sql += ", '" + value.Replace("'", "''") + "'";
                        }

                        fieldCounter++;
                    }

                    sql += ")";

                    oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                    oleDbCommand.ExecuteNonQuery();
                    lineCounter++;

                    if (lineCounter % 1000 == 0)
                    {
                        toolStripStatusLabelImportBatch.Text = "Imported " + lineCounter.ToString("#,0") + " of " + dataTableLoadFile.Rows.Count.ToString("#,0") + "...";
                        Application.DoEvents();
                    }
                }

                this.Cursor = Cursors.Default;
                lineCounter--;
                toolStripStatusLabelImportBatch.Text = "Imported " + lineCounter.ToString("#,0") + " of " + dataTableLoadFile.Rows.Count.ToString("#,0") + "...";
                MessageBox.Show(this, "Import is complete.", "Import Batch", MessageBoxButtons.OK, MessageBoxIcon.Information);
                toolStripStatusLabelImportBatch.Text = "Ready";
                
            //    Regex regex = new Regex(@"[|](?=([^\^]*\^[^\^]*\^)*(?![^\^]*\^))");
            //    StringBuilder outLine;
            //    StreamReader streamReaderLoadFile;
            //    OleDbConnection oleDbConnection = new OleDbConnection(DataManager.ConnectionString);
            //    oleDbConnection.Open();
            //    OleDbCommand oleDbCommand;
            //    string inputLine = "", fieldVal = "", page = "", sql;
            //    string loadFileName = "", srcFile = "", begdoc = "";
            //    int recCtr = 0, fieldCtr = 0, lineIndex = 0, batchCol = -1, pageCol = -1, lineCol = -1;
            //    int callDateCol = -1, callTimeCol = -1, callToNoCol = -1, callFromCol = -1, cityToCol = -1, stateToCol = -1, acctNoCol = -1;
            //    int durationCol = -1, srcFileCol = -1, begDocCol = -1;
            //    string d, company, custodian, volinfo;
            //    string r = "|~|";
            //    string batch = "", callDate = "", callTime = "", callToNo = "", callFrom = "", cityTo = "", stateTo = "", acctNo = "";
            //    string duration = "", begDoc = "";

            //    loadFileName = loadFile.Substring(loadFile.LastIndexOf("\\") + 1);
            //    streamReaderLoadFile = new StreamReader(loadFile);
                
            //    // Count the number of lines.
            //    while ((inputLine = streamReaderLoadFile.ReadLine()) != null)
            //    {
            //        recCtr++;
            //    }

            //    fileNum++;
            //    recTotal = recCtr - 1;
            //    grandTotal += recTotal; 
            //    recCtr = 0;
            //    streamReaderLoadFile.Close();
            //    streamReaderLoadFile = new StreamReader(loadFile);
            //    // Skip the header line.
            //    string headerRow = streamReaderLoadFile.ReadLine();
            //    headerRow = headerRow.Replace("^", "");
            //    string[] headers = headerRow.Split('|');
            //    batch = textBoxBatch.Text;
            //    company = textBoxCompany.Text;
            //    custodian = textBoxCustodian.Text;
            //    volinfo = textBoxVolume_Info.Text;
            //    int lineCtr = 1;

            //    foreach (string header in headers)
            //    {
            //        //else if (header.ToLower() == "page") pageCol = fieldCtr;
            //        //else if (header.ToLower() == "line") lineCol = fieldCtr;
            //        // else if (header.ToLower() == "batch") batchCol = fieldCtr;
            //        if (header.ToLower() == "calldate") callDateCol = fieldCtr;
            //        else if (header.ToLower() == "calltime") callTimeCol = fieldCtr;
            //        else if (header.ToLower() == "callto") callToNoCol = fieldCtr;
            //        else if (header.ToLower() == "callfrom") callFromCol = fieldCtr;
            //        else if (header.ToLower() == "city") cityToCol = fieldCtr;
            //        else if (header.ToLower() == "state") stateToCol = fieldCtr;
            //        else if (header.ToLower() == "acctno") acctNoCol = fieldCtr;
            //        else if (header.ToLower() == "duration") durationCol = fieldCtr;

            //        fieldCtr++;
            //    }

            //    if (batch == "")
            //    {
            //        batch = loadFile;
            //        batch = batch.Substring(batch.LastIndexOf('\\') + 1);
            //        batch = batch.Substring(0, batch.LastIndexOf('.'));
            //    }

            //    while ((inputLine = streamReaderLoadFile.ReadLine()) != null)
            //    {
            //        outLine = new StringBuilder();
            //        lineIndex = 0;
            //        fieldCtr = 0;
            //        d = "";
            //        callDate = ""; callTime = ""; callToNo = ""; callFrom = ""; cityTo = ""; stateTo = ""; acctNo = "";
            //        duration = "";

            //        //if (batchCol == -1)
            //        //    batch = fileBatch;
            //        //else
            //        //    batch = "";

            //        foreach (Match match in regex.Matches(inputLine))
            //        {
            //            fieldVal = inputLine.Substring(lineIndex, match.Index - lineIndex).Trim('^');
            //            fieldVal = fieldVal.Replace("'", "''");
            //            if (fieldCtr == batchCol) batch = fieldVal;
            //            else if (fieldCtr == pageCol) page = fieldVal;
            //            else if (fieldCtr == lineCol) line = fieldVal;
            //            else if (fieldCtr == callDateCol) callDate = fieldVal;
            //            else if (fieldCtr == callTimeCol) callTime = fieldVal;
            //            else if (fieldCtr == callToNoCol) callToNo = fieldVal;
            //            else if (fieldCtr == callFromCol) callFrom = fieldVal;
            //            else if (fieldCtr == cityToCol) cityTo = fieldVal;
            //            else if (fieldCtr == stateToCol) stateTo = fieldVal;
            //            else if (fieldCtr == acctNoCol) acctNo = fieldVal;
            //            else if (fieldCtr == durationCol) duration = fieldVal;
            //            else if (fieldCtr == srcFileCol) srcFile = fieldVal;
            //            else if (fieldCtr == begDocCol) begDoc = fieldVal;
            //            lineIndex = match.Index + 1;
            //            fieldCtr++;
            //        }

            //        fieldVal = inputLine.Substring(inputLine.LastIndexOf("|") + 1).Trim('^');
            //        fieldVal = fieldVal.Replace("'", "''");
            //        if (fieldCtr == batchCol) batch = fieldVal;
            //        else if (fieldCtr == pageCol) page = fieldVal;
            //        else if (fieldCtr == lineCol) line = fieldVal;
            //        else if (fieldCtr == callDateCol) callDate = fieldVal;
            //        else if (fieldCtr == callTimeCol) callTime = fieldVal;
            //        else if (fieldCtr == callToNoCol) callToNo = fieldVal;
            //        else if (fieldCtr == callFromCol) callFrom = fieldVal;
            //        else if (fieldCtr == cityToCol) cityTo = fieldVal;
            //        else if (fieldCtr == stateToCol) stateTo = fieldVal;
            //        else if (fieldCtr == acctNoCol) acctNo = fieldVal;
            //        else if (fieldCtr == durationCol) duration = fieldVal;
            //        else if (fieldCtr == srcFileCol) srcFile = fieldVal;
            //        else if (fieldCtr == begDocCol) begDoc = fieldVal;

            //        sql = "insert into Phone (Batch, Page, Line, Calldate, Calltime, Callto, Callfrom, City, State, " + 
            //              "Duration, Acctno, Srcfile, Begdoc#, Company, Custodian, LoadDate, VolInfo) values ('" + 
            //              batch + "', 1, " + lineCtr.ToString() + ", '" + callDate + "', '" + callTime + "', '" + 
            //              callToNo + "', '" + callFrom + "', '" + cityTo + "', '" + stateTo + "', '" + duration + "', '" +
            //              acctNo + "', '" + srcFile + "', '" + begDoc + "', '" + company + "', '" + custodian + "', '" + 
            //              DateTime.Now.ToShortDateString() + "', '" + volinfo + "')";

            //        oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            //        oleDbCommand.ExecuteNonQuery();

            //        //d = "|`|";
            //        //outLine.Append(callDate + d + callTime + d + callToNo + d + callFrom + d + cityTo + d + stateTo + d + duration + d + acctNo + d + page + d + line + d + srcFile + d + begDoc + d + batch);
            //        //GetDimensions(batch, page, line, out lineTop, out lineLeft, out lineWidth, out lineHeight, out pageWidth, out pageHeight);
            //        //outLine.Append(d + lineTop + d + lineLeft + d + lineWidth + d + lineHeight + d + pageWidth + d + pageHeight + r);
            //        //streamWriterSQLLoadFile.WriteLine(outLine.ToString());
            //        recCtr++;
            //        lineCtr++;

            //        if (recCtr % 100 == 0)
            //        {
            //            toolStripStatusLabelLoadDGWeb.Text = "Added " + recCtr.ToString("#,0") + " of " + recTotal.ToString("#,0") + " for " + loadFileName + ", load file " + fileNum.ToString() + " of " + loadFiles.Length.ToString();
            //            Application.DoEvents();
            //        }
            //    }

            //    oleDbCommand = new OleDbCommand("update Batch set Exported = 1 where Batch = '" + batch + "'", oleDbConnection);
            //    oleDbCommand.ExecuteNonQuery();
            }

            //toolStripStatusLabelLoadDGWeb.Text = "Ready";
            //this.Cursor = Cursors.Default;
            //MessageBox.Show(this, "DG Web has been loaded with " + grandTotal.ToString("#,0") + " records.", "Load DG Web", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool IsBatchDuplicate(string batchName)
        {
            string cs = "Provider=SQLOLEDB;Data Source=ATR-LSB-LSS01;Initial Catalog=DocuGenie;User ID=CMTS;Password=gErn8@f0Rm72";
            OleDbConnection oleDbConnection = new OleDbConnection(cs);
            oleDbConnection.Open();
            string sql = "SELECT * FROM Batch WHERE Batch = '" + batchName + "'";
            OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
            DataTable dataTable = new DataTable();
            oleDbDataAdapter.Fill(dataTable);
            oleDbConnection.Close();

            return dataTable.Rows.Count > 0;
        }

        public static void DeletePhone(string batch)
        {
            StringBuilder sql = new StringBuilder("DELETE FROM Phone ");
            sql.Append("WHERE Batch = '" + batch.Replace("'", "''") + "'");
            string cs = "Provider=SQLOLEDB;Data Source=ATR-LSB-LSS01;Initial Catalog=DocuGenie;User ID=CMTS;Password=gErn8@f0Rm72";
            OleDbConnection oleDbConnection = new OleDbConnection(cs);
            oleDbConnection.Open();
            OleDbCommand oleDbCommand = new OleDbCommand(sql.ToString(), oleDbConnection);
            oleDbCommand.ExecuteNonQuery();
            oleDbConnection.Close();
        }

        private void UpdateBatchTemplateFile(string batch, string templateFile)
        {

        }

        private void textFieldParser_RecordFailed(ref int CurrentLineNumber, string LineText, string ErrorMessage, ref bool Continue)
        {
            MessageBox.Show("Parsing error: " + ErrorMessage + ", Line #" + CurrentLineNumber.ToString());
        }

        private void radioButtonLoadFileType_CheckedChanged(object sender, EventArgs e)
        {
            if (textBoxLoadFile.Text.Trim().Length > 0 && openingTemplate == false)
                loadMappingControls();
        }

        private void buttonMapFields_Click(object sender, EventArgs e)
        {
            loadMappingControls();
        }

        private void buttonSaveTemplate_Click(object sender, EventArgs e)
        {
            saveFileDialogTemplate.ShowDialog(this);
        }

        private void saveFileDialogTemplate_FileOk(object sender, CancelEventArgs e)
        {
            if (saveFileDialogTemplate.FileName.Length > 0)
            {               
                using (StreamWriter sw = new StreamWriter(saveFileDialogTemplate.FileName, false))
                {
                    string mapPair = "", loadFileType = radioButtonBankStatement.Checked ? "BankStatement" : "PhoneBill";
                    sw.WriteLine("LoadFileType:" + loadFileType);
                    sw.WriteLine("Batch:" + textBoxBatch.Text);
                    sw.WriteLine("FieldDelim:" + comboBoxFieldDelim.SelectedItem.ToString());
                    sw.WriteLine("QuoteDelim:" + comboBoxQuoteDelim.SelectedItem.ToString());
                    sw.WriteLine("VolumeInfo:" + textBoxVolume_Info.Text);
                    sw.WriteLine("ATR_Load_Info:" + textBoxATRLoadInfo.Text);
                    sw.WriteLine("Company:" + textBoxCompany.Text);
                    sw.WriteLine("Custodian:" + textBoxCustodian.Text);
                    sw.WriteLine("CMTS_ID:" + textBoxCmtsIDWithPrefix.Text);
                    sw.WriteLine("Begdoc:" + textBoxBegdoc.Text);
                    sw.WriteLine("Srcfile:" + textBoxSrcfile.Text);
                    sw.WriteLine("DescriptionHeader:" + textBoxDescription.Text);
                    sw.WriteLine("Target:" + textBoxTargetNumber.Text);

                    foreach (Control control in groupBoxFieldMapping.Controls)
                    {
                        if (control.GetType().Equals(new Label().GetType()))
                        {
                            Label label = control as Label;
                            if (label.Text != "->")
                                mapPair = label.Text + ":";
                        }
                        else if (control.GetType().Equals(new ComboBox().GetType()))
                        {
                            ComboBox comboBox = control as ComboBox;
                            if (comboBox.SelectedIndex > -1)
                                mapPair += comboBox.SelectedItem.ToString();

                            sw.WriteLine(mapPair);
                        }
                    }
                }
                                
                textBoxTemplateFile.Text = saveFileDialogTemplate.FileName;

                //DSOFile.OleDocumentPropertiesClass props = new DSOFile.OleDocumentPropertiesClass();
                //props.Open(saveFileDialogTemplate.FileName, false, DSOFile.dsoFileOpenOptions.dsoOptionDefault);
                //props.SummaryProperties.Comments = textBoxDescription.Text;
                //props.Close(true); 
            }
        }

        private void openFileDialogTemplate_FileOk(object sender, CancelEventArgs e)
        {
            if (openFileDialogTemplate.FileName != "")
            {
                openingTemplate = true;
                groupBoxFieldMapping.Controls.Clear();
                string line, fieldName, selectedValue;
                int top = 25, left = 25, fieldCounter = 0;
                textBoxTemplateFile.Text = openFileDialogTemplate.FileName;
                Hashtable selectedFields = new Hashtable();

                using (StreamReader sr = new StreamReader(openFileDialogTemplate.FileName))
                {
                    for (int x = 0; x < 9; x++)
                    {
                        line = sr.ReadLine();
                    }

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split(':')[1] != "SYS_Exclude" && line.Split(':')[1] != "")
                            selectedFields.Add(line.Split(':')[1], line.Split(':')[1]);
                    }
                }

                using (StreamReader sr = new StreamReader(openFileDialogTemplate.FileName))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("LoadFileType:"))
                        {
                            if (line.Split(':')[1] == "BankStatement")
                                radioButtonBankStatement.Checked = true;
                            else
                                radioButtonPhoneBill.Checked = true;
                        }
                        else if (line.StartsWith("Batch:"))
                        {
                            textBoxBatch.Text = line.Split(':')[1];
                        }
                        else if (line.StartsWith("FieldDelim:"))
                        {
                            string fieldDelim = line.Split(':')[1];
                            if (comboBoxFieldDelim.Items.IndexOf(fieldDelim) > -1)
                                comboBoxFieldDelim.SelectedIndex = comboBoxFieldDelim.Items.IndexOf(fieldDelim);
                        }
                        else if (line.StartsWith("QuoteDelim:"))
                        {
                            string quoteDelim = line.Split(':')[1];
                            if (comboBoxQuoteDelim.Items.IndexOf(quoteDelim) > -1)
                                comboBoxQuoteDelim.SelectedIndex = comboBoxQuoteDelim.Items.IndexOf(quoteDelim);
                        }
                        else if (line.StartsWith("VolumeInfo:"))
                        {
                            textBoxVolume_Info.Text = line.Split(':')[1];
                        }
                        else if (line.StartsWith("ATR_Load_Info:"))
                        {
                            textBoxATRLoadInfo.Text = line.Split(':')[1];
                        }
                        else if (line.StartsWith("Company:"))
                        {
                            textBoxCompany.Text = line.Split(':')[1];
                        }
                        else if (line.StartsWith("Custodian:"))
                        {
                            textBoxCustodian.Text = line.Split(':')[1];
                        }
                        else if (line.StartsWith("CMTS_ID:"))
                        {
                            textBoxCmtsIDWithPrefix.Text = line.Split(':')[1];
                        }
                        else if (line.StartsWith("Begdoc:"))
                        {
                            textBoxBegdoc.Text = line.Split(':')[1];
                        }
                        else if (line.StartsWith("Srcfile:"))
                        {
                            textBoxSrcfile.Text = line.Split(':')[1];
                        }
                        else if (line.StartsWith("DescriptionHeader:"))
                        {
                            textBoxDescription.Text = line.Split(':')[1];
                        }
                        else if (line.StartsWith("Target:"))
                        {
                            textBoxTargetNumber.Text = line.Split(':')[1];
                        }
                        else
                        {
                            if (line != "")
                            {
                                fieldName = line.Split(':')[0];
                                selectedValue = line.Split(':')[1];
                                Label sourceFieldLabel = new Label();
                                sourceFieldLabel.Text = fieldName;
                                sourceFieldLabel.Top = top;
                                sourceFieldLabel.Left = left;
                                sourceFieldLabel.AutoSize = true;
                                groupBoxFieldMapping.Controls.Add(sourceFieldLabel);
                                Label arrowLabel = new Label();
                                arrowLabel.Top = top - 2;
                                arrowLabel.Left = left + 120;
                                arrowLabel.AutoSize = true;
                                arrowLabel.Text = "->";
                                groupBoxFieldMapping.Controls.Add(arrowLabel);
                                ComboBox destFieldList = new ComboBox();
                                destFieldList.Width = 130;
                                destFieldList.DropDownStyle = ComboBoxStyle.DropDownList;
                                destFieldList.Name = "destFieldList" + fieldCounter.ToString();
                                destFieldList.SelectedIndexChanged += new EventHandler(this.destFieldList_SelectedIndexChanged);
                                destFieldList.MouseDown += new MouseEventHandler(this.destFieldList_MouseDown);

                                if (radioButtonPhoneBill.Checked)
                                {
                                    destFieldList.Items.Add("SYS_Exclude");
                                    if (!selectedFields.ContainsKey("Acctno") || selectedValue == "Acctno") destFieldList.Items.Add("Acctno");
                                    if (!selectedFields.ContainsKey("Calldate") || selectedValue == "Calldate") destFieldList.Items.Add("Calldate");
                                    if (!selectedFields.ContainsKey("Calltime") || selectedValue == "Calltime") destFieldList.Items.Add("Calltime");
                                    if (!selectedFields.ContainsKey("Calldatetime") || selectedValue == "Calldatetime") destFieldList.Items.Add("Calldatetime");
                                    if (!selectedFields.ContainsKey("Callto") || selectedValue == "Callto") destFieldList.Items.Add("Callto");
                                    if (!selectedFields.ContainsKey("CalltoAlternate") || selectedValue == "CalltoAlternate") destFieldList.Items.Add("CalltoAlternate");
                                    if (!selectedFields.ContainsKey("Callfrom") || selectedValue == "Callfrom") destFieldList.Items.Add("Callfrom");
                                    if (!selectedFields.ContainsKey("City") || selectedValue == "City") destFieldList.Items.Add("City");
                                    if (!selectedFields.ContainsKey("State") || selectedValue == "State") destFieldList.Items.Add("State");
                                    if (!selectedFields.ContainsKey("Duration") || selectedValue == "Duration") destFieldList.Items.Add("Duration");
                                    if (!selectedFields.ContainsKey("Calltype") || selectedValue == "Calltype") destFieldList.Items.Add("Calltype");
                                }
                                else
                                {
                                    destFieldList.Items.Add("SYS_Exclude");
                                    destFieldList.Items.Add("AccountNumber");
                                    destFieldList.Items.Add("AcctHolder1");
                                    destFieldList.Items.Add("AcctHolder2");
                                    destFieldList.Items.Add("BankCDVolume");
                                    destFieldList.Items.Add("BankCDVolume2");
                                    destFieldList.Items.Add("BankName");
                                    destFieldList.Items.Add("CheckNum");
                                    destFieldList.Items.Add("Date");
                                    destFieldList.Items.Add("Amount");
                                    destFieldList.Items.Add("DepositAmount");
                                    destFieldList.Items.Add("Description");
                                    destFieldList.Items.Add("ItemAccount");
                                    destFieldList.Items.Add("ItemAcctHolder");
                                    destFieldList.Items.Add("ItemAmount");
                                    destFieldList.Items.Add("ItemDate");
                                    destFieldList.Items.Add("ItemImageKey1");
                                    destFieldList.Items.Add("ItemImageKey2");
                                    destFieldList.Items.Add("ItemRoutingTransit");
                                    destFieldList.Items.Add("ItemSequence");
                                    destFieldList.Items.Add("ItemSerial");
                                    destFieldList.Items.Add("ItemTransactionType");
                                    destFieldList.Items.Add("TransactionType");
                                    destFieldList.Items.Add("OtherDocID");
                                    destFieldList.Items.Add("PageNumber");
                                    destFieldList.Items.Add("Recipient");
                                    destFieldList.Items.Add("ReferenceFile");
                                    destFieldList.Items.Add("SequenceNumber");
                                    destFieldList.Items.Add("SourceFiles");
                                    destFieldList.Items.Add("StatementDate");
                                    destFieldList.Items.Add("TotalItems");
                                    destFieldList.Items.Add("TotalPages");
                                    destFieldList.Items.Add("Size");
                                    destFieldList.Items.Add("SYS_Group_Order");
                                    destFieldList.Items.Add("SYS_Err");
                                    destFieldList.Items.Add("SYS_Type");
                                    destFieldList.Items.Add("VolumeName");
                                }

                                if (destFieldList.Items.IndexOf(selectedValue) > -1)
                                    destFieldList.SelectedIndex = destFieldList.Items.IndexOf(selectedValue);

                                destFieldList.Top = top - 5;
                                destFieldList.Left = left + 150;
                                groupBoxFieldMapping.Controls.Add(destFieldList);
                                top += 25;
                                fieldCounter++;

                                if (fieldCounter == 22)
                                {
                                    this.Top = 50;
                                    top = 25;
                                    left += 340;
                                }
                            }
                        }
                    }
                }

                int height = 365 + (20 * fieldCounter);
                if (height > 1000) height = 900;
                int fieldMappingHeight = 90 + (20 * fieldCounter);
                if (fieldMappingHeight > 580) fieldMappingHeight = 580;
                groupBoxFieldMapping.Height = fieldMappingHeight;
                this.Height = height + 60;
                Application.DoEvents();
                openingTemplate = false;
            }
        }

        private void buttonBrowseTemplateFile_Click(object sender, EventArgs e)
        {
            openFileDialogTemplate.ShowDialog(this);
        }

        private void buttonResetFieldDropdownLists_Click(object sender, EventArgs e)
        {
            string selectedItem;

            foreach (Control control in groupBoxFieldMapping.Controls)
            {
                if (control.GetType().Equals(new ComboBox().GetType()))
                {
                    ComboBox destFieldList = control as ComboBox;
                    selectedItem = "";

                    if (destFieldList.SelectedIndex > -1)
                        selectedItem = destFieldList.SelectedItem.ToString();

                    destFieldList.Items.Clear();

                    if (radioButtonPhoneBill.Checked)
                    {
                        destFieldList.Items.Add("SYS_Exclude");
                        destFieldList.Items.Add("Acctno");
                        destFieldList.Items.Add("Calldate");
                        destFieldList.Items.Add("Calltime");
                        destFieldList.Items.Add("Calldatetime");
                        destFieldList.Items.Add("Callto");
                        destFieldList.Items.Add("Callfrom");
                        destFieldList.Items.Add("City");
                        destFieldList.Items.Add("State");
                        destFieldList.Items.Add("Duration");
                    }
                    else
                    {
                        destFieldList.Items.Add("SYS_Exclude");
                        destFieldList.Items.Add("AccountNumber");
                        destFieldList.Items.Add("AcctHolder1");
                        destFieldList.Items.Add("AcctHolder2");
                        destFieldList.Items.Add("BankCDVolume");
                        destFieldList.Items.Add("BankCDVolume2");
                        destFieldList.Items.Add("BankName");
                        destFieldList.Items.Add("CheckNum");
                        destFieldList.Items.Add("Date");
                        destFieldList.Items.Add("Amount");
                        destFieldList.Items.Add("DepositAmount");
                        destFieldList.Items.Add("Description");
                        destFieldList.Items.Add("ItemAccount");
                        destFieldList.Items.Add("ItemAcctHolder");
                        destFieldList.Items.Add("ItemAmount");
                        destFieldList.Items.Add("ItemDate");
                        destFieldList.Items.Add("ItemImageKey1");
                        destFieldList.Items.Add("ItemImageKey2");
                        destFieldList.Items.Add("ItemRoutingTransit");
                        destFieldList.Items.Add("ItemSequence");
                        destFieldList.Items.Add("ItemSerial");
                        destFieldList.Items.Add("ItemTransactionType");
                        destFieldList.Items.Add("TransactionType");
                        destFieldList.Items.Add("OtherDocID");
                        destFieldList.Items.Add("PageNumber");
                        destFieldList.Items.Add("Recipient");
                        destFieldList.Items.Add("ReferenceFile");
                        destFieldList.Items.Add("SequenceNumber");
                        destFieldList.Items.Add("SourceFiles");
                        destFieldList.Items.Add("StatementDate");
                        destFieldList.Items.Add("TotalItems");
                        destFieldList.Items.Add("TotalPages");
                        destFieldList.Items.Add("Size");
                        destFieldList.Items.Add("SYS_Group_Order");
                        destFieldList.Items.Add("SYS_Err");
                        destFieldList.Items.Add("SYS_Type");
                        destFieldList.Items.Add("VolumeName");
                    }

                    if (selectedItem != "" && destFieldList.Items.IndexOf(selectedItem) > -1)
                        destFieldList.SelectedIndex = destFieldList.Items.IndexOf(selectedItem);
                }
            }
        }

        private void buttonPreviewInputFile_Click(object sender, EventArgs e)
        {
            ImportPreview importPreview = new ImportPreview();
            string line, header = "";
            TextFieldParser textFieldParser;
            Application.DoEvents();
            string quoteString = comboBoxQuoteDelim.SelectedItem.ToString();
            char quoteChar = '0';
            DataTable dataTable;
            if (comboBoxQuoteDelim.SelectedIndex > -1 && comboBoxQuoteDelim.SelectedItem.ToString() != "") quoteChar = comboBoxQuoteDelim.SelectedItem.ToString().ToCharArray()[0];
            char fieldDelim = comboBoxFieldDelim.SelectedItem.ToString().ToCharArray()[0];
            string[] loadFiles = textBoxLoadFile.Text.Split(';');
            string userID = Environment.UserName.Replace("ATR\\", "");

            using (StreamReader sr = new StreamReader(textBoxLoadFile.Text))
            {
                line = sr.ReadLine();

                if (!line.Contains(comboBoxFieldDelim.SelectedItem.ToString()))
                {
                    MessageBox.Show("Cannot find field delimiter " + comboBoxFieldDelim.SelectedItem.ToString() + " in the load file.");
                    return;
                }
                else
                {
                    header = line;
                }
            }

            textFieldParser = new TextFieldParser(textBoxLoadFile.Text);
            textFieldParser.FieldDelimiter = fieldDelim;
            if (quoteChar != '0') textFieldParser.QuoteDelimiter = quoteChar;
            string[] colHeaders = header.Split(fieldDelim);
            TextFieldCollection textFieldCollection = new TextFieldCollection();
            string[] headerFields = header.Split(fieldDelim);

            foreach (string headerField in headerFields)
            {
                textFieldCollection.Add(new TextField(headerField, TypeCode.String, true));
            }

            textFieldParser.TextFields = textFieldCollection;
            textFieldParser.RecordFailed += new RecordFailedHandler(textFieldParser_RecordFailed);
            dataTable = textFieldParser.ParseToDataTable(0, 0);
            importPreview.DataTable = dataTable;
            importPreview.ShowDialog(this);
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            if (saveFileDialogTemplate.FileName == "" && openFileDialogTemplate.FileName == "")
            {
                MessageBox.Show("Open or save a template before printing.");
                return;
            }

            if (printDialog.ShowDialog(this) == DialogResult.Cancel)
                return;

            PrintTemplate();
        }

        private void PrintTemplate()
        {
            Application.DoEvents();

            PrintDocument doc = new PrintDocument();
            System.Drawing.Printing.Margins margins = new Margins(0, 0, 0, 0);
            doc.DefaultPageSettings.Margins = margins;
            doc.PrintPage += this.Doc_PrintPage;

            doc.DefaultPageSettings.Margins = margins;
            doc.PrintPage += this.Doc_PrintPage;
            printDialog.Document = doc;
            doc.Print();
        }

        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            int charCount = 0;
            int lineCount = 0;
            Font font = new Font("Arial", 10f);

            // Measure the specified string 'printText'
            // Calculate characters per line 'charCount'
            // Calcuate lines per page that will fit within 
            // the bounds of the page 'lineCount'  
            e.Graphics.MeasureString(printText, font,
                e.MarginBounds.Size, StringFormat.GenericTypographic,
                out charCount, out lineCount);

            // Determine the page bound and draw the string accordingly            
            e.Graphics.DrawString(printText, font, Brushes.Black,
                e.MarginBounds, StringFormat.GenericTypographic);

            // Now remove that part of the string that has been printed.
            printText = printText.Substring(charCount);

            // Check if any more pages left for printing
            if (printText.Length > 0)
                e.HasMorePages = true;
            else
                e.HasMorePages = false;
        }

        private void Doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font font = new Font("Segoe UI", 12);
            Font boldFont = new Font("Segoe UI", 12, FontStyle.Bold);
            Font smallFont = new Font("Segoe UI", 8, FontStyle.Italic);
            float lineHeight = font.GetHeight(e.Graphics);
            float y = 40f;
            float labelX = 40f;
            float dataX = 450f;
            string filePath = saveFileDialogTemplate.FileName == "" ? openFileDialogTemplate.FileName : saveFileDialogTemplate.FileName;
            string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1), line;

            e.Graphics.DrawString("DocuGenie Import Template " + fileName + " - " + DateTime.Now.ToShortDateString(), boldFont, Brushes.Black, labelX, y);
            y += lineHeight;
            y += lineHeight;

            using (StreamReader sr = new StreamReader(filePath))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    e.Graphics.DrawString(line.Split(':')[0], font, Brushes.Black, labelX, y);
                    e.Graphics.DrawString(line.Split(':')[1], boldFont, Brushes.Black, dataX, y);
                    y += lineHeight;
                }
            }

            e.Graphics.ScaleTransform(labelX, y);
        }

        private void buttonCreateSample_Click(object sender, EventArgs e)
        {
            saveFileDialogSample.ShowDialog(this);
        }

        private void saveFileDialogSample_FileOk(object sender, CancelEventArgs e)
        {
            if (saveFileDialogSample.FileName == "")
                return;

            string q = "\"", f = ",";
            string dgHeader = q + "VolumeInfo" + q + f +
                            q + "ATR_Load_Info" + q + f +
                            q + "Company" + q + f +
                            q + "Custodian" + q + f +
                            q + "CMTS_ID" + q + f +
                            q + "Begdoc" + q + f +
                            q + "Srcfile" + q + f +
                            q + "Description" + q + f;

            string srcHeader = q + "N/A" + q + f +
                               q + "N/A" + q + f +
                               q + "N/A" + q + f +
                               q + "N/A" + q + f +
                               q + "N/A" + q + f +
                               q + "N/A" + q + f +
                               q + "N/A" + q + f +
                               q + "N/A" + q + f;

            string quoteString = comboBoxQuoteDelim.SelectedItem.ToString();
            char quoteChar = '0';
            DataTable dataTable;
            if (comboBoxQuoteDelim.SelectedIndex > -1 && comboBoxQuoteDelim.SelectedItem.ToString() != "") quoteChar = comboBoxQuoteDelim.SelectedItem.ToString().ToCharArray()[0];
            char fieldDelim = comboBoxFieldDelim.SelectedItem.ToString().ToCharArray()[0];

            foreach (Control control in groupBoxFieldMapping.Controls)
            {
                if (control.GetType().Equals(new Label().GetType()))
                {
                    Label label = control as Label;
                    if (label.Text != "->")
                        srcHeader += q + label.Text + q + f;
                }
                else if (control.GetType().Equals(new ComboBox().GetType()))
                {
                    ComboBox comboBox = control as ComboBox;
                    if (comboBox.SelectedIndex > -1)
                        dgHeader += q + comboBox.SelectedItem.ToString() + q + f;
                }
            }

            if (dgHeader.EndsWith(f))
                dgHeader = dgHeader.Substring(0, dgHeader.Length - 1);

            if (srcHeader.EndsWith(f))
                srcHeader = srcHeader.Substring(0, srcHeader.Length - 1);

            using (StreamWriter sw = new StreamWriter(saveFileDialogSample.FileName))
            {
                sw.WriteLine(srcHeader);
                sw.WriteLine(dgHeader);
                string outLine, header = "";
                TextFieldParser textFieldParser;
                textFieldParser = new TextFieldParser(textBoxLoadFile.Text);
                textFieldParser.FieldDelimiter = fieldDelim;
                if (quoteChar != '0') textFieldParser.QuoteDelimiter = quoteChar;

                using (StreamReader sr = new StreamReader(textBoxLoadFile.Text))
                {
                    header = sr.ReadLine();
                }

                string[] colHeaders = header.Split(fieldDelim);
                TextFieldCollection textFieldCollection = new TextFieldCollection();
                string[] headerFields = header.Split(fieldDelim);

                foreach (string headerField in headerFields)
                {
                    textFieldCollection.Add(new TextField(headerField, TypeCode.String, true));
                }

                textFieldParser.TextFields = textFieldCollection;
                textFieldParser.RecordFailed += new RecordFailedHandler(textFieldParser_RecordFailed);
                dataTable = textFieldParser.ParseToDataTable(0, 0);                

                foreach (DataRow row in dataTable.Rows)
                {                    
                    outLine = q + textBoxVolume_Info.Text.Replace("\"", "'") + q + f +
                              q + textBoxATRLoadInfo.Text.Replace("\"", "'") + q + f +
                              q + textBoxCompany.Text.Replace("\"", "'") + q + f +
                              q + textBoxCustodian.Text.Replace("\"", "'") + q + f +
                              q + textBoxCmtsIDWithPrefix.Text + q + f +
                              q + textBoxBegdoc.Text + q + f +
                              q + textBoxSrcfile.Text.Replace("\"", "'") + q + f +
                              q + textBoxDescription.Text.Replace("\"", "'") + q + f;

                    foreach (object item in row.ItemArray)
                    {
                        outLine += q + item.ToString().Replace("\"", "'") + q + f;
                    }

                    outLine = outLine.Substring(0, outLine.Length - 1);
                    sw.WriteLine(outLine);
                }
            }

            MessageBox.Show(saveFileDialogSample.FileName + " sample file has been created.");
        }

        private void ImportBatch_Resize(object sender, EventArgs e)
        {
            groupBoxFieldMapping.Height = this.Height - 348;
        }

        private void buttonSelectBatch_Click(object sender, EventArgs e)
        {
            SelectBatchFolder selectBatchFolder = new SelectBatchFolder();

            if (selectBatchFolder.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                textBoxBatch.Text = selectBatchFolder.selectedPath;
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //private void GetDimensions(string batch, string page, string line, out string lineTop, out string lineLeft, out string lineWidth, out string lineHeight, out string pageWidth, out string pageHeight)
        //{
        //    int smallestX1 = 32000, smallestY1 = 32000, biggestX2 = 0, biggestY2 = 0;
        //    int intPage = int.Parse(page);
        //    int intLine = int.Parse(line);

        //    DataTable dataTable = DataManager.GetFieldsForExport(batch, intPage, intLine);

        //    if (dataTable.Rows.Count == 0)
        //    {
        //        lineTop = "0";
        //        lineLeft = "0";
        //        lineHeight = "0";
        //        lineWidth = "0";
        //        pageWidth = "0";
        //        pageHeight = "0";
        //        return;
        //    }

        //    pageWidth = dataTable.Rows[0]["Width"].ToString();
        //    pageHeight = dataTable.Rows[0]["Height"].ToString();

        //    foreach (DataRow dataRow in dataTable.Rows)
        //    {
        //        if (smallestX1 > int.Parse(dataRow["X1"].ToString())) smallestX1 = int.Parse(dataRow["X1"].ToString());
        //        if (smallestY1 > int.Parse(dataRow["Y1"].ToString())) smallestY1 = int.Parse(dataRow["Y1"].ToString());
        //        if (biggestX2 < int.Parse(dataRow["X2"].ToString())) biggestX2 = int.Parse(dataRow["X2"].ToString());
        //        if (biggestY2 < int.Parse(dataRow["Y2"].ToString())) biggestY2 = int.Parse(dataRow["Y2"].ToString());
        //    }

        //    lineTop = smallestY1.ToString();
        //    lineLeft = smallestX1.ToString();
        //    biggestX2 -= smallestX1;
        //    lineWidth = biggestX2.ToString();
        //    biggestY2 -= smallestY1;
        //    lineHeight = biggestY2.ToString();
        //}
    }
}