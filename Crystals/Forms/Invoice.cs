using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Data.SqlClient;
using Crystals.Controllers;
using System.Runtime.InteropServices;

namespace Crystals
{
    public partial class Invoice : Form
    {
        #region Datamembers
        private const int HTCAPTION = 0x2;
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCLBUTTONDOWN = 0xA1;

        [DllImportAttribute("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();

        public int AppId;
        public int SalesId;
        public string AppType;
        public double total = 0.0;
      
        public event Action<object, EventArgs> CancelInvoice;

        private System.IO.Stream streamToPrint;

        string streamType;

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]

        private static extern bool BitBlt
        (
             IntPtr hdcDest, // handle to destination DC

             int nXDest, // x-coord of destination upper-left corner

             int nYDest, // y-coord of destination upper-left corner

             int nWidth, // width of destination rectangle

             int nHeight, // height of destination rectangle

             IntPtr hdcSrc, // handle to source DC

             int nXSrc, // x-coordinate of source upper-left corner

             int nYSrc, // y-coordinate of source upper-left corner

             System.Int32 dwRop // raster operation code
         );
        #endregion

        public Invoice()
        {
            InitializeComponent();
            this.TopMost = false;
            this.AppId = -1;
            this.AppType = "";
        }

        protected override CreateParams CreateParams
        {
            get
            {
                // add the drop shadow flag for automatically drawing
                // a drop shadow around the form
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.F4))
            {
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private bool process_invoice(int AppointmentId, string type)
        {
            //Change Appointment Status

            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("Status", "'" + "Complete" + "'");
            d.Add("Type", "'" + type + "'");

            string where = " where Id = " + AppointmentId.ToString();
            Program.db.update("Appointments", d, where);

            // Deduct Automated Stock

            // Get All the Services Done

            d.Clear();

            d.Add("AppointmentId", AppointmentId.ToString());
            SqlDataReader reader = Program.db.get_where_custom("ServiceAppointments", d);

            List<int> ServiceIds = new List<int>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ServiceIds.Add(Convert.ToInt32(reader["ServiceId"]));
                }
            }

            d.Clear();
            reader.Close();

            //Stock Automation

            Stocks stk = new Stocks();
            stk.StockAutomated(ServiceIds);

            //Counter Customer Visits

            Customers cus = new Customers();

            Bookings booking = new Bookings();

            string cusId = booking.GetCustomerId(AppointmentId.ToString());
            if (cusId != null)
            {
                cus.IncrementVisits(cusId);
            }

            //Check For Package
            d.Clear();
            reader.Close();
            d.Add("AppointmentId", AppointmentId.ToString());
            reader = Program.db.get_where_custom("PackageAppointments", d);

            if (reader.HasRows)
            {
                //This is a Package Appointment
                d.Clear();
                reader.Read();
                d.Add("CustomerId", cusId);
                d.Add("PackageId", reader["PackageId"].ToString());
                reader.Close();
                //Increment Sessions
                reader = Program.db.get_where_custom("CustomerPackages", d);
                reader.Read();
                int currSes = Convert.ToInt32(reader["Sessions"]);
                ++currSes;
                d.Clear();
                reader.Close();
                d.Add("Sessions", currSes.ToString());
                string wc = " where CustomerId= " + cusId;
                Program.db.update("CustomerPackages", d, wc);

            }


            //Sales Entry

            d.Clear();
            reader.Close();

            reader = Program.db.get_where("Appointments", AppointmentId.ToString());

            if (reader.HasRows)
            {
                reader.Read();

                int CustomerId = Convert.ToInt32(reader["CustomerId"]);
                int AppId = Convert.ToInt32(reader["Id"]);
                double amount = Convert.ToDouble(reader["Price"]);

                Sales sale = new Sales();
                int sales_Id = sale.SalesEntryAppointment(CustomerId, AppId, ServiceIds, amount);
                if(sales_Id != -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool checkout(int AppointmentId, string type, Form owner)
        {

            this.AppId = AppointmentId;
            this.AppType = type;
            SqlDataReader reader = Program.db.get_where("Appointments", AppointmentId.ToString());

            if (reader.HasRows)
            {
                reader.Read();

                //get customer id
                int CustomerId = Convert.ToInt32(reader["CustomerId"]);
                //App Id
                int AppId = Convert.ToInt32(reader["Id"]);
                //Amount
                double amount = Convert.ToDouble(reader["Price"]);
                //ServiceIDs

                Dictionary<string, string> d = new Dictionary<string, string>();
                d.Add("AppointmentId", AppointmentId.ToString());
                SqlDataReader r = Program.db.get_where_custom("ServiceAppointments", d);

                List<int> ServiceIds = new List<int>();

                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        ServiceIds.Add(Convert.ToInt32(r["ServiceId"]));
                    }
                }

                //Check if it is from a package

                bool frompackage = false;
                d.Clear();
                r.Close();

                d.Add("AppointmentId", AppointmentId.ToString());
                r = Program.db.get_where_custom("PackageAppointments", d);

                if (r.HasRows)
                {
                    frompackage = true;
                }

                //Get Next Sales ID

                int sales_id = Program.db.GetLastInsertedID("Sales");
                sales_id++;

                //init Invoice Form

                this.init_invoice(sales_id, CustomerId, AppId, amount, ServiceIds, frompackage);
                this.ShowDialog(owner);
                return true;

            }
            else
            {
                return false;
            }
            

               
        }

        public void init_invoice(int SalesId, int CustomerId, int AppointmentId, double Amount, List<int> ServiceIds, bool frompackage)
        {
            this.SalesId = SalesId;
            //InitializeComponent();

            InvoiceNoLabel.Text = SalesId.ToString();
            AppId = AppointmentId;
            
            string therapistid = null;
            populate();

            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("AppointmentId", AppointmentId.ToString());
            SqlDataReader reader;
            // DataGridViewRow row = new DataGridViewRow();
            int rows = 0;
            // reader = Program.db.get_where("ServiceAppointments", AppointmentId.ToString());
            if (frompackage == false)
            {
                SqlDataReader therapistreader = Program.db.get_where("Appointments", AppointmentId.ToString());
                if (therapistreader.HasRows)
                {
                    therapistreader.Read();
                    therapistid = therapistreader["TherapistId"].ToString();
                }

                therapistreader = Program.db.get_where("Therapists", therapistid);
                if (therapistreader.HasRows)
                {
                    therapistreader.Read();
                    TherapistNameLabel.Text = therapistreader["Name"].ToString();
                }
                reader = Program.db.get_where_custom("ServiceAppointments", d);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string serviceid = reader["ServiceId"].ToString();
                        SqlDataReader reader1 = Program.db.get_where("Services", serviceid);

                        if (reader1.HasRows)
                        {
                            try
                            {
                                while (reader1.Read())
                                {
                                    DataGridViewRow row = new DataGridViewRow();

                                    gridInvoice.Rows.Add(row);

                                    gridInvoice.Rows[rows].Cells[0].Value = rows;
                                    gridInvoice.Rows[rows].Cells[1].Value = reader1["Name"];
                                    gridInvoice.Rows[rows].Cells[2].Value = reader1["Description"];
                                    gridInvoice.Rows[rows].Cells[3].Value = reader1["Price"];
                                    double price;
                                    Double.TryParse(reader1["Price"].ToString(), out price);

                                    total = total + price;

                                    rows++;
                                }
                            }
                            catch (System.NullReferenceException exp)
                            {
                                Log.AppError(exp.Message);
                            }
                        }
                    }
                }

                reader.Close();
            }
            else
            {
                // Package Invoice
                Packages p = new Packages();
                gridInvoice.Rows.Add();
                gridInvoice.Rows[rows].Cells[0].Value = rows;
                gridInvoice.Rows[rows].Cells[1].Value = p.GetPackageName(AppointmentId.ToString());
                gridInvoice.Rows[rows].Cells[2].Value = p.GetPackageDesc(AppointmentId.ToString());
                gridInvoice.Rows[rows].Cells[3].Value = p.GetPackagePrice(AppointmentId.ToString());
                double price = Convert.ToDouble(p.GetPackagePrice(AppointmentId.ToString()));

                total = total + price;

                //Att: Utsav
                //metroLabel9.Text = "";
            }

            double servicetax = 14.5;

            Settings company = new Settings();
            SqlDataReader c = company.GetCompanyDetails();

            if (c.HasRows)
            {
                c.Read();
                double.TryParse(c["SvcTaxPercentage"].ToString(), out servicetax);
            }

            inSTax.Text = servicetax.ToString();
   
            inTotal.Text = total.ToString();
           
            double totalwithtax = total + (total * (servicetax/100));
            inPayable.Text = totalwithtax.ToString();
            reader = Program.db.get_where("Customers", CustomerId.ToString());

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    inCusName.Text = reader["Name"].ToString();
                    inCusPh.Text = reader["Phone"].ToString();
                    inDate.Text = System.DateTime.Today.ToString("dd/MM/yyyy");
                }
            }
            reader.Close();
        }



        private void printDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            System.Drawing.Image image = System.Drawing.Image.FromStream(this.streamToPrint);

            int x = e.MarginBounds.X;

            int y = e.MarginBounds.Y;

            int width = image.Width;

            int height = image.Height;

            if ((width / e.MarginBounds.Width) > (height / e.MarginBounds.Height))
            {
                width = e.MarginBounds.Width;
                height = image.Height * e.MarginBounds.Width / image.Width;
            }
            else
            {
                height = e.MarginBounds.Height;
                width = image.Width * e.MarginBounds.Height / image.Height;
            }

            System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(x, y, width, height);

            e.Graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, System.Drawing.GraphicsUnit.Pixel);
        }

        private void print_job()
        {

            Graphics g1 = this.CreateGraphics();

            Image MyImage = new Bitmap(panel2.ClientRectangle.Width, panel2.ClientRectangle.Height, g1);

            //CaptureScreen();

            Graphics g2 = Graphics.FromImage(MyImage);

            IntPtr dc1 = g1.GetHdc();

            IntPtr dc2 = g2.GetHdc();

            BitBlt(dc2, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height, dc1, 0, 0, 13369376);

            g1.ReleaseHdc(dc1);

            g2.ReleaseHdc(dc2);

            var path = Path.GetTempPath();

            MyImage.Save(path + "/PrintPage.jpg", ImageFormat.Jpeg);

            FileStream fileStream = new FileStream(path + "/PrintPage.jpg", FileMode.Open, FileAccess.Read);

            StartPrint(fileStream, "Image");

            // fileStream.Close();

            //Create a PDF 
            iTextSharp.text.Rectangle pageSize = null;

            //string currFolder = System.IO.Directory.GetCurrentDirectory();
            //string foldername = DateTime.Today.ToString("mm-yyyy");
            //string desFolder = currFolder + "\\Sales Invoice\\" + foldername;
            //long timestamp = DateTime.Today.Ticks;

            string desFolder = RegistryLicense.GetInvoiceFolderLocation();

     
            string timestamp = DateTime.Now.ToString("MMddyyyyHHmmss");

            //Get Temp Path

            

            //Get Customer Name
            Customers cus = new Customers();
            Bookings app = new Bookings();
            string cusId = app.GetCustomerId(this.AppId.ToString());
            string cusName = cus.GetCustomerName(cusId);

            //Build file name
            string dstFilename = desFolder + timestamp + "_" + cusName + ".pdf";

            using (var srcImage = new Bitmap(fileStream))
            {
                pageSize = new iTextSharp.text.Rectangle(0, 0, srcImage.Width, srcImage.Height);
            }
            using (var ms = new MemoryStream())
            {
                var document = new iTextSharp.text.Document(pageSize, 0, 0, 0, 0);
                iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms).SetFullCompression();
                document.Open();
                var image = iTextSharp.text.Image.GetInstance(fileStream.Name);
                document.Add(image);
                document.Close();

                File.WriteAllBytes(dstFilename, ms.ToArray());
                
            }
            fileStream.Close();

            if (System.IO.File.Exists(path + "/PrintPage.jpg"))
            {
                
                System.IO.File.Delete(path + "/PrintPage.jpg");
            }

            //Save the PDF path in Invoice Table

            Sales sale = new Sales();
            sale.SetInvoicePath(SalesId.ToString(), dstFilename);

            //Save The Discount Given against the Sale
            //inDisVal.Text

            if (!string.IsNullOrWhiteSpace(inDisVal.Text))
            {
                sale.SetDiscount(SalesId.ToString(), Convert.ToDouble(inDisVal.Text));
            }
           
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            this.process_invoice(this.AppId, this.AppType);

            this.print_job();

            this.Hide();
        }

        public void StartPrint(Stream streamToPrint, string streamType)
        {
            this.printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);

            this.streamToPrint = streamToPrint;

            this.streamType = streamType;

            System.Windows.Forms.PrintDialog PrintDialog1 = new PrintDialog();

            PrintDialog1.AllowSomePages = true;

            PrintDialog1.ShowHelp = true;

            PrintDialog1.Document = printDoc;

            DialogResult result = PrintDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                printDoc.Print();
            }
        }

        public void populate()
        {
            SqlDataReader reader = Program.db.get("Company");
            string logo = "";
            string name = "";
            string ph = "";
            string address = "";
            if (reader.HasRows)
            {
                reader.Read();
                logo = reader["Logo"].ToString();
                name = reader["Name"].ToString();
                ph = reader["Phone"].ToString();
                address = reader["Address"].ToString();
                inSTno.Text = reader["STaxNumber"].ToString();
            }
            reader.Close();
            if (string.IsNullOrWhiteSpace(logo) == false && File.Exists(logo) == true)
            {
                inLogo.ImageLocation = logo;
            }
            //inComAdd.MaximumSize = new Size(100, 100);
            //inComAdd.AutoSize = true;
            inCName.Text = name;
            InCPh.Text = ph;
            inComAdd.Text = address;
            cbPaymentMode.SelectedItem = "Cash";

           // metroGrid1.ClearSelection();
        }

        private void inDPer_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(inDPer.Text) == false)
            {
                Settings company = new Settings();
                SqlDataReader reader = company.GetCompanyDetails();
                double maxdis = 0.0;
                if (reader.HasRows)
                {
                    reader.Read();
                    Double.TryParse(reader["MaxDiscount"].ToString(), out maxdis);
                }

                string dis = inDPer.Text;
                double discount = Convert.ToDouble(dis);
                if (discount <= maxdis)
                {
                    double discountval = (discount / 100) * total;
                    discountval = Math.Round(discountval, 2);

                    inDisVal.Text = discountval.ToString();
                    double tax = Convert.ToDouble(inSTax.Text);
                    discountval = Convert.ToDouble(inDisVal.Text);
                    double disprice = Math.Round((total - discountval), 2);
                    double totalpay = disprice + (disprice * (tax / 100));
                    totalpay = Math.Round(totalpay, 2);
                    inPayable.Text = totalpay.ToString();
                }
                else
                {
                    inDisVal.Text = "0";
                    inDPer.Text = "0";
                    MessageBox.Show("Max discount is set to: " + maxdis.ToString(), "Invalid discount rate");
                    inDPer.WithError = true;
                    return; 
                }
            }
            else
            {
                inDisVal.Text = "0";
                inDPer.Text = "0";
            }
        }

        private void inSTax_TextChanged(object sender, EventArgs e)
        {
        }

        private void cbPaymentMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Customers cus = new Customers();
            Bookings app = new Bookings();
            string cusId = app.GetCustomerId(this.AppId.ToString());
            Dictionary<string, string> SalesData = new Dictionary<string, string>();
            SalesData.Add("PaymentMode", "'" + cbPaymentMode.SelectedItem.ToString() + "'");
            Program.db.update("Sales", SalesData, " where Id=" + this.SalesId.ToString());
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (CancelInvoice != null)
            {
                CancelInvoice(this, null);
            }

            this.Close();
            this.Dispose(true);
        }

        private void inDPer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!UIUtility.IsValidDecimalNumber(sender, e))
            {
                e.Handled = true;
            }
        }
    }
}
