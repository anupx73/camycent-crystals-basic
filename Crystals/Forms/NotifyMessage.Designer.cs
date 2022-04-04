namespace Crystals
{
    partial class NotifyMessage
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
            this.labelMessage = new MetroFramework.Controls.MetroLabel();
            this.labelTitle = new MetroFramework.Controls.MetroLabel();
            this.lnkOK = new MetroFramework.Controls.MetroLink();
            this.picNotifyType = new System.Windows.Forms.PictureBox();
            this.lnkCopyClibrd = new MetroFramework.Controls.MetroLink();
            ((System.ComponentModel.ISupportInitialize)(this.picNotifyType)).BeginInit();
            this.SuspendLayout();
            // 
            // labelMessage
            // 
            this.labelMessage.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.labelMessage.ForeColor = System.Drawing.Color.White;
            this.labelMessage.Location = new System.Drawing.Point(208, 99);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(650, 80);
            this.labelMessage.TabIndex = 6;
            this.labelMessage.Text = "metroLabel1";
            this.labelMessage.UseCustomBackColor = true;
            this.labelMessage.UseCustomForeColor = true;
            this.labelMessage.WrapToLine = true;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.labelTitle.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(206, 67);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(118, 25);
            this.labelTitle.TabIndex = 7;
            this.labelTitle.Text = "metroLabel1";
            this.labelTitle.UseCustomBackColor = true;
            this.labelTitle.UseCustomForeColor = true;
            // 
            // lnkOK
            // 
            this.lnkOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkOK.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lnkOK.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.lnkOK.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.lnkOK.ForeColor = System.Drawing.Color.Black;
            this.lnkOK.Location = new System.Drawing.Point(0, 223);
            this.lnkOK.Name = "lnkOK";
            this.lnkOK.Size = new System.Drawing.Size(950, 32);
            this.lnkOK.TabIndex = 0;
            this.lnkOK.Text = "OK";
            this.lnkOK.UseCustomBackColor = true;
            this.lnkOK.UseCustomForeColor = true;
            this.lnkOK.UseSelectable = true;
            this.lnkOK.Click += new System.EventHandler(this.lnkOk_Click);
            // 
            // picNotifyType
            // 
            this.picNotifyType.Image = global::Crystals.Properties.Resources.info_warning;
            this.picNotifyType.Location = new System.Drawing.Point(103, 79);
            this.picNotifyType.Name = "picNotifyType";
            this.picNotifyType.Size = new System.Drawing.Size(64, 64);
            this.picNotifyType.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picNotifyType.TabIndex = 4;
            this.picNotifyType.TabStop = false;
            // 
            // lnkCopyClibrd
            // 
            this.lnkCopyClibrd.BackColor = System.Drawing.Color.Black;
            this.lnkCopyClibrd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkCopyClibrd.ForeColor = System.Drawing.Color.White;
            this.lnkCopyClibrd.Location = new System.Drawing.Point(84, 158);
            this.lnkCopyClibrd.Name = "lnkCopyClibrd";
            this.lnkCopyClibrd.Size = new System.Drawing.Size(108, 23);
            this.lnkCopyClibrd.TabIndex = 8;
            this.lnkCopyClibrd.Text = "Copy to clipboard";
            this.lnkCopyClibrd.UseCustomBackColor = true;
            this.lnkCopyClibrd.UseCustomForeColor = true;
            this.lnkCopyClibrd.UseSelectable = true;
            this.lnkCopyClibrd.Visible = false;
            this.lnkCopyClibrd.Click += new System.EventHandler(this.lnkCopyClipbrd_Click);
            // 
            // NotifyMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(17)))), ((int)(((byte)(65)))));
            this.ClientSize = new System.Drawing.Size(950, 255);
            this.Controls.Add(this.lnkCopyClibrd);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.lnkOK);
            this.Controls.Add(this.picNotifyType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NotifyMessage";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NotifyMessage";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.picNotifyType)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroLabel labelMessage;
        private MetroFramework.Controls.MetroLabel labelTitle;
        private MetroFramework.Controls.MetroLink lnkOK;
        private System.Windows.Forms.PictureBox picNotifyType;
        private MetroFramework.Controls.MetroLink lnkCopyClibrd;
    }
}