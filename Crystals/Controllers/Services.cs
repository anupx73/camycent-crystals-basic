using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crystals.Models;
using System.Data.SqlClient;

namespace Crystals.Controllers
{
    class Services
    {

        /// <summary>
        /// This Inserts or Updates the Services along with  
        /// all the Items required with its quantity.
        /// </summary>
        /// <param name="ServiceId">Service ID (int)</param>
        /// <param name="name">Name of the Service</param>
        /// <param name="duration">Duration of the Service</param>
        /// <param name="cost">Price of the Service</param>
        /// <param name="desc">Description of the Service</param>
        /// <param name="data">Key Should be Service ID and Value Should be Amount</param>
        /// <returns>True or False</returns>
        public Boolean RegisterService(int ServiceId, string name, int duration, double cost, string desc, Dictionary<int, double> data)
        {
            if (ServiceId == -1)
            {
                //Insert Service

                Boolean success = this.Create(name, duration, cost, desc, data);

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

                Dictionary<int, double> up = new Dictionary<int, double>();

                Dictionary<int, double> del = new Dictionary<int, double>();

                //Getting the List of all the Items in DB for the Service
                SqlDataReader r = this.ItemResourceReqd(ServiceId);

                List<int> rm = new List<int>();


                // Building the Update Dictionary
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        //Adding Everything to the Del Dictionary

                        del.Add(Convert.ToInt32(r["ItemId"]), Convert.ToDouble(r["Amount"]));

                        foreach (KeyValuePair<int, double> i in data)
                        {
                            //If The ItemId is in DB and also in the data sent
                            if (Convert.ToInt32(r["ItemId"]) == i.Key)
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

                foreach (KeyValuePair<int, double> i in del)
                {
                    //Now Temp has all the ItemId which user wants to remove from service association

                    Dictionary<string, string> delrow = new Dictionary<string, string>();
                    delrow.Add("ItemId", i.Key.ToString());
                    delrow.Add("ServiceId", ServiceId.ToString());

                    //Deleting

                    Program.db.delete_where_custom("ServiceResource", delrow);
                }


                //Update Service 

                Boolean success = this.udpate(ServiceId, name, duration, cost, desc, data);

                return success;



            }


        }


        /// <summary>
        /// Inserts the Service with the Items and its Quantity
        /// </summary>
        /// <param name="ServiceId">Service ID (int)</param>
        /// <param name="name">Name of the Service</param>
        /// <param name="duration">Duration of the Service</param>
        /// <param name="cost">Price of the Service</param>
        /// <param name="desc">Description of the Service</param>
        /// <param name="data">Key Should be Service ID and Value Should be Amount</param>
        /// <returns>True or False</returns>
        /// <returns></returns>
        private Boolean Create(string name, int duration, double cost, string desc, Dictionary<int, double> data)
        {
            try
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
                d.Add("Name", "'" + name + "'");
                d.Add("AHTime", "" + duration + "");
                d.Add("Price", "" + cost + "");
                d.Add("Description", "'" + desc + "'");
                d.Add("Active", "" + 1 + "");
                Boolean success = Program.db.insert("Services", d);
                if (success)
                {
                    int ServiceId = Program.db.GetLastInsertedID("Services");
                    foreach (KeyValuePair<int, double> i in data)
                    {
                        Dictionary<string, string> resdata = new Dictionary<string, string>();
                        resdata.Add("ServiceId", "" + ServiceId + "");
                        resdata.Add("ItemId", "" + i.Key + "");
                        resdata.Add("Amount", "" + i.Value + "");
                        Program.db.insert("ServiceResource", resdata);
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
        /// Updates the Service with the Items and its Quantity
        /// </summary>
        /// <param name="ServiceId">Service ID (int)</param>
        /// <param name="name">Name of the Service</param>
        /// <param name="duration">Duration of the Service</param>
        /// <param name="cost">Price of the Service</param>
        /// <param name="desc">Description of the Service</param>
        /// <param name="data">Key Should be Service ID and Value Should be Amount</param>
        /// <returns>True or False</returns>
        /// <returns></returns>
        private Boolean udpate(int ServiceId, string name, int duration, double cost, string desc, Dictionary<int, double> data)
        {
            try
            {
                string whereService = " where Id = " + ServiceId.ToString();
                Dictionary<string, string> d = new Dictionary<string, string>();
                d.Add("Name", "'" + name + "'");
                d.Add("AHTime", "" + duration + "");
                d.Add("Price", "" + cost + "");
                d.Add("Description", "'" + desc + "'");
                d.Add("Active", "" + 1 + "");
                try
                {
                    Program.db.update("Services", d, whereService);
                }
                catch (Exception ex)
                {
                    Log.SQLError(ex.Message);
                    return false;

                }



                foreach (KeyValuePair<int, double> i in data)
                {

                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("ServiceId", ServiceId.ToString());
                    res.Add("ItemId", i.Key.ToString());
                    res.Add("Amount", i.Value.ToString());

                    SqlDataReader reader = Program.db.get_where_custom("ServiceResource", res);

                    if (reader.HasRows)
                    {
                        //Update Item Set

                        string where = "ItemId = " + i.Key.ToString() + " and ServiceId = " + ServiceId;
                        try
                        {
                            Program.db.update("ServiceResource", res, where);
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
                            Program.db.insert("ServiceResource", res);
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
        /// Gives a list of all the all The items Reqd for a service
        /// Along with its Quantity
        /// </summary>
        /// <param name="ServiceId">Service ID</param>
        /// <returns>SQLDataReader</returns>
        public SqlDataReader ItemResourceReqd(int ServiceId)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("ServiceId", "" + ServiceId + "");

            SqlDataReader reader = Program.db.get_where_custom("ServiceResource", d);
            return reader;

        }

        /// <summary>
        /// Removes the Service with the Items and its Quantity
        /// </summary>
        /// <param name="ServiceId">Service ID (int)</param>
        /// <param name="name">Name of the Service</param>
        /// <param name="duration">Duration of the Service</param>
        /// <param name="cost">Price of the Service</param>
        /// <param name="desc">Description of the Service</param>
        /// <param name="data">Key Should be Service ID and Value Should be Amount</param>
        /// <returns>True or False</returns>
        /// <returns></returns>
        public Boolean DeRegisterService(int ServiceId)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();

            d.Add("ServiceId", ServiceId.ToString());

            Program.db.delete_where_custom("ServiceResource", d);
            if (Program.db.error == false)
            {
                Program.db.delete("Services", ServiceId);
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

        public string GetServiceID(string ServiceName)
        {
            try
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
                d.Add("Name", "'" + ServiceName + "'");
                SqlDataReader reader = Program.db.get_where_custom("Services", d);
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

        public string GetServiceDuration(string ServiceId)
        {
            try
            {
                SqlDataReader reader = Program.db.get_where("Services", ServiceId.ToString());
                reader.Read();
                string Duration = reader["AHTime"].ToString();
                return Duration;
            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);
                return null;
            }

        }

        public string GetServicePrice(string ServiceId)
        {
            try
            {
                SqlDataReader reader = Program.db.get_where("Services", ServiceId.ToString());
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

        public string GetServiceName(string ServiceId)
        {
            try
            {
                SqlDataReader reader = Program.db.get_where("Services", ServiceId.ToString());
                reader.Read();
                string name = reader["Name"].ToString();
                return name;
            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);
                return null;
            }

        }

        public string GetServiceDesc(string ServiceId)
        {
            try
            {
                SqlDataReader reader = Program.db.get_where("Services", ServiceId.ToString());
                reader.Read();
                string desc = reader["Description"].ToString();
                return desc;
            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);
                return null;
            }

        }

    }
}
