namespace Ban_REMOTE_MSSQL_Manual
{
    partial class frmMain
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
            this.btnSetup = new System.Windows.Forms.Button();
            this.txtIPs = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnSetup
            // 
            this.btnSetup.Location = new System.Drawing.Point(609, 12);
            this.btnSetup.Name = "btnSetup";
            this.btnSetup.Size = new System.Drawing.Size(75, 23);
            this.btnSetup.TabIndex = 0;
            this.btnSetup.Text = "적용";
            this.btnSetup.UseVisualStyleBackColor = true;
            this.btnSetup.Click += new System.EventHandler(this.btnSetup_Click);
            // 
            // txtIPs
            // 
            this.txtIPs.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtIPs.Location = new System.Drawing.Point(0, 0);
            this.txtIPs.Multiline = true;
            this.txtIPs.Name = "txtIPs";
            this.txtIPs.Size = new System.Drawing.Size(415, 535);
            this.txtIPs.TabIndex = 1;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 535);
            this.Controls.Add(this.txtIPs);
            this.Controls.Add(this.btnSetup);
            this.Name = "frmMain";
            this.Text = "IP수동처리";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSetup;
        private System.Windows.Forms.TextBox txtIPs;
    }
}

