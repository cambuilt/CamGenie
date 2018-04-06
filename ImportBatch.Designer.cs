namespace CamGenie
{
    partial class ImportBatch
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxLoadFile = new System.Windows.Forms.TextBox();
            this.labelLoadFile = new System.Windows.Forms.Label();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.openFileDialogLoadDGWeb = new System.Windows.Forms.OpenFileDialog();
            this.statusStripLoadDGWeb = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelImportBatch = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelBatch = new System.Windows.Forms.Label();
            this.textBoxBatch = new System.Windows.Forms.TextBox();
            this.groupBoxFieldMapping = new System.Windows.Forms.GroupBox();
            this.buttonBrowseTemplateFile = new System.Windows.Forms.Button();
            this.labelTemplateFile = new System.Windows.Forms.Label();
            this.textBoxTemplateFile = new System.Windows.Forms.TextBox();
            this.labelFieldDelim = new System.Windows.Forms.Label();
            this.labelQuoteDelim = new System.Windows.Forms.Label();
            this.groupBoxLoadFileType = new System.Windows.Forms.GroupBox();
            this.radioButtonBankStatement = new System.Windows.Forms.RadioButton();
            this.radioButtonPhoneBill = new System.Windows.Forms.RadioButton();
            this.textBoxVolume_Info = new System.Windows.Forms.TextBox();
            this.labelVolume_Info = new System.Windows.Forms.Label();
            this.textBoxATRLoadInfo = new System.Windows.Forms.TextBox();
            this.labelATRLoadInfo = new System.Windows.Forms.Label();
            this.textBoxCustodian = new System.Windows.Forms.TextBox();
            this.labelCustodian = new System.Windows.Forms.Label();
            this.textBoxCompany = new System.Windows.Forms.TextBox();
            this.labelCompany = new System.Windows.Forms.Label();
            this.textBoxCmtsIDWithPrefix = new System.Windows.Forms.TextBox();
            this.labelCMTSID = new System.Windows.Forms.Label();
            this.comboBoxFieldDelim = new System.Windows.Forms.ComboBox();
            this.comboBoxQuoteDelim = new System.Windows.Forms.ComboBox();
            this.buttonMapFields = new System.Windows.Forms.Button();
            this.buttonSaveTemplate = new System.Windows.Forms.Button();
            this.saveFileDialogTemplate = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialogTemplate = new System.Windows.Forms.OpenFileDialog();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonPreviewInputFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxBegdoc = new System.Windows.Forms.TextBox();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSrcfile = new System.Windows.Forms.TextBox();
            this.printDialog = new System.Windows.Forms.PrintDialog();
            this.buttonCreateSample = new System.Windows.Forms.Button();
            this.saveFileDialogSample = new System.Windows.Forms.SaveFileDialog();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxTargetNumber = new System.Windows.Forms.TextBox();
            this.buttonSelectBatch = new System.Windows.Forms.Button();
            this.statusStripLoadDGWeb.SuspendLayout();
            this.groupBoxLoadFileType.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxLoadFile
            // 
            this.textBoxLoadFile.Location = new System.Drawing.Point(92, 57);
            this.textBoxLoadFile.Name = "textBoxLoadFile";
            this.textBoxLoadFile.Size = new System.Drawing.Size(398, 21);
            this.textBoxLoadFile.TabIndex = 0;
            // 
            // labelLoadFile
            // 
            this.labelLoadFile.AutoSize = true;
            this.labelLoadFile.Location = new System.Drawing.Point(12, 60);
            this.labelLoadFile.Name = "labelLoadFile";
            this.labelLoadFile.Size = new System.Drawing.Size(54, 13);
            this.labelLoadFile.TabIndex = 1;
            this.labelLoadFile.Text = "Input file:";
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(496, 53);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(63, 26);
            this.buttonBrowse.TabIndex = 1;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.buttonClose.Location = new System.Drawing.Point(614, 82);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 33);
            this.buttonClose.TabIndex = 16;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.buttonLoad.Location = new System.Drawing.Point(614, 47);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(75, 33);
            this.buttonLoad.TabIndex = 15;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // openFileDialogLoadDGWeb
            // 
            this.openFileDialogLoadDGWeb.DefaultExt = "dat";
            this.openFileDialogLoadDGWeb.Filter = "Load files (*.dat, *.txt, *.csv)|*.dat;*.txt;*.csv|All files|*.*";
            this.openFileDialogLoadDGWeb.InitialDirectory = "\\\\atr-lsb-lss01\\DocuGenie\\DGProjects";
            this.openFileDialogLoadDGWeb.Multiselect = true;
            this.openFileDialogLoadDGWeb.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialogLoadDGWeb_FileOk);
            // 
            // statusStripLoadDGWeb
            // 
            this.statusStripLoadDGWeb.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStripLoadDGWeb.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelImportBatch});
            this.statusStripLoadDGWeb.Location = new System.Drawing.Point(0, 363);
            this.statusStripLoadDGWeb.Name = "statusStripLoadDGWeb";
            this.statusStripLoadDGWeb.Size = new System.Drawing.Size(709, 22);
            this.statusStripLoadDGWeb.TabIndex = 11;
            // 
            // toolStripStatusLabelImportBatch
            // 
            this.toolStripStatusLabelImportBatch.Name = "toolStripStatusLabelImportBatch";
            this.toolStripStatusLabelImportBatch.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabelImportBatch.Text = "Ready";
            // 
            // labelBatch
            // 
            this.labelBatch.AutoSize = true;
            this.labelBatch.Location = new System.Drawing.Point(12, 120);
            this.labelBatch.Name = "labelBatch";
            this.labelBatch.Size = new System.Drawing.Size(38, 13);
            this.labelBatch.TabIndex = 15;
            this.labelBatch.Text = "Batch:";
            // 
            // textBoxBatch
            // 
            this.textBoxBatch.Location = new System.Drawing.Point(92, 116);
            this.textBoxBatch.Name = "textBoxBatch";
            this.textBoxBatch.Size = new System.Drawing.Size(158, 21);
            this.textBoxBatch.TabIndex = 4;
            // 
            // groupBoxFieldMapping
            // 
            this.groupBoxFieldMapping.Location = new System.Drawing.Point(15, 277);
            this.groupBoxFieldMapping.Name = "groupBoxFieldMapping";
            this.groupBoxFieldMapping.Size = new System.Drawing.Size(674, 75);
            this.groupBoxFieldMapping.TabIndex = 12;
            this.groupBoxFieldMapping.TabStop = false;
            this.groupBoxFieldMapping.Text = "Field Mapping";
            // 
            // buttonBrowseTemplateFile
            // 
            this.buttonBrowseTemplateFile.Location = new System.Drawing.Point(496, 83);
            this.buttonBrowseTemplateFile.Name = "buttonBrowseTemplateFile";
            this.buttonBrowseTemplateFile.Size = new System.Drawing.Size(63, 26);
            this.buttonBrowseTemplateFile.TabIndex = 3;
            this.buttonBrowseTemplateFile.Text = "Browse";
            this.buttonBrowseTemplateFile.UseVisualStyleBackColor = true;
            this.buttonBrowseTemplateFile.Click += new System.EventHandler(this.buttonBrowseTemplateFile_Click);
            // 
            // labelTemplateFile
            // 
            this.labelTemplateFile.AutoSize = true;
            this.labelTemplateFile.Location = new System.Drawing.Point(12, 90);
            this.labelTemplateFile.Name = "labelTemplateFile";
            this.labelTemplateFile.Size = new System.Drawing.Size(72, 13);
            this.labelTemplateFile.TabIndex = 28;
            this.labelTemplateFile.Text = "Template file:";
            // 
            // textBoxTemplateFile
            // 
            this.textBoxTemplateFile.Location = new System.Drawing.Point(92, 87);
            this.textBoxTemplateFile.Name = "textBoxTemplateFile";
            this.textBoxTemplateFile.Size = new System.Drawing.Size(398, 21);
            this.textBoxTemplateFile.TabIndex = 2;
            // 
            // labelFieldDelim
            // 
            this.labelFieldDelim.AutoSize = true;
            this.labelFieldDelim.Location = new System.Drawing.Point(329, 119);
            this.labelFieldDelim.Name = "labelFieldDelim";
            this.labelFieldDelim.Size = new System.Drawing.Size(60, 13);
            this.labelFieldDelim.TabIndex = 30;
            this.labelFieldDelim.Text = "Field delim:";
            // 
            // labelQuoteDelim
            // 
            this.labelQuoteDelim.AutoSize = true;
            this.labelQuoteDelim.Location = new System.Drawing.Point(444, 120);
            this.labelQuoteDelim.Name = "labelQuoteDelim";
            this.labelQuoteDelim.Size = new System.Drawing.Size(68, 13);
            this.labelQuoteDelim.TabIndex = 32;
            this.labelQuoteDelim.Text = "Quote delim:";
            // 
            // groupBoxLoadFileType
            // 
            this.groupBoxLoadFileType.Controls.Add(this.radioButtonBankStatement);
            this.groupBoxLoadFileType.Controls.Add(this.radioButtonPhoneBill);
            this.groupBoxLoadFileType.Location = new System.Drawing.Point(15, 2);
            this.groupBoxLoadFileType.Name = "groupBoxLoadFileType";
            this.groupBoxLoadFileType.Size = new System.Drawing.Size(212, 41);
            this.groupBoxLoadFileType.TabIndex = 34;
            this.groupBoxLoadFileType.TabStop = false;
            // 
            // radioButtonBankStatement
            // 
            this.radioButtonBankStatement.AutoSize = true;
            this.radioButtonBankStatement.Enabled = false;
            this.radioButtonBankStatement.Location = new System.Drawing.Point(103, 15);
            this.radioButtonBankStatement.Name = "radioButtonBankStatement";
            this.radioButtonBankStatement.Size = new System.Drawing.Size(100, 17);
            this.radioButtonBankStatement.TabIndex = 1;
            this.radioButtonBankStatement.Text = "Bank statement";
            this.radioButtonBankStatement.UseVisualStyleBackColor = true;
            this.radioButtonBankStatement.CheckedChanged += new System.EventHandler(this.radioButtonLoadFileType_CheckedChanged);
            // 
            // radioButtonPhoneBill
            // 
            this.radioButtonPhoneBill.AutoSize = true;
            this.radioButtonPhoneBill.Checked = true;
            this.radioButtonPhoneBill.Location = new System.Drawing.Point(10, 15);
            this.radioButtonPhoneBill.Name = "radioButtonPhoneBill";
            this.radioButtonPhoneBill.Size = new System.Drawing.Size(70, 17);
            this.radioButtonPhoneBill.TabIndex = 0;
            this.radioButtonPhoneBill.TabStop = true;
            this.radioButtonPhoneBill.Text = "Phone bill";
            this.radioButtonPhoneBill.UseVisualStyleBackColor = true;
            this.radioButtonPhoneBill.CheckedChanged += new System.EventHandler(this.radioButtonLoadFileType_CheckedChanged);
            // 
            // textBoxVolume_Info
            // 
            this.textBoxVolume_Info.Location = new System.Drawing.Point(92, 146);
            this.textBoxVolume_Info.Name = "textBoxVolume_Info";
            this.textBoxVolume_Info.Size = new System.Drawing.Size(135, 21);
            this.textBoxVolume_Info.TabIndex = 7;
            // 
            // labelVolume_Info
            // 
            this.labelVolume_Info.AutoSize = true;
            this.labelVolume_Info.Location = new System.Drawing.Point(12, 149);
            this.labelVolume_Info.Name = "labelVolume_Info";
            this.labelVolume_Info.Size = new System.Drawing.Size(71, 13);
            this.labelVolume_Info.TabIndex = 35;
            this.labelVolume_Info.Text = "Volume_Info:";
            // 
            // textBoxATRLoadInfo
            // 
            this.textBoxATRLoadInfo.Location = new System.Drawing.Point(326, 146);
            this.textBoxATRLoadInfo.Name = "textBoxATRLoadInfo";
            this.textBoxATRLoadInfo.Size = new System.Drawing.Size(208, 21);
            this.textBoxATRLoadInfo.TabIndex = 8;
            // 
            // labelATRLoadInfo
            // 
            this.labelATRLoadInfo.AutoSize = true;
            this.labelATRLoadInfo.Location = new System.Drawing.Point(238, 149);
            this.labelATRLoadInfo.Name = "labelATRLoadInfo";
            this.labelATRLoadInfo.Size = new System.Drawing.Size(86, 13);
            this.labelATRLoadInfo.TabIndex = 37;
            this.labelATRLoadInfo.Text = "ATR_Load_Info:";
            // 
            // textBoxCustodian
            // 
            this.textBoxCustodian.Location = new System.Drawing.Point(326, 177);
            this.textBoxCustodian.Name = "textBoxCustodian";
            this.textBoxCustodian.Size = new System.Drawing.Size(208, 21);
            this.textBoxCustodian.TabIndex = 10;
            // 
            // labelCustodian
            // 
            this.labelCustodian.AutoSize = true;
            this.labelCustodian.Location = new System.Drawing.Point(264, 180);
            this.labelCustodian.Name = "labelCustodian";
            this.labelCustodian.Size = new System.Drawing.Size(59, 13);
            this.labelCustodian.TabIndex = 41;
            this.labelCustodian.Text = "Custodian:";
            // 
            // textBoxCompany
            // 
            this.textBoxCompany.Location = new System.Drawing.Point(92, 177);
            this.textBoxCompany.Name = "textBoxCompany";
            this.textBoxCompany.Size = new System.Drawing.Size(135, 21);
            this.textBoxCompany.TabIndex = 9;
            // 
            // labelCompany
            // 
            this.labelCompany.AutoSize = true;
            this.labelCompany.Location = new System.Drawing.Point(12, 180);
            this.labelCompany.Name = "labelCompany";
            this.labelCompany.Size = new System.Drawing.Size(56, 13);
            this.labelCompany.TabIndex = 39;
            this.labelCompany.Text = "Company:";
            // 
            // textBoxCmtsIDWithPrefix
            // 
            this.textBoxCmtsIDWithPrefix.Location = new System.Drawing.Point(92, 208);
            this.textBoxCmtsIDWithPrefix.Name = "textBoxCmtsIDWithPrefix";
            this.textBoxCmtsIDWithPrefix.Size = new System.Drawing.Size(79, 21);
            this.textBoxCmtsIDWithPrefix.TabIndex = 11;
            // 
            // labelCMTSID
            // 
            this.labelCMTSID.AutoSize = true;
            this.labelCMTSID.Location = new System.Drawing.Point(12, 211);
            this.labelCMTSID.Name = "labelCMTSID";
            this.labelCMTSID.Size = new System.Drawing.Size(52, 13);
            this.labelCMTSID.TabIndex = 43;
            this.labelCMTSID.Text = "CMTS ID:";
            // 
            // comboBoxFieldDelim
            // 
            this.comboBoxFieldDelim.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFieldDelim.FormattingEnabled = true;
            this.comboBoxFieldDelim.Items.AddRange(new object[] {
            ","});
            this.comboBoxFieldDelim.Location = new System.Drawing.Point(395, 116);
            this.comboBoxFieldDelim.Name = "comboBoxFieldDelim";
            this.comboBoxFieldDelim.Size = new System.Drawing.Size(44, 21);
            this.comboBoxFieldDelim.TabIndex = 44;
            // 
            // comboBoxQuoteDelim
            // 
            this.comboBoxQuoteDelim.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxQuoteDelim.FormattingEnabled = true;
            this.comboBoxQuoteDelim.Items.AddRange(new object[] {
            "",
            "\""});
            this.comboBoxQuoteDelim.Location = new System.Drawing.Point(515, 115);
            this.comboBoxQuoteDelim.Name = "comboBoxQuoteDelim";
            this.comboBoxQuoteDelim.Size = new System.Drawing.Size(44, 21);
            this.comboBoxQuoteDelim.TabIndex = 45;
            // 
            // buttonMapFields
            // 
            this.buttonMapFields.Location = new System.Drawing.Point(565, 157);
            this.buttonMapFields.Name = "buttonMapFields";
            this.buttonMapFields.Size = new System.Drawing.Size(124, 26);
            this.buttonMapFields.TabIndex = 18;
            this.buttonMapFields.Text = "Reset Mapped Fields";
            this.buttonMapFields.UseVisualStyleBackColor = true;
            this.buttonMapFields.Click += new System.EventHandler(this.buttonMapFields_Click);
            // 
            // buttonSaveTemplate
            // 
            this.buttonSaveTemplate.Location = new System.Drawing.Point(565, 185);
            this.buttonSaveTemplate.Name = "buttonSaveTemplate";
            this.buttonSaveTemplate.Size = new System.Drawing.Size(124, 26);
            this.buttonSaveTemplate.TabIndex = 19;
            this.buttonSaveTemplate.Text = "Save Template";
            this.buttonSaveTemplate.UseVisualStyleBackColor = true;
            this.buttonSaveTemplate.Click += new System.EventHandler(this.buttonSaveTemplate_Click);
            // 
            // saveFileDialogTemplate
            // 
            this.saveFileDialogTemplate.DefaultExt = "docx";
            this.saveFileDialogTemplate.Filter = "Template file (*.template)|*.template";
            this.saveFileDialogTemplate.InitialDirectory = "\\\\atr-lsb-lss01\\DocuGenie\\DGProjects\\ImportTemplates";
            this.saveFileDialogTemplate.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialogTemplate_FileOk);
            // 
            // openFileDialogTemplate
            // 
            this.openFileDialogTemplate.DefaultExt = "template";
            this.openFileDialogTemplate.Filter = "Template files (*.template)|*.template";
            this.openFileDialogTemplate.InitialDirectory = "\\\\atr-lsb-lss01\\DocuGenie\\DGProjects\\ImportTemplates";
            this.openFileDialogTemplate.RestoreDirectory = true;
            this.openFileDialogTemplate.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialogTemplate_FileOk);
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(250, 208);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(120, 21);
            this.textBoxDescription.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(182, 211);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 47;
            this.label1.Text = "Description:";
            // 
            // buttonPreviewInputFile
            // 
            this.buttonPreviewInputFile.Location = new System.Drawing.Point(565, 129);
            this.buttonPreviewInputFile.Name = "buttonPreviewInputFile";
            this.buttonPreviewInputFile.Size = new System.Drawing.Size(124, 26);
            this.buttonPreviewInputFile.TabIndex = 17;
            this.buttonPreviewInputFile.Text = "Preview Input File";
            this.buttonPreviewInputFile.UseVisualStyleBackColor = true;
            this.buttonPreviewInputFile.Click += new System.EventHandler(this.buttonPreviewInputFile_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 244);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 49;
            this.label2.Text = "Begdoc#:";
            // 
            // textBoxBegdoc
            // 
            this.textBoxBegdoc.Location = new System.Drawing.Point(92, 239);
            this.textBoxBegdoc.Name = "textBoxBegdoc";
            this.textBoxBegdoc.Size = new System.Drawing.Size(135, 21);
            this.textBoxBegdoc.TabIndex = 13;
            // 
            // buttonPrint
            // 
            this.buttonPrint.Location = new System.Drawing.Point(565, 213);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(124, 26);
            this.buttonPrint.TabIndex = 20;
            this.buttonPrint.Text = "Print Template";
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(238, 243);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 51;
            this.label3.Text = "Source file:";
            // 
            // textBoxSrcfile
            // 
            this.textBoxSrcfile.Location = new System.Drawing.Point(305, 239);
            this.textBoxSrcfile.Name = "textBoxSrcfile";
            this.textBoxSrcfile.Size = new System.Drawing.Size(229, 21);
            this.textBoxSrcfile.TabIndex = 14;
            // 
            // printDialog
            // 
            this.printDialog.UseEXDialog = true;
            // 
            // buttonCreateSample
            // 
            this.buttonCreateSample.Location = new System.Drawing.Point(565, 241);
            this.buttonCreateSample.Name = "buttonCreateSample";
            this.buttonCreateSample.Size = new System.Drawing.Size(124, 26);
            this.buttonCreateSample.TabIndex = 52;
            this.buttonCreateSample.Text = "Create Sample";
            this.buttonCreateSample.UseVisualStyleBackColor = true;
            this.buttonCreateSample.Click += new System.EventHandler(this.buttonCreateSample_Click);
            // 
            // saveFileDialogSample
            // 
            this.saveFileDialogSample.DefaultExt = "csv";
            this.saveFileDialogSample.Filter = "CSV file (*.csv)|*.csv";
            this.saveFileDialogSample.InitialDirectory = "\\\\atr-lsb-lss01\\DocuGenie\\DGProjects\\SampleTemplates";
            this.saveFileDialogSample.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialogSample_FileOk);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(376, 211);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 53;
            this.label4.Text = "Target:";
            // 
            // textBoxTargetNumber
            // 
            this.textBoxTargetNumber.Location = new System.Drawing.Point(422, 208);
            this.textBoxTargetNumber.Name = "textBoxTargetNumber";
            this.textBoxTargetNumber.Size = new System.Drawing.Size(112, 21);
            this.textBoxTargetNumber.TabIndex = 54;
            // 
            // buttonSelectBatch
            // 
            this.buttonSelectBatch.Location = new System.Drawing.Point(256, 113);
            this.buttonSelectBatch.Name = "buttonSelectBatch";
            this.buttonSelectBatch.Size = new System.Drawing.Size(63, 26);
            this.buttonSelectBatch.TabIndex = 55;
            this.buttonSelectBatch.Text = "Browse";
            this.buttonSelectBatch.UseVisualStyleBackColor = true;
            this.buttonSelectBatch.Click += new System.EventHandler(this.buttonSelectBatch_Click);
            // 
            // ImportBatch
            // 
            this.AcceptButton = this.buttonLoad;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(709, 385);
            this.Controls.Add(this.buttonSelectBatch);
            this.Controls.Add(this.textBoxTargetNumber);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonCreateSample);
            this.Controls.Add(this.textBoxSrcfile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonPrint);
            this.Controls.Add(this.textBoxBegdoc);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonPreviewInputFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.buttonSaveTemplate);
            this.Controls.Add(this.buttonMapFields);
            this.Controls.Add(this.comboBoxQuoteDelim);
            this.Controls.Add(this.comboBoxFieldDelim);
            this.Controls.Add(this.textBoxCmtsIDWithPrefix);
            this.Controls.Add(this.labelCMTSID);
            this.Controls.Add(this.textBoxCustodian);
            this.Controls.Add(this.labelCustodian);
            this.Controls.Add(this.textBoxCompany);
            this.Controls.Add(this.labelCompany);
            this.Controls.Add(this.textBoxATRLoadInfo);
            this.Controls.Add(this.labelATRLoadInfo);
            this.Controls.Add(this.textBoxVolume_Info);
            this.Controls.Add(this.labelVolume_Info);
            this.Controls.Add(this.groupBoxLoadFileType);
            this.Controls.Add(this.labelQuoteDelim);
            this.Controls.Add(this.labelFieldDelim);
            this.Controls.Add(this.buttonBrowseTemplateFile);
            this.Controls.Add(this.labelTemplateFile);
            this.Controls.Add(this.textBoxTemplateFile);
            this.Controls.Add(this.groupBoxFieldMapping);
            this.Controls.Add(this.labelBatch);
            this.Controls.Add(this.textBoxBatch);
            this.Controls.Add(this.statusStripLoadDGWeb);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.labelLoadFile);
            this.Controls.Add(this.textBoxLoadFile);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportBatch";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import Batch";
            this.Resize += new System.EventHandler(this.ImportBatch_Resize);
            this.statusStripLoadDGWeb.ResumeLayout(false);
            this.statusStripLoadDGWeb.PerformLayout();
            this.groupBoxLoadFileType.ResumeLayout(false);
            this.groupBoxLoadFileType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxLoadFile;
        private System.Windows.Forms.Label labelLoadFile;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialogLoadDGWeb;
        private System.Windows.Forms.StatusStrip statusStripLoadDGWeb;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelImportBatch;
        private System.Windows.Forms.Label labelBatch;
        private System.Windows.Forms.TextBox textBoxBatch;
        private System.Windows.Forms.GroupBox groupBoxFieldMapping;
        private System.Windows.Forms.Button buttonBrowseTemplateFile;
        private System.Windows.Forms.Label labelTemplateFile;
        private System.Windows.Forms.TextBox textBoxTemplateFile;
        private System.Windows.Forms.Label labelFieldDelim;
        private System.Windows.Forms.Label labelQuoteDelim;
        private System.Windows.Forms.GroupBox groupBoxLoadFileType;
        private System.Windows.Forms.RadioButton radioButtonBankStatement;
        private System.Windows.Forms.RadioButton radioButtonPhoneBill;
        private System.Windows.Forms.TextBox textBoxVolume_Info;
        private System.Windows.Forms.Label labelVolume_Info;
        private System.Windows.Forms.TextBox textBoxATRLoadInfo;
        private System.Windows.Forms.Label labelATRLoadInfo;
        private System.Windows.Forms.TextBox textBoxCustodian;
        private System.Windows.Forms.Label labelCustodian;
        private System.Windows.Forms.TextBox textBoxCompany;
        private System.Windows.Forms.Label labelCompany;
        private System.Windows.Forms.TextBox textBoxCmtsIDWithPrefix;
        private System.Windows.Forms.Label labelCMTSID;
        private System.Windows.Forms.ComboBox comboBoxFieldDelim;
        private System.Windows.Forms.ComboBox comboBoxQuoteDelim;
        private System.Windows.Forms.Button buttonMapFields;
        private System.Windows.Forms.Button buttonSaveTemplate;
        private System.Windows.Forms.SaveFileDialog saveFileDialogTemplate;
        private System.Windows.Forms.OpenFileDialog openFileDialogTemplate;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonPreviewInputFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxBegdoc;
        private System.Windows.Forms.Button buttonPrint;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSrcfile;
        private System.Windows.Forms.PrintDialog printDialog;
        private System.Windows.Forms.Button buttonCreateSample;
        private System.Windows.Forms.SaveFileDialog saveFileDialogSample;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxTargetNumber;
        private System.Windows.Forms.Button buttonSelectBatch;
    }
}