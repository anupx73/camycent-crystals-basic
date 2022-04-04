using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Crystals.Controllers
{
    class Bookings
    {
       public static string BookingId;
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
                            BookingId=reader["Id"].ToString();
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

        public int RegisterServiceBooking(int TherapistId, double duration, double Price, DateTime date, List<int> Services, string name, string phone, string email, int visits, string address, string type , string gender, string notes)
        {
            if(string.IsNullOrWhiteSpace(phone))
            {
                return -1;
            }

            Customers cus = new Customers();
            string cusId = cus.CustomerExsits(phone);
            int CustomerId;
            if (cusId == null)
            {
                Boolean resp = cus.AddCustomer(-1, name, Convert.ToDouble(phone), email, visits, address , gender);
                if (resp)
                {
                    CustomerId = Program.db.GetLastInsertedID("Customers");
                }
                else
                {
                    Log.AppError("Failed to create Customer, during Appointment. Appointments.cs - 93");
                    return -1;
                }
            }
            else
            {
                CustomerId = Convert.ToInt32(cusId);
                cus.AddCustomer(CustomerId, name, Convert.ToDouble(phone), email, visits, address, gender);
            }

            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("CustomerId", CustomerId.ToString());
            data.Add("TherapistId", TherapistId.ToString());
            data.Add("SessionDate", "'" + date.Date.ToString("yyyy-MM-dd") + "'");
            data.Add("StartTime", "'" + date.ToString("yyyy-MM-dd HH:mm") + "'");
            data.Add("Type", "'" + type + "'");

            DateTime endtime = Convert.ToDateTime(date.AddMinutes(duration));
            data.Add("EndTime", "'" + endtime.ToString("yyyy-MM-dd HH:mm") + "'");

            try
            {
                SqlDataReader reader = Program.db.get_where_custom("Appointments", data);
                if (reader.HasRows)
                {
                    reader.Read();
                    string where = " where Id = " + reader["Id"].ToString();

                    data.Add("Notes", "'" + notes + "'");
                    data.Add("Status", "'" + "Booked" + "'");
                    data.Add("Price", Price.ToString());

                    Program.db.update("Appointments", data, where);
                    reader.Close();
                }
                else
                {
                    data.Add("Notes", "'" + notes + "'");
                    data.Add("Status", "'" + "Booked" + "'");
                    data.Add("Price", Price.ToString());
                    Program.db.insert("Appointments", data);
                }
            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);
                return -1;
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
                        continue;
                    }
                    else
                    {
                        Program.db.insert("ServiceAppointments", d);
                    }
                    reader.Close();
                }

                return AppointmentId;
            }
            else
            {
                return - 1;
            }
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

        public int RegisterPackageBooking(int TherapistId, int CustomerId, double duration, double Price, DateTime date, string PackageId, string type, string gender, string notes)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("CustomerId", CustomerId.ToString());
            data.Add("TherapistId", TherapistId.ToString());
            data.Add("SessionDate", "'" + date.Date.ToString("yyyy-MM-dd") + "'");
            data.Add("StartTime", "'" + date.ToString("yyyy-MM-dd HH:mm") + "'");
            data.Add("Type", "'" + type + "'");
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
                    data.Add("Notes", "'" + notes + "'");
                    Program.db.update("Appointments", data, where);
                    reader.Close();
                }
                else
                {
                    data.Add("Notes", "'" + notes + "'");
                    data.Add("Status", "'" + "Booked" + "'");
                    data.Add("Price", Price.ToString());
                    Program.db.insert("Appointments", data);
                }
            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);
                return -1;
            }


            if (Program.db.error == false)
            {
                int AppointmentId = Program.db.GetLastInsertedID("Appointments");
                
                Dictionary<string, string> d = new Dictionary<string, string>();
                d.Add("PackageId", PackageId);
                d.Add("AppointmentId", AppointmentId.ToString());
                d.Add("Sessions", "1");
                Program.db.insert("PackageAppointments", d);
                return AppointmentId;
            }
            else
            {
                return -1;
            }
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

            string query = "select * from Appointments where SessionDate='" + date.ToString("yyyy-MM-dd") + "' and Status !='Canceled' order by Id desc";
            reader = Program.db.custom_reader_query(query);

            return reader;
        }

        public SqlDataReader GetAllAppointments()
        {
            SqlDataReader reader;

            string query = "select * from Appointments where Status !='Canceled' order by Id desc";
            reader = Program.db.custom_reader_query(query);

            return reader;
        }

        public bool CheckOut(int AppointmentId , string type, Form owner)
        {
            Invoice invoice = new Invoice();
            bool success = invoice.checkout(AppointmentId, type, owner);
            return success;

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
