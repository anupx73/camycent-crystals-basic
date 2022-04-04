using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Crystals.Models;
using System.Security.Cryptography;

namespace Crystals.Controllers
{
    class users
    {
        public static string umode = null;
        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public bool AddUser(string username, string password, string role)
        {

            
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("UserName", "'" + username + "'");

            SqlDataReader reader = Program.db.get_where_custom("users", data);
            if (reader.HasRows)
                return false;
            else
            {
                data.Add("Password", "'" + CalculateMD5Hash(password) + "'");
                data.Add("Role", "'" + role + "'");
                bool success = Program.db.insert("users", data);
                return success;
            }

        }

        public bool ChangePass(string userName, string oldPass, string newPass)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Username", "'" + userName + "'");
            SqlDataReader reader = Program.db.get_where_custom("users", data);
            data.Clear();
            reader.Read();

            string oldpass = reader["Password"].ToString();
            if (oldpass == CalculateMD5Hash(oldPass))
            {
                data.Add("Password", "'" + CalculateMD5Hash(newPass) + "'");
                string whereClause = " where UserName = '" + userName + "'";
                bool success = Program.db.update("users", data, whereClause);
                return success;
            }
            else
                return false;

        }
        public Boolean AuthUser(string username, string password, string role)
        {

            umode = role;
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("UserName", "'" + username + "'");
            data.Add("Password", "'" + CalculateMD5Hash(password) + "'");
            data.Add("Role", "'" + role + "'");

            SqlDataReader reader = Program.db.get_where_custom("users", data);
            if(!Program.db.error)
            {
                if (reader.HasRows)
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
