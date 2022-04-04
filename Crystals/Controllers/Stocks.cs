using Crystals.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystals.Controllers
{
    class Stocks
    {
        CrudDB db;
        public Stocks()
        {
            db = new CrudDB();
        }

        public double GetCurrentAmount(string id)
        {
            double CurrentAmount = 0;
            SqlDataReader reader = Program.db.get_where("Stocks", id);
            if (reader.HasRows)
            {
                reader.Read();
                CurrentAmount = Convert.ToDouble(reader["CurrentStock"]);
                return CurrentAmount;
            }
            return 0;
        }
        public bool Add(int itemId, string itemName, string unit, double amount)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("ItemName", "'" + itemName + "'");
            data.Add("Unit", "'" + unit + "'");
            data.Add("Amount", amount.ToString());
            DateTime now1 = DateTime.Now;
            string month = now1.ToString("MMMM");
            string stockmonth=null;
            Dictionary<string, string> datastock = new Dictionary<string, string>();
            datastock.Add("itemId", itemId.ToString());

            SqlDataReader reader = Program.db.get_where_custom("Stocks",datastock);
            if(reader.HasRows)
            {
                reader.Read();
                stockmonth=reader["Month"].ToString();
            }
            
            Boolean successItem = false;
            if (itemId == -1)
            {
                successItem = db.insert("items", data);
            }
            else
            {
                
                string wc = " where Id = " + itemId.ToString();
                successItem = Program.db.update("items", data, wc);
            }


            if (successItem)
            {
                data.Clear();
                int idVal = db.GetLastInsertedID("items");
                DateTime now = DateTime.Now;
                
                
                data.Add("OpeningStock", "'" + amount.ToString() + "'");
                data.Add("CurrentStock", "'" + amount.ToString() + "'");
                data.Add("ClosingStock", "'" + amount.ToString() + "'");
                data.Add("Month", "'" + now.ToString("MMMM") + "'");
                data.Add("Year", "'" + now.Year.ToString() + "'");
                if (itemId == -1)
                {
                    data.Add("itemId", "" + idVal + "");
                    successItem = db.insert("Stocks", data);
                }
                else
                {
                    if (month!=stockmonth)
                    {
                        Dictionary<string, string> data1 = new Dictionary<string, string>();
                        data1.Add("OpeningStock", "'" + amount.ToString() + "'");
                        data1.Add("CurrentStock", "'" + amount.ToString() + "'");
                        data1.Add("ClosingStock", "'" + amount.ToString() + "'");
                        data1.Add("Month", "'" + now.ToString("MMMM") + "'");
                        data1.Add("Year", "'" + now.Year.ToString() + "'");
                        data1.Add("itemId", "'" + itemId + "'");
                       
                        successItem = db.insert("Stocks", data1);
                    }
                    else
                    {
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2.Add("CurrentStock", "'" + amount.ToString() + "'");
                        data2.Add("ClosingStock", "'" + amount.ToString() + "'");
                        string wc = " where itemId = " + itemId.ToString();

                        data.Add("itemId", "" + itemId + "");
                        successItem = Program.db.update("Stocks", data2, wc);
                    }
                }
            }
            return successItem;
        }
        public bool delete(int itemId)
        {
            // string id="ItemId";
            Boolean successItem = false;
            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("ItemId", "" + itemId + "");

            successItem = Program.db.delete_where_custom("ServiceResource", data);
            successItem = Program.db.delete_where_custom("Stocks", data);
            successItem = Program.db.delete("items", itemId);

            return successItem;
        }
        public SqlDataReader select(string tableName)
        {
            return db.get(tableName);
        }


        public bool update(string stockMode, int ItemId, string month, string year, string amount)
        {

            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("itemId", "'" + ItemId + "'");
            SqlDataReader reader = db.get_where_custom("Stocks", data);
            if (reader.HasRows)
            {

                data.Clear();
                data.Add("Month", "'" + month + "'");
                data.Add("Year", "'" + year + "'");

                if (stockMode == "Opening Stock")
                    data.Add("OpeningStock", "" + amount + "");
                else
                    data.Add("ClosingStock", "" + amount + "");
                string whereClause = "itemId = " + ItemId;
                reader.Close();
                bool success = db.update("Stocks", data, whereClause);
                return success;
            }
            else
            {
                data.Clear();
                data.Add("Month", "'" + month + "'");
                data.Add("Year", "'" + year + "'");

                if (stockMode == "Opening Stock")
                    data.Add("OpeningStock", "" + amount + "");
                else
                    data.Add("ClosingStock", "" + amount + "");
                string whereClause = "itemId = " + ItemId;
                reader.Close();
                bool success = db.insert("Stocks", data);

                return false;
            }
        }

        public Boolean Search(string month)
        {
            return true;
        }
        public SqlDataReader GetStocksByDate(DateTime date)
        {
            SqlDataReader reader;

            Dictionary<string, string> d = new Dictionary<string, string>();

            d.Add("Month", "'" + date.ToString("MMMM") + "'");

            reader = Program.db.get_where_custom_desc("Stocks", d);

            return reader;
        }

        public void StockAutomated(List<int> ServiceIds)
        {
            SqlDataReader reader;
            Dictionary<string, string> d = new Dictionary<string, string>();

            foreach (var i in ServiceIds)
            {
                d.Add("ServiceId", i.ToString());

                reader = Program.db.get_where_custom("ServiceResource", d);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //Deduct Stock

                        //get Current Stock Amount

                        Dictionary<string, string> data = new Dictionary<string, string>();

                        data.Add("itemId", reader["ItemId"].ToString());

                        SqlDataReader sreader = Program.db.get_where_custom("Stocks", data);
                        if (sreader.HasRows)
                        {
                            sreader.Read();
                            double currAmt = Convert.ToDouble(sreader["CurrentStock"]);
                            double deAmt = Convert.ToDouble(reader["Amount"]);

                            sreader.Close();

                            d.Clear();

                            //Getting the Unit for the Item
                            sreader = Program.db.get_where("items", reader["ItemId"].ToString());
                            if (sreader.HasRows)
                            {
                                sreader.Read();

                                string unit = sreader["Unit"].ToString();

                                sreader.Close();
                                d.Clear();

                                if (unit == "LTRS" || unit == "KG")
                                {
                                    //Unit Conversion

                                    //   deAmt = deAmt * 1000;
                                }

                                currAmt = currAmt - deAmt;

                                d.Add("CurrentStock", currAmt.ToString());

                                //Updating the Stock.
                                string whereStocks = " where itemId = " + reader["ItemId"].ToString();
                                Program.db.update("Stocks", d, whereStocks);
                            }
                        }

                    }
                    reader.Close();
                }
                d.Clear();
            }
        }






    }
}
