using Crystals.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crystals.Controllers
{
    class Therapists
    {
        public Therapists()
        {
        }
        public bool Add(string name, string phNo, string email, string addr)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Name", "'" + name + "'");
            data.Add("Phone", "'" + phNo + "'");
            data.Add("Email", "'" + email + "'");
            data.Add("Address", "'" + addr + "'");

            Boolean success = Program.db.insert("dbo.Therapists", data);
            if (success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public SqlDataReader select(string tableName)
        {
            return Program.db.get(tableName);
        }
        public bool update(string phNo)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Phone", phNo);
            Program.db.get_where_custom("Therapists", data);
            return true;
        }
        public Boolean delete(Int32 Id)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Id", Id.ToString());

            return Program.db.delete_where_custom("Therapists", data);
            //db.delete("dbo.Therapists", data);
        }

        public string GetName(string Id)
        {
            String Name;
            SqlDataReader reader = Program.db.get_where("Therapists", Id);
            if (reader.HasRows)
            {
                reader.Read();
                Name = reader["Name"].ToString();
                reader.Close();
                return Name;

            }
            else
            {
                reader.Close();
                return null;
            }
        }

        public string GetId(string Name)
        {
            String Id;

            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("Name", "'" + Name + "'");
            SqlDataReader reader = Program.db.get_where_custom("Therapists", d);
            if (reader.HasRows)
            {
                reader.Read();
                Id = reader["Id"].ToString();
                reader.Close();
                return Id;

            }
            else
            {
                reader.Close();
                return null;
            }
        }

        public double GetTotalSaleByDate(string TherapistID, DateTime date)
        {

            Dictionary<string, string> d = new Dictionary<string, string>();

            //Get All Sales For that Therapists

            string day = Convert.ToDateTime(date).ToString("yyyy-MM-dd");

            d.Add("TherapistId", TherapistID);
            d.Add("SessionDate", "'" + day + "'");

            SqlDataReader reader = Program.db.get_where_custom("Appointments", d);

            d.Clear();

            double TotalSales = 0;

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    //Get the Sales Data

                    TotalSales = TotalSales + Convert.ToDouble(reader["Price"]);
                }
            }

            return TotalSales;

        }

        public double GetTotalCommissionByMonth(string TherapistID, DateTime date)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();

            string month = date.ToString("MM");

            //Get All Sales For that Therapists

            d.Add("TherapistId", TherapistID);

            SqlDataReader reader = Program.db.get_where_custom("Appointments", d);

            d.Clear();

            double TotalSales = 0;
            double comm = 0;

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    //Get the Sales Data
                    DateTime sdate = Convert.ToDateTime(reader["SessionDate"]);
                    string sm = sdate.ToString("MM");

                    if (month == sm)
                    {
                        TotalSales = TotalSales + Convert.ToDouble(reader["Price"]);
                    }

                }

                if (TotalSales != 0.0)
                {
                    comm = 0.05 * TotalSales;
                }
            }
            reader.Close();
            return comm;

        }

        public double GetTotalSalesByMonth(string TherapistID, DateTime date)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();

            string month = date.ToString("MM");

            //Get All Sales For that Therapists

            d.Add("TherapistId", TherapistID);

            SqlDataReader reader = Program.db.get_where_custom("Appointments", d);

            d.Clear();

            double TotalSales = 0;
            

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    //Get the Sales Data
                    DateTime sdate = Convert.ToDateTime(reader["SessionDate"]);
                    string sm = sdate.ToString("MM");

                    if (month == sm)
                    {
                        TotalSales = TotalSales + Convert.ToDouble(reader["Price"]);
                    }

                }

               
            }
            reader.Close();
            return TotalSales;

        }



    }
}
