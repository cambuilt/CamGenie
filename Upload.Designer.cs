namespace CamGenie
{
    partial class Upload
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
            this.components = new System.ComponentModel.Container();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxCustodian = new System.Windows.Forms.TextBox();
            this.textBoxCompany = new System.Windows.Forms.TextBox();
            this.textBoxCMTSID = new System.Windows.Forms.TextBox();
            this.textBoxATR_Load_Info = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxVolInfo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxTimeZone = new System.Windows.Forms.ComboBox();
            this.labelTimeZone = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(142, 303);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(148, 40);
            this.buttonOK.TabIndex = 20;
            this.buttonOK.Text = "Upload to Final QC";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxCustodian
            // 
            this.textBoxCustodian.Location = new System.Drawing.Point(124, 24);
            this.textBoxCustodian.Name = "textBoxCustodian";
            this.textBoxCustodian.Size = new System.Drawing.Size(146, 39);
            this.textBoxCustodian.TabIndex = 10;
            // 
            // textBoxCompany
            // 
            this.textBoxCompany.Location = new System.Drawing.Point(124, 53);
            this.textBoxCompany.Name = "textBoxCompany";
            this.textBoxCompany.Size = new System.Drawing.Size(146, 39);
            this.textBoxCompany.TabIndex = 12;
            // 
            // textBoxCMTSID
            // 
            this.textBoxCMTSID.Location = new System.Drawing.Point(124, 82);
            this.textBoxCMTSID.Name = "textBoxCMTSID";
            this.textBoxCMTSID.Size = new System.Drawing.Size(96, 39);
            this.textBoxCMTSID.TabIndex = 14;
            // 
            // textBoxATR_Load_Info
            // 
            this.textBoxATR_Load_Info.Location = new System.Drawing.Point(124, 111);
            this.textBoxATR_Load_Info.Name = "textBoxATR_Load_Info";
            this.textBoxATR_Load_Info.Size = new System.Drawing.Size(146, 39);
            this.textBoxATR_Load_Info.TabIndex = 16;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxVolInfo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBoxTimeZone);
            this.groupBox1.Controls.Add(this.labelTimeZone);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.textBoxATR_Load_Info);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBoxCMTSID);
            this.groupBox1.Controls.Add(this.textBoxCustodian);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBoxCompany);
            this.groupBox1.Location = new System.Drawing.Point(51, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(330, 223);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Batch Header";
            // 
            // textBoxVolInfo
            // 
            this.textBoxVolInfo.Location = new System.Drawing.Point(124, 170);
            this.textBoxVolInfo.Name = "textBoxVolInfo";
            this.textBoxVolInfo.Size = new System.Drawing.Size(146, 39);
            this.textBoxVolInfo.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 174);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 32);
            this.label1.TabIndex = 21;
            this.label1.Text = "Vol Info";
            // 
            // comboBoxTimeZone
            // 
            this.comboBoxTimeZone.FormattingEnabled = true;
            this.comboBoxTimeZone.Items.AddRange(new object[] {
            "UTC",
            "LOS",
            "EST",
            "CST",
            "MST",
            "PST"});
            this.comboBoxTimeZone.Location = new System.Drawing.Point(124, 140);
            this.comboBoxTimeZone.Name = "comboBoxTimeZone";
            this.comboBoxTimeZone.Size = new System.Drawing.Size(73, 40);
            this.comboBoxTimeZone.TabIndex = 20;
            // 
            // labelTimeZone
            // 
            this.labelTimeZone.AutoSize = true;
            this.labelTimeZone.Location = new System.Drawing.Point(36, 143);
            this.labelTimeZone.Name = "labelTimeZone";
            this.labelTimeZone.Size = new System.Drawing.Size(130, 32);
            this.labelTimeZone.TabIndex = 19;
            this.labelTimeZone.Text = "Time Zone";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(36, 114);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(168, 32);
            this.label8.TabIndex = 17;
            this.label8.Text = "ATR_Load_Info";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(36, 85);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(108, 32);
            this.label7.TabIndex = 15;
            this.label7.Text = "CMTS ID";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(36, 56);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(117, 32);
            this.label6.TabIndex = 13;
            this.label6.Text = "Company";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(122, 32);
            this.label5.TabIndex = 11;
            this.label5.Text = "Custodian";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(45, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(190, 32);
            this.label4.TabIndex = 24;
            this.label4.Text = "QC is complete. ";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(145, 26);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(471, 32);
            this.label15.TabIndex = 25;
            this.label15.Text = "Enter the known batch information below. ";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 342);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(432, 37);
            this.statusStrip1.TabIndex = 29;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(79, 32);
            this.toolStripStatusLabel.Text = "Ready";
            // 
            // Upload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 379);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonOK);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Upload";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Upload Settings";
            this.Load += new System.EventHandler(this.Upload_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxCustodian;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxCompany;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxCMTSID;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxATR_Load_Info;
        private System.Windows.Forms.Label labelTimeZone;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxTimeZone;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.TextBox textBoxVolInfo;
        private System.Windows.Forms.Label label1;
    }
}