using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Data {
    public class Mail {
        public static void send(string to, string from, string subject, string message, Boolean html, string host) {
            // see if we have credentials in web.config
            string smtpUser = "";
            string smtpPass = "";
            try {
                smtpUser = ConfigurationManager.AppSettings["smtpUser"].ToString();
                smtpPass = ConfigurationManager.AppSettings["smtpPass"].ToString();
            } catch (Exception xx) { }

            MailMessage msg = new MailMessage();
            msg.IsBodyHtml = html;
            msg.To.Add(to);
            msg.From = new MailAddress(from);
            msg.Subject = subject;
            msg.Body = message;
            SmtpClient smtp = new SmtpClient(host);
            if (smtpUser.Length > 0) {
                smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
            }            
            smtp.Send(msg);

        }
        public static void send(string to, string from, string subject, string message) {
            send(to, from, subject, message, false, "localhost");
        }
    }
}