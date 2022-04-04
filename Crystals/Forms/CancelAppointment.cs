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
using System.Data.SqlClient;
using MetroFramework;
using Crystals.Controllers;
using System.Runtime.InteropServices;

namespace Crystals
{
    public partial class CancelAppointment : Form
    {
        private const int HTCAPTION = 0x2;
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCLBUTTONDOWN = 0xA1;

        [DllImportAttribute("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();

        private int AppId;
        
        public event Action<object, EventArgs> CancelCancellation;

        public CancelAppointment()
        {
            InitializeComponent();
            this.TopMost = false;
            AppId = 0;
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

        public void set_appid(int appid)
        {
            this.AppId = appid;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string remarks = "Blank"; //textRemarks.Text;
            string sessiondate;
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("Status", "'" + "Canceled" + "'");
            // changes appointment status from booked to canceled
            Program.db.update("Appointments", d, " where Id=" + this.AppId.ToString() + "");
            SqlDataReader reader = Program.db.get_where("Appointments", this.AppId.ToString());
            while (reader.Read())
            {
                sessiondate = reader["SessionDate"].ToString();

                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("BookingId", "'" + this.AppId + "'");
                data.Add("Date", "'" + sessiondate + "'");

                Program.db.insert("CancelAppointments", data);

            }


            Dictionary<string, string> dt = new Dictionary<string, string>();
            d.Add("Remarks", "'" + remarks + "'");
            Program.db.update("CancelAppointments", dt, " where id=" + this.AppId.ToString());

            reader.Close();

            //get app Id
            reader = Program.db.get_where("CancelAppointments", this.AppId.ToString());
            if (reader.HasRows)
            {
                reader.Read();
                Customers cus = new Customers();
                Bookings app = new Bookings();
                string CustomerId = app.GetCustomerId(reader["BookingId"].ToString());
                string cusname = cus.GetCustomerName(CustomerId.ToString());

                string msg = "Date: " + DateTime.Now.ToString("yyyy-MM-dd") + "Customer Name: " + cusname + " Reason: " + remarks;
                AdminNotify.SendEmail("Appointment Canceled", msg);
                reader.Close();
            }
            this.Close();
        }

        private void CancelAppointment_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //textRemarks.Focus();
            timer1.Stop();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (CancelCancellation != null)
            {
                CancelCancellation(this, null);
            }

            this.Close();
            this.Dispose(true);
        }
    }
}
