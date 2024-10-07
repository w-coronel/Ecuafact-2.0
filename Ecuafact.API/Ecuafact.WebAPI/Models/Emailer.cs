using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Ecuafact.WebAPI.Models
{
    internal class Emailer
    {
        public static Emailer Create(string filename, string subject, string recipients) { return new Emailer(filename, subject, recipients); }

        public Emailer(string filename, string subject, string recipients)
        {
            Filename = filename;
            Subject = subject;

            var addresses = recipients?.Split(',', ';', ' ');
            if (addresses != null)
                foreach (var item in addresses)
                {
                    if (!string.IsNullOrEmpty(item) && item.Contains("@"))
                    {
                        Recipients.Add(new MailAddress(item));
                    }
                }
        }

        public MailMessage Message { get; private set; } = new MailMessage
        {
            From = new MailAddress(Constants.Emailing.SenderAddress, Constants.Emailing.SenderName),
            IsBodyHtml = true
        };

        public string _filename;
        public string Filename
        {
            get
            {
                return Path.GetFileName(_filename);
            }
            set
            {
                _filename = Path.Combine(Constants.ServerPath, "Email", $"{value}.html");

                if (File.Exists(_filename))
                {
                    Template = File.ReadAllText(_filename);
                }
            }
        }

        public string Template { get; set; }
        public string Subject { get { return Message?.Subject; } set { Message.Subject = value; } }
        public AttachmentCollection Attachments { get { return Message?.Attachments; } }
        public MailAddressCollection Recipients { get { return Message?.To; } }
        public NameValueCollection Parameters { get; set; } = new NameValueCollection();

        public bool Send()
        {
            try
            {
                if (Recipients.Count == 0)
                {
                    throw new Exception($"Las direcciones de correo del destinatario: {Recipients.FirstOrDefault()} son inválidas.");
                }

                Message.Body = Template;
                
                foreach (var key in Parameters.AllKeys)
                {
                    Message.Body = Message.Body.Replace("{" + key.ToUpper() + "}", Parameters[key]);
                    Message.Body = Message.Body.Replace("[" + key + "]", Parameters[key]);
                } 
                
                var smtp = new SmtpClient
                {
                    Port = Constants.Emailing.Port,
                    Host = Constants.Emailing.SmtpServer,
                    EnableSsl = Constants.Emailing.EnableSSL,
                    UseDefaultCredentials = Constants.Emailing.DefaultCredentials,
                    Credentials = new NetworkCredential(Constants.Emailing.Username, Constants.Emailing.Password),
                    DeliveryMethod = SmtpDeliveryMethod.Network                   
                };
                
                smtp.Send(Message);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"EMAILING.SENDMAIL.{Message.To.FirstOrDefault()}",
                    "Asunto:", Message.Subject, "Mensaje:", Message.Body, "Excepcion: ", ex);
            
                return false;
            }
            
        }
    }
}