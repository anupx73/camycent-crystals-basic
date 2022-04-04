using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Crystals.Controllers
{
    class AdminNotify
    {
        public static void SendEmail(string subject, string msg)
        {
            try
            {
                SqlDataReader reader =  Program.db.get("Company");
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                if(reader.HasRows)
                {
                    reader.Read();
                    message.From = new MailAddress("crystals.spa2233@gmail.com");
                    message.To.Add(new MailAddress(reader["AdminEmail"].ToString()));
                    message.Subject = reader["Name"].ToString() +":"+ subject;
                    message.Body = msg;

                    smtp.Port = 587;
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("crystals.spa2233@gmail.com", "Test123!@#");
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(message);

                }


               
            }
            catch (Exception ex)
            {
                Log.AppError(ex.Message);
            }
        }
    }
}
