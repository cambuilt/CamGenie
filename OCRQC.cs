using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;
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
    public partial class OCRQC : Form
    {
        public string BatchPath = "";
        public string BatchName = "";
        public string BatchType = "";
        public string BatchTableName = "Phone";
        public string CallDateYear = "";
        public MainForm mainForm;
        public IOcrEngine OcrEngine;
        public IOcrPage LineOcrPage;
        private IOcrPage ocrPageZones;
        private RasterCodecs codecs = new RasterCodecs();
        private DataTable dataTableUniqueFieldValues = null;
        private DataTable dataTableUniqueFieldValueInstances = null;
        private OleDbConnection oleDbConnection = null;
        private OleDbCommand oleDbCommand = null;
        OleDbDataAdapter oleDbDataAdapter;
        private int fieldIndex = 0, uniqueFieldValueRowNumber = 0, selectedZoneID = 0;
        private List<RasterImage> images = new List<RasterImage>();
        private string selectedPage = "", selectedFieldID = "", fieldName = "";
        private bool showedMemoryMessage = false;

        public OCRQC()
        {
            InitializeComponent();
        }

        public void PerformQC()
        {
            string cs = "Provider=SQLOLEDB;Data Source=ATR-LSB-LSS01;Initial Catalog=CamGenie;User ID=CMTS;Password=gErn8@f0Rm72", sql;
            fieldIndex = BatchType == "PhoneBill" ? 1 : BatchType == "BankStatement" ? 11 : 20;
            fieldName = GetFieldName(fieldIndex);
            comboBoxWhereComparison.SelectedIndex = 0;
            oleDbConnection = new OleDbConnection(cs);
            oleDbConnection.Open();
            string minimumConfidence = Properties.Settings.Default.MinimumConfidence;
            sql = "select Confirmed from Field where Batch = '" + BatchName + "' and Confirmed = 1";
            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
            DataTable dataTable = new DataTable();
            oleDbDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count == 0)
            {
                sql = "update Field set Confirmed = 1 where Batch = '" + BatchName + "' and FieldID = 1 and ConfidenceAdvantage >= " + minimumConfidence + " and Confirmed = 0 and " +
                             "isdate(ValueAdvantage + '/" + CallDateYear + "') = 1";
                oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                oleDbCommand.ExecuteNonQuery();
                sql = "update Field set Confirmed = 1 where Batch = '" + BatchName + "' and FieldID = 2 and (((ValueAdvantage != null and (right(ValueAdvantage, 1) = 'A') or " +
                      "RIGHT(ValueAdvantage, 1) = 'P') and ISDATE('1/1/2000 ' + ValueAdvantage + 'M') = 1) or ISDATE('1/1/2000 ' + ValueAdvantage) = 1) and " +
                      "(ConfidenceAdvantage >= " + minimumConfidence + ")"; // " or ValueAdvantage = ValueAdvantage)";
                oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                oleDbCommand.ExecuteNonQuery();
                sql = "update Field set Confirmed = 1 where Batch = '" + BatchName + "' and FieldID in (3,4) and ValueAdvantage != null and ISNUMERIC(replace(ValueAdvantage, '-', '')) = 1 and " +
                      "(ConfidenceAdvantage >= " + minimumConfidence + ")"; // " or ValueAdvantage = ValueAdvantage)";
                oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                oleDbCommand.ExecuteNonQuery();
                sql = "update Field set Confirmed = 1 where Batch = '" + BatchName + "' and FieldID in (5, 6, 50) and ValueAdvantage != null and " +
                      "(ConfidenceAdvantage >= " + minimumConfidence + ")"; // " or ValueAdvantage = ValueAdvantage)";
                oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                oleDbCommand.ExecuteNonQuery();
            }
            comboBoxGlobalReplaceField.SelectedIndex = 0;
        }

        private void LoadUniqueFieldValues()
        {
            showedMemoryMessage = false;
            int endFieldIndex = 9;

            if (BatchType == "BankStatement")
                endFieldIndex = 19;

            while (true)
            {
                string sql = "select ValueAdvantage from Field where Batch = '" + BatchName + "' and FieldID = " + fieldIndex.ToString() + " and Confirmed = 0 and ValueAdvantage != '' group by ValueAdvantage order by ValueAdvantage";
                oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
                dataTableUniqueFieldValues = new DataTable();
                oleDbDataAdapter.Fill(dataTableUniqueFieldValues);

                if (dataTableUniqueFieldValues.Rows.Count > 0 || fieldIndex == endFieldIndex)
                {
                    break;
                }
                else
                {
                    fieldIndex += 1;

                    if (BatchType == "PhoneBill")
                    {
                        comboBoxGlobalReplaceField.SelectedIndex = fieldIndex - 1;
                        comboBoxGlobalInsertField.SelectedIndex = fieldIndex - 1;
                    }
                    else if (BatchType == "BankStatement")
                    {
                        comboBoxGlobalReplaceField.SelectedIndex = fieldIndex - 1 - 10;
                        comboBoxGlobalInsertField.SelectedIndex = fieldIndex - 1 - 10;
                    }

                    fieldName = GetFieldName(fieldIndex);
                }
            }
        }

        private void LoadUniqueFieldValueInstanceImages()
        {
            string zoneFile, sql, imageFile = "", imagePath;
            int page = 0, zoneID = 1;

            if (ocrPageZones != null)
            {
                ocrPageZones.Dispose();
                ocrPageZones = null;
            }

            if (uniqueFieldValueRowNumber >= dataTableUniqueFieldValues.Rows.Count)
                return;

            OcrZone zone;
            LeadRect zoneRect;
            RasterImage pageImage = null, zoneImage;
            textBoxValue.Text = dataTableUniqueFieldValues.Rows[uniqueFieldValueRowNumber][0].ToString();
            sql = "select Page, ZoneID from Field where Batch = '" + BatchName + "' and FieldID = " + fieldIndex.ToString() + " and (ValueAdvantage = '" + textBoxValue.Text.Replace("'", "''") + "') order by Page, Line";
            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
            dataTableUniqueFieldValueInstances = new DataTable();
            oleDbDataAdapter.Fill(dataTableUniqueFieldValueInstances);
            images = new List<RasterImage>();

            foreach (DataRow instanceRow in dataTableUniqueFieldValueInstances.Rows)
            {
                zoneID = int.Parse(instanceRow["ZoneID"].ToString());

                if (page != int.Parse(instanceRow[0].ToString()))
                {
                    page = int.Parse(instanceRow[0].ToString());
                    sql = "select top 1 Srcfile from " + BatchTableName + " where Batch = '" + BatchName + "' and Page = " + page.ToString();
                    oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                    oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
                    DataTable dataTableSrcfile = new System.Data.DataTable();
                    oleDbDataAdapter.Fill(dataTableSrcfile);

                    if (dataTableSrcfile.Rows.Count > 0)
                    {
                        imageFile = dataTableSrcfile.Rows[0][0].ToString();
                        imagePath = @"I:\" + BatchName.Split(' ')[0] + @"\" + imageFile;

                        if (imageFile.Split('\\').Length == 3)
                            imageFile = imageFile.Split('\\')[2];
                        else
                            imageFile = imageFile.Split('\\')[1];

                        zoneFile = imagePath.Substring(0, imagePath.LastIndexOf(".")) + ".ozf";

                        if (File.Exists(zoneFile))
                        {
                            try
                            {
                                pageImage = codecs.Load(imagePath);

                                if (pageImage.XResolution < 150)
                                    pageImage.XResolution = 150;

                                if (pageImage.YResolution < 150)
                                    pageImage.YResolution = 150;

                                ocrPageZones = OcrEngine.CreatePage(pageImage, OcrImageSharingMode.AutoDispose);
                                ocrPageZones.LoadZones(zoneFile);
                            }
                            catch (Exception ex)
                            {
                                if (showedMemoryMessage == false)
                                {
                                    Messager.ShowInformation(this, "Could not open " + imagePath + ". " + ex.Message);
                                    showedMemoryMessage = true;
                                }
                                pageImage = null;
                            }
                        }
                    }
                }

                if (pageImage != null)
                {                    
                    zone = GetPageZone(ocrPageZones, zoneID);
                    zoneRect = zone.Bounds.ToRectangle(150, 150);
                    zoneImage = pageImage.Clone(zoneRect);
                    zoneImage.CustomData.Add("Page", page.ToString());
                    zoneImage.CustomData.Add("ZoneID", instanceRow["ZoneID"].ToString());
                    zoneImage.CustomData.Add("FieldID", fieldIndex.ToString());
                    images.Add(zoneImage);
                }

                if (images.Count > 59)
                    break;
            }

            DisplayUniqueFieldValueInstanceImagePage();
        }

        private OcrZone GetPageZone(IOcrPage ocrPageZones, int zoneID)
        {
            foreach (OcrZone zone in ocrPageZones.Zones)
            {
                if (zone.Id == zoneID)
                    return zone;
            }

            return new OcrZone();
        }

        private void DisplayUniqueFieldValueInstanceImagePage()
        {
            int viewerCounter = 1;
            int endIndex = images.Count < 60 ? images.Count : 60;
            ImageViewer viewer;

            for (int index = 0; index < endIndex; index++)
            {
                viewer = (ImageViewer)this.Controls.Find("imageViewer" + viewerCounter.ToString(), true)[0];
                viewer.Image = images[index];

                if (BatchType == "BankStatement")
                    viewer.Zoom(ControlSizeMode.None, viewer.ScaleFactor - 0.4, viewer.DefaultZoomOrigin);

                viewerCounter += 1;
            }

            if (viewerCounter < 60)
            {
                for (int index = viewerCounter; index < 61; index++)
                {
                    viewer = (ImageViewer)this.Controls.Find("imageViewer" + index.ToString(), true)[0];
                    viewer.Image = null;
                }
            }

            int recNum = uniqueFieldValueRowNumber + 1;
            toolStripStatusLabel.Text = "QC: " + GetFieldName(fieldIndex) + " " + recNum.ToString() + " of " + dataTableUniqueFieldValues.Rows.Count.ToString();
        }

        private void QC_Load(object sender, EventArgs e)
        {
            if (BatchType == "PhoneBill")
            {
                comboBoxGlobalReplaceField.Items.Add("Calldate");
                comboBoxGlobalReplaceField.Items.Add("Calltime");
                comboBoxGlobalReplaceField.Items.Add("Callto");
                comboBoxGlobalReplaceField.Items.Add("Callfrom");
                comboBoxGlobalReplaceField.Items.Add("City");
                comboBoxGlobalReplaceField.Items.Add("State");
                comboBoxGlobalReplaceField.Items.Add("CityState");
                comboBoxGlobalReplaceField.Items.Add("Duration");
                comboBoxGlobalReplaceField.Items.Add("Origination");
                comboBoxGlobalReplaceField.Items.Add("Calltype");
                comboBoxGlobalInsertField.Items.Add("Calldate");
                comboBoxGlobalInsertField.Items.Add("Calltime");
                comboBoxGlobalInsertField.Items.Add("Callto");
                comboBoxGlobalInsertField.Items.Add("Callfrom");
                comboBoxGlobalInsertField.Items.Add("City");
                comboBoxGlobalInsertField.Items.Add("State");
                comboBoxGlobalInsertField.Items.Add("CityState");
                comboBoxGlobalInsertField.Items.Add("Duration");
                comboBoxGlobalInsertField.Items.Add("Origination");
                comboBoxGlobalInsertField.Items.Add("Calltype");
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
                comboBoxGlobalInsertField.Items.Add("AccountNumber");
                comboBoxGlobalInsertField.Items.Add("CheckNumber");
                comboBoxGlobalInsertField.Items.Add("CardNumber");
                comboBoxGlobalInsertField.Items.Add("Amount");
                comboBoxGlobalInsertField.Items.Add("TransactionDate");
                comboBoxGlobalInsertField.Items.Add("TransactionType");
                comboBoxGlobalInsertField.Items.Add("RoutingNumber");
                comboBoxGlobalInsertField.Items.Add("AccountHolder1");
                comboBoxGlobalInsertField.Items.Add("AccountHolder2");
                comboBoxGlobalInsertField.Items.Add("Description");
                comboBoxGlobalInsertField.Items.Add("DepositAmount");
                BatchTableName = "BankStatement";
                textBoxValue.Font = new Font("Segoe UI", 20);
            }

            PerformQC();
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
                    fieldName = "Callto";
                    break;
                case 4:
                    fieldName = "Callfrom";
                    break;
                case 5:
                    fieldName = "City";
                    break;
                case 6:
                    fieldName = "State";
                    break;
                case 7:
                    fieldName = "CityState";
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
                    fieldName = "AccountNumber";
                    break;
                case 12:
                    fieldName = "CheckNumber";
                    break;
                case 13:
                    fieldName = "CardNumber";
                    break;
                case 14:
                    fieldName = "Amount";
                    break;
                case 15:
                    fieldName = "TransactionDate";
                    break;
                case 16:
                    fieldName = "TransactionType";
                    break;
                case 17:
                    fieldName = "RoutingNumber";
                    break;
                case 18:
                    fieldName = "AccountHolder1";
                    break;
                case 19:
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

        private void buttonClose_Click(object sender, EventArgs e)
        {
            oleDbConnection.Close();
            this.Close();
        }

        private void buttonNextValue_Click(object sender, EventArgs e)
        {
            uniqueFieldValueRowNumber += 1;
            this.Cursor = Cursors.AppStarting;

            if (uniqueFieldValueRowNumber == dataTableUniqueFieldValues.Rows.Count)
            {
                fieldIndex += 1;
                int listIndex = fieldIndex;

                if (BatchType == "BankStatement")
                    listIndex -= 10;

                if (listIndex > comboBoxGlobalReplaceField.Items.Count - 1)
                {
                    this.Cursor = Cursors.Default;
                    prepareForUpload();
                    return;
                }
                comboBoxGlobalReplaceField.SelectedIndex = listIndex - 1;
                comboBoxGlobalInsertField.SelectedIndex = listIndex - 1;
                fieldName = GetFieldName(fieldIndex);
                LoadUniqueFieldValues();
            }

            LoadUniqueFieldValueInstanceImages();
            this.Cursor = Cursors.Default;
        }

        private void prepareForUpload()
        {
            string batchConfigFile = BatchPath + "\\batch.config";
            bool foundHeaderFields = false;

            if (File.Exists(batchConfigFile) && BatchType == "PhoneBill")
            {
                string line;

                using (StreamReader sr = new StreamReader(batchConfigFile))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("Acctno"))
                        { 
                            foundHeaderFields = true;
                            break;
                        }
                    }
                }
            }

            if (foundHeaderFields == false)
            {
                Messager.ShowInformation(this, "QC is complete. Set the header field values in File -> Batch Properties, then select Upload.");
            }
            else
            {
                Upload upload = new Upload();
                upload.BatchName = BatchName;
                upload.BatchType = BatchType;
                upload.BatchPath = BatchPath;
                upload.BatchTableName = BatchTableName;
                upload.mainForm = mainForm;
                upload.PageTotal = mainForm.images.Length;
                upload.ShowDialog(this);
                this.Close();
            }
        }

        private void buttonPrevValue_Click(object sender, EventArgs e)
        {
            uniqueFieldValueRowNumber -= 1;
            this.Cursor = Cursors.AppStarting;

            if (uniqueFieldValueRowNumber == -1)
            {
                if (fieldIndex > 1)
                {
                    fieldIndex -= 1;
                    fieldName = GetFieldName(fieldIndex);
                    LoadUniqueFieldValues();
                }
                else
                {
                    uniqueFieldValueRowNumber += 1;
                }
            }

            LoadUniqueFieldValueInstanceImages();
            this.Cursor = Cursors.Default;
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            string fieldName = GetFieldName(fieldIndex);

            if (fieldName.ToLower().Contains("date"))
            {
                DateTime dateTest;

                if (textBoxValue.Text.Split('/').Length == 2)
                {                    
                    if (DateTime.TryParse(textBoxValue.Text + "/2012", out dateTest) == false)
                    {
                        Messager.ShowInformation(this, textBoxValue.Text + " is an invalid date.");
                        return;
                    }
                }
                else if (BatchType == "BankStatement")
                {
                    if (DateTime.TryParse(textBoxValue.Text, out dateTest) == false)
                    {
                        Messager.ShowInformation(this, textBoxValue.Text + " is an invalid date.");
                        return;
                    }
                }
            }

            if (fieldName.ToLower().Contains("amount"))
            {
                double doubleTest;

                if (double.TryParse(textBoxValue.Text, out doubleTest) == false)
                {
                    Messager.ShowInformation(this, textBoxValue.Text + " is an invalid number.");
                    return;
                }
            }

            if (uniqueFieldValueRowNumber > -1 && uniqueFieldValueRowNumber < dataTableUniqueFieldValues.Rows.Count)
            { 
                string sql = "update Field set ValueAdvantage = '" + textBoxValue.Text.Replace("'", "''") + "', Confirmed = 1 where Batch = '" + BatchName + "' and FieldID = " + fieldIndex.ToString() + " and ValueAdvantage = '" + dataTableUniqueFieldValues.Rows[uniqueFieldValueRowNumber][0].ToString().Replace("'", "''") + "'";
                oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                oleDbCommand.ExecuteNonQuery();
            }

            buttonNextValue_Click(null, null);
        }

        private void imageViewer_Click(object sender, EventArgs e)
        {
            ImageViewer imageViewer = (ImageViewer)sender;
        }

        private void QC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (this.ActiveControl == textBoxInstance)
                {
                    this.Cursor = Cursors.AppStarting;
                    string sql = "update Field set ValueAdvantage = '" + textBoxInstance.Text.Replace("'", "''") + "', " + 
                                 "ValueProfessional = '" + textBoxInstance.Text.Replace("'", "''") + "', Confirmed = 1 where Batch = '" + BatchName + "' and Page = " + selectedPage +
                                 " and ZoneID = " + selectedZoneID.ToString() + " and FieldID = " + selectedFieldID;
                    oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                    oleDbCommand.ExecuteNonQuery();
                    LoadUniqueFieldValues();
                    uniqueFieldValueRowNumber = 0;
                    LoadUniqueFieldValueInstanceImages();
                    textBoxInstance.Visible = false;
                    this.Cursor = Cursors.Default;
                }
                else if (this.ActiveControl == textBoxValue)
                {
                    buttonAccept_Click(null, null);
                }
                e.Handled = true;
            }
            else if (e.KeyChar == (char)27)
            {
                textBoxInstance.Visible = false;
                e.Handled = true;
            }
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.AppStarting;
            string sql = "update Field set ValueAdvantage = ";

            if (textBoxReplacementTarget.Text == "*")
            {
                sql += "'" + textBoxReplacementValue.Text.Replace("'", "''") + "'";
            }
            else if (comboBoxPosition.Text.Trim() == "")
            {
                sql += "replace(ValueAdvantage, '" +
                       textBoxReplacementTarget.Text.Replace("'", "''") + "', '" +
                       textBoxReplacementValue.Text.Replace("'", "''") + "')";
            }
            else if (comboBoxPosition.Text == "end")
            {
                int replacementLength = textBoxReplacementTarget.Text.Length;
                sql += "left(ValueAdvantage, len(ValueAdvantage) - " + replacementLength.ToString() + ") + '" +
                        textBoxReplacementValue.Text.Replace("'", "''") + "'";
            }
            else if (textBoxLength.Text.Trim() != "")
            {
                string pos = comboBoxPosition.Text.Trim();
                int leftLength = int.Parse(pos);
                int length = int.Parse(textBoxLength.Text);

                if (leftLength > 1)
                    leftLength -= 1;

                sql += "left(ValueAdvantage, " + leftLength.ToString() + ") + '" + textBoxReplacementValue.Text.Replace("'", "''") + "'";

                if (leftLength + 1 < length)
                {
                    int rightLength = length - leftLength - 1;
                    sql += " + right(ValueAdvantage, " + rightLength.ToString() + ")";
                }
            }
            else
            {
                Messager.ShowInformation(this, "Length is required when character position is entered.");
                this.Cursor = Cursors.Default;
                return;
            }

            sql += ", ValueProfessional = ";

            if (textBoxReplacementTarget.Text == "*")
            {
                sql += "'" + textBoxReplacementValue.Text.Replace("'", "''") + "'";
            }
            else if (comboBoxPosition.Text.Trim() == "")
            {
                sql += "replace(ValueProfessional, '" +
                       textBoxReplacementTarget.Text.Replace("'", "''") + "', '" +
                       textBoxReplacementValue.Text.Replace("'", "''") + "')";
            }
            else if (comboBoxPosition.Text == "end")
            {
                int replacementLength = textBoxReplacementTarget.Text.Length;
                sql += "left(ValueProfessional, len(ValueProfessional) - " + replacementLength.ToString() + ") + '" +
                        textBoxReplacementValue.Text.Replace("'", "''") + "'";
            }
            else if (textBoxLength.Text.Trim() != "")
            {
                string pos = comboBoxPosition.Text.Trim();
                int leftLength = int.Parse(pos);
                int length = int.Parse(textBoxLength.Text);

                if (leftLength > 1)
                    leftLength -= 1;

                sql += "left(ValueProfessional, " + leftLength.ToString() + ") + '" + textBoxReplacementValue.Text.Replace("'", "''") + "'";

                if (leftLength + 1 < length)
                {
                    int rightLength = length - leftLength - 1;
                    sql += " + right(ValueProfessional, " + rightLength.ToString() + ")";
                }
            }

            if (textBoxReplacementTarget.Text == "*")
                textBoxValue.Text = textBoxReplacementValue.Text;
            else
                textBoxValue.Text = textBoxValue.Text.Replace(textBoxReplacementTarget.Text, textBoxReplacementValue.Text);

            int fieldID = comboBoxGlobalReplaceField.SelectedIndex + 1;

            if (BatchType == "BankStatement")
                fieldID += 10;

            sql += " where Batch = '" + BatchName + "' and FieldID = " + fieldID.ToString();

            if (textBoxWhereValue.Text.Trim() != "")
            {
                sql += " and (ValueAdvantage ";

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

                sql += " or ValueProfessional ";

                if (comboBoxWhereComparison.SelectedIndex == 0)
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

                sql += ")";
            }
            else
            {
                sql += " and (ValueAdvantage like '%" + textBoxReplacementTarget.Text.Replace("'", "''") + "%' or ValueProfessional like '%" + textBoxReplacementTarget.Text.Replace("'", "''") + "%')";
            }

            if (textBoxLength.Text.Trim() != "")
                sql += " and len(ValueAdvantage) = " + textBoxLength.Text;

            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            int recs = oleDbCommand.ExecuteNonQuery();

            if (recs > 0)
            {
                LoadUniqueFieldValues();
                uniqueFieldValueRowNumber = 0;
                DisplayUniqueFieldValueInstanceImagePage();
            }

            this.Cursor = Cursors.Default;
            buttonClear.PerformClick();
            Messager.ShowInformation(this, recs.ToString() + " records were updated.");
        }

        private void comboBoxGlobalReplaceField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxGlobalReplaceField.SelectedIndex > -1)
            {
                labelFieldName.Text = comboBoxGlobalReplaceField.SelectedItem.ToString();
                comboBoxGlobalInsertField.SelectedIndex = comboBoxGlobalReplaceField.SelectedIndex;

                buttonReplace.Enabled = textBoxReplacementTarget.Text.Trim() != "" && textBoxReplacementValue.Text.Trim() != "";
                buttonClear.Enabled = textBoxReplacementTarget.Text.Trim() != "" && textBoxReplacementValue.Text.Trim() != "";

                this.Cursor = Cursors.AppStarting;
                LoadUniqueFieldValues();
                uniqueFieldValueRowNumber = 0;

                if (dataTableUniqueFieldValues.Rows.Count > 0)
                {
                    LoadUniqueFieldValueInstanceImages();
                    buttonAcceptValid.Visible = false;

                    if (fieldIndex == 1 || fieldIndex == 15)
                    {
                        buttonAcceptValid.Visible = true;
                        buttonAcceptValid.Text = "Accept Valid Dates";
                    }
                    else if (fieldIndex == 2)
                    {
                        buttonAcceptValid.Visible = true;
                        buttonAcceptValid.Text = "Accept Valid Times";
                    }
                    else if (fieldIndex == 3 || fieldIndex == 4)
                    {
                        buttonAcceptValid.Visible = true;
                        buttonAcceptValid.Text = "Accept Valid Phone #s";
                    }
                    else if (fieldIndex == 5)
                    {
                        buttonAcceptValid.Visible = true;
                        buttonAcceptValid.Text = "Accept Valid Cities";
                    }
                    else if (fieldIndex == 8)
                    {
                        buttonAcceptValid.Visible = true;
                        buttonAcceptValid.Text = "Accept Valid Durations";
                    }
                    else if (fieldIndex == 9)
                    {
                        buttonAcceptValid.Visible = true;
                        buttonAcceptValid.Text = "Accept Valid Originations";
                    }
                    else if (fieldIndex == 11)
                    {
                        buttonAcceptValid.Visible = true;
                        buttonAcceptValid.Text = "Accept Valid Account Numbers";
                    }
                    else if (fieldIndex == 12)
                    {
                        buttonAcceptValid.Visible = true;
                        buttonAcceptValid.Text = "Accept Valid Check Numbers";
                    }
                    else if (fieldIndex == 13)
                    {
                        buttonAcceptValid.Visible = true;
                        buttonAcceptValid.Text = "Accept Valid Card Numbers";
                    }
                    else if (fieldIndex == 14 || fieldIndex == 21)
                    {
                        buttonAcceptValid.Visible = true;
                        buttonAcceptValid.Text = "Accept Valid Amounts";
                    }
                }

                this.Cursor = Cursors.Default;
            }
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

        private void textBoxReplacementTarget_TextChanged(object sender, EventArgs e)
        {
            buttonReplace.Enabled = textBoxReplacementTarget.Text.Trim() != "" && comboBoxGlobalReplaceField.SelectedIndex > -1;
            buttonClear.Enabled = textBoxReplacementTarget.Text.Trim() != "" && comboBoxGlobalReplaceField.SelectedIndex > -1;
        }

        private void imageViewer_MouseUp(object sender, MouseEventArgs e)
        {
            ImageViewer imageViewer = (ImageViewer)sender;

            if (imageViewer.Image == null)
                return;

            selectedPage = imageViewer.Image.CustomData["Page"].ToString();
            selectedZoneID = int.Parse(imageViewer.Image.CustomData["ZoneID"].ToString());
            selectedFieldID = imageViewer.Image.CustomData["FieldID"].ToString();
            string sql = "select ValueAdvantage from Field where Batch = '" + BatchName + "' and Page = " + selectedPage +
                         " and ZoneID = " + selectedZoneID;
            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
            DataTable dataTable = new DataTable();
            oleDbDataAdapter.Fill(dataTable);

            if (e.Button == System.Windows.Forms.MouseButtons.Left && imageViewer.Image != null)
            {
                textBoxInstance.Top = imageViewer.Top;
                textBoxInstance.Left = imageViewer.Left;
                textBoxInstance.Text = dataTable.Rows[0][0].ToString();
                textBoxInstance.Visible = true;
                textBoxInstance.BringToFront();
                textBoxInstance.SelectionLength = 0;
                textBoxInstance.SelectionStart = 0;
                textBoxInstance.Focus();
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip.Show(imageViewer.Left + this.Left + 80, this.Top + imageViewer.Top + imageViewer.Height - 5);
            }
        }

        private void contextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Equals(showPageToolStripMenuItem))
            {
                mainForm.toolStripTextBoxPageNumber.Text = selectedPage;
                mainForm.LoadImage();
                mainForm.FocusZone(selectedZoneID);
            }
        }

        private void textBoxReplacementValue_TextChanged(object sender, EventArgs e)
        {
            buttonReplace.Enabled = textBoxReplacementTarget.Text.Trim() != "" && textBoxReplacementValue.Text.Trim() != "" && comboBoxGlobalReplaceField.SelectedIndex > -1;
            buttonClear.Enabled = textBoxReplacementTarget.Text.Trim() != "" && textBoxReplacementValue.Text.Trim() != "" && comboBoxGlobalReplaceField.SelectedIndex > -1;
        }

        private void comboBoxGlobalInsertField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxGlobalInsertField.SelectedIndex > -1)
            {
                labelInsertFieldName.Text = comboBoxGlobalInsertField.SelectedItem.ToString();
            }
        }

        private void textBoxInsert_TextChanged(object sender, EventArgs e)
        {
            buttonInsert.Enabled = textBoxInsertCharacter.Text != "" && textBoxInsertPosition.Text.Trim() != "";
            buttonInsertClear.Enabled = textBoxInsertCharacter.Text != "" && textBoxInsertPosition.Text.Trim() != "";
        }

        private void buttonInsert_Click(object sender, EventArgs e)
        {
            string sql = "", leftLen = "1", rightLen = "1";
            int recs = 0, num;

            if (checkBoxFromLast.Checked == false)
            {
                num = int.Parse(textBoxInsertPosition.Text) - 1;
                leftLen = num.ToString();
                num = int.Parse(textBoxInsertFieldValueLength.Text) - num;
                rightLen = num.ToString();
            }
            else 
            {
                num = int.Parse(textBoxInsertPosition.Text) - 1;
                leftLen = "LEN(ValueAdvantage) - " + num.ToString();
                rightLen = num.ToString();
            }

            sql = "update Field set ValueAdvantage = left(ValueAdvantage, " + leftLen.ToString() + ") + '" +
                  textBoxInsertCharacter.Text + "' + right(ValueAdvantage, " + rightLen.ToString() + ") where " +
                  "Batch = '" + BatchName + "' and " +
                  "FieldID = " + fieldIndex.ToString() + " and Confirmed = 0 and ValueAdvantage not like '%" +
                  textBoxInsertCharacter.Text + "%'";

            if (textBoxInsertFieldValueLength.Text.Trim() != "")
                sql += " and len(ValueAdvantage) = " + textBoxInsertFieldValueLength.Text;

            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            recs = oleDbCommand.ExecuteNonQuery();
            sql = sql.Replace("ValueAdvantage", "ValueProfessional");
            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            oleDbCommand.ExecuteNonQuery();

            if (checkBoxFromLast.Checked == false)
            {
                textBoxValue.Text = textBoxValue.Text.Substring(0, int.Parse(leftLen)) + textBoxInsertCharacter.Text + textBoxValue.Text.Substring(int.Parse(leftLen), int.Parse(rightLen));
            }
            else
            {
                num = int.Parse(textBoxInsertPosition.Text) - 1;
                textBoxValue.Text = textBoxValue.Text.Substring(0, textBoxValue.Text.Length - num) + textBoxInsertCharacter.Text + textBoxValue.Text.Substring(textBoxValue.Text.Length - num, num);
            }

            if (recs > 0)
            {
                this.Cursor = Cursors.AppStarting;
                LoadUniqueFieldValues();
                uniqueFieldValueRowNumber = 0;
                DisplayUniqueFieldValueInstanceImagePage();
                this.Cursor = Cursors.Default;
            }

            buttonInsertClear.PerformClick();
            Messager.ShowInformation(this, recs.ToString() + " " + labelInsertFieldName.Text + " values were updated.");
        }

        private void buttonAcceptValid_Click(object sender, EventArgs e)
        {
            string sql = "update Field set Confirmed = 1 where Batch = '" + BatchName + "' and FieldID = " + fieldIndex + " ";

            if (fieldIndex == 1)
                sql += "and ISDATE(ValueAdvantage + '/2012') = 1 and Confirmed = 0";
            else if (fieldIndex == 2)
                sql += "and ISDATE(ValueAdvantage) = 1 and Confirmed = 0";
            else if (fieldIndex == 3 || fieldIndex == 4)
                sql += "and ISNUMERIC(REPLACE(ValueAdvantage, '-', '')) = 1 and Confirmed = 0";
            else if (fieldIndex == 5 || fieldIndex == 9)
                sql += "and dbo.isAlpha(ValueAdvantage) = 1 and Confirmed = 0";
            else if (fieldIndex == 8 || (fieldIndex > 10 && fieldIndex < 15))
                sql += "and ISNUMERIC(ValueAdvantage) = 1 and Confirmed = 0";
            else if (fieldIndex == 15)
                sql += "and ISDATE(ValueAdvantage) = 1 and Confirmed = 0";
            
            oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            int recs = oleDbCommand.ExecuteNonQuery();

            if (recs > 0)
            {
                this.Cursor = Cursors.AppStarting;
                LoadUniqueFieldValues();
                uniqueFieldValueRowNumber = 0;
                DisplayUniqueFieldValueInstanceImagePage();
                this.Cursor = Cursors.Default;
            }

            Messager.ShowInformation(this, recs.ToString() + " records have been accepted.");
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxReplacementTarget.Text = "";
            textBoxReplacementValue.Text = "";
            comboBoxPosition.Text = "";
            textBoxLength.Text = "";
            textBoxWhereValue.Text = "";
        }

        private void buttonInsertClear_Click(object sender, EventArgs e)
        {
            textBoxInsertCharacter.Text = "";
            textBoxInsertFieldValueLength.Text = "";
            textBoxInsertPosition.Text = "";
            checkBoxFromLast.Checked = false;
        }
    }
}
