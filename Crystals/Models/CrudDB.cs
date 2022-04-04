using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Crystals.Models
{
    class CrudDB
    {
        #region Constructors
        public CrudDB()
        {
            this.connStr = null;
            this.conn = null;
            this.error = false;
            this.tablename = null;
            this.q = null;

        }
        #endregion

        #region DataMembers
        private string connStr;
        private string q;
        private string tablename;
        private SqlConnection conn;
        public Boolean error;
        #endregion

        #region GetterSetters
        public Boolean getError()
        {
            return this.error;
        }

        public void setConnStr(Boolean Error)
        {
            this.error = Error;

        }

        public string getConnStr()
        {
            return this.connStr;
        }

        public void setConnStr(string connStr)
        {
            this.connStr = connStr;

        }


        public void setTable(string table)
        {
            this.tablename = table;
        }

        private string getTable()
        {
            return this.tablename;
        }
        #endregion

        #region Connection
        public Boolean connectDB()
        {
            string dataDirectory = RegistryLicense.GetDataBaseLocation();
            if (string.IsNullOrEmpty(dataDirectory))
            {
                return false;
            }

            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);
            try
            {
                if (this.connStr == null)
                {
                    this.connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|Crystals.mdf;integrated security=false;MultipleActiveResultSets=true";
                }
                this.conn = new SqlConnection(this.connStr);
                this.conn.Open();
            }
            catch (Exception ex)
            {
                Log.SQLError("ERR:101 Failed to Connect DB" + ex.Message);
                return false;
            }

            return true;
        }

        public Boolean disconnectedDB()
        {
            try
            {
                this.conn.Close();
            }
            catch (Exception ex)
            {
                Log.SQLError(ex.ToString(), false);
                return false;
            }

            return true;

        }

        #endregion

        #region Utilities
        private string BuildWhereQuery(Dictionary<string, string> d)
        {
            string query = "";
            int count = d.Count;
            int i = 1;
            foreach (KeyValuePair<string, string> data in d)
            {
                query = query + data.Key + " = " + data.Value;
                if (i != count)
                {
                    query = query + " and ";
                }

                i++;

            }

            return query;
        }
        private void checkConn()
        {
            this.error = false;
            try
            {
                if (this.conn == null || this.conn.State == System.Data.ConnectionState.Closed)
                {
                    this.connectDB();
                }
            }
            catch (Exception ex)
            {
                this.connectDB();
                Log.SQLError(ex.Message);
            }

        }
        public SqlDataReader custom_query(string tablename, string query)
        {

            this.checkConn();
           
            SqlCommand cmd = new SqlCommand(query, this.conn);

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                return reader;
            }
            catch (Exception ex)
            {
                //Log to be implemented  

                Log.SQLError(ex.ToString(), false);

                //Setting Error
                this.error = true;
                return null;
            }
        }
        private string BuildInsertQuery(Dictionary<string, string> d)
        {
            string q = "";
            string fields = "(";
            string values = "(";

            int count = d.Count();
            int i = 1;

            foreach (KeyValuePair<string, string> data in d)
            {
                fields = fields + data.Key;
                values = values + data.Value;

                if (i != count)
                {
                    fields = fields + " , ";
                    values = values + " , ";
                }
                else
                {
                    fields = fields + ") ";
                    values = values + ") ";
                }
                i++;

            }

            q = fields + "values" + values;

            return q;
        }

        private string BuildUpdateQuery(Dictionary<string, string> d)
        {
            string q = "";

            int count = d.Count();
            int i = 1;

            foreach (KeyValuePair<string, string> data in d)
            {
                if (i < count)
                    q = data.Key + " = " + data.Value + "," + q;
                else
                    q = q + data.Key + " = " + data.Value;
                i++;
            }

            return q;
        }

        public SqlDataReader Search(string tablename, string SearchTerm, string SearchBy)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();

            res.Add(SearchBy, SearchTerm);
            SqlDataReader results = Program.db.get_where_custom(tablename, res);

            return results;


        }
        #endregion

        #region SelectQueries
        /// <summary>
        /// Produces select * from table and returns the Reader.
        /// </summary>
        /// <param name="tablename">The name of the table in the operation needs to be done.</param>
        /// <returns></returns>
        public SqlDataReader get(string tablename)
        {
            this.checkConn();
            q = "select * from " + tablename + " ;";
            SqlCommand cmd = new SqlCommand(q, this.conn);

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                return reader;
            }
            catch (Exception ex)
            {
                //Log to be implemented  

                Log.SQLError(ex.ToString(), false);

                //Setting Error
                this.error = true;
                return null;
            }

        }


        public SqlDataReader get_desc(string tablename)
        {
            this.checkConn();
            q = "select * from " + tablename + " order by Id desc ;";
            SqlCommand cmd = new SqlCommand(q, this.conn);

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                return reader;
            }
            catch (Exception ex)
            {
                //Log to be implemented  

                Log.SQLError(ex.ToString(), false);

                //Setting Error
                this.error = true;
                return null;
            }

        }

        public SqlDataReader get_with_limits(string tablename, string limit, string offset, string orderby)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the row from the table based on the Id
        /// </summary>
        /// <param name="tablename">The name of the table in which the operation must take place</param>
        /// <param name="id">The primary key field</param>
        /// <returns></returns>
        public SqlDataReader get_where(string tablename, string id)
        {
            q = "select * from " + tablename + " where Id=" + id + " ;";
            SqlCommand cmd = new SqlCommand(q, this.conn);

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                return reader;
            }
            catch (Exception ex)
            {
                //Log to be implemented  

                Log.SQLError(ex.ToString(), false);

                //Setting Error
                this.error = true;
                return null;
            }
        }

        /// <summary>
        /// This function takes a Dictionary and builds a custom where statement.
        /// viz. "select * from table where col1=val1 and col2=val2 and col3=val3" and so on.
        /// After Executing the query the function returns the SqlDataReader.
        /// </summary>
        /// <param name="tablename">Name of the Table</param>
        /// <param name="d">The string,string dictionary.</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader get_where_custom(string tablename, Dictionary<string, string> d)
        {

            //Parses the Dictionary and builds the where clause.
            try
            {
                this.checkConn();
                string whereclause = this.BuildWhereQuery(d);
                string q = "select *  from " + tablename + " where " + whereclause;
                SqlCommand cmd = new SqlCommand(q, this.conn);
                SqlDataReader reader = cmd.ExecuteReader();
                return reader;
            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);
                this.error = true;
                return null;
            }

        }


        public SqlDataReader get_where_custom_desc(string tablename, Dictionary<string, string> d)
        {

            //Parses the Dictionary and builds the where clause.
            try
            {
                this.checkConn();
                string whereclause = this.BuildWhereQuery(d);
                string q = "select *  from " + tablename + " where " + whereclause + " order by Id desc";
                SqlCommand cmd = new SqlCommand(q, this.conn);
                SqlDataReader reader = cmd.ExecuteReader();
                return reader;
            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);
                this.error = true;
                return null;
            }

        }



        #endregion

        #region insertQueries

        public Boolean insert(string tablename, Dictionary<string, string> data)
        {
            try
            {
                this.checkConn();
                string insertq = this.BuildInsertQuery(data);
                string q = "insert into " + tablename + " " + insertq;
                SqlCommand cmd = new SqlCommand(q, this.conn);
                int success = cmd.ExecuteNonQuery();
                if (success == 1)
                {
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
                this.error = true;
                return false;
            }
        }

        public int GetLastInsertedID(string tablename)
        {
            int ID = 0;
            try
            {
                this.checkConn();
                q = "select Id from " + tablename + " order by Id desc ;";
                SqlCommand cmd = new SqlCommand(q, this.conn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ID = Convert.ToInt32(reader["Id"]);
                        break;
                    }
                    reader.Close();
                    return ID;
                }
                else
                {
                    ID = -1;
                    return ID;
                }


            }
            catch (Exception ex)
            {
                Log.SQLError(ex.Message);
                return -1;
            }

        }
        #endregion

        #region UpdateQueries
        public bool update(string tablename, Dictionary<string, string> data, string whereClause)
        {
            try
            {
                this.checkConn();
                string updateq = this.BuildUpdateQuery(data);
                string q = "update " + tablename + " Set " + updateq + whereClause;
                SqlCommand cmd = new SqlCommand(q, this.conn);
                int success = cmd.ExecuteNonQuery();
                if (success >= 0)
                {
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
                this.error = true;
                return false;
            }
        }
        #endregion

        #region DeleteQueries
        public Boolean delete(string tablename, int Id)
        {
            try
            {
                this.checkConn();

                string q = "delete from  " + tablename + " where Id = " + Id;
                SqlCommand cmd = new SqlCommand(q, this.conn);
                int success = cmd.ExecuteNonQuery();
                if (success >= 0)
                {
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
                this.error = true;
                return false;
            }

        }

        /// <summary>
        /// This function takes a Dictionary and builds a custom where statement.
        /// viz. "delete from table where col1=val1 and col2=val2 and col3=val3" and so on.
        /// After Executing the query the function returns the SqlDataReader.
        /// </summary>
        /// <param name="tablename">Name of the Table</param>
        /// <param name="d">The string,string dictionary.</param>
        /// <returns>SqlDataReader</returns>
        public Boolean delete_where_custom(string tablename, Dictionary<string, string> d)
        {
            this.checkConn();
            //Parses the Dictionary and builds the where clause.
            string whereclause = this.BuildWhereQuery(d);
            string q = "delete  from " + tablename + " where " + whereclause;
            SqlCommand cmd = new SqlCommand(q, this.conn);
            int success = cmd.ExecuteNonQuery();
            if (success >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region SQLFuntions
        public SqlDataReader count_where(string tablename, List<string> col, string[] val)
        {
            throw new NotImplementedException();
        }

        public SqlDataReader count_all(string tablename)
        {
            throw new NotImplementedException();
        }

        public SqlDataReader get_max(string tablename, string col)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region StoredProcedure
        public Boolean CallProcedure(string ProcedureName)
        {
            SqlCommand cmd = new SqlCommand(ProcedureName, this.conn);

            //Specify that the SqlCommand is a stored procedure
            cmd.CommandType = System.Data.CommandType.StoredProcedure;


            //Open the connection and execute the query

            int affected = cmd.ExecuteNonQuery();
            if (affected == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public Boolean CallProcedure(string ProcedureName, Dictionary<string, string> input)
        {

            SqlCommand cmd = new SqlCommand(ProcedureName, this.conn);

            //Specify that the SqlCommand is a stored procedure
            cmd.CommandType = System.Data.CommandType.StoredProcedure;


            //Add the input parameters to the command object
            foreach (KeyValuePair<string, string> inp in input)
            {
                cmd.Parameters.AddWithValue(inp.Key, inp.Value);

            }
            int affected = cmd.ExecuteNonQuery();
            if (affected == 0)
            {
                return true;
            }
            else
            {
                return false;
            }



        }

        //public Boolean CallProcedure(string ProcedureName , List<string> input , List<string> output)
        //{

        //    //Add the input parameters to the command object
        //    cmd.Parameters.AddWithValue(“@Name”, txtEmployeeName.Text);
        //    cmd.Parameters.AddWithValue(“@Gender”, ddlGender.SelectedValue);
        //    cmd.Parameters.AddWithValue(“@Salary”, txtSalary.Text);

        //       //Add the output parameter to the command object
        //    SqlParameter outPutParameter = new SqlParameter();
        //    outPutParameter.ParameterName = “@EmployeeId”;
        //    outPutParameter.SqlDbType = System.Data.SqlDbType.Int;
        //    outPutParameter.Direction = System.Data.ParameterDirection.Output;
        //    cmd.Parameters.Add(outPutParameter);

        //}

        #endregion

        #region CustomQueries
        public SqlDataReader custom_reader_query(string query)
        {
            this.checkConn();
            SqlCommand cmd = new SqlCommand(query, this.conn);

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                return reader;
            }
            catch (Exception ex)
            {
                //Log to be implemented  

                Log.SQLError(ex.ToString(), false);

                //Setting Error
                this.error = true;
                return null;
            }
        }
        #endregion

    }
}
