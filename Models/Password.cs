using System;
using System.Net;
using System.Net.Mail;
using ParkingProject.Models.DAL;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Text;

namespace ParkingProject.Models
{
    public class Password
    {
        public static int UpdatePassword(string mail, string currentPassword, string password1, string password2)
        {
            DataServices ds = new DataServices();
            return ds.UpdatePassword(mail, currentPassword, password1, password2);
        }

        public static bool SendinEmail(string recipients)
        {

            DataServices ds = new DataServices();
            string password = ds.ReadPaswword(recipients);
            string subject = "Your TempPassword";
            string content = "";
            string from = "CoParking@outlook.co.il";

            bool success = recipients != null;

            if (success)
            {
                SmtpClient gmailClient = new SmtpClient
                {
                    Host = "smtp-mail.outlook.com",
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential("CoParking@outlook.co.il", "I12345678!")               
                };

                using (MailMessage gMessage = new MailMessage(from, recipients, subject, content))
                {
                    var body = new StringBuilder();
                    body.AppendFormat("<html><head></head><body> Your TempPassword is <b>" + password + "</b><br />" + "<a href='{0}'>Click here to RESET</a></body></html>", "http://localhost:3000/ResetPassword");
                    gMessage.IsBodyHtml = true;
                    gMessage.Body = body.ToString();


                    gMessage.To.Add(recipients);

                    try
                    {
                        gmailClient.Send(gMessage);
                        success = true;
                    }
                    catch (Exception) { success = false; }
                }
            }
            return success;
        }
    }
}