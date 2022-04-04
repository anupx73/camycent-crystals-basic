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
using MetroFramework;
using Crystals.Controllers;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using MetroFramework.Controls;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;

namespace Crystals
{
    public partial class MainForm : Form
    {
        #region DataMembers
        private const int HTCAPTION = 0x2;
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int CS_DBLCLKS = 0x8;
        public static int salesrow = 0;

        [DllImportAttribute("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();

        private string loggedinUserLevel = string.Empty;
        private string loggedinUserName = string.Empty;
        private SupportForm support = null;
        private NotifyForm notifyForm = null;
        public static int PackListCount = 0;
        public static Boolean isPackageAppointment = false;
        public static List<int> serviceIds;
        public static int therapistId;
        private System.Windows.Forms.Timer firstRunNotification = null;
        #endregion

        #region Constructors

        public MainForm()
        {
            InitializeComponent();
            this.TopMost = false;
            this.Text = "Crystals";
            this.Icon = Properties.Resources.app_icon;
        }

        public MainForm(string userLevel, string userName)
        {
            InitializeComponent();
            this.TopMost = false;
            this.Text = "Crystals";
            this.Icon = Properties.Resources.app_icon;
            loggedinUserLevel = userLevel;
            loggedinUserName = userName;
        }


        #endregion

        public void SetUserLevel(string userLevel, string userName)
        {
            loggedinUserLevel = userLevel;
            loggedinUserName = userName;
        }

        #region FormLoaders
        private void LoadAppoinments()
        {
            listBoxPacks.Enabled = false;
            listBoxPacks.Items.Clear();
            listBoxPacks.Items.Add("No Packages Found");
            aptRadioCustExist.Enabled = false;
            aptRadioCustNew.Enabled = false;
            //buttonSave.Enabled = false;
            UIUtility.DisableButton(aptButtonSave, true);
            //buttonAppointmentDelete.Enabled = true;
            UIUtility.DisableButton(aptButtonClear, false);
            aptRadioCustExist.Select();
            ServiceListCount = 0;
            aptDTBooking.Value = DateTime.Today;
            aptDTFilter.Value = DateTime.Today;

            listBoxPackService.ClearSelected();
            aptTextCustName.Text = "";
            aptTxtCustEmail.Text = "";
            aptTxtCustPh.Text = "";
            apttxtCustVisit.Text = "";
            aptTxtCustAddr.Text = "";
            apttxtNotes.Text = "";
            aptTxtSearchPhone.Text = "";
            aptLabelAvailibility.Text = "";

            aptTxtTimeHr.Text = DateTime.Now.ToString("HH");
            aptTxtTimeMin.Text = DateTime.Now.ToString("mm");
            aptLabelPWTax.Text = "0";

            //Populate Appoints Tab

            this.populateAppointments();
            SqlDataReader reader;

            SList = new List<string>();

            reader = Program.db.get("Services");

            if (reader.HasRows)
            {
                listBoxPackService.Items.Clear();
                while (reader.Read())
                {
                    listBoxPackService.Items.Add(reader["Name"]);
                }
                reader.Close();
            }

            reader = Program.db.get("Therapists");
            if (reader.HasRows)
            {
                aptComboThep.Items.Clear();
                while (reader.Read())
                {
                    aptComboThep.Items.Add(reader["Name"]);
                }
                reader.Close();
            }

            aptTextCustName.WithError = false;
            aptTxtCustEmail.WithError = false;
            aptTxtCustPh.WithError = false;

            aptComboThep.Select();
            aptComboThep.Enabled = false;
            aptComboThep.PromptText = "Choose Therapist";
            listBoxHistory.Items.Clear();
            listBoxHistory.Items.Add("No History Available");
            gridBookings.ClearSelection();

            //aptTxtSearchPhone.Focus();
            if (metroTabControl.SelectedIndex == 0)
            {
                timerDefaultFocus.Start();
            }
        }

        private void LoadUsers()
        {
            Settings s = new Settings();
            SqlDataReader reader = s.GetCompanyDetails();
            if (reader.HasRows)
            {
                reader.Read();
                if (!string.IsNullOrWhiteSpace(reader["Logo"].ToString()) && System.IO.File.Exists(reader["Logo"].ToString()))
                {
                    pictureBoxPhoto.Image = new Bitmap(reader["Logo"].ToString());
                    pictureBoxPhoto.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                settTxtName.Text = reader["Name"].ToString();
                settTxtPhone.Text = reader["Phone"].ToString();
                settTxtEmail.Text = reader["Email"].ToString();
                settTxtAddr.Text = reader["Address"].ToString();
                settTxtAdminEmail.Text = reader["AdminEmail"].ToString();
                settTxtServiceTax.Text = reader["SvcTaxPercentage"].ToString();
                settTxtSvcTax.Text = reader["STaxNumber"].ToString();
                settTxtCommi.Text = reader["Commision"].ToString();
                settTxtMaxDisc.Text = reader["MAxDiscount"].ToString();
            }

            reader.Close();
            if (loggedinUserLevel == "Store Manager" || loggedinUserLevel == "Customer Manager")
                return;

            reader = Program.db.get("users");
            if (reader.HasRows)
            {
                try
                {
                    while (reader.Read())
                    {
                        metroComboBox11.Items.Add(reader["Username"]);
                    }
                }
                catch (System.NullReferenceException exp)
                {
                    Log.AppError(exp.Message);
                }
            }
        }

        private void LoadCustomers()
        {
            string custid = null, packageid = null;
            SqlDataReader reader = Program.db.get("CustomerPackages");

            if (reader.HasRows)
            {
                int row = 0;
                gridPackCustomers.Rows.Clear();
                gridPackCustomers.AllowUserToAddRows = true;
                while (reader.Read())
                {
                    try
                    {
                        gridPackCustomers.Rows.Add();
                        gridPackCustomers.Rows[row].Cells[0].Value = reader["Id"].ToString();
                        packageid = reader["PackageId"].ToString();
                        SqlDataReader reader2 = Program.db.get_where("Packages", packageid);
                        reader2.Read();
                        gridPackCustomers.Rows[row].Cells[1].Value = reader2["Name"];
                        custid = reader["CustomerId"].ToString();
                        SqlDataReader reader1 = Program.db.get_where("Customers", custid);
                        reader1.Read();
                        gridPackCustomers.Rows[row].Cells[2].Value = reader1["Name"].ToString();
                        gridPackCustomers.Rows[row].Cells[3].Value = Convert.ToDateTime(reader["StartDate"]).ToString("yyyy-MM-dd");
                        gridPackCustomers.Rows[row].Cells[4].Value = Convert.ToDateTime(reader["ExpireDate"]).ToString("yyyy-MM-dd");
                        gridPackCustomers.Rows[row].Cells[5].Value = reader["Sessions"].ToString();
                        List<int> val = new List<int>();
                        Packages package = new Packages();
                        val = package.ValidServices(custid, packageid);
                        if (val.Count <= 0)
                        {
                            gridPackCustomers.Rows[row].Cells[6].Value = "Invalid";
                        }
                        else
                        {
                            gridPackCustomers.Rows[row].Cells[6].Value = "Valid";
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.AppError(ex.Message);
                    }

                    row++;
                }
            }
            gridPackCustomers.AllowUserToAddRows = false;
            gridPackCustomers.ClearSelection();

            Populate_AllCustomers(custid, packageid);
        }
        private void LoadBilling()
        {
            metroGrid11.PerformLayout();

            SqlDataReader reader = Program.db.get_desc("Customers");

            if (reader.HasRows)
            {
                int row = 0;
                metroGrid1.Rows.Clear();
                metroGrid1.AllowUserToAddRows = true;
                while (reader.Read())
                {
                    try
                    {
                        metroGrid1.Rows.Add();
                        metroGrid1.Rows[row].Cells[0].Value = reader["Id"].ToString();
                        metroGrid1.Rows[row].Cells[1].Value = reader["Name"].ToString();
                        metroGrid1.Rows[row].Cells[2].Value = reader["Phone"].ToString();

                        metroGrid1.Rows[row].Cells[3].Value = reader["Email"].ToString();
                        metroGrid1.Rows[row].Cells[4].Value = reader["Address"].ToString();
                        metroGrid1.Rows[row].Cells[5].Value = reader["Visits"].ToString();
                    }
                    catch (Exception ex)
                    {
                        Log.AppError(ex.Message);
                    }

                    row++;
                }


            }
            metroGrid1.AllowUserToAddRows = false;
            reader.Close();

            Bookings app = new Bookings();

            reader = app.GetLiveAppointments();

            Customers cus = new Customers();

            if (reader.HasRows)
            {
                metroGrid11.Rows.Clear();
                metroGrid11.AllowUserToAddRows = true;
                int row = 0;
                while (reader.Read())
                {
                    metroGrid11.Rows.Add();
                    metroGrid11.Rows[row].Cells[0].Value = reader["Id"].ToString();
                    metroGrid11.Rows[row].Cells[1].Value = cus.GetCustomerName(reader["CustomerId"].ToString());
                    metroGrid11.Rows[row].Cells[2].Value = reader["StartTime"].ToString();
                    metroGrid11.Rows[row].Cells[3].Value = reader["EndTime"].ToString();
                    metroGrid11.Rows[row].Cells[4].Value = reader["Price"].ToString();
                    row++;
                }
                metroGrid11.AllowUserToAddRows = false;
            }

            reader.Close();

            reader = Program.db.get("Packages");

            if (reader.HasRows)
            {
                metroComboBox1.Items.Clear();
                while (reader.Read())
                {
                    metroComboBox1.Items.Add(reader["Name"].ToString());
                }
            }

            reader.Close();

            this.populateSessions();
        }

        private void LoadPackages()
        {
            if (loggedinUserLevel == "Customer Manager")
                return;

            Packages package = new Packages();
            SqlDataReader reader = Program.db.get_desc("Packages");
            if (reader.HasRows)
            {
                gridPackageList.AllowUserToAddRows = true;
                gridPackageList.Rows.Clear();
                try
                {
                    int rows = 0;
                    while (reader.Read())
                    {
                        DataGridViewRow row = (DataGridViewRow)gridPackageList.Rows[0].Clone();

                        gridPackageList.Rows.Add(row);

                        gridPackageList.Rows[rows].Cells[0].Value = reader["Id"];
                        gridPackageList.Rows[rows].Cells[1].Value = reader["Name"];
                        gridPackageList.Rows[rows].Cells[2].Value = reader["Price"];
                        gridPackageList.Rows[rows].Cells[3].Value = reader["Duration"];
                        gridPackageList.Rows[rows].Cells[4].Value = reader["Value"];
                        gridPackageList.Rows[rows].Cells[5].Value = reader["Description"];


                        rows++;
                    }

                    gridPackageList.AllowUserToAddRows = false;
                }
                catch (System.NullReferenceException exp)
                {
                    Log.AppError(exp.Message);
                }
            }

            reader.Close();
            Services obj = new Services();
            reader = Program.db.get("Services");

            if (reader.HasRows)
            {
                try
                {
                    gridPckSvcIncl.Rows.Clear();
                    int rows = 0;
                    gridPckSvcIncl.AllowUserToAddRows = true;
                    while (reader.Read())
                    {
                        DataGridViewRow row = (DataGridViewRow)gridPckSvcIncl.Rows[0].Clone();

                        gridPckSvcIncl.Rows.Add(row);

                        gridPckSvcIncl.Rows[rows].Cells[0].Value = reader["Id"];
                        gridPckSvcIncl.Rows[rows].Cells[1].Value = reader["Name"];
                        gridPckSvcIncl.Rows[rows].Cells[2].Value = reader["Price"];
                        gridPckSvcIncl.Rows[rows].Cells[3].Value = "";
                        gridPckSvcIncl.Rows[rows].Cells[4].Value = false;

                        rows++;
                    }

                    gridPckSvcIncl.AllowUserToAddRows = false;
                }
                catch (System.NullReferenceException exp)
                {
                    Log.AppError(exp.Message);
                }
            }
            reader.Close();

            //Load Customers in the grid.
            reader = Program.db.get_desc("Customers");

            if (reader.HasRows)
            {
                gridPckCustomers.AllowUserToAddRows = true;
                gridPckCustomers.Rows.Clear();
                try
                {
                    int rows = 0;
                    while (reader.Read())
                    {
                        //  DataGridViewRow row = (DataGridViewRow)gridPackageList.Rows[0].Clone();

                        gridPckCustomers.Rows.Add();

                        gridPckCustomers.Rows[rows].Cells[0].Value = reader["Id"].ToString();
                        gridPckCustomers.Rows[rows].Cells[1].Value = reader["Name"].ToString();
                        gridPckCustomers.Rows[rows].Cells[2].Value = reader["Phone"].ToString();
                        gridPckCustomers.Rows[rows].Cells[3].Value = reader["Visits"].ToString();

                        rows++;
                    }

                    gridPackageList.AllowUserToAddRows = false;
                }
                catch (System.NullReferenceException exp)
                {
                    Log.AppError(exp.Message);
                }
            }

            gridPackageList.ClearSelection();
            gridPckCustomers.ClearSelection();
        }

        private void LoadSales()// implement sales load here
        {
            double amount, total = 0, totalwithtax = 0, tax = 0, monthtax = 0, totalmonthtax = 0;
            // string discount=null;
            Sales s = new Sales();
            SqlDataReader reader = s.GetSalesByDate(DateTime.Today);
            double cashsale = 0.0, cardsale = 0.0, amt = 0.0, discount = 0.0;
            string packageId = null;
            if (reader.HasRows)
            {

                while (reader.Read())
                {

                    packageId = reader["PackageId"].ToString();
                    if (packageId == "")
                    {

                        double.TryParse(reader["Discount"].ToString(), out discount);



                        amount = Convert.ToDouble(reader["amount"]);

                        total = total + amount - discount;

                        string paymentmode = reader["PaymentMode"].ToString();
                        if (paymentmode == "Cash")
                        {
                            amt = Convert.ToDouble(reader["Amount"]);
                            cashsale = cashsale + amt - discount;




                        }
                        else
                        {
                            amt = Convert.ToDouble(reader["Amount"]);


                            cardsale = cardsale + amt - discount;

                        }
                    }
                    else
                    {

                    }
                }
            }

            double servicetax = 14.5;

            Settings company = new Settings();
            SqlDataReader c = company.GetCompanyDetails();

            if (c.HasRows)
            {
                c.Read();
                double.TryParse(c["SvcTaxPercentage"].ToString(), out servicetax);
            }

            salLabelDailyWTax.Text = "\u20B9 " + total.ToString();
            double MonthSales = s.GetSalesByMonth(DateTime.Today);
            salLabelMonthWTax.Text = "\u20B9 " + MonthSales.ToString();
            tax = total * (servicetax / 100);
            totalwithtax = total + tax;
            monthtax = MonthSales * (servicetax / 100);
            totalmonthtax = MonthSales + monthtax;
            salLabelMonthTax.Text = "\u20B9 " + totalmonthtax.ToString("n2");
            salLabelDailyTax.Text = "\u20B9 " + totalwithtax.ToString("n2");
            salLabelDailyTotal.Text = "\u20B9 " + cashsale.ToString("n2");
            salLabelMonthTotal.Text = "\u20B9 " + cardsale.ToString("n2");
            reader.Close();

            populateSalesComm();


            populate_Sales();

        }


        private void LoadServices()      // implement service load here
        {
            if (loggedinUserLevel == "Customer Manager")
                return;

            //txtServiceId.Text = "";
            svcTxtDesc.Text = "";
            svcTxtName.Text = "";
            svcTxtDur.Text = "";
            svcTxtCost.Text = "";

            Services ob = new Services();

            SqlDataReader reader = Program.db.get_desc("Services");
            if (reader.HasRows)
            {
                try
                {
                    int rows = 0;
                    svcGridList.AllowUserToAddRows = true;
                    svcGridList.Rows.Clear();

                    while (reader.Read())
                    {
                        svcGridList.Rows.Add();
                        svcGridList.Rows[rows].Cells[0].Value = reader["Id"];
                        svcGridList.Rows[rows].Cells[1].Value = reader["Name"];
                        svcGridList.Rows[rows].Cells[2].Value = reader["AHTime"];
                        svcGridList.Rows[rows].Cells[3].Value = reader["Price"];
                        svcGridList.Rows[rows].Cells[4].Value = reader["Description"];
                        rows++;
                    }

                    svcGridList.AllowUserToAddRows = false;
                }
                catch (System.NullReferenceException exp)
                {
                    Log.AppError(exp.Message);
                }
            }

            svcGridList.ClearSelection();
            Stocks stk = new Stocks();
            reader = Program.db.get_desc("items");
            if (reader.HasRows)
            {
                try
                {
                    int rows = 0;
                    svcGridItem.Rows.Clear();
                    svcGridItem.AllowUserToAddRows = true;
                    while (reader.Read())
                    {
                        svcGridItem.Rows.Add();

                        svcGridItem.Rows[rows].Cells[0].Value = reader["Id"].ToString();
                        svcGridItem.Rows[rows].Cells[1].Value = reader["ItemName"].ToString();
                        string unit = reader["Unit"].ToString();
                        if (unit == "LTRS")
                        {
                            svcGridItem.Rows[rows].Cells[2].Value = "ML";
                        }
                        else
                        {
                            svcGridItem.Rows[rows].Cells[2].Value = reader["Unit"].ToString();
                        }
                        rows++;
                    }
                    svcGridItem.AllowUserToAddRows = false;
                }
                catch (System.NullReferenceException exp)
                {
                    Log.AppError(exp.Message);

                }
            }
            reader.Close();
        }

        private void LoadStocks()
        {
            this.populateStock();
        }


        private void LoadUserTabs()
        {
            this.metroTabControl.SelectedTab = metroTabAppointment;
            //remove billing tab
            metroTabControl.TabPages.Remove(metroTabBilling);
            if (loggedinUserLevel == "Store Manager")
            {
                metroTabControl.TabPages.Remove(metroTabSettings);
                metroTabControl.TabPages.Remove(metroTabTherapist);
            }
            else if (loggedinUserLevel == "Customer Manager")
            {
                metroTabControl.TabPages.Remove(metroTabServices);
                metroTabControl.TabPages.Remove(metroTabPackages);
                metroTabControl.TabPages.Remove(metroTabSettings);
                metroTabControl.TabPages.Remove(metroTabTherapist);
            }
        }

        private void LoadTherapists()// implement Therapist load here
        {
            if (loggedinUserLevel == "Store Manager" || loggedinUserLevel == "Customer Manager")
                return;


            SqlDataReader reader = Program.db.get_desc("Therapists");
            if (reader.HasRows)
            {
                try
                {
                    grdiTherapist.Rows.Clear();
                    grdiTherapist.AllowUserToAddRows = true;
                    int rows = 0;
                    while (reader.Read())
                    {
                        grdiTherapist.Rows.Add();
                        grdiTherapist.Rows[rows].Cells[0].Value = reader["Id"];
                        grdiTherapist.Rows[rows].Cells[1].Value = reader["Name"];
                        grdiTherapist.Rows[rows].Cells[2].Value = reader["Status"];
                        grdiTherapist.Rows[rows].Cells[3].Value = reader["Phone"];
                        grdiTherapist.Rows[rows].Cells[4].Value = reader["Email"];
                        grdiTherapist.Rows[rows].Cells[5].Value = reader["Address"];
                        rows++;
                    }
                    grdiTherapist.AllowUserToAddRows = false;
                }
                catch (System.NullReferenceException exp)
                {
                    Log.AppError(exp.Message);
                }
            }
            reader.Close();
            grdiTherapist.ClearSelection();

            populateCommisions();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            LoadUserTabs();
            svcBtnDel.Visible = false;

            notifyForm = new NotifyForm();
            notifyForm.MakeActiveParent += notifyForm_MakeActiveParent;

            metroLabelLoggedInUser.Text = "Logged in as: " + loggedinUserName + " (" + loggedinUserLevel + ")";
            //UIUtility.DisableButton(svcBtnSave, true);
            UIUtility.DisableButton(btnBuyPackage, true);
            UIUtility.DisableButton(button2, true);
            UIUtility.DisableButton(btnCheckOut, true);

            aptLabelPriceSymbol.Text = "\u20B9";
            aptLabelPriceWSymbol.Text = "\u20B9";
            svcLabelPrice.Text = "\u20B9";
            pckLabelsymbol1.Text = "\u20B9";
            pckLabelsymbol2.Text = "\u20B9";

            PopulateCurrentDateTimeForAppointment();
            LoadAppoinments();
            LoadBilling();
            LoadPackages();
            LoadSales();
            LoadStocks();
            LoadTherapists();
            LoadServices();
            LoadUsers();
            LoadCustomers();

            if (RegistryLicense.IsFirstRun())
            {
                metroTabControl.SelectedTab = metroTabSettings;
                firstRunNotification = new System.Windows.Forms.Timer();
                firstRunNotification.Interval = 500;
                firstRunNotification.Tick += firstRunNotification_Tick;
                firstRunNotification.Start();
            }
        }

        void firstRunNotification_Tick(object sender, EventArgs e)
        {
            firstRunNotification.Stop();
            string notifyMessage = "This is your first run of Crystals. Please setup your Company details.\r\nNote: It's highly recommended that you change the default Username/Password now.";
            notifyForm.ShowNotification(null, notifyMessage, "Setup Crystals");
        }
        #endregion

        #region Populators
        private void Populate_AllCustomers(string CustomerId, string PackageId)
        {
            SqlDataReader reader3 = Program.db.get("Customers");

            if (reader3.HasRows)
            {
                int row = 0;
                gridAllCustomers.Rows.Clear();
                gridAllCustomers.AllowUserToAddRows = true;
                while (reader3.Read())
                {
                    try
                    {
                        gridAllCustomers.Rows.Add();
                        gridAllCustomers.Rows[row].Cells[0].Value = reader3["Id"].ToString();


                        gridAllCustomers.Rows[row].Cells[1].Value = reader3["Name"];

                        gridAllCustomers.Rows[row].Cells[2].Value = reader3["Phone"].ToString();
                        gridAllCustomers.Rows[row].Cells[3].Value = reader3["Email"].ToString();
                        gridAllCustomers.Rows[row].Cells[4].Value = reader3["Address"].ToString();
                        gridAllCustomers.Rows[row].Cells[5].Value = reader3["Visits"].ToString();

                    }
                    catch (Exception ex)
                    {
                        Log.AppError(ex.Message);
                    }

                    row++;
                }
                gridAllCustomers.AllowUserToAddRows = false;
            }

            gridAllCustomers.ClearSelection();
        }

        private void populateCustomers()
        {
            gridAllCustomers.PerformLayout();

            SqlDataReader reader = Program.db.get_desc("Customers");

            if (reader.HasRows)
            {
                int row = 0;
                gridAllCustomers.Rows.Clear();
                gridAllCustomers.AllowUserToAddRows = true;
                while (reader.Read())
                {
                    try
                    {
                        gridAllCustomers.Rows.Add();
                        gridAllCustomers.Rows[row].Cells[0].Value = reader["Id"].ToString();
                        gridAllCustomers.Rows[row].Cells[1].Value = reader["Name"].ToString();
                        gridAllCustomers.Rows[row].Cells[2].Value = reader["Phone"].ToString();

                        gridAllCustomers.Rows[row].Cells[3].Value = reader["Email"].ToString();
                        gridAllCustomers.Rows[row].Cells[4].Value = reader["Address"].ToString();
                        gridAllCustomers.Rows[row].Cells[5].Value = reader["Visits"].ToString();
                    }
                    catch (Exception ex)
                    {
                        Log.AppError(ex.Message);
                    }

                    row++;
                }


            }
            metroGrid1.AllowUserToAddRows = false;
            reader.Close();
        }

        private void populateSalesComm()
        {
            SqlDataReader reader = Program.db.get_desc("Therapists");

            if (reader.HasRows)
            {
                DateTime date = DateTime.Today;
                string month = date.ToString("MMMM");
                gridSalesCommision.Rows.Clear();
                int row = 0;
                gridSalesCommision.AllowUserToAddRows = true;
                Therapists t = new Therapists();
                while (reader.Read())
                {
                    double comm = t.GetTotalCommissionByMonth(reader["Id"].ToString(), date);
                    double Sales = t.GetTotalSalesByMonth(reader["Id"].ToString(), date);
                    gridSalesCommision.Rows.Add();
                    gridSalesCommision.Rows[row].Cells[0].Value = reader["Name"].ToString();
                    gridSalesCommision.Rows[row].Cells[1].Value = month;
                    gridSalesCommision.Rows[row].Cells[2].Value = Sales.ToString();
                    gridSalesCommision.Rows[row].Cells[3].Value = comm.ToString();
                    row++;
                }

                gridSalesCommision.AllowUserToAddRows = false;
            }
        }


        void populateStock()
        {
            DateTime now1 = DateTime.Now;
            string month = now1.ToString("MMMM");
            Dictionary<string, string> data1 = new Dictionary<string, string>();
            data1.Add("Month", "'" + month + "'");

            SqlDataReader reader = Program.db.get_where_custom("Stocks", data1);
            string id;
            //SqlDataReader reader1;
            if (reader.HasRows)
            {
                gridStock.Rows.Clear();
                gridStock.AllowUserToAddRows = true;
                try
                {
                    int rows = 0;
                    while (reader.Read())
                    {
                        DataGridViewRow row = (DataGridViewRow)gridStock.Rows[0].Clone();
                        SqlDataReader reader1;
                        gridStock.Rows.Add();
                        id = reader["itemId"].ToString();
                        reader1 = Program.db.get_where("items", id);
                        gridStock.Rows[rows].Cells[0].Value = reader["itemId"];
                        while (reader1.Read())
                        {
                            gridStock.Rows[rows].Cells[1].Value = reader1["ItemName"];
                        }
                        gridStock.Rows[rows].Cells[2].Value = reader["Month"];
                        gridStock.Rows[rows].Cells[3].Value = reader["OpeningStock"];
                        gridStock.Rows[rows].Cells[4].Value = reader["ClosingStock"];
                        gridStock.Rows[rows].Cells[5].Value = reader["CurrentStock"];
                        rows++;
                    }

                    gridStock.AllowUserToAddRows = false;
                }
                catch (System.NullReferenceException exp)
                {
                    Log.AppError(exp.Message);
                }
            }
            gridStock.ClearSelection();
        }

        public void populateSessions()
        {
            Bookings app = new Bookings();
            SqlDataReader reader = app.GetLiveAppointments();
            metroGrid11.Rows.Clear();
            if (reader.HasRows)
            {
                int row = 0;
                metroGrid11.Rows.Clear();
                Customers cus = new Customers();
                while (reader.Read())
                {
                    metroGrid11.Rows.Add();
                    metroGrid11.Rows[row].Cells[0].Value = reader["id"].ToString();
                    metroGrid11.Rows[row].Cells[1].Value = cus.GetCustomerName(reader["CustomerId"].ToString());
                    metroGrid11.Rows[row].Cells[2].Value = Convert.ToDateTime(reader["StartTime"]).ToString("HH:mm");
                    metroGrid11.Rows[row].Cells[3].Value = Convert.ToDateTime(reader["EndTime"]).ToString("HH:mm");
                    metroGrid11.Rows[row].Cells[4].Value = reader["Price"].ToString();
                    row++;
                }
            }
        }

        public void populate_Sales()
        {
            gridSalesInfo.Rows.Clear();
            string cussId;
            SqlDataReader reader = Program.db.get_desc("Sales");
            if (reader.HasRows)
            {
                gridSalesInfo.AllowUserToAddRows = true;
                int rows = 0;
                double servicetax = 14.5;

                Settings company = new Settings();
                SqlDataReader c = company.GetCompanyDetails();

                if (c.HasRows)
                {
                    c.Read();
                    double.TryParse(c["SvcTaxPercentage"].ToString(), out servicetax);
                }
                while (reader.Read())
                {
                    gridSalesInfo.Rows.Add();
                    cussId = reader["CustomerId"].ToString();
                    SqlDataReader reader1 = Program.db.get_where("Customers", cussId);

                    DateTime date = Convert.ToDateTime(reader["Date"]);

                    gridSalesInfo.Rows[rows].Cells[0].Value = date.ToString("yyyy-MM-dd");


                    while (reader1.Read())
                    {
                        gridSalesInfo.Rows[rows].Cells[1].Value = reader1["Name"];
                    }

                    gridSalesInfo.Rows[rows].Cells[2].Value = reader["Amount"];
                    double sales = Convert.ToDouble(reader["Amount"]);
                    double saleswithtax = sales + (sales * (servicetax / 100));
                    gridSalesInfo.Rows[rows].Cells[3].Value = saleswithtax;
                    gridSalesInfo.Rows[rows].Cells[4].Value = reader["PaymentMode"];
                    gridSalesInfo.Rows[rows].Cells[5].Value = reader["discount"];
                    gridSalesInfo.Rows[rows].Cells[6].Value = reader["Id"];


                    rows++;

                }
                gridSalesInfo.AllowUserToAddRows = false;
                gridSalesInfo.ClearSelection();
            }
        }


        public void populate_Sales(DateTime FilterDate)
        {
            gridSalesInfo.Rows.Clear();
            string cussId;

            Sales sl = new Sales();
            SqlDataReader reader = sl.GetSalesByDate(FilterDate);
            if (reader.HasRows)
            {
                gridSalesInfo.AllowUserToAddRows = true;
                int rows = 0;
                double servicetax = 14.5;

                Settings company = new Settings();
                SqlDataReader c = company.GetCompanyDetails();

                if (c.HasRows)
                {
                    c.Read();
                    double.TryParse(c["SvcTaxPercentage"].ToString(), out servicetax);
                }
                while (reader.Read())
                {
                    gridSalesInfo.Rows.Add();
                    cussId = reader["CustomerId"].ToString();
                    SqlDataReader reader1 = Program.db.get_where("Customers", cussId);

                    DateTime date = Convert.ToDateTime(reader["Date"]);

                    gridSalesInfo.Rows[rows].Cells[0].Value = date.ToString("yyyy-MM-dd");


                    while (reader1.Read())
                    {
                        gridSalesInfo.Rows[rows].Cells[1].Value = reader1["Name"];
                    }

                    gridSalesInfo.Rows[rows].Cells[2].Value = reader["Amount"];
                    double sales = Convert.ToDouble(reader["Amount"]);
                    double saleswithtax = sales + (sales * (servicetax / 100));
                    gridSalesInfo.Rows[rows].Cells[3].Value = saleswithtax;
                    gridSalesInfo.Rows[rows].Cells[4].Value = reader["PaymentMode"];
                    gridSalesInfo.Rows[rows].Cells[5].Value = reader["discount"];
                    gridSalesInfo.Rows[rows].Cells[6].Value = reader["Id"];


                    rows++;

                }
                gridSalesInfo.AllowUserToAddRows = false;
                gridSalesInfo.ClearSelection();
            }
        }


        public void populateCommisions()
        {
            SqlDataReader reader = Program.db.get_desc("Therapists");
            if (reader.HasRows)
            {

                Therapists t = new Therapists();
                DateTime now = metroDateTime2.Value;
                int row = 0;
                gridThepSales.AllowUserToAddRows = true;

                double commision = 0.0;

                Settings company = new Settings();
                SqlDataReader c = company.GetCompanyDetails();

                if (c.HasRows)
                {
                    c.Read();
                    double.TryParse(c["Commision"].ToString(), out commision);
                }
                while (reader.Read())
                {
                    double sales = t.GetTotalSaleByDate(reader["Id"].ToString(), now);

                    if (sales != 0.0)
                    {

                        //  metroGrid6.Rows.Clear();


                        gridThepSales.Rows.Add();
                        gridThepSales.Rows[row].Cells[0].Value = now.ToString("yyyy-MM-dd");
                        gridThepSales.Rows[row].Cells[1].Value = reader["Name"];

                        gridThepSales.Rows[row].Cells[2].Value = sales;
                        gridThepSales.Rows[row].Cells[3].Value = (commision / 100) * sales;
                        row++;

                    }
                    else
                    {

                    }

                }
                gridThepSales.AllowUserToAddRows = false;
                gridThepSales.ClearSelection();
            }
        }

        public void populateAppointments()
        {
            gridBookings.Rows.Clear();
            DateTime d = System.DateTime.Today;
            Bookings app = new Bookings();
            SqlDataReader reader = app.GetAllAppointments();

            if (reader.HasRows)
            {
                int rows = 0;
                while (reader.Read())
                {
                    gridBookings.Rows.Add();
                    SqlDataReader CusInfo = Program.db.get_where("Customers", reader["CustomerId"].ToString());
                    CusInfo.Read();
                    gridBookings.Rows[rows].Cells[0].Value = reader["Id"];
                    gridBookings.Rows[rows].Cells[1].Value = CusInfo["Name"];
                    gridBookings.Rows[rows].Cells[2].Value = CusInfo["Phone"];
                    gridBookings.Rows[rows].Cells[3].Value = CusInfo["Visits"];
                    gridBookings.Rows[rows].Cells[4].Value = Convert.ToDateTime(reader["SessionDate"]).ToString("yyyy-MM-dd");
                    gridBookings.Rows[rows].Cells[5].Value = Convert.ToDateTime(reader["StartTime"]).ToString("HH:mm");
                    gridBookings.Rows[rows].Cells[6].Value = Convert.ToDateTime(reader["EndTime"]).ToString("HH:mm");
                    Therapists t = new Therapists();
                    string TName = t.GetName(reader["TherapistId"].ToString());
                    gridBookings.Rows[rows].Cells[7].Value = TName;

                    if (reader["Type"].ToString() == "walking   ")
                    {
                        gridBookings.Rows[rows].Cells[8].Value = "Walking";
                        SetActionCell_NA(gridBookings.Rows[rows].Cells[10]);
                        SetActionCell_NA(gridBookings.Rows[rows].Cells[11]);
                    }
                    else
                    {

                        gridBookings.Rows[rows].Cells[8].Value = "Appointment";

                        if (reader["Status"].ToString() != "Complete  ")
                        {
                            gridBookings.Rows[rows].Cells[10].Value = "Checkout";
                            gridBookings.Rows[rows].Cells[11].Value = "Cancel";
                        }
                        else
                        {
                            SetActionCell_NA(gridBookings.Rows[rows].Cells[10]);
                            SetActionCell_NA(gridBookings.Rows[rows].Cells[11]);
                        }
                    }

                    gridBookings.Rows[rows].Cells[9].Value = reader["Notes"].ToString();


                    rows++;

                }
            }


        }

        public void populateServices()
        {
            gridBookings.Rows.Clear();

            SqlDataReader reader = Program.db.get_desc("Services");

            if (reader.HasRows)
            {
                int rows = 0;
                while (reader.Read())
                {
                    gridBookings.Rows.Add();
                    svcGridList.Rows[rows].Cells[0].Value = reader["Id"];
                    svcGridList.Rows[rows].Cells[1].Value = reader["Name"];
                    svcGridList.Rows[rows].Cells[1].Value = reader["Price"];
                    svcGridList.Rows[rows].Cells[1].Value = reader["Description"];
                    rows++;
                }
            }
        }

        private void BookingsPopulate(DateTime date, bool showall)
        {
            gridBookings.Rows.Clear();
            SqlDataReader reader;
            Bookings app = new Bookings();
            if (showall == false)
            {
                reader = app.GetAppointmentsByDate(date);
            }
            else
            {
                reader = app.GetAllAppointments();
            }
            if (reader.HasRows)
            {

                int rows = 0;

                while (reader.Read())
                {
                    gridBookings.Rows.Add();
                    SqlDataReader CusInfo = Program.db.get_where("Customers", reader["CustomerId"].ToString());
                    CusInfo.Read();
                    gridBookings.Rows[rows].Cells[0].Value = reader["Id"];
                    gridBookings.Rows[rows].Cells[1].Value = CusInfo["Name"];
                    gridBookings.Rows[rows].Cells[2].Value = CusInfo["Phone"];
                    gridBookings.Rows[rows].Cells[3].Value = CusInfo["Visits"];
                    gridBookings.Rows[rows].Cells[4].Value = Convert.ToDateTime(reader["SessionDate"]).ToString("yyyy-MM-dd");
                    gridBookings.Rows[rows].Cells[5].Value = Convert.ToDateTime(reader["StartTime"]).ToString("HH:mm");
                    gridBookings.Rows[rows].Cells[6].Value = Convert.ToDateTime(reader["EndTime"]).ToString("HH:mm");
                    Therapists t = new Therapists();
                    string TName = t.GetName(reader["TherapistId"].ToString());
                    gridBookings.Rows[rows].Cells[7].Value = TName;



                    if (reader["Type"].ToString() == "walking   ")
                    {
                        gridBookings.Rows[rows].Cells[8].Value = "Walking";
                        SetActionCell_NA(gridBookings.Rows[rows].Cells[10]);
                        SetActionCell_NA(gridBookings.Rows[rows].Cells[11]);
                    }
                    else
                    {
                        gridBookings.Rows[rows].Cells[8].Value = "Appointment";
                        if (reader["Status"].ToString() != "Complete  ")
                        {
                            gridBookings.Rows[rows].Cells[10].Value = "Checkout";
                            gridBookings.Rows[rows].Cells[11].Value = "Cancel";
                        }
                        else
                        {
                            SetActionCell_NA(gridBookings.Rows[rows].Cells[10]);
                            SetActionCell_NA(gridBookings.Rows[rows].Cells[11]);
                        }
                    }

                    gridBookings.Rows[rows].Cells[9].Value = reader["Notes"].ToString();



                    rows++;

                }
            }
        }

        #endregion



        void notifyForm_MakeActiveParent(object arg1, EventArgs arg2)
        {
            timerFormActive.Start();
        }

        private void buttonAddService_Click(object sender, EventArgs e)  // service add
        {
            string ServiceName = svcTxtName.Text;
            int duration = Convert.ToInt32(svcTxtDur.Text);
            double cost = Convert.ToDouble(svcTxtCost.Text);
            string description = svcTxtDesc.Text;
            Services ob = new Services();
            Dictionary<int, double> ServiceData = new Dictionary<int, double>();

            int c = svcGridItem.RowCount;
            int rows = 0;
            double quant = 0;
            int Id = 0;
            foreach (DataGridViewRow row in svcGridItem.Rows)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[4];
                //   chk.v
                if (Convert.ToBoolean(chk.Value) == true)
                {
                    try
                    {
                        string Quantity = svcGridItem.Rows[rows].Cells[3].Value.ToString();
                        string id = svcGridItem.Rows[rows].Cells[0].Value.ToString();// do database operation with quantity here
                        string unit = svcGridItem.Rows[rows].Cells[2].Value.ToString();
                        bool result1 = Int32.TryParse(id, out Id);
                        bool result2 = Double.TryParse(Quantity, out quant);


                        //This is Not an Update
                        // In Case of an Update u get the Values in ML so u cant divide it with 1000 again !
                        //handle it asi

                        if (result1 && result2)
                        {
                            if (unit == "ML")
                            {
                                quant = quant / 1000;
                                ServiceData.Add(Id, quant);
                                svcGridItem.EndEdit();
                            }
                            else
                            {
                                ServiceData.Add(Id, quant);
                                svcGridItem.EndEdit();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.AppError(ex.Message);
                    }
                }

                rows++;
            }
            Services service = new Services();

            int ServiceId;
            if (!string.IsNullOrEmpty(txtServiceId.Text))
            {
                ServiceId = Convert.ToInt32(txtServiceId.Text);
            }
            else
            {
                ServiceId = -1;
            }

            Boolean success = service.RegisterService(ServiceId, ServiceName, duration, cost, description, ServiceData);
            notifyForm.ShowNotification(true, "Service created successfully", "New service");
            LoadServices();
        }


        private void SetActionCell_NA(DataGridViewCell cell)
        {
            cell.Value = "N/A";

            DataGridViewLinkCell linkCell = cell as DataGridViewLinkCell;
            if (linkCell != null)
            {
                linkCell.LinkBehavior = LinkBehavior.NeverUnderline;
            }
        }



        private void stkBtnSave_Click(object sender, EventArgs e)
        {
            if (IsBlankTextField(stkTxtName, stkTxtAmount))
            {
                return;
            }
            if (string.IsNullOrEmpty(stkComboUnit.SelectedItem.ToString()))
            {
                return;
            }

            try
            {
                Stocks obj = new Stocks();
                string name = stkTxtName.Text;
                string unit = stkComboUnit.SelectedItem.ToString();
                double amount = Convert.ToDouble(stkTxtAmount.Text);
                double CurrAmount = 0;
                int itemId = -1;

                if (!string.IsNullOrWhiteSpace(txtItemId.Text))
                {
                    itemId = Convert.ToInt32(txtItemId.Text);
                    CurrAmount = obj.GetCurrentAmount(itemId.ToString());
                    amount = amount + CurrAmount;
                }

                bool success = success = obj.Add(itemId, name, unit, amount);
                if (success)
                {
                    notifyForm.ShowNotification(true, "Item updated successfully", "Success");

                    this.populateStock();
                    this.LoadServices();
                    stkBtnNew.PerformClick();
                    timerDefaultFocus.Start();
                }
                else
                {
                    notifyForm.ShowNotification(false, "Something went wrong while saving Item. Please contact Camycent if you continue to get this.", "System Error");
                }
            }
            catch (Exception ex)
            {
                Log.AppError(ex.Message, true);
            }
        }

        private void buttonChangePass_Click(object sender, EventArgs e)   // user settings change password
        {
            string username = metroComboBox11.SelectedItem.ToString();
            string password = metroTextBox26.Text;
            string n = metroTextBox6.Text;

            users user = new users();
            if (user.ChangePass(username, password, n))
                notifyForm.ShowNotification(true, "Password changed successfully", "Change Password");
            else
                notifyForm.ShowNotification(false, "Failed to change password", "Change Password");

        }

        private void buttonAddUser_Click(object sender, EventArgs e)   // user settings Add new user
        {
            string username = metroTextBox27.Text;
            string password = metroTextBox28.Text;
            string role = metroComboBox10.SelectedItem.ToString();
            users user = new users();
            if (user.AddUser(username, password, role))
                notifyForm.ShowNotification(true, "New user added successfully", "Add User");
            else
                notifyForm.ShowNotification(false, "Failed to create new user account", "Add User");

        }

        private void buttonAddPackage_Click(object sender, EventArgs e) // package addition here
        {
            if (IsBlankTextField(pckTxtName, pckTxtValidity, pckTxtCost))
            {
                return;
            }

            string PackageName = pckTxtName.Text;
            int Duration = Convert.ToInt32(pckTxtValidity.Text);
            int Cost = Convert.ToInt32(pckTxtCost.Text);
            int Worth = 0;
            if (!string.IsNullOrEmpty(pckTxtWorth.Text))
                Worth = Convert.ToInt32(pckTxtWorth.Text);

            string Desc = pckTxtDesc.Text;
            int rows = 0;
            int SessionNo;
            int Id = 0;
            Dictionary<int, int> PackageData = new Dictionary<int, int>();

            foreach (DataGridViewRow row in gridPckSvcIncl.Rows)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[4];
                //   chk.v
                if (Convert.ToBoolean(chk.Value) == true)
                {
                    try
                    {
                        string sessions = gridPckSvcIncl.Rows[rows].Cells[3].Value.ToString();
                        string id = gridPckSvcIncl.Rows[rows].Cells[0].Value.ToString();// do database operation with quantity here 
                        string cost = gridPckSvcIncl.Rows[rows].Cells[2].Value.ToString();
                        bool result1 = Int32.TryParse(id, out Id);
                        bool result2 = Int32.TryParse(sessions, out SessionNo);

                        if (result1 && result2)
                        {
                            PackageData.Add(Id, SessionNo);

                            gridPckSvcIncl.EndEdit();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.AppError(ex.Message);
                    }
                }

                rows++;
            }
            Packages ob = new Packages();

            int PackageId;

            if (!string.IsNullOrEmpty(txtPackageId.Text))
            {
                PackageId = Convert.ToInt32(txtPackageId.Text);
            }
            else
            {
                PackageId = -1;
            }

            Boolean success = ob.RegisterPackage(PackageId, PackageName, Duration, Cost, Worth, Desc, PackageData);

            if (success)
            {
                LoadPackages();
                LoadBilling();
                pckTxtName.WithError = false;
                pckTxtValidity.WithError = false;
                pckTxtCost.WithError = false;
                pckTxtWorth.WithError = false;

                notifyForm.ShowNotification(true, "Package updated successfully", "Package update");
                pckBtnNew.PerformClick();
                timerDefaultFocus.Start();
            }
            else
            {
                notifyForm.ShowNotification(false, "Package creation fauled. Please contact Camycent support if you continue to see this.", "System Error");
            }
        }

        private void metroGrid2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                this.UseWaitCursor = true;
                int row = e.RowIndex;

                //Fetching Row Data
                int Id = Convert.ToInt32(svcGridList.Rows[row].Cells[0].Value);
                string Name = svcGridList.Rows[row].Cells[1].Value.ToString();
                double Duration = Convert.ToDouble(svcGridList.Rows[row].Cells[2].Value);
                double Cost = Convert.ToDouble(svcGridList.Rows[row].Cells[3].Value);
                string Desc = svcGridList.Rows[row].Cells[4].Value.ToString();

                //Setting data in the Add/Update Form
                svcTxtName.Text = Name;
                svcTxtDur.Text = Duration.ToString();
                svcTxtCost.Text = Cost.ToString();
                svcTxtDesc.Text = Desc;
                txtServiceId.Text = Id.ToString();
                svcBtnSave.Text = "Update Service";

                //Getting the Data for Required Service
                Services service = new Services();

                int iRows = 0;
                foreach (DataGridViewRow iRow in svcGridItem.Rows)
                {
                    svcGridItem.Rows[iRows].Cells[3].Value = "";
                    svcGridItem.Rows[iRows].Cells[4].Value = false;
                    iRows++;
                }

                int Itemid;
                try
                {
                    int rows = 0;
                    foreach (DataGridViewRow iRowz in svcGridItem.Rows)
                    {

                        SqlDataReader reader = service.ItemResourceReqd(Id);
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                DataGridViewRow gridrow = (DataGridViewRow)svcGridItem.Rows[0].Clone();
                                Itemid = Convert.ToInt32(reader["ItemId"]);
                                if (Convert.ToInt32(svcGridItem.Rows[rows].Cells[0].Value) == Itemid)
                                {
                                    //taking a new reader which contains the item information of that specific selected item id
                                    SqlDataReader ItemReader = Program.db.get_where("items", Itemid.ToString());
                                    while (ItemReader.Read())
                                    {
                                        svcGridItem.Rows[rows].Cells[0].Value = ItemReader["Id"];
                                        svcGridItem.Rows[rows].Cells[1].Value = ItemReader["ItemName"];
                                        if (ItemReader["Unit"].ToString() == "LTRS")
                                        {
                                            svcGridItem.Rows[rows].Cells[2].Value = "ML";
                                            double a = Convert.ToDouble(reader["Amount"]);
                                            a = a * 1000;
                                            if (a != 0)
                                            {
                                                svcGridItem.Rows[rows].Cells[4].Value = true;

                                            }
                                            svcGridItem.Rows[rows].Cells[3].Value = a.ToString();
                                        }
                                        else
                                        {
                                            svcGridItem.Rows[rows].Cells[2].Value = ItemReader["Unit"];

                                            svcGridItem.Rows[rows].Cells[3].Value = reader["Amount"];
                                            if (reader["Amount"] != null)
                                            {
                                                svcGridItem.Rows[rows].Cells[4].Value = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        reader.Close();
                        rows++;
                    }
                }
                catch (System.NullReferenceException exp)
                {
                    Log.AppError(exp.Message);
                }

                this.UseWaitCursor = false;
            }
            catch (Exception ex)
            {
                Log.AppError(ex.Message);
                return;
            }
        }

        private void buttonNewEntry_Click(object sender, EventArgs e)
        {
            txtServiceId.Text = "";
            svcTxtName.Text = "";
            svcTxtDur.Text = "";
            svcTxtCost.Text = "";
            svcTxtDesc.Text = "";
            svcBtnSave.Text = "Add Service";
            svcTxtName.ReadOnly = false;
            svcTxtDur.ReadOnly = false;
            svcTxtCost.ReadOnly = false;
            svcTxtDesc.ReadOnly = false;
            svcTxtName.Focus();
        }

        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
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

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.F4))
            {
                this.Close();
                Application.Exit();
                return true;
            }
            //Ctrl + N
            if (keyData == (Keys.Control | Keys.N))
            {
                timerDefaultFocus.Stop();

                //Bookings
                if (metroTabControl.SelectedIndex == 0)
                {
                    aptButtonClear.PerformClick();
                }
                //Svc
                else if (metroTabControl.SelectedIndex == 1)
                {
                    svcBtnNew.PerformClick();
                }
                //Pack
                else if (metroTabControl.SelectedIndex == 2)
                {
                    pckBtnNew.PerformClick();
                }
                //Thep
                else if (metroTabControl.SelectedIndex == 3)
                {
                    thepBtnNew.PerformClick();
                }
                //Cust
                else if (metroTabControl.SelectedIndex == 4)
                {
                    custBtnNew.PerformClick();
                }
                //Stock
                else if (metroTabControl.SelectedIndex == 5)
                {
                    stkBtnNew.PerformClick();
                }

                return true;
            }
            //Ctrl + S
            else if (keyData == (Keys.Control | Keys.S))
            {
                timerDefaultFocus.Stop();

                //Bookings
                if (metroTabControl.SelectedIndex == 0)
                {
                    if (aptButtonSave.Enabled)
                    {
                        aptButtonSave.PerformClick();
                    }
                }
                //Svc
                else if (metroTabControl.SelectedIndex == 1)
                {
                    svcBtnSave.PerformClick();
                }
                //Pack
                else if (metroTabControl.SelectedIndex == 2)
                {
                    pckBtnSave.PerformClick();
                }
                //Thep
                else if (metroTabControl.SelectedIndex == 3)
                {
                    thepBtnSave.PerformClick();
                }
                //Cust
                else if (metroTabControl.SelectedIndex == 4)
                {
                    custBtnSave.PerformClick();
                }
                //Stock
                else if (metroTabControl.SelectedIndex == 5)
                {
                    stkBtnSave.PerformClick();
                }
                //Setting
                else if (metroTabControl.SelectedIndex == 7)
                {
                    settBtnUpdate.PerformClick();
                }

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void lnkSite_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.camycent.com");
        }

        private void metroLinkSupport_Click(object sender, EventArgs e)
        {
            if (support == null)
            {
                support = new SupportForm();
            }
            support.ShowDialog();
        }

        private bool IsBlankTextField(params MetroTextBox[] txtBoxes)
        {
            bool blank = false;

            for (int i = 0; i < txtBoxes.Length; i++)
            {
                if ((String.IsNullOrWhiteSpace(txtBoxes[i].Text)) || (txtBoxes[i].WithError))
                {
                    txtBoxes[i].WithError = true;
                    blank = true;
                }
            }

            if (blank)
            {
                notifyForm.ShowNotification(true, "Please correct the marked fields before continuing", "Invalid field!");
            }

            return blank;
        }

        private void buttonServiceAdd_Click(object sender, EventArgs e)
        {
            if (IsBlankTextField(svcTxtName, svcTxtDur, svcTxtCost))
            {
                return;
            }

            string ServiceName = svcTxtName.Text;
            int duration = Convert.ToInt32(svcTxtDur.Text);
            int ServiceId = -1;
            if (!string.IsNullOrEmpty(txtServiceId.Text))
            {
                ServiceId = Convert.ToInt32(txtServiceId.Text);
            }
            else
            {
                ServiceId = -1;
            }

            double cost = Convert.ToDouble(svcTxtCost.Text);
            string description = svcTxtDesc.Text;
            Services ob = new Services();

            //THis Dictionay contains the ItemID and the Quantity.
            Dictionary<int, double> ServiceData = new Dictionary<int, double>();
            int c = svcGridItem.RowCount;
            int rows = 0;
            double quant = 0;
            int Id = 0;
            foreach (DataGridViewRow row in svcGridItem.Rows)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[4];
                //   chk.v
                if (Convert.ToBoolean(chk.Value) == true)
                {
                    try
                    {
                        string Quantity = svcGridItem.Rows[rows].Cells[3].Value.ToString();
                        string id = svcGridItem.Rows[rows].Cells[0].Value.ToString();// do database operation with quantity here 
                        string unit = svcGridItem.Rows[rows].Cells[2].Value.ToString();
                        bool result1 = Int32.TryParse(id, out Id);
                        bool result2 = Double.TryParse(Quantity, out quant);

                        //This is Not an Update
                        // In Case of an Update u get the Values in ML so u cant divide it with 1000 again !
                        //handle it asi

                        if (result1 && result2)
                        {
                            if (unit == "ML")
                            {
                                quant = quant / 1000;

                                //Pass this to DB
                                ServiceData.Add(Id, quant);
                                svcGridItem.EndEdit();
                            }
                            else
                            {
                                ServiceData.Add(Id, quant);
                                svcGridItem.EndEdit();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.AppError(ex.Message);
                    }
                }

                rows++;
            }
            Services service = new Services();
            Boolean success = service.RegisterService(ServiceId, ServiceName, duration, cost, description, ServiceData);

            notifyForm.ShowNotification(true, "Service updated successfully", "Service update");

            this.LoadServices();
            this.LoadAppoinments();
            this.LoadPackages();

            svcTxtName.WithError = false;
            svcTxtDur.WithError = false;
            svcTxtCost.WithError = false;

            timerDefaultFocus.Start();
        }

        private void buttonServiceNew_Click(object sender, EventArgs e)
        {
            txtServiceId.Text = "";
            svcTxtName.Text = "";
            svcTxtDur.Text = "";
            svcTxtCost.Text = "";
            svcTxtDesc.Text = "";

            int rows = 0;
            foreach (DataGridViewRow row in svcGridItem.Rows)
            {
                svcGridItem.Rows[rows].Cells[3].Value = "";
                svcGridItem.Rows[rows].Cells[4].Value = false;
                rows++;
            }
            svcTxtName.Focus();
            svcBtnSave.Text = "Add Service";
        }

        private void buttonServiceDelete_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = this.svcGridList.CurrentRow;
            string idStr = row.Cells["Column37"].Value.ToString();
            int id = -1;
            try
            {
                id = Convert.ToInt32(idStr);
            }
            catch (Exception ex)
            {
                Log.AppError(ex.Message);
                return;
            }
            Services obj = new Services();
            obj.DeRegisterService(id);

            this.populateServices();

            svcGridList.Refresh();
            svcGridList.ClearSelection();
        }

        private void btnSearchAppointment_Click(object sender, EventArgs e)
        {
            if (aptTxtSearchPhone.Text.Length < UIUtility.PhoneMinLength)
            {
                return;
            }

            //aptButtonClear.PerformClick();
            SqlDataReader reader = Program.db.Search("Customers", aptTxtSearchPhone.Text, "Phone");
            if (reader.HasRows)
            {
                reader.Read();
                aptTextCustName.Text = reader["Name"].ToString();
                aptTxtCustPh.Text = reader["Phone"].ToString();
                aptTxtCustEmail.Text = reader["Email"].ToString();
                apttxtCustVisit.Text = reader["Visits"].ToString();
                aptTxtCustAddr.Text = reader["Address"].ToString();
                aptRadioCustExist.Enabled = true;
                aptRadioCustExist.Focus();
                aptRadioCustExist.Enabled = false;
                aptComboThep.Enabled = true;

                //Service History

                string CusId = reader["Id"].ToString();

                reader.Close();

                Bookings app = new Bookings();
                reader = app.GetAppointmentList(CusId);
                if (reader.HasRows)
                {
                    int count = 0;
                    Services s = new Services();
                    listBoxHistory.Items.Clear();
                    gridBookings.Rows.Clear();
                    int rows = 0;
                    while (reader.Read())
                    {
                        List<int> Sids = app.GetServiceList(reader["Id"].ToString());

                        foreach (var i in Sids)
                        {
                            string sName = s.GetServiceName(i.ToString());
                            listBoxHistory.Items.Add(sName);
                            ++count;
                            if (count == 4)
                            {
                                break;
                            }
                        }

                        if (count == 4)
                        {
                            break;
                        }

                        //Change Grid
                        gridBookings.Rows.Clear();
                        reader = app.GetAppointmentList(CusId);
                        if (reader.HasRows)
                        {
                            rows = 0;
                            while (reader.Read())
                            {
                                gridBookings.Rows.Add();
                                SqlDataReader CusInfo = Program.db.get_where("Customers", reader["CustomerId"].ToString());
                                CusInfo.Read();
                                gridBookings.Rows[rows].Cells[0].Value = reader["Id"];
                                gridBookings.Rows[rows].Cells[1].Value = CusInfo["Name"];
                                gridBookings.Rows[rows].Cells[2].Value = CusInfo["Phone"];
                                gridBookings.Rows[rows].Cells[3].Value = CusInfo["Visits"];
                                gridBookings.Rows[rows].Cells[4].Value = Convert.ToDateTime(reader["SessionDate"]).ToString("yyyy-MM-dd");
                                gridBookings.Rows[rows].Cells[5].Value = Convert.ToDateTime(reader["StartTime"]).ToString("HH:mm");
                                gridBookings.Rows[rows].Cells[6].Value = Convert.ToDateTime(reader["EndTime"]).ToString("HH:mm");
                                Therapists t = new Therapists();
                                string TName = t.GetName(reader["TherapistId"].ToString());
                                gridBookings.Rows[rows].Cells[7].Value = TName;

                                if (reader["Type"].ToString() == "walking   ")
                                {
                                    gridBookings.Rows[rows].Cells[8].Value = "Walking";
                                    SetActionCell_NA(gridBookings.Rows[rows].Cells[10]);
                                    SetActionCell_NA(gridBookings.Rows[rows].Cells[11]);
                                }
                                else
                                {
                                    gridBookings.Rows[rows].Cells[8].Value = "Appointment";
                                    if (reader["Status"].ToString() != "Complete  ")
                                    {
                                        gridBookings.Rows[rows].Cells[10].Value = "Checkout";
                                        gridBookings.Rows[rows].Cells[11].Value = "Cancel";
                                    }
                                    else
                                    {
                                        SetActionCell_NA(gridBookings.Rows[rows].Cells[10]);
                                        SetActionCell_NA(gridBookings.Rows[rows].Cells[11]);
                                    }
                                }

                                gridBookings.Rows[rows].Cells[9].Value = reader["Notes"].ToString();

                                rows++;
                            }
                        }
                    }
                }
                else
                {
                    listBoxHistory.Items.Clear();
                    listBoxHistory.Items.Add("No Service History Found");
                }

                //Package History
                reader.Close();
                Packages pack = new Packages();
                List<int> packids = new List<int>();
                packids = pack.GetPackageCustomer(CusId);
                if (packids.Count > 0)
                {
                    listBoxPacks.Enabled = true;
                    listBoxPacks.Items.Clear();
                    foreach (var i in packids)
                    {
                        string packname = pack.GetPackageName(i.ToString());
                        listBoxPacks.Items.Add(packname);
                    }
                }
                else
                {
                    listBoxPacks.Items.Clear();
                    listBoxPacks.Items.Add("No Packages Found");
                    listBoxPacks.Enabled = false;
                }
                aptTextCustName.Focus();
            }
            else
            {
                aptTxtCustPh.Text = aptTxtSearchPhone.Text;
                aptRadioCustNew.Enabled = true;
                aptRadioCustNew.Focus();
                aptRadioCustNew.Enabled = false;
                aptComboThep.Enabled = true;
            }
            aptTextCustName.Focus();
        }

        private void metroGrid8_EndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (svcGridItem.Rows[e.RowIndex].Cells[3].Value != null)
            {
                svcGridItem.Rows[e.RowIndex].Cells[4].Value = true;
                svcGridItem.Rows[e.RowIndex].Cells[4].ReadOnly = false;
            }
            else
            {
                svcGridItem.Rows[e.RowIndex].Cells[4].Value = false;
                svcGridItem.Rows[e.RowIndex].Cells[4].ReadOnly = true;
            }
        }

        private void metroGrid7_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;

            //Fetching Row Data
            int Id = Convert.ToInt32(gridPackageList.Rows[row].Cells[0].Value);
            string Name = gridPackageList.Rows[row].Cells[1].Value.ToString();
            double Cost = Convert.ToDouble(gridPackageList.Rows[row].Cells[2].Value);
            int Duration = Convert.ToInt32(gridPackageList.Rows[row].Cells[3].Value);
            double Worth = Convert.ToDouble(gridPackageList.Rows[row].Cells[4].Value);
            string Desc = gridPackageList.Rows[row].Cells[1].Value.ToString();

            //Setting data in the Add/Update Form
            pckTxtName.Text = Name;
            pckTxtValidity.Text = Duration.ToString();
            pckTxtCost.Text = Cost.ToString();
            pckTxtWorth.Text = Worth.ToString();
            pckTxtDesc.Text = Desc;
            txtPackageId.Text = Id.ToString();


            Packages package = new Packages();
            SqlDataReader reader = package.PackageServices(Id);

            int iRows = 0;
            foreach (DataGridViewRow iRow in gridPckSvcIncl.Rows)
            {
                gridPckSvcIncl.Rows[iRows].Cells[3].Value = null;
                gridPckSvcIncl.Rows[iRows].Cells[4].Value = false;
                iRows++;
            }

            int servid;
            if (reader != null)
            {
                if (reader.HasRows)
                {
                    try
                    {
                        while (reader.Read())
                        {
                            foreach (DataGridViewRow iRow in gridPckSvcIncl.Rows)
                            {
                                int rows = Convert.ToInt32(iRow.Index);
                                DataGridViewRow gridrow = (DataGridViewRow)gridPckSvcIncl.Rows[0].Clone();


                                servid = Convert.ToInt32(reader["ServiceId"]);
                                if (Convert.ToInt32(gridPckSvcIncl.Rows[rows].Cells[0].Value) == servid)
                                {
                                    //taking a new reader which contains the item information of that specific selected item id
                                    SqlDataReader ServReader = Program.db.get_where("Services", servid.ToString());
                                    while (ServReader.Read())
                                    {
                                        gridPckSvcIncl.Rows[rows].Cells[0].Value = ServReader["Id"];
                                        gridPckSvcIncl.Rows[rows].Cells[1].Value = ServReader["Name"];
                                        gridPckSvcIncl.Rows[rows].Cells[2].Value = ServReader["Price"];
                                        gridPckSvcIncl.Rows[rows].Cells[3].Value = reader["Sessions"];
                                        if (reader["Sessions"] != null)
                                        {
                                            gridPckSvcIncl.Rows[rows].Cells[4].Value = true;
                                        }
                                        else
                                        {
                                            gridPckSvcIncl.Rows[rows].Cells[4].Value = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (System.NullReferenceException exp)
                    {
                        Log.AppError(exp.Message);
                    }
                }
            }
        }

        private void buttonHRUp_Click(object sender, EventArgs e)
        {
            int timeHr = Convert.ToInt32(aptTxtTimeHr.Text);
            if (timeHr >= 23)
                timeHr = 0;
            else
                timeHr++;

            string strOutput = String.Format("{0:00}", timeHr);
            aptTxtTimeHr.Text = strOutput;
        }

        private void buttonHRDown_Click(object sender, EventArgs e)
        {
            int timeHr = Convert.ToInt32(aptTxtTimeHr.Text);
            if (timeHr <= 0)
                timeHr = 23;
            else
                timeHr--;

            string strOutput = String.Format("{0:00}", timeHr);
            aptTxtTimeHr.Text = strOutput;
        }

        private void buttonMinUp_Click(object sender, EventArgs e)
        {
            int timeMin = Convert.ToInt32(aptTxtTimeMin.Text);
            if (timeMin >= 59)
                timeMin = 0;
            else
                timeMin++;

            string strOutput = String.Format("{0:00}", timeMin);
            aptTxtTimeMin.Text = strOutput;
        }

        private void buttonMinDown_Click(object sender, EventArgs e)
        {
            int timeMin = Convert.ToInt32(aptTxtTimeMin.Text);
            if (timeMin <= 0)
                timeMin = 59;
            else
                timeMin--;

            string strOutput = String.Format("{0:00}", timeMin);
            aptTxtTimeMin.Text = strOutput;
        }

        private void PopulateCurrentDateTimeForAppointment()
        {
            aptTxtTimeHr.Text = DateTime.Now.ToString("HH");
            aptTxtTimeMin.Text = DateTime.Now.ToString("mm");
        }

        private int getTherapistId()
        {
            //Att: ANP
            if (aptComboThep.SelectedIndex == -1)
                return -1;

            string therapistName = aptComboThep.SelectedItem.ToString();
            Dictionary<string, string> data = new Dictionary<string, string>();
            SqlDataReader reader = null;
            data.Add("Name", "'" + therapistName + "'");
            reader = Program.db.get_where_custom("Therapists", data);
            reader.Read();
            int therapistId = Convert.ToInt32(reader["Id"]);
            reader.Close();
            data.Clear();
            return therapistId;
        }

        public DateTime GetAppointmentDate()
        {
            int hr = Convert.ToInt32(aptTxtTimeHr.Text);
            int min = Convert.ToInt32(aptTxtTimeMin.Text);
            int year = Convert.ToInt32(aptDTBooking.Value.ToString("yyyy"));
            int month = Convert.ToInt32(aptDTBooking.Value.ToString("MM"));
            int day = Convert.ToInt32(aptDTBooking.Value.ToString("dd"));

            DateTime appointmentDate = new DateTime(year, month, day, hr, min, 0);
            return appointmentDate;
        }

        public void GetAvailibilityStatus()
        {
            therapistId = getTherapistId();
            double duration = Convert.ToDouble(aptLabelDuration.Text);

            DateTime appdate = this.GetAppointmentDate();
            Bookings obj = new Bookings();
            bool ifAvailable = obj.AppointmentAvailable(therapistId, appdate, duration);
            if (ifAvailable)
            {
                aptLabelAvailibility.Text = "Available";
            }
            else
            {
                string id = Bookings.BookingId;

                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("TherapistId", "" + therapistId + "");
                data.Add("Id", "" + id + "");
                SqlDataReader reader = Program.db.get_where_custom("Appointments", data);
                while (reader.Read())
                {
                    aptLabelAvailibility.Text = "After " + reader["EndTime"];
                }
            }
        }

        private void metroComboBoxTherapist_SelectedIndexChanged(object sender, EventArgs e)
        {
            therapistId = getTherapistId();
            
            //Att: ANP
            if (therapistId == -1)
            {
                return;
            }

            serviceIds = new List<int>();
            Dictionary<string, string> d = new Dictionary<string, string>();

            ListBox.SelectedObjectCollection col = listBoxPackService.SelectedItems;

            if (col.Count != 0)
            {
                Services s = new Services();
                foreach (var item in listBoxPackService.SelectedItems)
                {
                    string sid = s.GetServiceID(item.ToString());
                    serviceIds.Add(Convert.ToInt32(sid));
                }

                double duration = Convert.ToDouble(aptLabelDuration.Text);

                DateTime appdate = this.GetAppointmentDate();

                Bookings obj = new Bookings();

                bool ifAvailable = obj.AppointmentAvailable(therapistId, appdate, duration);
                if (ifAvailable)
                {
                    aptLabelAvailibility.Text = "Available";
                }
                else
                {
                    string id = Bookings.BookingId;
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("TherapistId", "" + therapistId + "");
                    data.Add("Id", "" + id + "");
                    SqlDataReader reader = Program.db.get_where_custom("Appointments", data);
                    while (reader.Read())
                    {
                        aptLabelAvailibility.Text = "After " + reader["EndTime"];
                    }
                }
            }
            else
            {
                notifyForm.ShowNotification(null, "Please select a Service first before continuing", "Choose service");
            }
        }

        private void PackageNew_Click(object sender, EventArgs e)
        {
            txtPackageId.Text = "";

            pckTxtName.Text = "";
            pckTxtName.WithError = false;
            pckTxtValidity.Text = "";
            pckTxtValidity.WithError = false;
            pckTxtCost.Text = "";
            pckTxtCost.WithError = false;
            pckTxtWorth.Text = "";
            pckTxtWorth.WithError = false;
            pckTxtDesc.Text = "";

            foreach (DataGridViewRow row in gridPckSvcIncl.Rows)
            {
                gridPckSvcIncl.Rows[row.Index].Cells[3].Value = "";
                gridPckSvcIncl.Rows[row.Index].Cells[4].Value = false;
            }

            pckTxtName.Focus();
        }

        private void PackageDelete_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = this.gridPackageList.CurrentRow;

            if (row == null)
            {
                return;
            }

            string idStr = row.Cells["Column32"].Value.ToString();
            int id = -1;
            try
            {
                id = Convert.ToInt32(idStr);
            }
            catch (Exception ex)
            {
                Log.AppError(ex.Message);
                return;
            }
            Packages obj = new Packages();
            obj.DeRegisterPackage(id);

            Packages ob = new Packages();
            int rows;
            SqlDataReader reader = Program.db.get("Packages");
            if (reader.HasRows)
            {
                try
                {
                    rows = 0;
                    while (reader.Read())
                    {
                        row = (DataGridViewRow)gridPackageList.Rows[0].Clone();

                        gridPackageList.Rows[rows].Cells[0].Value = reader["Id"];
                        gridPackageList.Rows[rows].Cells[1].Value = reader["Name"];
                        gridPackageList.Rows[rows].Cells[2].Value = reader["Duration"];
                        gridPackageList.Rows[rows].Cells[3].Value = reader["Price"];
                        gridPackageList.Rows[rows].Cells[4].Value = reader["Value"];
                        gridPackageList.Rows[rows].Cells[5].Value = reader["Description"];

                        rows++;
                    }
                    gridPackageList.Rows.RemoveAt(rows);
                }
                catch (System.NullReferenceException exp)
                {
                    Log.AppError(exp.Message);
                }
            }
            else
            {
                rows = 0;
                gridPackageList.Rows[rows].Cells[0].Value = "";
                gridPackageList.Rows[rows].Cells[1].Value = "";
                gridPackageList.Rows[rows].Cells[2].Value = "";
                gridPackageList.Rows[rows].Cells[3].Value = "";
                gridPackageList.Rows[rows].Cells[4].Value = "";
                gridPackageList.Rows[rows].Cells[5].Value = "";
            }
            gridPackageList.Refresh();
            gridPackageList.ClearSelection();
        }

        private void buttonTherapistAdd_Click(object sender, EventArgs e)
        {
            string name = thepTxtName.Text;
            string phone = thepTxtPhoner.Text;
            string email = thepTxtEmail.Text;
            string address = thepTxtAddr.Text;
            Therapists ob = new Therapists();

            bool status = ob.Add(name, phone, email, address);
            if (status)
            {
                notifyForm.ShowNotification(true, "Therapist updated successfully", "Therapist updated");
                thepTxtName.WithError = false;
                thepTxtPhoner.WithError = false;

                LoadTherapists();
                LoadAppoinments();
                thepBtnNew.PerformClick();
                timerDefaultFocus.Start();
            }
            else
            {
                notifyForm.ShowNotification(false, "Something went wrong. Please contact Camycent support if you continue to get this.", "System Error");
            }
        }

        private void buttonTherapistDelete_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = this.grdiTherapist.CurrentRow;
            string idStr = row.Cells["Column20"].Value.ToString();
            int id = -1;
            try
            {
                id = Convert.ToInt32(idStr);
            }
            catch (Exception ex)
            {
                Log.AppError(ex.Message);
                return;
            }
            Therapists obj = new Therapists();
            obj.delete(id);

            Therapists ob = new Therapists();

            int rows;
            SqlDataReader reader = Program.db.get("Therapists");
            if (reader.HasRows)
            {
                try
                {
                    rows = 0;
                    while (reader.Read())
                    {
                        row = (DataGridViewRow)grdiTherapist.Rows[0].Clone();

                        grdiTherapist.Rows[rows].Cells[0].Value = reader["Id"];
                        grdiTherapist.Rows[rows].Cells[1].Value = reader["Name"];
                        grdiTherapist.Rows[rows].Cells[2].Value = reader["Status"];
                        grdiTherapist.Rows[rows].Cells[3].Value = reader["Phone"];
                        grdiTherapist.Rows[rows].Cells[4].Value = reader["Email"];
                        grdiTherapist.Rows[rows].Cells[5].Value = reader["Address"];

                        rows++;
                    }
                    grdiTherapist.Rows.RemoveAt(rows);
                }
                catch (System.NullReferenceException exp)
                {
                    Log.AppError(exp.Message);
                }
            }
            else
            {
                rows = 0;
                grdiTherapist.Rows[rows].Cells[0].Value = "";
                grdiTherapist.Rows[rows].Cells[1].Value = "";
                grdiTherapist.Rows[rows].Cells[2].Value = "";
                grdiTherapist.Rows[rows].Cells[3].Value = "";
                grdiTherapist.Rows[rows].Cells[4].Value = "";
            }
            grdiTherapist.Refresh();
            grdiTherapist.ClearSelection();
        }

        public static int ServiceListCount;
        public static List<string> SList;

        private void listBoxPackService_SelectedIndexChanged(object sender, EventArgs e)
        {
            Services s = new Services();
            int CurrCount;
            ListBox.SelectedObjectCollection SelectedItems = listBoxPackService.SelectedItems;
            CurrCount = SelectedItems.Count;

            int dur = Convert.ToInt32(aptLabelDuration.Text);
            int price = Convert.ToInt32(aptLabelPrice.Text);

            if (CurrCount > ServiceListCount)
            {
                if (SList.Count == 0)
                {
                    string sid = s.GetServiceID(listBoxPackService.SelectedItem.ToString());
                    string SDur = s.GetServiceDuration(sid);
                    string SPrice = s.GetServicePrice(sid);

                    int TotalPrice = price + Convert.ToInt32(SPrice);
                    int TotalDur = dur + Convert.ToInt32(SDur);

                    aptLabelPrice.Text = TotalPrice.ToString();
                    aptLabelDuration.Text = TotalDur.ToString();

                    SList.Add(listBoxPackService.SelectedItem.ToString());
                }
                else
                {
                    foreach (var i in SelectedItems)
                    {
                        if (SList.Contains(i.ToString()) == false)
                        {
                            string sid = s.GetServiceID(i.ToString());
                            string SDur = s.GetServiceDuration(sid);
                            string SPrice = s.GetServicePrice(sid);

                            int TotalPrice = price + Convert.ToInt32(SPrice);
                            int TotalDur = dur + Convert.ToInt32(SDur);

                            aptLabelPrice.Text = TotalPrice.ToString();
                            aptLabelDuration.Text = TotalDur.ToString();

                            SList.Add(i.ToString());
                        }
                    }
                }

                aptComboThep.Enabled = true;
            }
            else
            {
                //Item Removed

                if (CurrCount == 0)
                {
                    aptLabelDuration.Text = "0";
                    aptLabelPrice.Text = "0";
                    aptLabelPWTax.Text = "0";
                    SList.Clear();
                    aptComboThep.Enabled = false;
                    ServiceListCount = 0;
                    return;
                }

                //Find the ServiceId of the Item Removed

                string remove = "";

                foreach (var i in SList)
                {
                    if (SelectedItems.Contains(i.ToString()) == false)
                    {
                        //This is the Item Removed


                        string sid = s.GetServiceID(i.ToString());
                        string SDur = s.GetServiceDuration(sid);
                        string SPrice = s.GetServicePrice(sid);

                        int TotalPrice = price - Convert.ToInt32(SPrice);
                        int TotalDur = dur - Convert.ToInt32(SDur);

                        aptLabelPrice.Text = TotalPrice.ToString();
                        aptLabelDuration.Text = TotalDur.ToString();

                        remove = i.ToString();

                    }


                }

                SList.Remove(remove);

            }
            double servicetax = 14.5;

            Settings company = new Settings();
            SqlDataReader c = company.GetCompanyDetails();

            if (c.HasRows)
            {
                c.Read();
                double.TryParse(c["SvcTaxPercentage"].ToString(), out servicetax);
            }
            ServiceListCount = CurrCount;
            aptComboThep.Enabled = true;
            if (aptLabelPrice.Text != "0")
            {
                double cp = Convert.ToDouble(aptLabelPrice.Text);
                double cwt = (servicetax / 100) * cp;
                cwt = cwt + cp;

                aptLabelPWTax.Text = cwt.ToString("n2");
            }
            else
            {
                aptLabelPWTax.Text = "0";
            }
        }

        private void register_booking(string type)
        {
            if (string.IsNullOrWhiteSpace(aptLabelDuration.Text) || aptLabelDuration.Text == "0")
            {
                notifyForm.ShowNotification(null, "Please choose a Service before continuing", "Choose service");
                return;
            }
            if (aptComboThep.SelectedIndex == -1)
            {
                notifyForm.ShowNotification(null, "Please choose a Therapists before continuing", "Choose therapist");
                return;
            }
            if (IsBlankTextField(aptTxtCustPh))
            {
                return;
            }

            List<int> ServiceIds = new List<int>();
            int thearapistId;
            double Duration;

            ListBox.SelectedObjectCollection col = listBoxPackService.SelectedItems;
            Services s = new Services();

            foreach (var i in col)
            {
                ServiceIds.Add(Convert.ToInt32(s.GetServiceID(i.ToString())));
            }
            string tName;
            try
            {
                tName = aptComboThep.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                notifyForm.ShowNotification(null, "Please choose a Therapist to continue", "Choose therapist");
                Log.AppError(ex.Message);
                return;
            }


            Therapists t = new Therapists();
            thearapistId = Convert.ToInt32(t.GetId(tName));

            Duration = Convert.ToDouble(aptLabelDuration.Text);
            double Price = Convert.ToDouble(aptLabelPrice.Text);

            Bookings app = new Bookings();

            DateTime appDate = this.GetAppointmentDate();

            //Customer Info

            string name = aptTextCustName.Text;
            string phone = null;
            try
            {
                phone = aptTxtCustPh.Text;

            }
            catch (Exception ex)
            {
                Log.AppError(ex.Message);
            }

            string email;
            int visits;
            string address;

            if (string.IsNullOrWhiteSpace(aptTxtCustEmail.Text))
            {
                email = null;
            }
            else
            {
                email = aptTxtCustEmail.Text;
            }
            if (string.IsNullOrWhiteSpace(aptTxtCustAddr.Text))
            {
                address = null;
            }
            else
            {
                address = aptTxtCustAddr.Text;
            }
            if (string.IsNullOrWhiteSpace(apttxtCustVisit.Text))
            {
                visits = 0;
            }
            else
            {
                visits = Convert.ToInt32(apttxtCustVisit.Text);
            }
            string gender = null;
            if (aptRadioFemale.Checked == true)
            {
                gender = "Female";
            }
            else
            {
                gender = "Male";
            }

            string notes = apttxtNotes.Text;

            //gets appointment Id now.
            int success = -1;
            if (aptRadioAppoint.Checked == true && aptLabelAvailibility.Text != "Available")
            {
                notifyForm.ShowNotification(null, "Appointment Not available. Please choose a different time or therapist", "Not available");
                return;
            }
            else
            {
                if (isPackageAppointment)
                {
                    string packname = listBoxPacks.SelectedItem.ToString();
                    Packages p = new Packages();
                    string pId = p.GetPackageID(packname);
                    Customers cus = new Customers();
                    string cusId = cus.CustomerExsits(aptTxtCustPh.Text);

                    success = app.RegisterPackageBooking(thearapistId, Convert.ToInt32(cusId), Duration, Price, appDate, pId, type, gender, notes);

                }
                else
                {
                    success = app.RegisterServiceBooking(thearapistId, Duration, Price, appDate, ServiceIds, name, phone, email, visits, address, type, gender, notes);
                }
            }

            if (success != -1)
            {
                if (type == "app")
                {
                   notifyForm.ShowNotification(true, "Appointment booked successfully", "Booked");
                }
                else
                {
                   notifyForm.ShowNotification(true, "Walking billing updated successfully. Please continue to generate invoce.", "Booked");
                }

                if (type == "walking")
                {
                    //Generate Invoice
                    ShowInvoice(success, type);
                }

                this.LoadAppoinments();
                this.populateSessions();
                this.populateCustomers();
            }
            else
            {
                Log.AppError("Appointment Failed");
                notifyForm.ShowNotification(false, "Something went wrong. If you continue to get this please contact Camycent Support!", "System Error");
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            //Save the Appointment

            if (aptRadioAppoint.Checked == true)
            {
                register_booking("app");
            }
            else
            {
                register_booking("walking");
            }

        }

        private void metroTextBoxName_TextChanged(object sender, EventArgs e)
        {
            //buttonSave.Enabled = true;
            UIUtility.DisableButton(aptButtonSave, false);
        }

        private void gridBookings_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //  buttonAppointmentDelete.PerformClick();
            int row = e.RowIndex;

            // ignore column header click
            if (row == -1)
            {
                return;
            }

            //Fetching Row Data
            int Id = Convert.ToInt32(gridBookings.Rows[row].Cells[0].Value);



            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("Id", "'" + Id + "'");

            SqlDataReader appreader = Program.db.get_where_custom("Appointments", d);
            d.Clear();
            appreader.Read();
            int custId = Convert.ToInt32(appreader["CustomerId"].ToString());

            d.Add("Id", "'" + custId + "'");
            SqlDataReader reader = Program.db.get_where_custom("Customers", d);
            if (reader.HasRows)
            {
                reader.Read();
                aptTextCustName.Text = reader["Name"].ToString();
                aptTxtCustPh.Text = reader["Phone"].ToString();
                aptTxtCustEmail.Text = reader["Email"].ToString();
                apttxtCustVisit.Text = reader["Visits"].ToString();
                aptTxtCustAddr.Text = reader["Address"].ToString();
                aptRadioCustExist.Enabled = true;
                aptRadioCustExist.Focus();
                aptRadioCustExist.Enabled = false;

                aptDTBooking.Value = Convert.ToDateTime(appreader["SessionDate"]);
                aptTxtTimeHr.Text = Convert.ToDateTime(appreader["StartTime"]).ToString("HH");
                aptTxtTimeMin.Text = Convert.ToDateTime(appreader["StartTime"]).ToString("mm");



                //Select Services

                Bookings app = new Bookings();

                Services s = new Services();

                List<int> Sids = app.GetServiceList(appreader["id"].ToString());



                listBoxPackService.BeginUpdate();
                try
                {

                    // do with listBoxPackService.Items[i]

                    foreach (var j in Sids)
                    {
                        int index = -1;
                        string SName = s.GetServiceName(j.ToString());
                        index = listBoxPackService.FindString(SName);
                        if (index != -1)
                        {
                            listBoxPackService.SetSelected(index, true);
                        }

                    }

                }
                finally
                {
                    listBoxPackService.EndUpdate();
                }



                Therapists t = new Therapists();
                string tName = t.GetName(appreader["TherapistId"].ToString());
                aptComboThep.SelectedItem = tName;


                string CusId = reader["Id"].ToString();

                reader.Close();


                reader = app.GetAppointmentList(CusId);
                if (reader.HasRows)
                {
                    int count = 0;

                    listBoxHistory.Items.Clear();
                    while (reader.Read())
                    {
                        List<int> serIds = app.GetServiceList(reader["Id"].ToString());

                        foreach (var i in serIds)
                        {
                            string sName = s.GetServiceName(i.ToString());
                            listBoxHistory.Items.Add(sName);
                            ++count;
                            if (count == 4)
                            {
                                break;
                            }
                        }

                        if (count == 4)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    listBoxHistory.Items.Clear();
                    listBoxHistory.Items.Add("No Service History Found");
                }

                //Package History

                reader.Close();
                Packages pack = new Packages();
                List<int> packids = new List<int>();
                packids = pack.GetPackageCustomer(CusId);
                if (packids.Count > 0)
                {
                    listBoxPacks.Enabled = true;
                    listBoxPacks.Items.Clear();
                    foreach (var i in packids)
                    {
                        string packname = pack.GetPackageName(i.ToString());
                        listBoxPacks.Items.Add(packname);
                    }

                }
                else
                {
                    listBoxPacks.Items.Clear();
                    listBoxPacks.Items.Add("No Packages Found");
                    listBoxPacks.Enabled = false;
                }

                reader.Close();


            }
            else
            {
                aptTextCustName.Focus();
                reader.Close();
            }

            if (e.ColumnIndex == 10 && gridBookings.Rows[row].Cells[10].Value.ToString() == "Checkout")
            {
                ShowInvoice(Id, "app");
            }
            if (e.ColumnIndex == 11 && gridBookings.Rows[row].Cells[11].Value.ToString() == "Cancel")
            {
                cancel_app(Id);
            }
        }



        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Package Combo in Customers

            string selected = metroComboBox1.SelectedItem.ToString();
            Packages pack = new Packages();
            string packId = pack.GetPackageID(selected);

            SqlDataReader reader = pack.PackageServices(Convert.ToInt32(packId));

            if (reader.HasRows)
            {
                int row = 0;
                while (reader.Read())
                {
                    SqlDataReader s = Program.db.get_where("Services", reader["ServiceId"].ToString());
                    if (s.HasRows)
                    {
                        s.Read();
                        metroGrid10.Rows.Add();
                        metroGrid10.Rows[row].Cells[0].Value = s["Name"].ToString();
                        metroGrid10.Rows[row].Cells[1].Value = s["Price"].ToString();
                        metroGrid10.Rows[row].Cells[2].Value = reader["Sessions"].ToString();
                        s.Close();

                    }

                    row++;
                }
            }

            reader.Close();

            string price = pack.GetPackagePrice(packId);
            lblPackagePrice.Text = price;
            //btnBuyPackage.Enabled = true;
            UIUtility.DisableButton(btnBuyPackage, false);

        }

        private void buy_package()
        {
            if (gridPackageList.SelectedRows.Count == 0)
            {
                notifyForm.ShowNotification(null, "Please choose a Package first", "No Package selected");
            }
            else if (gridPckCustomers.SelectedRows.Count == 0)
            {
                notifyForm.ShowNotification(null, "Please choose a Customer first", "No Customer selected");
            }
            else
            {
                Packages pack = new Packages();
                int customer_row = gridPckCustomers.CurrentCell.RowIndex;
                int package_row = gridPackageList.CurrentCell.RowIndex;

                string customer_id = gridPckCustomers.Rows[customer_row].Cells[0].Value.ToString();
                string pack_id = gridPackageList.Rows[package_row].Cells[0].Value.ToString();

                bool? success = pack.BuyPackage(Convert.ToInt32(customer_id), Convert.ToInt32(pack_id));
                if (success == true)
                {
                    notifyForm.ShowNotification(true, "Package sold successfully", "Buy Packge");
                    this.LoadSales();
                    this.LoadCustomers();
                }
                else if (success == false)
                {
                    notifyForm.ShowNotification(false, "Something went wrong please try again later", "Buy packge failed");
                }
            }
        }

        private void metroGrid1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;

            metroTextBox2.Text = metroGrid1.Rows[row].Cells[1].Value.ToString();
            metroTextBox4.Text = metroGrid1.Rows[row].Cells[2].Value.ToString();
            metroTextBox5.Text = metroGrid1.Rows[row].Cells[3].Value.ToString();
            metroTextBox29.Text = metroGrid1.Rows[row].Cells[4].Value.ToString();
            metroTextBox8.Text = metroGrid1.Rows[row].Cells[5].Value.ToString();
        }

        private void aptBtnClear_Click(object sender, EventArgs e)
        {          
            UIUtility.DisableButton(aptButtonSave, true);

            ServiceListCount = 0;
            aptDTBooking.Value = DateTime.Today;
            aptDTFilter.Value = DateTime.Today;

            aptTextCustName.Text = "";
            aptTxtCustEmail.Text = "";
            aptTxtCustPh.Text = "";
            apttxtCustVisit.Text = "";
            aptTxtCustAddr.Text = "";
            apttxtNotes.Text = "";
            aptTxtSearchPhone.Text = "";
            aptLabelPWTax.Text = "0";

            aptTxtTimeHr.Text = DateTime.Now.ToString("HH");
            aptTxtTimeMin.Text = DateTime.Now.ToString("mm");

            //Populate Appoints Tab
            //this.populateAppointments();
            //SqlDataReader reader;

            //SList = new List<string>();

            //reader = Program.db.get("Services");

            //if (reader.HasRows)
            //{
            //    listBoxPackService.Items.Clear();
            //    while (reader.Read())
            //    {
            //        listBoxPackService.Items.Add(reader["Name"]);
            //    }
            //    reader.Close();
            //}

            //reader = Program.db.get("Therapists");
            //if (reader.HasRows)
            //{
            //    aptComboThep.Items.Clear();
            //    while (reader.Read())
            //    {
            //        aptComboThep.Items.Add(reader["Name"]);
            //    }
            //    reader.Close();
            //}

            aptTextCustName.WithError = false;
            aptTxtCustEmail.WithError = false;
            aptTxtCustPh.WithError = false;

            aptComboThep.SelectedIndex = -1;
            aptComboThep.Select();
            aptComboThep.Enabled = false;
            aptComboThep.PromptText = "Choose Therapist";

            listBoxHistory.Items.Clear();
            listBoxHistory.Items.Add("No History Available");

            listBoxPacks.Enabled = false;
            listBoxPacks.Items.Clear();
            listBoxPacks.Items.Add("No Packages Found");

            listBoxPackService.ClearSelected();
            gridBookings.ClearSelection();

            aptLabelAvailibility.Text = "N/A";
            aptTxtSearchPhone.Focus();
        }

        private void metroDateTime3_ValueChanged(object sender, EventArgs e)
        {
            if (aptRadioFilter.Checked == true)
            {
                DateTime date = aptDTFilter.Value;
                BookingsPopulate(date, false);
            }
        }

        public static int customer_row;

        public void ShowInvoice(int AppId, string type)
        {
            Bookings app = new Bookings();
            bool? success = app.CheckOut(AppId, type, this);
            if (success == true)
            {
                LoadSales();
                this.populateSessions();
                this.populateCommisions();
                this.populateAppointments();
            }
            else if (success == false)
            {
                Log.AppError("Checkout Failed");
                notifyForm.ShowNotification(false, "Something went wrong during checkout, please restart the application.", "System Error");
            }
        }

        private void metroDateTime2_ValueChanged(object sender, EventArgs e)
        {
            this.populateCommisions();
        }

        private void metroLinkPhoto_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();

            open.Filter = "Image Files(*.jpg; *.png; *.gif; *.bmp)|*.jpg; *.png; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                pictureBoxPhoto.Image = new Bitmap(open.FileName);
                pictureBoxPhoto.SizeMode = PictureBoxSizeMode.StretchImage;
                string logoImageFile = open.FileName;
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                folder = folder + "\\Crystals\\";

                if (!System.IO.Directory.Exists(folder))
                {
                    System.IO.Directory.CreateDirectory(folder);
                }
                string ext = System.IO.Path.GetExtension(logoImageFile);
                folder = folder + "Logo" + ext;

                if (System.IO.File.Exists(folder))
                {
                    System.IO.File.Delete(folder);
                }

                System.IO.File.Copy(logoImageFile, folder);

                Settings s = new Settings();
                s.UpdatePic(folder);
            }
        }

        private void metroDateTime5_ValueChanged(object sender, EventArgs e)
        {
            double salesamount = 0, withtax = 0;
            string cussId;
            DateTime cdate = SalesDateFilter.Value;
            Sales s = new Sales();
            SqlDataReader reader = s.GetSalesByDate(cdate);
            if (reader.HasRows)
            {
                int rows = 0;
                gridSalesInfo.Rows.Clear();
                double servicetax = 14.5;

                Settings company = new Settings();
                SqlDataReader c = company.GetCompanyDetails();

                if (c.HasRows)
                {
                    c.Read();
                    double.TryParse(c["SvcTaxPercentage"].ToString(), out servicetax);
                }
                while (reader.Read())
                {
                    gridSalesInfo.Rows.Add();
                    cussId = reader["CustomerId"].ToString();
                    SqlDataReader reader1 = Program.db.get_where("Customers", cussId);

                    DateTime date = Convert.ToDateTime(reader["Date"]);

                    gridSalesInfo.Rows[rows].Cells[0].Value = date.ToString("yyyy-MM-dd");
                    //metroGrid12.Rows[rows].Cells[1].Value = reader["Id"];
                    gridSalesInfo.Rows[rows].Cells[2].Value = reader["Amount"];
                    salesamount = Convert.ToDouble(reader["Amount"]);
                    withtax = salesamount + (salesamount * (servicetax / 100));
                    while (reader1.Read())
                    {
                        gridSalesInfo.Rows[rows].Cells[1].Value = reader1["Name"];
                    }

                    gridSalesInfo.Rows[rows].Cells[3].Value = withtax;
                    gridSalesInfo.Rows[rows].Cells[4].Value = reader["PaymentMode"];
                    gridSalesInfo.Rows[rows].Cells[5].Value = reader["Discount"];
                    gridSalesInfo.Rows[rows].Cells[6].Value = reader["id"];
                    rows++;

                }
                gridSalesInfo.AllowUserToAddRows = false;
            }
            else
            {
                gridSalesInfo.Rows.Clear();
            }
        }

        private void listBoxPacks_SelectedValueChanged(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection col = listBoxPacks.SelectedItems;
            if (col.Count > 0)
            {
                //Get Customer ID
                string CusPh = aptTxtCustPh.Text;
                Customers cus = new Customers();
                string CusId = cus.CustomerExsits(CusPh);
                string packname = listBoxPacks.SelectedItem.ToString();
                Packages p = new Packages();
                string packId = p.GetPackageID(packname);

                List<int> vsids = new List<int>();
                vsids = p.ValidServices(CusId, packId);

                if (vsids.Count > 0)
                {
                    Services s = new Services();
                    isPackageAppointment = true;
                    listBoxPackService.Items.Clear();
                    foreach (var i in vsids)
                    {
                        string sName = s.GetServiceName(i.ToString());
                        listBoxPackService.Items.Add(sName);
                    }
                }
                else
                {
                    listBoxPackService.Items.Clear();
                    listBoxPackService.Items.Add("No Valid Services Found");
                }
            }
            else
            {
                listBoxPackService.Items.Clear();
                SqlDataReader reader = Program.db.get("Services");
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        listBoxPackService.Items.Add(reader["Name"].ToString());
                    }
                }
                isPackageAppointment = false;
                reader.Close();
            }

        }

        private void metroGrid3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            string itemId = gridStock.Rows[row].Cells[0].Value.ToString();

            SqlDataReader reader = Program.db.get_where("items", itemId);
            if (reader.HasRows)
            {
                reader.Read();
                stkTxtName.Text = gridStock.Rows[row].Cells[1].Value.ToString();
                stkComboUnit.SelectedItem = reader["Unit"].ToString();
                stkTxtAmount.Text = gridStock.Rows[row].Cells[4].Value.ToString();
                stkBtnSave.Text = "Update";
                txtItemId.Text = itemId;
            }
            else
            {
                notifyForm.ShowNotification(false, "Something Went wrong. Please try Again.", "Database Error");
            }
        }

        private void button1_Click(object sender, EventArgs e)    // delete stock
        {
            try
            {
                if (string.IsNullOrWhiteSpace(stkTxtName.Text) || string.IsNullOrEmpty(stkComboUnit.SelectedItem.ToString()) || double.IsNaN(Convert.ToDouble(stkTxtAmount.Text)))
                {
                    //notifyForm.ShowNotification(this.FindForm(), "Name, Unit or Amount Cannot be Empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Stocks obj = new Stocks();
                    string name = stkTxtName.Text;
                    string unit = stkComboUnit.SelectedItem.ToString();
                    double amount = Convert.ToDouble(stkTxtAmount.Text);
                    int itemId;
                    if (string.IsNullOrWhiteSpace(txtItemId.Text))
                    {
                        itemId = -1;
                    }
                    else
                    {
                        itemId = Convert.ToInt32(txtItemId.Text);
                    }

                    bool success = success = obj.delete(itemId);
                    if (success)
                    {
                        this.populateStock();
                        //notifyForm.ShowNotification(this.FindForm(), "Item Deleted Successfully", "DB Update", MessageBoxButtons.OK);
                    }
                    else
                    {
                        //notifyForm.ShowNotification(this.FindForm(), "Item Delete UnSuccessful", "DB Update", MessageBoxButtons.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.AppError(ex.Message, true);
            }
        }

        private void metroDateTime6_ValueChanged(object sender, EventArgs e)
        {
            DateTime date = metroDateTime6.Value;
            string id;
            Stocks app = new Stocks();
            SqlDataReader reader = app.GetStocksByDate(date);
            if (reader.HasRows)
            {
                gridStock.Rows.Clear();
                try
                {
                    int rows = 0;
                    while (reader.Read())
                    {
                        SqlDataReader reader1;
                        gridStock.Rows.Add();
                        id = reader["itemId"].ToString();
                        reader1 = Program.db.get_where("items", id);
                        gridStock.Rows[rows].Cells[0].Value = reader["itemId"];
                        while (reader1.Read())
                        {
                            gridStock.Rows[rows].Cells[1].Value = reader1["ItemName"];
                        }
                        gridStock.Rows[rows].Cells[2].Value = reader["Month"];
                        gridStock.Rows[rows].Cells[3].Value = reader["OpeningStock"];
                        gridStock.Rows[rows].Cells[4].Value = reader["ClosingStock"];
                        gridStock.Rows[rows].Cells[5].Value = reader["CurrentStock"];

                        rows++;
                    }
                }
                catch (System.NullReferenceException exp)
                {
                    Log.AppError(exp.Message);
                }
            }
            else
            {
                gridStock.Rows.Clear();
            }
        }

        private bool cancelAppointmentCancelled = false;
        public void cancel_app(int appId)
        {
            cancelAppointmentCancelled = false;

            CancelAppointment cancel = new CancelAppointment();
            cancel.CancelCancellation += cancel_CancelCancellation;

            cancel.set_appid(appId);
            cancel.ShowDialog();

            if (!cancelAppointmentCancelled)
            {
                this.populateSessions();
                this.LoadAppoinments();
            }
        }

        void cancel_CancelCancellation(object arg1, EventArgs arg2)
        {
            cancelAppointmentCancelled = true;
            timerFormActive.Start();

            Form sender = (Form)arg1;
            if (sender != null)
            {
                sender.Close();
                sender.Dispose();
            }
        }

        private void metroGrid11_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            UIUtility.DisableButton(btnCheckOut, false);
            UIUtility.DisableButton(button2, false);
            customer_row = e.RowIndex;
        }

        private void buttonUpdateInfo_Click(object sender, EventArgs e)
        {
            if (!ValidateSettingsFields())
            {
                return;
            }

            string name = settTxtName.Text;
            string phone = settTxtPhone.Text;
            string email = settTxtEmail.Text;
            string address = settTxtAddr.Text;
            string adminEmail = settTxtAdminEmail.Text;
            string stno = settTxtSvcTax.Text;
            string stper = settTxtServiceTax.Text;
            string commision = settTxtCommi.Text;
            string maxdiscount = settTxtMaxDisc.Text;
            Settings s = new Settings();

            Boolean success = s.UpdateCompany(name, phone, email, address, adminEmail, stno, stper, commision, maxdiscount);
            if (success)
            {
                settTxtName.WithError = false;
                settTxtPhone.WithError = false;
                settTxtAddr.WithError = false;
                notifyForm.ShowNotification(true, "Details saved successfully", "Company Details");
            }
            else
            {
                notifyForm.ShowNotification(false, "Something Went wrong during details save, Please try again.", "Error");
            }
        }

        private void metroGrid9_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (gridPckSvcIncl.Rows[e.RowIndex].Cells[3].Value != null)
            {
                gridPckSvcIncl.Rows[e.RowIndex].Cells[4].Value = true;
                gridPckSvcIncl.Rows[e.RowIndex].Cells[4].ReadOnly = false;
            }
            else
            {
                gridPckSvcIncl.Rows[e.RowIndex].Cells[4].Value = false;
                gridPckSvcIncl.Rows[e.RowIndex].Cells[4].ReadOnly = true;
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            string id = gridAllCustomers.Rows[row].Cells[0].Value.ToString();
            SqlDataReader reader = Program.db.get_where("Customers", id);
            if (reader.HasRows)
            {
                reader.Read();
                string custid = reader["Id"].ToString();
                custTxtName.Text = reader["Name"].ToString();
                custTxtPhone.Text = reader["Phone"].ToString();
                custTxtEmail.Text = reader["Email"].ToString();
                custTxtVisit.Text = reader["Visits"].ToString();
                custTxtAddr.Text = reader["Address"].ToString();
                metroTextBox34.Text = custid;
                custBtnSave.Text = "Update";
            }
        }

        private void custSave_Click(object sender, EventArgs e)
        {
            string name = "";
            string Email = "";
            double Phone = 0.0;

            if (IsBlankTextField(custTxtName, custTxtPhone))
            {
                return;
            }

            name = custTxtName.Text;
            Phone = Convert.ToDouble(custTxtPhone.Text);
            int id = -1;
            int Visits = 0;

            if (string.IsNullOrWhiteSpace(metroTextBox34.Text) == false)
            {
                id = Convert.ToInt32(metroTextBox34.Text);
            }

            if (!string.IsNullOrWhiteSpace(custTxtEmail.Text))
            {
                Email = custTxtEmail.Text;
            }

            if (!string.IsNullOrWhiteSpace(custTxtVisit.Text))
            {
                Visits = Convert.ToInt32(custTxtVisit.Text);
            }

            string Address = "";
            if (!string.IsNullOrWhiteSpace(custTxtAddr.Text))
            {
                Address = custTxtAddr.Text;
            }

            Customers cust = new Customers();
            bool success = cust.AddCustomer(id, name, Phone, Email, Visits, Address, null);
            if (success)
            {
                notifyForm.ShowNotification(true, "Customer informarion updated successfully", "Success");
                custTxtName.WithError = false;
                custTxtPhone.WithError = false;
                custTxtEmail.WithError = false;
                this.LoadCustomers();
                this.LoadBilling();
                custBtnNew.PerformClick();
                timerDefaultFocus.Start();
            }
            else
            {
                notifyForm.ShowNotification(false, "Failed to update customer info", "Error");
            }
        }

        private void metroGrid12_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            string SalesId = gridSalesInfo.Rows[row].Cells[6].Value.ToString();
            if (string.IsNullOrWhiteSpace(SalesId) == false)
            {
                Sales s = new Sales();
                string Path = s.GetInvoicePath(SalesId);
                if (string.IsNullOrWhiteSpace(Path) == false)
                {
                    System.Diagnostics.Process.Start(Path);
                }
            }
            if (users.umode == "Store Owner")
            {
                DeleteSalesButton.Enabled = true;
            }
            salesrow = e.RowIndex;
        }

        private void metroDateTime4_ValueChanged(object sender, EventArgs e)
        {
            double amount, total = 0;

            double servicetax = 14.5;

            Settings company = new Settings();
            SqlDataReader c = company.GetCompanyDetails();

            if (c.HasRows)
            {
                c.Read();
                double.TryParse(c["SvcTaxPercentage"].ToString(), out servicetax);
            }

            Sales s = new Sales();
            SqlDataReader reader = s.GetSalesByDate(metroDateTime4.Value);
            double cashsale = 0.0, cardsale = 0.0, amt = 0.0;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    amount = Convert.ToDouble(reader["amount"]);
                    total = total + amount;
                    string paymentmode = reader["PaymentMode"].ToString();
                    if (paymentmode == "Cash")
                    {
                        amt = Convert.ToDouble(reader["Amount"]);
                        cashsale = cashsale + amt;

                    }
                    else
                    {
                        amt = Convert.ToDouble(reader["Amount"]);
                        cardsale = cardsale + amt;
                    }
                }
            }

            salLabelDailyTotal.Text = cashsale.ToString("n2");
            salLabelMonthTotal.Text = cardsale.ToString("n2");
            double dailyTotal = cashsale + cardsale;
            salLabelDailyWTax.Text = dailyTotal.ToString("n2");
            dailyTotal = (cashsale + cardsale) + ((cashsale + cardsale) * (servicetax / 100));
            salLabelDailyTax.Text = "\u20B9 " + dailyTotal.ToString("n2");
        }

        private void metroTextBoxTimeHr_TextChanged(object sender, EventArgs e)
        {
            if (this.FindForm().Visible == true)
            {
                if (!string.IsNullOrWhiteSpace(aptLabelDuration.Text) && aptComboThep.SelectedItem != null)
                {
                    this.GetAvailibilityStatus();
                }
            }

        }

        private void metroTextBoxTimeMin_TextChanged(object sender, EventArgs e)
        {
            if (this.FindForm().Visible == true)
            {
                if (!string.IsNullOrWhiteSpace(aptLabelDuration.Text) && aptComboThep.SelectedItem != null)
                {
                    this.GetAvailibilityStatus();
                }
            }
        }

        private void metroDateTime1_ValueChanged(object sender, EventArgs e)
        {
            if (this.FindForm().Visible == true)
            {
                if (!string.IsNullOrWhiteSpace(aptLabelDuration.Text) && aptComboThep.SelectedItem != null)
                {
                    this.GetAvailibilityStatus();
                }
            }
        }

        private void DeleteSalesButton_Click(object sender, EventArgs e)
        {
            int salesid = Convert.ToInt32(gridSalesInfo.Rows[salesrow].Cells[6].Value);
            Sales sale = new Sales();
            bool success = false;
            success = sale.delete(salesid);
            if (success)
            {
                //notifyForm.ShowNotification(true, "Sales Deleted", "DB Update", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
            else
            {
                //notifyForm.ShowNotification(this.FindForm(), "Failed!!!!", "Failed", MessageBoxButtons.OK, MessageBoxIcon.None);
            }

            populate_Sales();

        }

        private void button6_Click(object sender, EventArgs e)
        {
            string name = WlktCustName.Text;
            string phone = WlkCustPhone.Text;
            string email = WlkCustEmail.Text;
            string address = WlktCustAdd.Text;
        }

        private void walking_settings()
        {
            aptRadioAppoint.Checked = false;
            aptRadioWalking.Checked = true;
            aptDTBooking.Enabled = false;
            aptTxtTimeHr.Enabled = false;
            aptTxtTimeMin.Enabled = false;
            aptButtonSave.Text = "Checkout";
            buttonHRUp.Enabled = false;
            buttonHRDown.Enabled = false;
            buttonMinDown.Enabled = false;
            buttonMinUp.Enabled = false;
            metroLabel65.Visible = false;
            aptLabelAvailibility.Visible = false;
        }

        private void appointment_settings()
        {
            aptRadioAppoint.Checked = true;
            aptRadioWalking.Checked = false;
            aptDTBooking.Enabled = true;
            aptTxtTimeHr.Enabled = true;
            aptTxtTimeMin.Enabled = true;
            aptButtonSave.Text = "Save";
            buttonHRUp.Enabled = true;
            buttonHRDown.Enabled = true;
            buttonMinDown.Enabled = true;
            buttonMinUp.Enabled = true;
            metroLabel65.Visible = true;
            aptLabelAvailibility.Visible = true;
            aptLabelAvailibility.Text = "N/A";
        }

        private void BookingWalk_Click(object sender, EventArgs e)
        {
            walking_settings();
        }

        private void BookingApp_Click(object sender, EventArgs e)
        {
            appointment_settings();
        }

        public static void ShowCalendar(DateTimePicker picker, MouseEventArgs clickEvent)
        {
            if (picker != null)
            {
                // Remove any existing event to prevent an infinite loop.
                //var suppressor = new EventSuppressor(picker);
                //suppressor.Suppress();

                // Get the position on the button.
                int x = picker.Width - 10;
                int y = picker.Height / 2;
                int lParam = x + y * 0x00010000;

                // Ignore if the calendar button was clicked
                if (clickEvent.X < picker.Width - 20)
                {
                    SendMessage(picker.Handle, WM_LBUTTONDOWN, 1, lParam);
                    SendMessage(picker.Handle, WM_LBUTTONUP, 1, lParam);
                }

                //suppressor.Resume();
            }
        }

        private void dateTime_MouseDown(object sender, MouseEventArgs e)
        {
            var control = (MetroDateTime)sender;
            ShowCalendar(control, e);
        }

        private void textBoxNumericKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBoxDecimalKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!UIUtility.IsValidDecimalNumber(sender, e))
            {
                e.Handled = true;
            }
        }

        private void aptTxtSearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && aptTxtSearchPhone.Text.Length >= UIUtility.PhoneMinLength)
            {
                aptBtnGo.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

        private void Validate_PhoneField(object sender, EventArgs e)
        {
            var control = (MetroTextBox)sender;
            if (control.Text.Length < UIUtility.PhoneMinLength)
            {
                control.WithError = true;
            }
        }

        private void Validate_EmailField(object sender, EventArgs e)
        {
            var control = (MetroTextBox)sender;
            if (!UIUtility.IsValidEmailAddress(control.Text))
            {
                control.WithError = true;
            }
        }

        private void aptRadioShowAll_CheckedChanged(object sender, EventArgs e)
        {
            if (aptRadioShowAll.Checked == true)
            {
                DateTime date = DateTime.Now;
                BookingsPopulate(date, true);
            }
        }

        private void aptRadioFilter_CheckedChanged_1(object sender, EventArgs e)
        {
            if (aptRadioFilter.Checked == true)
            {
                DateTime date = aptDTFilter.Value;
                BookingsPopulate(date, false);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            buy_package();
        }

        private void timerDefaultFocus_Tick(object sender, EventArgs e)
        {
            //Bookings
            if (metroTabControl.SelectedIndex == 0)
            {
                aptTxtSearchPhone.Focus();
            }
            //Service
            else if (metroTabControl.SelectedIndex == 1)
            {
                svcTxtName.Focus();
            }
            //Package
            else if (metroTabControl.SelectedIndex == 2)
            {
                pckTxtName.Focus();
            }
            //Thep
            else if (metroTabControl.SelectedIndex == 3)
            {
                thepTxtName.Focus();
            }
            //Cust
            else if (metroTabControl.SelectedIndex == 4)
            {
                custTxtSearch.Focus();
            }
            //Stock
            else if (metroTabControl.SelectedIndex == 5)
            {
                stkTxtName.Focus();
            }
            //Setting
            else if (metroTabControl.SelectedIndex == 7)
            {
                settTxtName.Focus();
            }

            timerDefaultFocus.Stop();
        }

        private void timerFormActive_Tick(object sender, EventArgs e)
        {
            metroTabControl.Focus();
            timerFormActive.Stop();
            timerDefaultFocus.Start();
        }
        
        private void metroTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            timerDefaultFocus.Start();
        }

        private void thepBtnNew_Click(object sender, EventArgs e)
        {
            thepTxtName.Text = "";
            thepTxtName.WithError = false;
            thepTxtPhoner.Text = "";
            thepTxtPhoner.WithError = false;
            thepTxtEmail.Text = "";
            thepTxtEmail.WithError = false;
            thepTxtAddr.Text = "";
            thepTxtAddr.WithError = false;

            thepTxtName.Focus();
        }

        private void custTxtSearch_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                custBtnGo.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

        private void custBtnNew_Click(object sender, EventArgs e)
        {
            custTxtName.Text = "";
            custTxtSearch.Text = "";
            custTxtPhone.Text = "";
            custTxtEmail.Text = "";
            custTxtAddr.Text = "";

            custTxtName.Focus();
        }

        private void stkBtnNew_Click(object sender, EventArgs e)
        {
            stkTxtName.Text = "";
            stkTxtAmount.Text = "";
            stkComboUnit.SelectedIndex = -1;
            stkComboUnit.Text = "";
        }

        private void search_customers(string phone)
        {
            SqlDataReader reader = Program.db.Search("Customers", phone, "Phone");

            if (reader.HasRows)
            {
                int rows = 0;
                gridAllCustomers.Rows.Clear();
                while (reader.Read())
                {
                    gridAllCustomers.Rows.Add();

                    gridAllCustomers.Rows[rows].Cells[0].Value = reader["Id"].ToString();
                    gridAllCustomers.Rows[rows].Cells[1].Value = reader["Name"].ToString();
                    gridAllCustomers.Rows[rows].Cells[2].Value = reader["Phone"].ToString();
                    gridAllCustomers.Rows[rows].Cells[3].Value = reader["Email"].ToString();
                    gridAllCustomers.Rows[rows].Cells[4].Value = reader["Address"].ToString();
                    gridAllCustomers.Rows[rows].Cells[5].Value = reader["Visits"].ToString();

                    rows++;
                }
            }
            else
            {
                notifyForm.ShowNotification(null, "No customer found with number: " + phone, "Customer not found");
            }
        }

        private void custBtnGo_Click(object sender, EventArgs e)
        {
            if (CusRadioSearch.Checked == true)
            {
                populateCustomers();
                if (custTxtSearch.Text.Length >= UIUtility.PhoneMinLength)
                {
                    string term = custTxtSearch.Text;
                    if (term.All(char.IsDigit))
                    {
                        search_customers(term);
                    }
                }
            }
        }

        private void toTitleCase_Leave(object sender, EventArgs e)
        {
            MetroTextBox textBox = (MetroTextBox)sender;
            if (textBox != null)
            {
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                if (myTI != null)
                {
                    textBox.Text = myTI.ToTitleCase(textBox.Text);
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Program.db.disconnectedDB();
        }

        private void CusRadioShowAll_CheckedChanged(object sender, EventArgs e)
        {
            if(CusRadioShowAll.Checked == true)
            {
                populateCustomers();
            }
        }

        private void SalesRadioShowAll_CheckedChanged(object sender, EventArgs e)
        {
            if(SalesRadioShowAll.Checked == true)
            {
                populate_Sales();
            }
        }

        private void SalesRadioFilter_CheckedChanged(object sender, EventArgs e)
        {
            if(SalesRadioFilter.Checked == true)
            {
                DateTime date = SalesDateFilter.Value;
                populate_Sales(date);
            }
        }

        private bool ValidateSettingsFields()
        {
            //name, ph null check
            if (IsBlankTextField(settTxtName, settTxtPhone, settTxtAddr, settTxtSvcTax, settTxtServiceTax, settTxtCommi, settTxtMaxDisc))
            {
                return false;
            }

            //tax info >= 100 check
            double percentValue;
            double.TryParse(settTxtServiceTax.Text, out percentValue);
            if (percentValue >= 100.0F)
            {
                settTxtServiceTax.WithError = true;
                notifyForm.ShowNotification(true, "Service tax cannot be more than 100", "Invalid input");
                return false;
            }
            double.TryParse(settTxtCommi.Text, out percentValue);
            if (percentValue >= 100.0F)
            {
                settTxtCommi.WithError = true;
                notifyForm.ShowNotification(true, "Commition cannot be more than 100", "Invalid input");
                return false;
            }
            double.TryParse(settTxtMaxDisc.Text, out percentValue);
            if (percentValue >= 100.0F)
            {
                settTxtMaxDisc.WithError = true;
                notifyForm.ShowNotification(true, "Max discount cannot be more than 100", "Invalid input");
                return false;
            }

            return true;
        }

        private void aptRadioWalking_MouseClick(object sender, MouseEventArgs e)
        {
            aptTxtSearchPhone.Focus();
        }
    }
}
