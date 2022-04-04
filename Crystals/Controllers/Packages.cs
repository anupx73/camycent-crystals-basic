using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace Crystals.Controllers
{
    class Packages
    {
        /// <summary>
        /// This Both Inserts or Updates the packages along with the Services allowed
        /// and the No of sessions alloted with for the Service.
        /// </summary>
        /// <param name="PackageId">Package ID</param>
        /// <param name="name">Name of the Service (String)</param>
        /// <param name="duration">Validity of the Service In Months (int)</param>
        /// <param name="price">Price of the package (double)</param>
        /// <param name="value">Value of services Alloted (double)</param>
        /// <param name="desc">Description of the Package (string)</param>
        /// <param name="data"> This is a Dictionary of int , int. They Key should be the ServiceID and The Value Should Be NoOfSessions</param>
        /// <returns>True or False</returns>
        public Boolean RegisterPackage(int PackageId, string name, int duration, double price, double value, string desc, Dictionary<int, int> data)
        {
            if (PackageId == -1)
            {
                //Insert Service

                Boolean success = this.Create(name, duration, price, value, desc, data);

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

                //Separating the Items to be Updated and Deleted.

                Dictionary<int, int> up = new Dictionary<int, int>();

                Dictionary<int, int> del = new Dictionary<int, int>();

                //Getting the List of all the Items in DB for the Service
                SqlDataReader r = this.PackageServices(PackageId);

                List<int> rm = new List<int>();


                // Building the Update Dictionary
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        //Adding Everything to the Del Dictionary

                        del.Add(Convert.ToInt32(r["ServiceId"]), Convert.ToInt32(r["Sessions"]));

                        foreach (KeyValuePair<int, int> i in data)
                        {
                            //If The ItemId is in DB and also in the data sent
                            if (Convert.ToInt32(r["ServiceId"]) == i.Key)
                            {
                                //Adding the Row in the Update Dictionary
                                up.Add(i.Key, i.Value);

                                //Removing the DB Found Item from the temp Dictionary
                                rm.Add(i.Key);
                                break;

                            }
                        }

                    }

                    r.Close();
                }

                //Removing items to be Updated from the Main Dictionary

                foreach (int i in rm)
                {
                    del.Remove(i);
                }

                //Building the Delete Dictionary

                foreach (KeyValuePair<int, int> i in del)
                {
                    //Now Temp has all the ItemId which user wants to remove from service association

                    Dictionary<string, string> delrow = new Dictionary<string, string>();
                    delrow.Add("ServiceId", i.Key.ToString());
                    delrow.Add("PackageId", PackageId.ToString());

                    //Deleting

                    Program.db.delete_where_custom("PackageDetails", delrow);
                }


                //Update Service 

                Boolean success = this.udpate(PackageId, name, duration, price, value, desc, data);

                return success;



            }


        }

        /// <summary>
        /// This Inserts the Package with its Services
        /// </summary>
        /// <param name="PackageId">Package ID</param>
        /// <param name="name">Name of the Service (String)</param>
        /// <param name="duration">Validity of the Service In Months (int)</param>
        /// <param name="price">Price of the package (double)</param>
        /// <param name="value">Value of services Alloted (double)</param>
        /// <param name="desc">Description of the Package (string)</param>
        /// <param name="data"> This is a Dictionary of int , int. They Key should be the ServiceID and The Value Should Be NoOfSessions</param>
        /// <returns>True or False</returns>
        private Boolean Create(string name, int duration, double price, double value, string desc, Dictionary<int, int> data)
        {
            try
            {
                Dictionary<string, string> d = new Dictionary<string, string>();

                d.Add("Name", "'" + name + "'");
                d.Add("Duration", "" + duration + "");
                d.Add("Price", "" + price + "");
                d.Add("Value", "" + value + "");
                d.Add("Description", "'" + desc + "'");

                Boolean success = Program.db.insert("Packages", d);
                if (success)
                {
                    int PackageId = Program.db.GetLastInsertedID("Packages");
                    foreach (KeyValuePair<int, int> i in data)
                    {
                        Dictionary<string, string> resdata = new Dictionary<string, string>();
                        resdata.Add("PackageId", "" + PackageId + "");
                        resdata.Add("ServiceId", "" + i.Key + "");
                        resdata.Add("Sessions", "" + i.Value + "");
                        Program.db.insert("PackageDetails", resdata);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// This Updates the packages along with its Services and Sessions Alloted.
        /// </summary>
        /// <param name="PackageId">Package ID</param>
        /// <param name="name">Name of the Service (String)</param>
        /// <param name="duration">Validity of the Service In Months (int)</param>
        /// <param name="price">Price of the package (double)</param>
        /// <param name="value">Value of services Alloted (double)</param>
        /// <param name="desc">Description of the Package (string)</param>
        /// <param name="data"> This is a Dictionary of int , int. They Key should be the ServiceID and The Value Should Be NoOfSessions</param>
        /// <returns>True or False</returns>
        /// <returns></returns>
        private Boolean udpate(int PackageId, string name, int duration, double price, double value, string desc, Dictionary<int, int> data)
        {
            try
            {
                string wherePackage = "Id = " + PackageId.ToString();
                Dictionary<string, string> d = new Dictionary<string, string>();

                d.Add("Name", "'" + name + "'");
                d.Add("Duration", "" + duration + "");
                d.Add("Price", "" + price + "");
                d.Add("Value", "" + value + "");
                d.Add("Description", "'" + desc + "'");

                try
                {
                    Program.db.update("Packages", d, wherePackage);
                }
                catch (Exception ex)
                {
                    Log.SQLError(ex.Message);
                    return false;

                }



                foreach (KeyValuePair<int, int> i in data)
                {

                    Dictionary<string, string> resdata = new Dictionary<string, string>();
                    resdata.Add("PackageId", "" + PackageId + "");
                    resdata.Add("ServiceId", "" + i.Key + "");
                    resdata.Add("Sessions", "" + i.Value + "");

                    SqlDataReader reader = Program.db.get_where_custom("PackageDetails", resdata);

                    if (reader.HasRows)
                    {
                        //Update Item Set

                        string where = "ServiceId = " + i.Key.ToString() + " and PackageId = " + PackageId;
                        try
                        {
                            Program.db.update("PackageDetails", resdata, where);
                        }
                        catch (Exception ex)
                        {
                            Log.SQLError(ex.Message);
                            return false;
                        }

                    }
                    else
                    {
                        //insert Item Set

                        try
                        {
                            Program.db.insert("PackageDetails", resdata);
                        }
                        catch (Exception ex)
                        {
                            Log.SQLError(ex.Message);
                            return false;
                        }
                    }

                    reader.Close();


                }

                return true;

            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gives the List of all the Services and its alloted sessions for a Package
        /// </summary>
        /// <param name="PackageId">Package ID</param>
        /// <returns>SQLDataReader</returns>
        public SqlDataReader PackageServices(int PackageId)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("PackageId", "" + PackageId + "");

            SqlDataReader reader = Program.db.get_where_custom("PackageDetails", d);
            return reader;

        }

        /// <summary>
        /// Removes the Package based on the PackageId.
        /// And also removes all the Service Allocation associated with the package
        /// </summary>
        /// <param name="PackageId">Package ID</param>
        /// <returns>True or False</returns>
        public Boolean DeRegisterPackage(int PackageId)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();

            d.Add("PackageId", PackageId.ToString());

            Program.db.delete_where_custom("PackageDetails", d);
            if (Program.db.error == false)
            {
                Program.db.delete("Packages", PackageId);
                if (Program.db.error == false)
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

        public string GetPackageID(string PackageName)
        {
            try
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
                d.Add("Name", "'" + PackageName + "'");
                SqlDataReader reader = Program.db.get_where_custom("Packages", d);
                reader.Read();
                string ServiceId = reader["Id"].ToString();
                return ServiceId;
            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);
                return null;
            }

        }

        public string GetPackagePrice(string PackageId)
        {
            try
            {
                SqlDataReader reader = Program.db.get_where("Packages", PackageId.ToString());
                reader.Read();
                string Price = reader["Price"].ToString();
                return Price;
            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);
                return null;
            }

        }

        public string GetPackageDuration(string PackageId)
        {
            try
            {
                SqlDataReader reader = Program.db.get_where("Packages", PackageId.ToString());
                reader.Read();
                string Duration = reader["Duration"].ToString();
                return Duration;
            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);
                return null;
            }

        }

        public string GetPackageName(string PackageId)
        {
            try
            {
                SqlDataReader reader = Program.db.get_where("Packages", PackageId.ToString());
                reader.Read();
                string Name = reader["Name"].ToString();
                return Name;
            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);
                return null;
            }

        }

       
        public string GetPackageDesc(string PackageId)
        {
            try
            {
                SqlDataReader reader = Program.db.get_where("Packages", PackageId.ToString());
                reader.Read();
                string Desc = reader["Description"].ToString();
                return Desc;
            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);
                return null;
            }

        }

        public bool? BuyPackage(int CustomerId, int PackageId)
        {
            int duration = Convert.ToInt32(this.GetPackageDuration(PackageId.ToString()));

            DateTime startdate = Convert.ToDateTime(DateTime.Today.ToString("yyyy-MM-dd"));
            DateTime expiredate = Convert.ToDateTime(startdate.AddMonths(duration).ToString("yyyy-MM-dd"));

            Dictionary<string, string> d = new Dictionary<string, string>();

            d.Add("PackageId", PackageId.ToString());
            d.Add("CustomerId", CustomerId.ToString());
            d.Add("StartDate", "'" + startdate.ToString("yyyy-MM-dd") + "'");
            d.Add("ExpireDate", "'" + expiredate.ToString("yyyy-MM-dd") + "'");

            try
            {
                Program.db.insert("CustomerPackages", d);
                if (Program.db.error == false)
                {
                    //Sales Entry Here 
                    Packages pack = new Packages();
                    double amount = Convert.ToDouble(pack.GetPackagePrice(PackageId.ToString()));

                    Sales s = new Sales();
                    return s.SalesEntryPackages(CustomerId, PackageId, amount);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.AppError(ex.Message);
                return false;
            }
        }

        public List<int> GetPackageCustomer(string CustomerId)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("CustomerId", CustomerId);
            SqlDataReader reader = Program.db.get_where_custom_desc("CustomerPackages", d);
            List<int> pIds = new List<int>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    pIds.Add(Convert.ToInt32(reader["PackageId"]));
                }

                reader.Close();
                return pIds;
            }
            else
            {
                reader.Close();
                return pIds;
            }
        }

        public List<int> ValidServices(string CustomerId, string PackageId)
        {
            List<int> sIds = new List<int>();
            DateTime date = DateTime.Now;
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("CustomerId", CustomerId);
            d.Add("PackageId", PackageId);
            SqlDataReader reader = Program.db.get_where_custom("CustomerPackages", d);
            if (reader.HasRows)
            {
                reader.Read();
                DateTime expDate = Convert.ToDateTime(reader["ExpireDate"]);
                long todayticks = date.Ticks;
                long expireticks = expDate.Ticks;

                if (todayticks > expireticks)
                {
                    return sIds;

                }
                else
                {
                    int CurrSess = Convert.ToInt32(reader["Sessions"]);
                    reader.Close();
                    d.Clear();

                    //Check package Allowed Sessions
                    d.Add("PackageId", PackageId);
                   

                    reader = Program.db.get_where_custom("PackageDetails" , d);
                    if(reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            int AllowedSess = Convert.ToInt32(reader["Sessions"]);
                            if (CurrSess >= AllowedSess)
                            {
                                continue;
                            }
                            else
                            {
                                sIds.Add(Convert.ToInt32(reader["ServiceId"]));
                                continue;
                            }

                        }

                        return sIds;
                       
                    }
                    else
                    {
                        return sIds;
                    }
                }

            }
            else
            {
                return sIds;
            }

        }


    }
}
