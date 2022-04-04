

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Drawing.Printing;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Data.Sql;
using System.Data.SqlClient;
using MetroFramework;

namespace Crystals.Controllers
{
    public partial class Form1 : Form
    {
        private const int HTCAPTION = 0x2;
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int CS_DBLCLKS = 0x8;

        [DllImportAttribute("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();

        private string lastInvoicePdfFile = String.Empty;
        private static int custId, packId;
        private static double amnt;

        private static List<int> SIDS;
        private static bool isPackage;
        public Form1()
        {
            InitializeComponent();
            SetLinkLabelsTransparent();
        }

        public Form1(int CustomerId, int PackageId, double Amount, List<int> ServiceIds, bool fromPackage)
        {
            SIDS = new List<int>();
            SIDS = ServiceIds;
            custId = CustomerId;
            packId = PackageId;
            amnt = Amount;
            isPackage = fromPackage;



            InitializeComponent();

            if (fromPackage == false)
            {
                label7.Text = "Appointment#";
            }
            SetLinkLabelsTransparent();
            PopulateData();
        }
        private static double amntWithTax = 0;
        private void PopulateData()
        {
            List<string> custDetails = new List<string>();
            List<string> salesDetails = new List<string>();

            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("Id", custId.ToString());
            SqlDataReader reader = Program.db.get_where_custom("Customers", d);

            comboBox2.SelectedItem = "Cash";

            if (reader.HasRows)
            {
                reader.Read();
                //customer Name
                textBoxContactPer.Text = reader["Name"].ToString();
                //customer Ph
                textBoxPh.Text = reader["Phone"].ToString();
                //customer address
                textBoxAdd.Text = reader["Address"].ToString();
                //customer email
                textBoxEmail.Text = reader["Email"].ToString();

            }
            //Customer ID
            textBoxInvoice.Text = custId.ToString();
            //Appointment/PackageId
            textBoxCustID.Text = packId.ToString();
            textBoxTotalAdded.Text = amnt.ToString();
            textBoxTax.Text = "14.5";

            amntWithTax = amnt + (amnt * .145);
            textBoxTotalPay.Text = amntWithTax.ToString();
            textBoxAdv.Text = "0";


            if (dateTimePicker1.Text != null)
                custDetails.Add(dateTimePicker1.Text.ToString());//date
            else
                custDetails.Add("");
            custDetails.Add(textBoxCompany.Text);//company name
            custDetails.Add(textBoxAdd.Text);//address
            custDetails.Add(textBoxContactPer.Text);//contact person
            custDetails.Add(textBoxPh.Text);//phone number
            custDetails.Add(textBoxEmail.Text);//mail id
            custDetails.Add(textBoxCustID.Text);//customer id
            if (comboBox1.SelectedItem != null)
                custDetails.Add(comboBox1.SelectedItem.ToString());//designation
            else custDetails.Add("");
            custDetails.Add(textBoxEmpName.Text);//employee name
            custDetails.Add(textBoxRemarks.Text);//remarks
            textBoxTotalPay.Text = amntWithTax.ToString();
            custDetails.Add(textBoxTotalAdded.Text);//total
            custDetails.Add(textBoxAdv.Text);//advance paid
            custDetails.Add(amntWithTax.ToString());//total payable
            Packages pack = new Packages();
            string pName = "";
            try
            {


                SqlDataReader reader1 = Program.db.get_where("Packages", packId.ToString());
                reader1.Read();
                // Desc = reader1["Description"].ToString();
                pName = reader1["Name"].ToString();


            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);

            }

            if (isPackage == false)
            {
                int count = 0;
                int rows = 0;
                Services s = new Services();

                foreach (var i in SIDS)
                {
                    rows++;
                    ++count;
                    //SNo
                    salesDetails.Add(count.ToString());
                    //Name
                    salesDetails.Add(s.GetServiceName(i.ToString()));
                    //Desc
                    salesDetails.Add(s.GetServiceDesc(i.ToString()));
                    //Amount
                    salesDetails.Add(s.GetServicePrice(i.ToString()));
                    int j = 0;
                    try
                    {
                        dataGridView1.Rows.Add();

                        for (int col = 0; col < dataGridView1.Rows[rows].Cells.Count; col++)
                        {

                            dataGridView1.Rows[rows].Cells[col].Value = salesDetails[j++];
                        }

                    }
                    catch (System.NullReferenceException exp)
                    {
                        Log.AppError("Exception occurred from Create. Message: " + exp.ToString());
                    }
                    
                    salesDetails.Clear();

                }

            }
            else
            {
                //package selling Invoice
                Packages p = new Packages();
                salesDetails.Clear();
                salesDetails.Add("1");
                salesDetails.Add(pName);
                salesDetails.Add(p.GetPackageDesc(packId.ToString()));
                salesDetails.Add(p.GetPackagePrice(packId.ToString()));
                int i = 0;
                try
                {
                    for (int rows = 0; rows < dataGridView1.Rows.Count; rows++)
                    {
                        for (int col = 0; col < dataGridView1.Rows[rows].Cells.Count; col++)
                        {

                            dataGridView1.Rows[rows].Cells[col].Value = salesDetails[i++];
                        }
                    }
                }
                catch (System.NullReferenceException exp)
                {
                    Log.AppError("Exception occurred from Create. Message: " + exp.ToString());
                }



            }

        }

        private void button2_Close(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Minimize(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                // add the drop shadow flag for automatically drawing
                // a drop shadow around the form
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DBLCLKS;
                return cp;
            }
        }

        private void menuStrip1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void menuStrip1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Focused)
            {
                Focus();
            }
        }

        private void SetLinkLabelsTransparent()
        {
            for (int ix = this.Controls.Count - 1; ix >= 0; --ix)
            {
                LinkLabel linkLabel = this.Controls[ix] as LinkLabel;
                if (linkLabel != null)
                {
                    Point pos = this.PointToScreen(linkLabel.Location);
                    pos = pictureBox1.PointToClient(pos);
                    linkLabel.Parent = pictureBox1;
                    linkLabel.Location = pos;
                    linkLabel.BackColor = Color.Transparent;
                }
            }
        }

        private void linkLabelCreate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateNewInvoice();
        }
        private void linkLabelOpen_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenExistingInvoice();
        }
        private void linkLabelSave_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveInvoice();
        }
        private void linkLabelPrint_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PrintDataToPrinter();

            #region RawPrintCode
            // Allow the user to select a file.
            //OpenFileDialog ofd = new OpenFileDialog();

            //if (DialogResult.OK == ofd.ShowDialog(this))
            //{
            //    // Allow the user to select a printer.
            //    PrintDialog pd = new PrintDialog();
            //    pd.PrinterSettings = new PrinterSettings();
            //    pd.AllowCurrentPage = true;
            //    pd.AllowSomePages = true;

            //    pd.PrinterSettings.PrintRange = PrintRange.Selection;

            //    if (DialogResult.OK == pd.ShowDialog(this))
            //    {
            //        // Print the file to the printer.
            //        //ExtractPages(ofd.FileName, "Temp.pdf", pd.PrinterSettings.FromPage, pd.PrinterSettings.ToPage);
            //        RawPrinterHelper.SendFileToPrinter(pd.PrinterSettings.PrinterName, ofd.FileName);
            //        //RawPrinterHelper.SendFileToPrinter(pd.PrinterSettings.PrinterName, @"Temp.pdf");
            //        //System.IO.File.Delete(@"Temp.pdf");
            //    }
            //}
            #endregion
        }
        private void linkLabelEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SendInvoiceEmail();
        }

        private void CreateNewInvoice()
        {
            textBoxInvoice.Text = "";//invNo
            dateTimePicker1.Text = "";
            textBoxCompany.Text = "";//company name
            textBoxAdd.Text = "";//address
            textBoxContactPer.Text = "";//contact person
            textBoxPh.Text = "";//phone number
            textBoxEmail.Text = "";//mail id
            textBoxCustID.Text = "";//customer id
            comboBox1.SelectedItem = null;//designation
            textBoxEmpName.Text = "";//employee name
            textBoxRemarks.Text = "";//remarks
            textBoxTotalAdded.Text = "";//total
            textBoxAdv.Text = "";//advance paid
            textBoxTotalPay.Text = "";//total payable
            comboBox2.SelectedIndex = 0;

            try
            {
                for (int rows = 0; rows < dataGridView1.Rows.Count; rows++)
                {
                    for (int col = 0; col < dataGridView1.Rows[rows].Cells.Count; col++)
                    {

                        dataGridView1.Rows[rows].Cells[col].Value = "";
                    }
                }
            }
            catch (System.NullReferenceException exp)
            {
                Log.AppError("Exception occured from Create. Message: " + exp.ToString());
            }

            //GeneratePDF();
        }

        private void SaveInvoice()
        {
            DateTime timeNow = DateTime.Now;
            string format = "yyyy-MMMM-dd";


            string subPath = "InvoiceRepo"; // your code goes here

            bool exists = System.IO.Directory.Exists(subPath);

            if (!exists)
                System.IO.Directory.CreateDirectory(subPath);

            string pdfFilename = "\\Invoice-" + custId + "-" + timeNow.ToString(format) + ".pdf";
            // string xmlFilename = "\\Report-" + timeNow.ToString(format) + ".xml";//
            lastInvoicePdfFile = Application.StartupPath + pdfFilename;
            GeneratePDF(pdfFilename);
            //GenerateXML(xmlFilename);
        }

        private void PrintDataToPrinter()
        {
            string invoiceNo;
            List<string> custDetails = new List<string>();
            List<string> salesDetails = new List<string>();

            invoiceNo = textBoxInvoice.Text;//invNo
            if (dateTimePicker1.Text != null)
                custDetails.Add(dateTimePicker1.Text.ToString());//date
            else
                custDetails.Add("");
            custDetails.Add(textBoxCompany.Text);//company name
            custDetails.Add(textBoxAdd.Text);//address
            custDetails.Add(textBoxContactPer.Text);//contact person
            custDetails.Add(textBoxPh.Text);//phone number
            custDetails.Add(textBoxEmail.Text);//mail id
            custDetails.Add(textBoxCustID.Text);//customer id
            if (comboBox1.SelectedItem != null)
                custDetails.Add(comboBox1.SelectedItem.ToString());//designation
            else custDetails.Add("");
            custDetails.Add(textBoxEmpName.Text);//employee name
            custDetails.Add(textBoxRemarks.Text);//remarks
            custDetails.Add(textBoxTotalAdded.Text);//total
            custDetails.Add(textBoxAdv.Text);//advance paid
            custDetails.Add(textBoxTotalPay.Text);//total payable
            custDetails.Add(comboBox2.SelectedItem.ToString());//payment mode cash or card

            try
            {
                for (int rows = 0; rows < dataGridView1.Rows.Count; rows++)
                {
                    for (int col = 0; col < dataGridView1.Rows[rows].Cells.Count; col++)
                    {
                        if ((dataGridView1.Rows[rows].Cells[col].Value == null)
                            || (dataGridView1.Rows[rows].Cells[col].Value.ToString() == ""))
                        {
                            dataGridView1.Rows[rows].Cells[col].Value = "0";
                            salesDetails.Add("0");
                        }
                        else
                            salesDetails.Add(dataGridView1.Rows[rows].Cells[col].Value.ToString());
                    }
                }
            }
            catch (System.NullReferenceException exp)
            {
                Log.AppError("Exception occured from DirectPrint. Message: " + exp.ToString());
            }

            // DirectPrint result = new DirectPrint(invoiceNo, custDetails, salesDetails);
            //if (result == true)
            {
                //MessageBox.Show("Report Has Been Generated", "Invoice Done!");
                //this.Dispose();
            }
        }

        private void GenerateXML(string fileName)
        {
            string invoiceNo;
            List<string> custDetails = new List<string>();
            List<string> salesDetails = new List<string>();

            invoiceNo = textBoxInvoice.Text;//invNo
            if (dateTimePicker1.Text != null)
                custDetails.Add(dateTimePicker1.Text.ToString());//date
            else
                custDetails.Add("");
            custDetails.Add(textBoxCompany.Text);//company name
            custDetails.Add(textBoxAdd.Text);//address
            custDetails.Add(textBoxContactPer.Text);//contact person
            custDetails.Add(textBoxPh.Text);//phone number
            custDetails.Add(textBoxEmail.Text);//mail id
            custDetails.Add(textBoxCustID.Text);//customer id
            if (comboBox1.SelectedItem != null)
                custDetails.Add(comboBox1.SelectedItem.ToString());//designation
            else custDetails.Add("");
            custDetails.Add(textBoxEmpName.Text);//employee name
            custDetails.Add(textBoxRemarks.Text);//remarks
            custDetails.Add(textBoxTotalAdded.Text);//total
            custDetails.Add(textBoxAdv.Text);//advance paid
            custDetails.Add(textBoxTotalPay.Text);//total payable

            try
            {
                for (int rows = 0; rows < dataGridView1.Rows.Count; rows++)
                {
                    for (int col = 0; col < dataGridView1.Rows[rows].Cells.Count; col++)
                    {
                        if ((dataGridView1.Rows[rows].Cells[col].Value == null)
                            || (dataGridView1.Rows[rows].Cells[col].Value.ToString() == ""))
                        {
                            dataGridView1.Rows[rows].Cells[col].Value = "0";
                            salesDetails.Add("0");
                        }
                        else
                            salesDetails.Add(dataGridView1.Rows[rows].Cells[col].Value.ToString());
                    }
                }
            }
            catch (System.NullReferenceException exp)
            {
                Log.AppError("Exception occured from GeneratePDF. Message: " + exp.ToString());
            }

            XmlOperations xmlOp = new XmlOperations(fileName, invoiceNo, custDetails, salesDetails);
        }

        private void GeneratePDF(string fileName)
        {
            string invoiceNo;
            List<string> custDetails = new List<string>();
            List<string> salesDetails = new List<string>();

            string addr = "", name = "", phn = "", mailId = "";


            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("Id", custId.ToString());
            SqlDataReader reader = Program.db.get_where_custom("Customers", d);



            if (reader.HasRows)
            {
                reader.Read();
                name = reader["Name"].ToString();
                phn = reader["Phone"].ToString();
                addr = reader["Address"].ToString();
                mailId = reader["Email"].ToString();

            }
            SqlDataReader reader1 = Program.db.get_where("Sales", packId.ToString());
            reader1.Read();
            Dictionary<string, string> SalesData = new Dictionary<string, string>();
            //sales update
            SalesData.Add("PaymentMode","'" +comboBox2.SelectedItem.ToString()+"'");
            Program.db.update("Sales", SalesData, " where AppointmentId=" + packId);



            invoiceNo = "";//invNo
            custDetails.Add(DateTime.Today.ToString("yyyy-MM-dd"));
            custDetails.Add(textBoxCompany.Text);//company name
            custDetails.Add(addr);//address
            custDetails.Add(name);//customer name
            custDetails.Add(phn);//phone number
            custDetails.Add(mailId);//mail id
            custDetails.Add(custId.ToString());//customer id

            if (comboBox1.SelectedItem != null)
                custDetails.Add(comboBox1.SelectedItem.ToString());//designation
            else custDetails.Add("");

            custDetails.Add(textBoxEmpName.Text);//employee name
            custDetails.Add("");//remarks

            invoiceNo = custId.ToString() + DateTime.Today.GetHashCode();//invNo

            custDetails.Add(textBoxTotalAdded.Text);//total
            custDetails.Add(textBoxAdv.Text);//advance paid
            custDetails.Add(amntWithTax.ToString());//total payable
            custDetails.Add(comboBox2.SelectedItem.ToString());//payment mode cash or card

            try
            {
                for (int rows = 0; rows < dataGridView1.Rows.Count; rows++)
                {
                    for (int col = 0; col < dataGridView1.Rows[rows].Cells.Count; col++)
                    {
                        if ((dataGridView1.Rows[rows].Cells[col].Value == null)
                            || (dataGridView1.Rows[rows].Cells[col].Value.ToString() == ""))
                        {
                            dataGridView1.Rows[rows].Cells[col].Value = "0";
                            salesDetails.Add("0");
                        }
                        else
                            salesDetails.Add(dataGridView1.Rows[rows].Cells[col].Value.ToString());
                    }
                }
            }
            catch (System.NullReferenceException exp)
            {
                Log.AppError("Exception occured from GeneratePDF. Message: " + exp.ToString());
            }

            bool result = new PDFCreator().Build(fileName, invoiceNo, custDetails, salesDetails, isPackage);
            if (result == true)
            {
                MetroMessageBox.Show(this.FindForm(), "Report Has Been Generated", "Invoice Done!");
                this.Dispose();
            }
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            this.dataGridView1.Rows[e.RowIndex].Cells[0].Value = (e.RowIndex + 1).ToString();
        }

        private void textBoxAdv_TextChanged(object sender, EventArgs e)
        {
            string tempAdv = textBoxAdv.Text, tempTot = textBoxTotalAdded.Text;
            if ((tempAdv.Length > 0) && (tempTot.Length > 0))
                textBoxTotalPay.Text = (Int32.Parse(tempTot) - Int32.Parse(tempAdv)).ToString();
            else
                textBoxTotalPay.Text = textBoxTotalAdded.Text;
            dataGridView1.Refresh();
        }

        private void dataGridView1_EndEditCell(object sender, DataGridViewCellEventArgs e)
        {
            calculateTotal = 0;
            try
            {

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string cell1 = "", cell3 = "", cell4 = "", cell5 = "";
                    if (row.Cells[1].Value == null)
                    {
                        cell1 = "0";
                    }
                    else
                        cell1 = row.Cells[1].Value.ToString();

                    if (row.Cells[3].Value == null)
                    {
                        cell3 = "0";
                    }
                    else
                        cell3 = row.Cells[3].Value.ToString();

                    if (row.Cells[4].Value != null)
                        cell4 = row.Cells[4].Value.ToString();

                    //if (System.Text.RegularExpressions.Regex.IsMatch(cell1, "[^0-9]"))
                    //{
                    //    //MessageBox.Show("Please enter only numbers.");
                    //    row.Cells[1].Value = "0";
                    //    cell1 = "0";
                    //}

                    if (System.Text.RegularExpressions.Regex.IsMatch(cell3, "[^0-9]"))
                    {
                        MessageBox.Show("Please enter only numbers.");
                        row.Cells[3].Value = "0";
                        cell3 = "0";
                    }

                    if (System.Text.RegularExpressions.Regex.IsMatch(cell4, "[^0-9]"))
                    {
                        MessageBox.Show("Please enter integer between 0 to 100.");
                        row.Cells[4].Value = "0";
                        cell4 = "0";
                    }

                    if (cell3.Length != 0)
                    {
                        if ((row.Cells[4].Value == null) || (cell4.Length == 0))
                        {
                            row.Cells[4].Value = "0";
                            cell4 = row.Cells[4].Value.ToString();
                            if ((cell1 != "") && (cell3 != ""))
                                row.Cells[5].Value = Int32.Parse(cell1) * Int32.Parse(cell3);
                            else
                            {
                                row.Cells[1].Value = "0";
                                row.Cells[3].Value = "0";
                                cell1 = "0";
                                cell3 = "0";
                            }
                            dataGridView1.Refresh();
                        }
                        else
                            row.Cells[5].Value = (Int32.Parse(cell1)
                                                * (Int32.Parse(cell3)
                                                - (Int32.Parse(cell3)
                                                * Int32.Parse(cell4)) / 100)).ToString();
                    }

                    cell5 = row.Cells[5].Value.ToString();
                    if (cell5.Length == 0)
                    {
                        cell5 = "0";
                    }

                    calculateTotal += Int32.Parse(cell5);

                }

                textBoxTotalAdded.Text = calculateTotal.ToString();
                string tempAdv = textBoxAdv.Text;
                if (tempAdv.Length > 0)
                    textBoxTotalPay.Text = (amntWithTax - Double.Parse(textBoxAdv.Text)).ToString();
                else
                    textBoxTotalPay.Text = amntWithTax.ToString();

                dataGridView1.Refresh();

            }
            catch (NullReferenceException dex)
            {
                textBoxTotalAdded.Text = calculateTotal.ToString();
                string tempAdv = textBoxAdv.Text;
                if (tempAdv.Length > 0)
                    textBoxTotalPay.Text = (amntWithTax - Double.Parse(textBoxAdv.Text)).ToString();
                else
                    textBoxTotalPay.Text = amntWithTax.ToString();

                dataGridView1.Refresh();
                Log.AppError("Exception occured from dataGridView1_EndEditCell. Message: " + dex.ToString());
            }
        }

        private static int calculateTotal;

        private void textBoxPh_TextChanged(object sender, EventArgs e)
        {

            if (System.Text.RegularExpressions.Regex.IsMatch(textBoxPh.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                textBoxPh.Text = "";
            }

        }

        private void buttonDelRow_Click(object sender, EventArgs e)
        {
            try
            {
                int currentRow = dataGridView1.CurrentCell.RowIndex;
                for (int col = 0; col < dataGridView1.Rows[currentRow].Cells.Count; col++)
                {
                    dataGridView1.Rows[currentRow].Cells[col].Value = "";
                    //string value = dataGridView1.Rows[rows].Cells[col].Value.ToString();

                }
                calculateTotal = 0;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string cell5 = "";

                    cell5 = row.Cells[5].Value.ToString();
                    if (cell5.Length == 0)
                    {
                        cell5 = "0";
                    }

                    calculateTotal += Int32.Parse(cell5);

                }
                textBoxTotalAdded.Text = calculateTotal.ToString();
                string tempAdv = textBoxAdv.Text;
                if (tempAdv.Length > 0)
                    textBoxTotalPay.Text = (amntWithTax - Double.Parse(textBoxAdv.Text)).ToString();
                else
                    textBoxTotalPay.Text = amntWithTax.ToString();

                dataGridView1.Refresh();

            }
            catch (NullReferenceException rex)
            {
                textBoxTotalAdded.Text = calculateTotal.ToString();
                string tempAdv = textBoxAdv.Text;
                if (tempAdv.Length > 0)
                    textBoxTotalPay.Text = (amntWithTax - Double.Parse(textBoxAdv.Text)).ToString();
                else
                    textBoxTotalPay.Text = amntWithTax.ToString();

                dataGridView1.Refresh();
                Log.AppError("Exception occured from buttonDelRow_Click, Message: " + rex.ToString());
                return;
            }
        }

        private void buttonAddRow_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();

            dataGridView1.Rows.Add(row);
        }

        private void OpenExistingInvoice()
        {
            OpenFileDialog res = new OpenFileDialog();

            List<string> sales_Details = new List<string>();

            //Filter
            res.Filter = "XML Files|*.xml";

            string xmlToStr = "";

            string Qty, Desc, UnitPrice, Discount, Total;

            //When the user select the file
            if (res.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(res.FileName);
                xmlToStr = sr.ReadToEnd();
                sr.Close();
            }
            using (StringReader stringReader = new StringReader(xmlToStr))
            using (XmlTextReader reader = new XmlTextReader(stringReader))
            {
                bool exists = xmlToStr.Contains("<InvoiceData>");
                if (exists == false)
                {
                    MessageBox.Show("Invalid XML file.",
                               "Important Note",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error,
                               MessageBoxDefaultButton.Button1);
                    return;
                }
                else
                {
                    string thisStr = "";
                    while (reader.Read())
                    {
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Company"))
                        {
                            thisStr = reader.ReadElementContentAsString();
                            textBoxCompany.Text = thisStr;
                        }
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "CustomerID"))
                        {
                            thisStr = reader.ReadElementContentAsString();
                            textBoxCustID.Text = thisStr;
                        }
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "ContactPerson"))
                        {
                            thisStr = reader.ReadElementContentAsString();
                            textBoxContactPer.Text = thisStr;
                        }
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Phone"))
                        {
                            thisStr = reader.ReadElementContentAsString();
                            textBoxPh.Text = thisStr;
                        }
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Email"))
                        {
                            thisStr = reader.ReadElementContentAsString();
                            textBoxEmail.Text = thisStr;
                        }
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Address"))
                        {
                            thisStr = reader.ReadElementContentAsString();
                            textBoxAdd.Text = thisStr;
                        }
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Number"))
                        {
                            thisStr = reader.ReadElementContentAsString();
                            textBoxInvoice.Text = thisStr;
                        }
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Date"))
                        {
                            thisStr = reader.ReadElementContentAsString();
                            dateTimePicker1.Text = thisStr;
                        }
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "EmpPost"))
                        {
                            thisStr = reader.ReadElementContentAsString();
                            comboBox1.SelectedItem = thisStr;
                        }
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "EmpName"))
                        {
                            thisStr = reader.ReadElementContentAsString();
                            textBoxEmpName.Text = thisStr;
                        }

                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Item"))
                        {
                            if (reader.HasAttributes)
                                thisStr = reader.GetAttribute("id");
                            sales_Details.Add(thisStr);
                        }
                        if (reader.Name.ToString() == "Qty")
                        {
                            Qty = reader.ReadString().Trim();
                            sales_Details.Add(Qty);
                        }
                        if (reader.Name.ToString() == "Desc")
                        {
                            Desc = reader.ReadString().Trim();
                            sales_Details.Add(Desc);
                        }
                        if (reader.Name.ToString() == "UnitPrice")
                        {
                            UnitPrice = reader.ReadString().Trim();
                            sales_Details.Add(UnitPrice);
                        }
                        if (reader.Name.ToString() == "Discount")
                        {
                            Discount = reader.ReadString().Trim();
                            sales_Details.Add(Discount);
                        }
                        if (reader.Name.ToString() == "LineTotal")
                        {
                            Total = reader.ReadString().Trim();
                            sales_Details.Add(Total);
                        }

                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "TotalAmount"))
                        {
                            thisStr = reader.ReadElementContentAsString();
                            textBoxTotalAdded.Text = thisStr;
                        }
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "AdvancePaid"))
                        {
                            thisStr = reader.ReadElementContentAsString();
                            textBoxAdv.Text = thisStr;
                        }
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Payable"))
                        {
                            thisStr = reader.ReadElementContentAsString();
                            textBoxTotalPay.Text = thisStr;
                        }
                    }
                }
            }
            int index = 0, noOfRows = sales_Details.Count / 6 - dataGridView1.Rows.Count;
            if (noOfRows > 0)
                while (noOfRows >= 1)
                {
                    DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                    dataGridView1.Rows.Add(row);
                    noOfRows--;
                }

            try
            {
                for (int rows = 0; rows < dataGridView1.Rows.Count; rows++)
                {
                    for (int col = 0; col < dataGridView1.Rows[rows].Cells.Count; col++, index++)
                    {

                        dataGridView1.Rows[rows].Cells[col].Value = sales_Details[index];
                    }
                }
            }
            catch (System.NullReferenceException exp)
            {

                Log.AppError("Exception occured from Open XML. Message: " + exp.ToString());
            }
        }

        public void ExtractPages(string sourcePdfPath, string outputPdfPath,
                                int startPage, int endPage)
        {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;

            try
            {
                // Intialize a new PdfReader instance with the contents of the source Pdf file:
                reader = new PdfReader(sourcePdfPath);

                // For simplicity, I am assuming all the pages share the same size
                // and rotation as the first page:
                sourceDocument = new Document(reader.GetPageSizeWithRotation(startPage));

                // Initialize an instance of the PdfCopyClass with the source 
                // document and an output file stream:
                pdfCopyProvider = new PdfCopy(sourceDocument,
                    new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create));

                sourceDocument.Open();

                // Walk the specified range and add the page copies to the output file:
                for (int i = startPage; i <= endPage; i++)
                {
                    importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                    pdfCopyProvider.AddPage(importedPage);
                }
                sourceDocument.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SendInvoiceEmail()
        {
            if ((lastInvoicePdfFile.Length == 0) || (textBoxEmail.Text.Length == 0))
            {
                MessageBox.Show("No Invoice/Mail address found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            EmailSender.Email("anupam.saha@outlook.com", lastInvoicePdfFile);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveInvoice();
        }
    }
}
