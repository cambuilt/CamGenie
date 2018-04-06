namespace CamGenie
{
    partial class ImportPreview
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
            this.dataGridViewImport = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewImport)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewImport
            // 
            this.dataGridViewImport.AllowUserToAddRows = false;
            this.dataGridViewImport.AllowUserToDeleteRows = false;
            this.dataGridViewImport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewImport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewImport.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewImport.Name = "dataGridViewImport";
            this.dataGridViewImport.ReadOnly = true;
            this.dataGridViewImport.RowHeadersVisible = false;
            this.dataGridViewImport.Size = new System.Drawing.Size(728, 437);
            this.dataGridViewImport.TabIndex = 0;
            // 
            // ImportPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(728, 437);
            this.Controls.Add(this.dataGridViewImport);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.MinimizeBox = false;
            this.Name = "ImportPreview";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import Preview";
            this.Load += new System.EventHandler(this.ImportPreview_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewImport)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewImport;
    }
}