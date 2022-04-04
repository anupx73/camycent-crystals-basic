namespace Crystals
{
    partial class Invoice
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.printDoc = new System.Drawing.Printing.PrintDocument();
            this.inLogo = new System.Windows.Forms.PictureBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.inCusName = new MetroFramework.Controls.MetroLabel();
            this.inCusPh = new MetroFramework.Controls.MetroLabel();
            this.gridInvoice = new System.Windows.Forms.DataGridView();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Item = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Desc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel6 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel7 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel8 = new MetroFramework.Controls.MetroLabel();
            this.inTotal = new MetroFramework.Controls.MetroTextBox();
            this.inDPer = new MetroFramework.Controls.MetroTextBox();
            this.inDisVal = new MetroFramework.Controls.MetroTextBox();
            this.inSTax = new MetroFramework.Controls.MetroTextBox();
            this.inPayable = new MetroFramework.Controls.MetroTextBox();
            this.inCName = new MetroFramework.Controls.MetroLabel();
            this.InCPh = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.inSTno = new MetroFramework.Controls.MetroLabel();
            this.inDate = new MetroFramework.Controls.MetroLabel();
            this.inComAdd = new MetroFramework.Controls.MetroLabel();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.cbPaymentMode = new MetroFramework.Controls.MetroComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.metroLabel11 = new MetroFramework.Controls.MetroLabel();
            this.TherapistNameLabel = new MetroFramework.Controls.MetroLabel();
            this.metroLabel13 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel12 = new MetroFramework.Controls.MetroLabel();
            this.btnPrint = new System.Windows.Forms.Button();
            this.InvoiceNoLabel = new MetroFramework.Controls.MetroLabel();
            this.metroLabel10 = new MetroFramework.Controls.MetroLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.metroLabel9 = new MetroFramework.Controls.MetroLabel();
            ((System.ComponentModel.ISupportInitialize)(this.inLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridInvoice)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // inLogo
            // 
            this.inLogo.Location = new System.Drawing.Point(551, 3);
            this.inLogo.Name = "inLogo";
            this.inLogo.Size = new System.Drawing.Size(126, 114);
            this.inLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.inLogo.TabIndex = 0;
            this.inLogo.TabStop = false;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(18, 9);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(24, 19);
            this.metroLabel1.TabIndex = 2;
            this.metroLabel1.Text = "To";
            // 
            // inCusName
            // 
            this.inCusName.AutoSize = true;
            this.inCusName.Location = new System.Drawing.Point(18, 28);
            this.inCusName.Name = "inCusName";
            this.inCusName.Size = new System.Drawing.Size(106, 19);
            this.inCusName.TabIndex = 3;
            this.inCusName.Text = "Customer Name";
            // 
            // inCusPh
            // 
            this.inCusPh.AutoSize = true;
            this.inCusPh.Location = new System.Drawing.Point(18, 52);
            this.inCusPh.Name = "inCusPh";
            this.inCusPh.Size = new System.Drawing.Size(107, 19);
            this.inCusPh.TabIndex = 4;
            this.inCusPh.Text = "Customer Phone";
            // 
            // gridInvoice
            // 
            this.gridInvoice.AllowUserToAddRows = false;
            this.gridInvoice.AllowUserToDeleteRows = false;
            this.gridInvoice.AllowUserToResizeRows = false;
            this.gridInvoice.BackgroundColor = System.Drawing.Color.White;
            this.gridInvoice.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridInvoice.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridInvoice.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(17)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(17)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridInvoice.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.gridInvoice.ColumnHeadersHeight = 25;
            this.gridInvoice.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridInvoice.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.Item,
            this.Desc,
            this.Price});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridInvoice.DefaultCellStyle = dataGridViewCellStyle6;
            this.gridInvoice.EnableHeadersVisualStyles = false;
            this.gridInvoice.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.gridInvoice.GridColor = System.Drawing.Color.White;
            this.gridInvoice.Location = new System.Drawing.Point(3, 134);
            this.gridInvoice.Name = "gridInvoice";
            this.gridInvoice.ReadOnly = true;
            this.gridInvoice.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(17)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(17)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridInvoice.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.gridInvoice.RowHeadersVisible = false;
            this.gridInvoice.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle8.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.gridInvoice.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.gridInvoice.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridInvoice.Size = new System.Drawing.Size(674, 191);
            this.gridInvoice.TabIndex = 5;
            // 
            // id
            // 
            this.id.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.id.FillWeight = 10F;
            this.id.HeaderText = "#";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            // 
            // Item
            // 
            this.Item.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Item.FillWeight = 30F;
            this.Item.HeaderText = "Item";
            this.Item.Name = "Item";
            this.Item.ReadOnly = true;
            // 
            // Desc
            // 
            this.Desc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Desc.FillWeight = 40F;
            this.Desc.HeaderText = "Description";
            this.Desc.Name = "Desc";
            this.Desc.ReadOnly = true;
            // 
            // Price
            // 
            this.Price.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Price.FillWeight = 20F;
            this.Price.HeaderText = "Price";
            this.Price.Name = "Price";
            this.Price.ReadOnly = true;
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.Location = new System.Drawing.Point(402, 354);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(66, 19);
            this.metroLabel4.TabIndex = 6;
            this.metroLabel4.Text = "Total (Rs.)";
            // 
            // metroLabel5
            // 
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.Location = new System.Drawing.Point(402, 373);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(81, 19);
            this.metroLabel5.TabIndex = 7;
            this.metroLabel5.Text = "Discount (%)";
            // 
            // metroLabel6
            // 
            this.metroLabel6.AutoSize = true;
            this.metroLabel6.Location = new System.Drawing.Point(402, 392);
            this.metroLabel6.Name = "metroLabel6";
            this.metroLabel6.Size = new System.Drawing.Size(137, 19);
            this.metroLabel6.TabIndex = 8;
            this.metroLabel6.Text = "Discount Amount (Rs.)";
            // 
            // metroLabel7
            // 
            this.metroLabel7.AutoSize = true;
            this.metroLabel7.Location = new System.Drawing.Point(402, 411);
            this.metroLabel7.Name = "metroLabel7";
            this.metroLabel7.Size = new System.Drawing.Size(97, 19);
            this.metroLabel7.TabIndex = 9;
            this.metroLabel7.Text = "Service Tax (%)";
            // 
            // metroLabel8
            // 
            this.metroLabel8.AutoSize = true;
            this.metroLabel8.Location = new System.Drawing.Point(401, 430);
            this.metroLabel8.Name = "metroLabel8";
            this.metroLabel8.Size = new System.Drawing.Size(116, 19);
            this.metroLabel8.TabIndex = 10;
            this.metroLabel8.Text = "Total Payable (Rs.)";
            // 
            // inTotal
            // 
            // 
            // 
            // 
            this.inTotal.CustomButton.Image = null;
            this.inTotal.CustomButton.Location = new System.Drawing.Point(86, 1);
            this.inTotal.CustomButton.Name = "";
            this.inTotal.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.inTotal.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.inTotal.CustomButton.TabIndex = 1;
            this.inTotal.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.inTotal.CustomButton.UseSelectable = true;
            this.inTotal.CustomButton.Visible = false;
            this.inTotal.Enabled = false;
            this.inTotal.Lines = new string[0];
            this.inTotal.Location = new System.Drawing.Point(551, 350);
            this.inTotal.MaxLength = 32767;
            this.inTotal.Name = "inTotal";
            this.inTotal.PasswordChar = '\0';
            this.inTotal.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.inTotal.SelectedText = "";
            this.inTotal.SelectionLength = 0;
            this.inTotal.SelectionStart = 0;
            this.inTotal.Size = new System.Drawing.Size(108, 23);
            this.inTotal.TabIndex = 11;
            this.inTotal.UseCustomBackColor = true;
            this.inTotal.UseSelectable = true;
            this.inTotal.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.inTotal.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // inDPer
            // 
            // 
            // 
            // 
            this.inDPer.CustomButton.Image = null;
            this.inDPer.CustomButton.Location = new System.Drawing.Point(86, 1);
            this.inDPer.CustomButton.Name = "";
            this.inDPer.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.inDPer.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.inDPer.CustomButton.TabIndex = 1;
            this.inDPer.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.inDPer.CustomButton.UseSelectable = true;
            this.inDPer.CustomButton.Visible = false;
            this.inDPer.Lines = new string[0];
            this.inDPer.Location = new System.Drawing.Point(551, 370);
            this.inDPer.MaxLength = 6;
            this.inDPer.Name = "inDPer";
            this.inDPer.PasswordChar = '\0';
            this.inDPer.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.inDPer.SelectedText = "";
            this.inDPer.SelectionLength = 0;
            this.inDPer.SelectionStart = 0;
            this.inDPer.Size = new System.Drawing.Size(108, 23);
            this.inDPer.TabIndex = 12;
            this.inDPer.UseCustomBackColor = true;
            this.inDPer.UseSelectable = true;
            this.inDPer.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.inDPer.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.inDPer.TextChanged += new System.EventHandler(this.inDPer_TextChanged);
            this.inDPer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.inDPer_KeyPress);
            // 
            // inDisVal
            // 
            // 
            // 
            // 
            this.inDisVal.CustomButton.Image = null;
            this.inDisVal.CustomButton.Location = new System.Drawing.Point(86, 1);
            this.inDisVal.CustomButton.Name = "";
            this.inDisVal.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.inDisVal.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.inDisVal.CustomButton.TabIndex = 1;
            this.inDisVal.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.inDisVal.CustomButton.UseSelectable = true;
            this.inDisVal.CustomButton.Visible = false;
            this.inDisVal.Enabled = false;
            this.inDisVal.Lines = new string[0];
            this.inDisVal.Location = new System.Drawing.Point(551, 389);
            this.inDisVal.MaxLength = 32767;
            this.inDisVal.Name = "inDisVal";
            this.inDisVal.PasswordChar = '\0';
            this.inDisVal.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.inDisVal.SelectedText = "";
            this.inDisVal.SelectionLength = 0;
            this.inDisVal.SelectionStart = 0;
            this.inDisVal.Size = new System.Drawing.Size(108, 23);
            this.inDisVal.TabIndex = 13;
            this.inDisVal.UseCustomBackColor = true;
            this.inDisVal.UseSelectable = true;
            this.inDisVal.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.inDisVal.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // inSTax
            // 
            // 
            // 
            // 
            this.inSTax.CustomButton.Image = null;
            this.inSTax.CustomButton.Location = new System.Drawing.Point(86, 1);
            this.inSTax.CustomButton.Name = "";
            this.inSTax.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.inSTax.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.inSTax.CustomButton.TabIndex = 1;
            this.inSTax.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.inSTax.CustomButton.UseSelectable = true;
            this.inSTax.CustomButton.Visible = false;
            this.inSTax.Enabled = false;
            this.inSTax.Lines = new string[0];
            this.inSTax.Location = new System.Drawing.Point(551, 410);
            this.inSTax.MaxLength = 32767;
            this.inSTax.Name = "inSTax";
            this.inSTax.PasswordChar = '\0';
            this.inSTax.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.inSTax.SelectedText = "";
            this.inSTax.SelectionLength = 0;
            this.inSTax.SelectionStart = 0;
            this.inSTax.Size = new System.Drawing.Size(108, 23);
            this.inSTax.TabIndex = 14;
            this.inSTax.UseCustomBackColor = true;
            this.inSTax.UseSelectable = true;
            this.inSTax.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.inSTax.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.inSTax.TextChanged += new System.EventHandler(this.inSTax_TextChanged);
            // 
            // inPayable
            // 
            // 
            // 
            // 
            this.inPayable.CustomButton.Image = null;
            this.inPayable.CustomButton.Location = new System.Drawing.Point(86, 1);
            this.inPayable.CustomButton.Name = "";
            this.inPayable.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.inPayable.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.inPayable.CustomButton.TabIndex = 1;
            this.inPayable.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.inPayable.CustomButton.UseSelectable = true;
            this.inPayable.CustomButton.Visible = false;
            this.inPayable.Enabled = false;
            this.inPayable.Lines = new string[0];
            this.inPayable.Location = new System.Drawing.Point(551, 432);
            this.inPayable.MaxLength = 32767;
            this.inPayable.Name = "inPayable";
            this.inPayable.PasswordChar = '\0';
            this.inPayable.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.inPayable.SelectedText = "";
            this.inPayable.SelectionLength = 0;
            this.inPayable.SelectionStart = 0;
            this.inPayable.Size = new System.Drawing.Size(108, 23);
            this.inPayable.TabIndex = 15;
            this.inPayable.UseCustomBackColor = true;
            this.inPayable.UseSelectable = true;
            this.inPayable.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.inPayable.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // inCName
            // 
            this.inCName.AutoSize = true;
            this.inCName.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.inCName.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.inCName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(17)))), ((int)(((byte)(65)))));
            this.inCName.Location = new System.Drawing.Point(327, 9);
            this.inCName.Name = "inCName";
            this.inCName.Size = new System.Drawing.Size(141, 25);
            this.inCName.TabIndex = 16;
            this.inCName.Text = "Company Name";
            this.inCName.UseCustomForeColor = true;
            // 
            // InCPh
            // 
            this.InCPh.Location = new System.Drawing.Point(329, 98);
            this.InCPh.Name = "InCPh";
            this.InCPh.Size = new System.Drawing.Size(183, 19);
            this.InCPh.TabIndex = 18;
            this.InCPh.Text = "Company Phone";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(18, 407);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(127, 19);
            this.metroLabel2.TabIndex = 19;
            this.metroLabel2.Text = "Service Tax Number";
            // 
            // inSTno
            // 
            this.inSTno.AutoSize = true;
            this.inSTno.Location = new System.Drawing.Point(161, 407);
            this.inSTno.Name = "inSTno";
            this.inSTno.Size = new System.Drawing.Size(79, 19);
            this.inSTno.TabIndex = 20;
            this.inSTno.Text = "0000000000";
            // 
            // inDate
            // 
            this.inDate.AutoSize = true;
            this.inDate.Location = new System.Drawing.Point(57, 98);
            this.inDate.Name = "inDate";
            this.inDate.Size = new System.Drawing.Size(36, 19);
            this.inDate.TabIndex = 21;
            this.inDate.Text = "Date";
            // 
            // inComAdd
            // 
            this.inComAdd.Location = new System.Drawing.Point(329, 38);
            this.inComAdd.Name = "inComAdd";
            this.inComAdd.Size = new System.Drawing.Size(210, 60);
            this.inComAdd.TabIndex = 22;
            this.inComAdd.Text = "CompanyAddress";
            this.inComAdd.WrapToLine = true;
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(18, 434);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(99, 19);
            this.metroLabel3.TabIndex = 23;
            this.metroLabel3.Text = "Payment Mode";
            // 
            // cbPaymentMode
            // 
            this.cbPaymentMode.FontSize = MetroFramework.MetroComboBoxSize.Small;
            this.cbPaymentMode.FormattingEnabled = true;
            this.cbPaymentMode.ItemHeight = 19;
            this.cbPaymentMode.Items.AddRange(new object[] {
            "Cash",
            "Card"});
            this.cbPaymentMode.Location = new System.Drawing.Point(164, 431);
            this.cbPaymentMode.Name = "cbPaymentMode";
            this.cbPaymentMode.Size = new System.Drawing.Size(86, 25);
            this.cbPaymentMode.TabIndex = 24;
            this.cbPaymentMode.UseSelectable = true;
            this.cbPaymentMode.SelectedIndexChanged += new System.EventHandler(this.cbPaymentMode_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.metroLabel11);
            this.panel2.Controls.Add(this.TherapistNameLabel);
            this.panel2.Controls.Add(this.metroLabel13);
            this.panel2.Controls.Add(this.cbPaymentMode);
            this.panel2.Controls.Add(this.metroLabel3);
            this.panel2.Controls.Add(this.inComAdd);
            this.panel2.Controls.Add(this.metroLabel12);
            this.panel2.Controls.Add(this.inDate);
            this.panel2.Controls.Add(this.inSTno);
            this.panel2.Controls.Add(this.metroLabel2);
            this.panel2.Controls.Add(this.InCPh);
            this.panel2.Controls.Add(this.inCName);
            this.panel2.Controls.Add(this.inPayable);
            this.panel2.Controls.Add(this.inSTax);
            this.panel2.Controls.Add(this.inDisVal);
            this.panel2.Controls.Add(this.inDPer);
            this.panel2.Controls.Add(this.inTotal);
            this.panel2.Controls.Add(this.metroLabel8);
            this.panel2.Controls.Add(this.metroLabel7);
            this.panel2.Controls.Add(this.metroLabel6);
            this.panel2.Controls.Add(this.metroLabel5);
            this.panel2.Controls.Add(this.metroLabel4);
            this.panel2.Controls.Add(this.gridInvoice);
            this.panel2.Controls.Add(this.inCusPh);
            this.panel2.Controls.Add(this.inCusName);
            this.panel2.Controls.Add(this.metroLabel1);
            this.panel2.Controls.Add(this.inLogo);
            this.panel2.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel2.Location = new System.Drawing.Point(5, 51);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(680, 561);
            this.panel2.TabIndex = 1;
            // 
            // metroLabel11
            // 
            this.metroLabel11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(17)))), ((int)(((byte)(65)))));
            this.metroLabel11.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel11.ForeColor = System.Drawing.Color.White;
            this.metroLabel11.Location = new System.Drawing.Point(0, 479);
            this.metroLabel11.Name = "metroLabel11";
            this.metroLabel11.Size = new System.Drawing.Size(680, 25);
            this.metroLabel11.TabIndex = 28;
            this.metroLabel11.Text = "Thank you for your Business. Hope to see you soon.";
            this.metroLabel11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel11.UseCustomBackColor = true;
            this.metroLabel11.UseCustomForeColor = true;
            // 
            // TherapistNameLabel
            // 
            this.TherapistNameLabel.Location = new System.Drawing.Point(161, 381);
            this.TherapistNameLabel.Name = "TherapistNameLabel";
            this.TherapistNameLabel.Size = new System.Drawing.Size(165, 19);
            this.TherapistNameLabel.TabIndex = 27;
            // 
            // metroLabel13
            // 
            this.metroLabel13.AutoSize = true;
            this.metroLabel13.Location = new System.Drawing.Point(18, 381);
            this.metroLabel13.Name = "metroLabel13";
            this.metroLabel13.Size = new System.Drawing.Size(124, 19);
            this.metroLabel13.TabIndex = 26;
            this.metroLabel13.Text = "You were served by";
            // 
            // metroLabel12
            // 
            this.metroLabel12.AutoSize = true;
            this.metroLabel12.Location = new System.Drawing.Point(18, 98);
            this.metroLabel12.Name = "metroLabel12";
            this.metroLabel12.Size = new System.Drawing.Size(36, 19);
            this.metroLabel12.TabIndex = 21;
            this.metroLabel12.Text = "Date";
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(17)))), ((int)(((byte)(65)))));
            this.btnPrint.FlatAppearance.BorderSize = 0;
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(532, 618);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(153, 26);
            this.btnPrint.TabIndex = 0;
            this.btnPrint.Text = "&Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // InvoiceNoLabel
            // 
            this.InvoiceNoLabel.AutoSize = true;
            this.InvoiceNoLabel.FontSize = MetroFramework.MetroLabelSize.Small;
            this.InvoiceNoLabel.Location = new System.Drawing.Point(123, 29);
            this.InvoiceNoLabel.Name = "InvoiceNoLabel";
            this.InvoiceNoLabel.Size = new System.Drawing.Size(23, 15);
            this.InvoiceNoLabel.TabIndex = 26;
            this.InvoiceNoLabel.Text = "No";
            // 
            // metroLabel10
            // 
            this.metroLabel10.AutoSize = true;
            this.metroLabel10.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel10.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(17)))), ((int)(((byte)(65)))));
            this.metroLabel10.Location = new System.Drawing.Point(5, 23);
            this.metroLabel10.Name = "metroLabel10";
            this.metroLabel10.Size = new System.Drawing.Size(80, 25);
            this.metroLabel10.TabIndex = 26;
            this.metroLabel10.Text = "INVOICE";
            this.metroLabel10.UseCustomForeColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BackgroundImage = global::Crystals.Properties.Resources.pad_top;
            this.panel1.Location = new System.Drawing.Point(556, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(137, 25);
            this.panel1.TabIndex = 74;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(17)))), ((int)(((byte)(65)))));
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(562, 8);
            this.panel3.TabIndex = 73;
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(17)))), ((int)(((byte)(65)))));
            this.buttonCancel.FlatAppearance.BorderSize = 0;
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.ForeColor = System.Drawing.Color.White;
            this.buttonCancel.Location = new System.Drawing.Point(5, 618);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(153, 26);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // metroLabel9
            // 
            this.metroLabel9.AutoSize = true;
            this.metroLabel9.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel9.Location = new System.Drawing.Point(85, 29);
            this.metroLabel9.Name = "metroLabel9";
            this.metroLabel9.Size = new System.Drawing.Size(36, 15);
            this.metroLabel9.TabIndex = 26;
            this.metroLabel9.Text = "No. #";
            // 
            // Invoice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(693, 664);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.metroLabel9);
            this.Controls.Add(this.InvoiceNoLabel);
            this.Controls.Add(this.metroLabel10);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.panel2);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Invoice";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "INVOICE";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.inLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridInvoice)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Drawing.Printing.PrintDocument printDoc;
        private System.Windows.Forms.PictureBox inLogo;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel inCusName;
        private MetroFramework.Controls.MetroLabel inCusPh;
        private System.Windows.Forms.DataGridView gridInvoice;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item;
        private System.Windows.Forms.DataGridViewTextBoxColumn Desc;
        private System.Windows.Forms.DataGridViewTextBoxColumn Price;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        private MetroFramework.Controls.MetroLabel metroLabel6;
        private MetroFramework.Controls.MetroLabel metroLabel7;
        private MetroFramework.Controls.MetroLabel metroLabel8;
        private MetroFramework.Controls.MetroTextBox inTotal;
        private MetroFramework.Controls.MetroTextBox inDPer;
        private MetroFramework.Controls.MetroTextBox inDisVal;
        private MetroFramework.Controls.MetroTextBox inSTax;
        private MetroFramework.Controls.MetroTextBox inPayable;
        private MetroFramework.Controls.MetroLabel inCName;
        private MetroFramework.Controls.MetroLabel InCPh;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroLabel inSTno;
        private MetroFramework.Controls.MetroLabel inDate;
        private MetroFramework.Controls.MetroLabel inComAdd;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroComboBox cbPaymentMode;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnPrint;
        private MetroFramework.Controls.MetroLabel InvoiceNoLabel;
        private MetroFramework.Controls.MetroLabel TherapistNameLabel;
        private MetroFramework.Controls.MetroLabel metroLabel10;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button buttonCancel;
        private MetroFramework.Controls.MetroLabel metroLabel11;
        private MetroFramework.Controls.MetroLabel metroLabel13;
        private MetroFramework.Controls.MetroLabel metroLabel12;
        private MetroFramework.Controls.MetroLabel metroLabel9;
    }
}