namespace CamGenie
{
    partial class LineRemove
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
            this.checkBoxRemoveHorizontalLines = new System.Windows.Forms.CheckBox();
            this.checkBoxRemoveVerticalLines = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxMinimumLength = new System.Windows.Forms.TextBox();
            this.textBoxMaximumWidth = new System.Windows.Forms.TextBox();
            this.textBoxMaximumGap = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBoxRemoveHorizontalLines
            // 
            this.checkBoxRemoveHorizontalLines.AutoSize = true;
            this.checkBoxRemoveHorizontalLines.Checked = true;
            this.checkBoxRemoveHorizontalLines.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRemoveHorizontalLines.Location = new System.Drawing.Point(40, 19);
            this.checkBoxRemoveHorizontalLines.Name = "checkBoxRemoveHorizontalLines";
            this.checkBoxRemoveHorizontalLines.Size = new System.Drawing.Size(311, 36);
            this.checkBoxRemoveHorizontalLines.TabIndex = 0;
            this.checkBoxRemoveHorizontalLines.Text = "Remove Horizontal Lines";
            this.checkBoxRemoveHorizontalLines.UseVisualStyleBackColor = true;
            // 
            // checkBoxRemoveVerticalLines
            // 
            this.checkBoxRemoveVerticalLines.AutoSize = true;
            this.checkBoxRemoveVerticalLines.Location = new System.Drawing.Point(40, 45);
            this.checkBoxRemoveVerticalLines.Name = "checkBoxRemoveVerticalLines";
            this.checkBoxRemoveVerticalLines.Size = new System.Drawing.Size(278, 36);
            this.checkBoxRemoveVerticalLines.TabIndex = 1;
            this.checkBoxRemoveVerticalLines.Text = "Remove Vertical Lines";
            this.checkBoxRemoveVerticalLines.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 32);
            this.label1.TabIndex = 2;
            this.label1.Text = "Minimum Length";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(193, 32);
            this.label2.TabIndex = 3;
            this.label2.Text = "Maximum Width";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 141);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(171, 32);
            this.label3.TabIndex = 4;
            this.label3.Text = "Maximum Gap";
            // 
            // textBoxMinimumLength
            // 
            this.textBoxMinimumLength.Location = new System.Drawing.Point(225, 79);
            this.textBoxMinimumLength.Name = "textBoxMinimumLength";
            this.textBoxMinimumLength.Size = new System.Drawing.Size(50, 39);
            this.textBoxMinimumLength.TabIndex = 5;
            this.textBoxMinimumLength.Text = "150";
            this.textBoxMinimumLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxMaximumWidth
            // 
            this.textBoxMaximumWidth.Location = new System.Drawing.Point(245, 108);
            this.textBoxMaximumWidth.Name = "textBoxMaximumWidth";
            this.textBoxMaximumWidth.Size = new System.Drawing.Size(30, 39);
            this.textBoxMaximumWidth.TabIndex = 6;
            this.textBoxMaximumWidth.Text = "5";
            this.textBoxMaximumWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxMaximumGap
            // 
            this.textBoxMaximumGap.Location = new System.Drawing.Point(245, 137);
            this.textBoxMaximumGap.Name = "textBoxMaximumGap";
            this.textBoxMaximumGap.Size = new System.Drawing.Size(30, 39);
            this.textBoxMaximumGap.TabIndex = 7;
            this.textBoxMaximumGap.Text = "3";
            this.textBoxMaximumGap.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(326, 12);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(102, 30);
            this.buttonOK.TabIndex = 8;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(326, 46);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(102, 30);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // LineRemove
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(446, 179);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxMaximumGap);
            this.Controls.Add(this.textBoxMaximumWidth);
            this.Controls.Add(this.textBoxMinimumLength);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxRemoveVerticalLines);
            this.Controls.Add(this.checkBoxRemoveHorizontalLines);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(7, 8, 7, 8);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LineRemove";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Line Remove";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LineRemove_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxRemoveHorizontalLines;
        private System.Windows.Forms.CheckBox checkBoxRemoveVerticalLines;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxMinimumLength;
        private System.Windows.Forms.TextBox textBoxMaximumWidth;
        private System.Windows.Forms.TextBox textBoxMaximumGap;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}