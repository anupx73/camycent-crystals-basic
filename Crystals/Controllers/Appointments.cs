using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Crystals.Controllers
{
    class Appointments
    {
       public static string AppId;
        public Boolean AppointmentAvailable(int TherapistId, DateTime date, double duration)
        {



            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("TherapistId", TherapistId.ToString());

            string sessiondate = date.ToString("yyyy-MM-dd");

            data.Add("SessionDate", "'" + sessiondate + "'");
            data.Add("Status", "'" + "Booked" + "'");

            SqlDataReader reader = Program.db.get_where_custom("Appointments", data);
            Boolean available = true;

            if (reader.HasRows)
            {
                //The Therapist has Appointments on that date

                //Check if the Time for the Appointment OverLaps any of his/her Appointments on that day


                try
                {
                    while (reader.Read())
                    {

                        DateTime start = Convert.ToDateTime(reader["StartTime"]);
                        DateTime end = Convert.ToDateTime(reader["EndTime"]);
                        DateTime sessionend = Convert.ToDateTime(date.AddMinutes(duration));

                        //booked start a
                        long startticks = start.Ticks;
                        //booked end b
                        long endticks = end.Ticks;
                        //requested start c
                        long sesstart = date.Ticks;
                        //requested end d
                        long sesend = sessionend.Ticks;
                        //(StartA <= EndB)  and  (EndA >= StartB)
                        if (sesstart > endticks || sesend < startticks)
                        {
                            available = true;
                        }
                        else
                        {
                            AppId=reader["Id"].ToString();
                            available = false;
                        }



                    }

                }
                catch (Exception ex)
                {
                    Log.AppError(ex.Message);
                    return false;
                }


            }
            else
            {
                return true;
            }

            return available;


        }

        public Boolean RegisterServiceAppoinment(int TherapistId, double duration, double Price, DateTime date, List<int> Services, string name, string phone, string email, int visits, string address)
        {

            //Check Customer Id

            if(string.IsNullOrWhiteSpace(phone))
            {
                return false;
            }

            Customers cus = new Customers();
            string cusId = cus.CustomerExsits(phone);
            int CustomerId;
            if (cusId == null)
            {
                Boolean resp = cus.AddCustomer(-1, name, Convert.ToDouble(phone), email, visits, address);
                if (resp)
                {
                    CustomerId = Program.db.GetLastInsertedID("Customers");
                }
                else
                {
                    Log.AppError("Failed to create Customer, during Appointment. Appointments.cs - 93");
                    return false;
                }
            }
            else
            {
                CustomerId = Convert.ToInt32(cusId);
            }

            Boolean success = false;

            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("CustomerId", CustomerId.ToString());
            data.Add("TherapistId", TherapistId.ToString());
            data.Add("SessionDate", "'" + date.Date.ToString("yyyy-MM-dd") + "'");
            data.Add("StartTime", "'" + date.ToString("yyyy-MM-dd HH:mm") + "'");

            DateTime endtime = Convert.ToDateTime(date.AddMinutes(duration));
            data.Add("EndTime", "'" + endtime.ToString("yyyy-MM-dd HH:mm") + "'");



            try
            {
                SqlDataReader reader = Program.db.get_where_custom("Appointments", data);
                if (reader.HasRows)
                {
                    reader.Read();
                    string where = " where Id = " + reader["Id"].ToString();

                    data.Add("Status", "'" + "Booked" + "'");
                    data.Add("Price", Price.ToString());

                    Program.db.update("Appointments", data, where);
                    reader.Close();
                }
                else
                {
                    data.Add("Status", "'" + "Booked" + "'");
                    data.Add("Price", Price.ToString());
                    Program.db.insert("Appointments", data);
                }
            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);
                return false;
            }

            if (Program.db.error == false)
            {
                int AppointmentId = Program.db.GetLastInsertedID("Appointments");
                foreach (int id in Services)
                {
                    Dictionary<string, string> d = new Dictionary<string, string>();
                    d.Add("ServiceId", id.ToString());
                    d.Add("AppointmentId", AppointmentId.ToString());
                    d.Add("CustomerId", CustomerId.ToString());

                    SqlDataReader reader = Program.db.get_where_custom("ServiceAppointments", d);

                    if (reader.HasRows)
                    {
                        success = true;
                        continue;
                    }
                    else
                    {
                        Program.db.insert("ServiceAppointments", d);

                        if (Program.db.error == false)
                        {
                            success = true;
                        }
                        else
                        {
                            success = false;
                        }
                    }

                    reader.Close();

                }

            }



            return success;
        }

        public SqlDataReader GetLiveAppointments()
        {
            SqlDataReader reader;

            DateTime now = DateTime.Today;

            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("SessionDate", "'" + now.ToString("yyyy-MM-dd") + "'");
            d.Add("Status", "'" + "Booked" + "'");

            reader = Program.db.get_where_custom_desc("Appointments", d);

            return reader;

        }

        public Boolean RegisterPackageAppoinment(int TherapistId, int CustomerId, double duration, double Price, DateTime date, string PackageId)
        {
            Boolean success = false;

            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("CustomerId", CustomerId.ToString());
            data.Add("TherapistId", TherapistId.ToString());
            data.Add("SessionDate", "'" + date.Date.ToString("yyyy-MM-dd") + "'");
            data.Add("StartTime", "'" + date.ToString("yyyy-MM-dd HH:mm") + "'");

            DateTime endtime = Convert.ToDateTime(date.AddMinutes(duration));
            data.Add("EndTime", "'" + endtime.ToString("yyyy-MM-dd HH:mm") + "'");



            try
            {
                SqlDataReader reader = Program.db.get_where_custom("Appointments", data);
                if (reader.HasRows)
                {
                    reader.Read();
                    string where = " Id = " + reader["Id"].ToString();

                    data.Add("Status", "'" + "Booked" + "'");
                    data.Add("Price", Price.ToString());

                    Program.db.update("Appointments", data, where);
                    reader.Close();
                }
                else
                {
                    data.Add("Status", "'" + "Booked" + "'");
                    data.Add("Price", Price.ToString());
                    Program.db.insert("Appointments", data);
                }
            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);
                return false;
            }


            if (Program.db.error == false)
            {
                int AppointmentId = Program.db.GetLastInsertedID("Appointments");
                
                Dictionary<string, string> d = new Dictionary<string, string>();
                d.Add("PackageId", PackageId);
                d.Add("AppointmentId", AppointmentId.ToString());
                d.Add("Sessions", "1");
                success = Program.db.insert("PackageAppointments", d);
            }

            return success;
        }

        public List<int> GetServiceList(string AppointmentId)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();

            d.Add("AppointmentId", AppointmentId);

            List<int> ServiceIds = new List<int>();

            SqlDataReader reader = Program.db.get_where_custom("ServiceAppointments", d);

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ServiceIds.Add(Convert.ToInt32(reader["ServiceId"]));

                }
                reader.Close();
            }
            return ServiceIds;
        }

        public SqlDataReader GetAppointmentsByDate(DateTime date)
        {
            SqlDataReader reader;

            Dictionary<string, string> d = new Dictionary<string, string>();

            d.Add("SessionDate", "'" + date.ToString("yyyy-MM-dd") + "'");

            reader = Program.db.get_where_custom_desc("Appointments", d);

            return reader;
        }



        public Boolean CheckOut(int AppointmentId)
        {
            //Change Appointment Status

            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("Status", "'" + "Complete" + "'");

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

            string cusId = this.GetCustomerId(AppointmentId.ToString());
            if (cusId != null)
            {
                cus.IncrementVisits(cusId);
            }

            //Check For Package
            d.Clear();
            reader.Close();
            d.Add("AppointmentId", AppointmentId.ToString());
            reader = Program.db.get_where_custom("PackageAppointments", d);
                   
            if(reader.HasRows)
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
                Boolean success = sale.SalesEntryAppointment(CustomerId, AppId, ServiceIds , amount);

                if (success)
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

        public SqlDataReader GetAppointmentList(string CustomerId)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("CustomerId", CustomerId);

            SqlDataReader reader = Program.db.get_where_custom_desc("Appointments", d);
            return reader;


        }

        public string GetCustomerId(string AppId)
        {
            SqlDataReader reader = Program.db.get_where("Appointments", AppId);
            if (reader.HasRows)
            {
                reader.Read();
                string CusId = reader["CustomerId"].ToString();
                return CusId;

            }
            else
            {
                return null;
            }
        }


    }
}
