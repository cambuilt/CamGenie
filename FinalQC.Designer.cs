namespace CamGenie
{
    partial class FinalQC
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
            this.dataGridViewFinalQC = new System.Windows.Forms.DataGridView();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.buttonUpload = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxPosition = new System.Windows.Forms.ComboBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonReplace = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxWhereValue = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.comboBoxWhereComparison = new System.Windows.Forms.ComboBox();
            this.labelFieldName = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxLength = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxReplacementValue = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxReplacementTarget = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxGlobalReplaceField = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelPositionSuffix = new System.Windows.Forms.Label();
            this.buttonSetFilter = new System.Windows.Forms.Button();
            this.buttonRemoveFilter = new System.Windows.Forms.Button();
            this.buttonInsertRow = new System.Windows.Forms.Button();
            this.buttonDeleteRow = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFinalQC)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewFinalQC
            // 
            this.dataGridViewFinalQC.AllowUserToAddRows = false;
            this.dataGridViewFinalQC.AllowUserToDeleteRows = false;
            this.dataGridViewFinalQC.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFinalQC.Location = new System.Drawing.Point(3, 13);
            this.dataGridViewFinalQC.Name = "dataGridViewFinalQC";
            this.dataGridViewFinalQC.RowHeadersWidth = 30;
            this.dataGridViewFinalQC.Size = new System.Drawing.Size(1087, 666);
            this.dataGridViewFinalQC.TabIndex = 0;
            this.dataGridViewFinalQC.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewFinalQC_CellEndEdit);
            this.dataGridViewFinalQC.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewFinalQC_CellEnter);
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 690);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1397, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // buttonUpload
            // 
            this.buttonUpload.Location = new System.Drawing.Point(1181, 641);
            this.buttonUpload.Name = "buttonUpload";
            this.buttonUpload.Size = new System.Drawing.Size(122, 38);
            this.buttonUpload.TabIndex = 2;
            this.buttonUpload.Text = "Upload to DG Web";
            this.buttonUpload.UseVisualStyleBackColor = true;
            this.buttonUpload.Click += new System.EventHandler(this.buttonUpload_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxPosition);
            this.groupBox1.Controls.Add(this.buttonClear);
            this.groupBox1.Controls.Add(this.buttonReplace);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.textBoxWhereValue);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.comboBoxWhereComparison);
            this.groupBox1.Controls.Add(this.labelFieldName);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.textBoxLength);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBoxReplacementValue);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBoxReplacementTarget);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.comboBoxGlobalReplaceField);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.labelPositionSuffix);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(1118, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(252, 314);
            this.groupBox1.TabIndex = 75;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Global Replace";
            // 
            // comboBoxPosition
            // 
            this.comboBoxPosition.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxPosition.FormattingEnabled = true;
            this.comboBoxPosition.Items.AddRange(new object[] {
            "end"});
            this.comboBoxPosition.Location = new System.Drawing.Point(65, 132);
            this.comboBoxPosition.Name = "comboBoxPosition";
            this.comboBoxPosition.Size = new System.Drawing.Size(74, 40);
            this.comboBoxPosition.TabIndex = 7;
            this.comboBoxPosition.TextChanged += new System.EventHandler(this.comboBoxPosition_TextChanged);
            // 
            // buttonClear
            // 
            this.buttonClear.Enabled = false;
            this.buttonClear.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClear.Location = new System.Drawing.Point(141, 270);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(84, 29);
            this.buttonClear.TabIndex = 12;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // buttonReplace
            // 
            this.buttonReplace.Enabled = false;
            this.buttonReplace.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonReplace.Location = new System.Drawing.Point(19, 270);
            this.buttonReplace.Name = "buttonReplace";
            this.buttonReplace.Size = new System.Drawing.Size(84, 29);
            this.buttonReplace.TabIndex = 11;
            this.buttonReplace.Text = "Replace";
            this.buttonReplace.UseVisualStyleBackColor = true;
            this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(208, 234);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(70, 32);
            this.label14.TabIndex = 80;
            this.label14.Text = "(opt.)";
            // 
            // textBoxWhereValue
            // 
            this.textBoxWhereValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxWhereValue.Location = new System.Drawing.Point(74, 230);
            this.textBoxWhereValue.Name = "textBoxWhereValue";
            this.textBoxWhereValue.Size = new System.Drawing.Size(121, 39);
            this.textBoxWhereValue.TabIndex = 10;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(16, 234);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(114, 32);
            this.label12.TabIndex = 77;
            this.label12.Text = "the value";
            // 
            // comboBoxWhereComparison
            // 
            this.comboBoxWhereComparison.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWhereComparison.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxWhereComparison.FormattingEnabled = true;
            this.comboBoxWhereComparison.Items.AddRange(new object[] {
            "equals",
            "starts with",
            "ends with",
            "contains"});
            this.comboBoxWhereComparison.Location = new System.Drawing.Point(140, 198);
            this.comboBoxWhereComparison.Name = "comboBoxWhereComparison";
            this.comboBoxWhereComparison.Size = new System.Drawing.Size(90, 40);
            this.comboBoxWhereComparison.TabIndex = 9;
            // 
            // labelFieldName
            // 
            this.labelFieldName.AutoSize = true;
            this.labelFieldName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFieldName.Location = new System.Drawing.Point(52, 203);
            this.labelFieldName.Name = "labelFieldName";
            this.labelFieldName.Size = new System.Drawing.Size(76, 32);
            this.labelFieldName.TabIndex = 76;
            this.labelFieldName.Text = "[field]";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(14, 203);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 32);
            this.label10.TabIndex = 75;
            this.label10.Text = "where";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(208, 170);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 32);
            this.label9.TabIndex = 14;
            this.label9.Text = "(opt.)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(208, 135);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 32);
            this.label8.TabIndex = 13;
            this.label8.Text = "(opt.)";
            // 
            // textBoxLength
            // 
            this.textBoxLength.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxLength.Location = new System.Drawing.Point(169, 167);
            this.textBoxLength.Name = "textBoxLength";
            this.textBoxLength.Size = new System.Drawing.Size(26, 39);
            this.textBoxLength.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(14, 170);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(309, 32);
            this.label7.TabIndex = 11;
            this.label7.Text = "with a field value length of ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(142, 135);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(115, 32);
            this.label6.TabIndex = 9;
            this.label6.Text = "char. pos.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(14, 135);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 32);
            this.label5.TabIndex = 7;
            this.label5.Text = "at the";
            // 
            // textBoxReplacementValue
            // 
            this.textBoxReplacementValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxReplacementValue.Location = new System.Drawing.Point(65, 101);
            this.textBoxReplacementValue.Name = "textBoxReplacementValue";
            this.textBoxReplacementValue.Size = new System.Drawing.Size(130, 39);
            this.textBoxReplacementValue.TabIndex = 6;
            this.textBoxReplacementValue.TextChanged += new System.EventHandler(this.textBoxReplacementValue_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(14, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 32);
            this.label4.TabIndex = 5;
            this.label4.Text = "with";
            // 
            // textBoxReplacementTarget
            // 
            this.textBoxReplacementTarget.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxReplacementTarget.Location = new System.Drawing.Point(65, 71);
            this.textBoxReplacementTarget.Name = "textBoxReplacementTarget";
            this.textBoxReplacementTarget.Size = new System.Drawing.Size(130, 39);
            this.textBoxReplacementTarget.TabIndex = 4;
            this.textBoxReplacementTarget.TextChanged += new System.EventHandler(this.textBoxReplacementTarget_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 32);
            this.label3.TabIndex = 3;
            this.label3.Text = "replace ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(165, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 32);
            this.label2.TabIndex = 2;
            this.label2.Text = ",";
            // 
            // comboBoxGlobalReplaceField
            // 
            this.comboBoxGlobalReplaceField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGlobalReplaceField.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxGlobalReplaceField.FormattingEnabled = true;
            this.comboBoxGlobalReplaceField.Location = new System.Drawing.Point(44, 31);
            this.comboBoxGlobalReplaceField.Name = "comboBoxGlobalReplaceField";
            this.comboBoxGlobalReplaceField.Size = new System.Drawing.Size(121, 40);
            this.comboBoxGlobalReplaceField.TabIndex = 1;
            this.comboBoxGlobalReplaceField.SelectedIndexChanged += new System.EventHandler(this.comboBoxGlobalReplaceField_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "For";
            // 
            // labelPositionSuffix
            // 
            this.labelPositionSuffix.AutoSize = true;
            this.labelPositionSuffix.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPositionSuffix.Location = new System.Drawing.Point(86, 135);
            this.labelPositionSuffix.Name = "labelPositionSuffix";
            this.labelPositionSuffix.Size = new System.Drawing.Size(29, 32);
            this.labelPositionSuffix.TabIndex = 10;
            this.labelPositionSuffix.Text = "  ";
            // 
            // buttonSetFilter
            // 
            this.buttonSetFilter.Location = new System.Drawing.Point(1137, 347);
            this.buttonSetFilter.Name = "buttonSetFilter";
            this.buttonSetFilter.Size = new System.Drawing.Size(80, 30);
            this.buttonSetFilter.TabIndex = 76;
            this.buttonSetFilter.Text = "Set Filter";
            this.buttonSetFilter.UseVisualStyleBackColor = true;
            // 
            // buttonRemoveFilter
            // 
            this.buttonRemoveFilter.Location = new System.Drawing.Point(1240, 347);
            this.buttonRemoveFilter.Name = "buttonRemoveFilter";
            this.buttonRemoveFilter.Size = new System.Drawing.Size(110, 30);
            this.buttonRemoveFilter.TabIndex = 77;
            this.buttonRemoveFilter.Text = "Remove Filter";
            this.buttonRemoveFilter.UseVisualStyleBackColor = true;
            // 
            // buttonInsertRow
            // 
            this.buttonInsertRow.Location = new System.Drawing.Point(1135, 400);
            this.buttonInsertRow.Name = "buttonInsertRow";
            this.buttonInsertRow.Size = new System.Drawing.Size(122, 30);
            this.buttonInsertRow.TabIndex = 78;
            this.buttonInsertRow.Text = "Insert Row";
            this.buttonInsertRow.UseVisualStyleBackColor = true;
            this.buttonInsertRow.Click += new System.EventHandler(this.buttonInsertRow_Click);
            // 
            // buttonDeleteRow
            // 
            this.buttonDeleteRow.Location = new System.Drawing.Point(1135, 436);
            this.buttonDeleteRow.Name = "buttonDeleteRow";
            this.buttonDeleteRow.Size = new System.Drawing.Size(122, 30);
            this.buttonDeleteRow.TabIndex = 79;
            this.buttonDeleteRow.Text = "Delete Row";
            this.buttonDeleteRow.UseVisualStyleBackColor = true;
            this.buttonDeleteRow.Click += new System.EventHandler(this.buttonDeleteRow_Click);
            // 
            // FinalQC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1397, 712);
            this.Controls.Add(this.buttonDeleteRow);
            this.Controls.Add(this.buttonInsertRow);
            this.Controls.Add(this.buttonRemoveFilter);
            this.Controls.Add(this.buttonSetFilter);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonUpload);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.dataGridViewFinalQC);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FinalQC";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Final QC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FinalQC_FormClosing);
            this.Load += new System.EventHandler(this.FinalQC_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFinalQC)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewFinalQC;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.Button buttonUpload;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxPosition;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button buttonReplace;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxWhereValue;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox comboBoxWhereComparison;
        private System.Windows.Forms.Label labelFieldName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxLength;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxReplacementValue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxReplacementTarget;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxGlobalReplaceField;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelPositionSuffix;
        private System.Windows.Forms.Button buttonSetFilter;
        private System.Windows.Forms.Button buttonRemoveFilter;
        private System.Windows.Forms.Button buttonInsertRow;
        private System.Windows.Forms.Button buttonDeleteRow;
    }
}