using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MetroFramework;

namespace Crystals.Controllers
{
    class Customers
    {
        public Boolean AddCustomer(int CustomerId, string name, double phone, string email, int visits, string address , string gender)
        {
            Dictionary<string, string> resdata = new Dictionary<string, string>();
            resdata.Add("Name", "'" + name + "'");
            resdata.Add("Phone", phone.ToString());

            if (email != null)
            {
                resdata.Add("Email", "'" + email + "'");
            }
            resdata.Add("Visits", visits.ToString());
            if (address != null)
            {
                resdata.Add("Address", "'" + address + "'");
            }

            if(gender != null)
            {
                resdata.Add("Gender", "'" + gender + "'");
            }


            if (CustomerId == -1)
            {
                //Insert customer

                Boolean success = Program.db.insert("Customers", resdata);

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
                //Update Customer 

                string where = " where Id = " + CustomerId;

                Boolean success = Program.db.update("Customers", resdata, where);

                if (success)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

        }

        public string GetCustomerName(string CustomerId)
        {
            String CusName;
            SqlDataReader reader = Program.db.get_where("Customers", CustomerId);
            if (reader.HasRows)
            {
                reader.Read();
                CusName = reader["Name"].ToString();
                reader.Close();
                return CusName;
            }
            else
            {
                reader.Close();
                return null;
            }




        }

        public string CustomerExsits(string Phone)
        {
            if(string.IsNullOrWhiteSpace(Phone))
            {
                return null;
            }
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("Phone", Phone);
            SqlDataReader reader = Program.db.get_where_custom("Customers", d);

            

            if (reader.HasRows)
            {
                reader.Read();
                string CusId = reader["Id"].ToString();
                return CusId;
            }
            else
            {
                string CusId = null;
                return CusId;

            }
        }




        public Boolean IncrementVisits(string CustomerId)
        {
            SqlDataReader reader = Program.db.get_where("Customers", CustomerId);
            if (reader.HasRows)
            {
                reader.Read();
                int CurrCount = Convert.ToInt32(reader["Visits"]);
                CurrCount++;

                Dictionary<string, string> d = new Dictionary<string, string>();
                d.Add("Visits", CurrCount.ToString());
                string where = " where Id=" + CustomerId;
                Boolean success = Program.db.update("Customers", d, where);
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

    }
}
