using MetroFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crystals
{
    class Log
    {
        //static string Message;
        static string DBFile = "DBLOG.txt", 
            AppFile = "AppLOG.txt", 
            CommonFile = "CommonLOG.txt";

        static public void SQLError(string msg, bool MsgBox = false)
        {
            FileStream fileStream = null;
            StreamWriter writer = null;
            StringBuilder message = new StringBuilder();

            try
            {
                fileStream = new FileStream(DBFile, FileMode.OpenOrCreate,
                          FileAccess.Write);
                writer = new StreamWriter(fileStream);

                // Set the file pointer to the end of the file
                writer.BaseStream.Seek(0, SeekOrigin.End);

                // Create the message
                message.Append(System.DateTime.Now.ToString())
                   .Append("\t\t").Append(msg);

                // Force the write to the underlying file
                writer.WriteLine(message.ToString());
                writer.Flush();
                if (MsgBox == true)
                {
                    MessageBox.Show(msg);
                }
            }
            finally
            {
                if (writer != null) writer.Close();
            }

            GeneralError(msg);

            return;
        }
        static public void AppError(string msg, bool MsgBox = false)
        {
            FileStream fileStream = null;
            StreamWriter writer = null;
            StringBuilder message = new StringBuilder();

            try
            {
                fileStream = new FileStream(AppFile, FileMode.OpenOrCreate,
                          FileAccess.Write);
                writer = new StreamWriter(fileStream);

                // Set the file pointer to the end of the file
                writer.BaseStream.Seek(0, SeekOrigin.End);

                // Create the message
                message.Append(System.DateTime.Now.ToString())
                   .Append("\t\t").Append(msg);

                // Force the write to the underlying file
                writer.WriteLine(message.ToString());
                writer.Flush();
                if (MsgBox == true)
                {
                    MessageBox.Show(msg);
                }
            }
            finally
            {
                if (writer != null) writer.Close();
            }

            GeneralError(msg);
            
            return;
        }

        static public void GeneralError(string msg, bool MsgBox = false)
        {
            FileStream fileStream = null;
            StreamWriter writer = null;
            StringBuilder message = new StringBuilder();

            try
            {
                fileStream = new FileStream(CommonFile, FileMode.OpenOrCreate,
                          FileAccess.Write);
                writer = new StreamWriter(fileStream);

                // Set the file pointer to the end of the file
                writer.BaseStream.Seek(0, SeekOrigin.End);

                // Create the message
                message.Append(System.DateTime.Now.ToString())
                   .Append("\t\t").Append(msg);

                // Force the write to the underlying file
                writer.WriteLine(message.ToString());
                writer.Flush();
                if (MsgBox == true)
                {
                    MessageBox.Show(msg);
                }
            }
            finally
            {
                if (writer != null) writer.Close();
            }

           // EmailSender.Email("developers@camycent.com", message.ToString());
            return;
        }

        static public void AdminError(string msg, bool MsgBox = false)
        { 
            return; 
        }
    }

    class EmailSender
    {
        private static string serverAddress = "mail.camycent.com";
        private static int serverPort = 25;
        private static string mailUsername = "samiran.banerjee@camycent.com";
        private static string mailPassword = "Welcome123!@#";
        private static string mailDisplayName = "Camycent Solutions";
        private static string mailSubject = "Crystal";

        /// <summary>
        /// Send an email from [DELETED]
        /// </summary>
        /// <param name="to">Message to address</param>
        /// <param name="attachment">Attachment file path</param>
        public static void Email(string to, string msg)
        {
            try
            {
                // Build Email body structure
                MailMessage mail = new MailMessage();
                mail.Body = msg;
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(to));
                mail.From = new MailAddress(mailUsername, mailDisplayName, Encoding.UTF8);
                mail.Subject = mailSubject;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.Priority = MailPriority.Normal;

                // Build SMTP protocol structure
                SmtpClient smtp = new SmtpClient();
                smtp.Host = serverAddress;
                smtp.Port = serverPort;
                smtp.EnableSsl = false;
                smtp.Timeout = 10 * 1000;
                smtp.Credentials = new System.Net.NetworkCredential(mailUsername, mailPassword);

                // Send the Email message
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send email message with attachment, error: \r\n\r\n" + ex.ToString(),
                  "Sending failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}


