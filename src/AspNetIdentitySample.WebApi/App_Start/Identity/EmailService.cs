using Microsoft.AspNet.Identity;
using SendGrid;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;

namespace AspNetIdentitySample.WebApi.Identity
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            return ConfigSendGridasync(message);
            //return SendMail(message);
        }

        // Implementação do SendGrid
        private Task ConfigSendGridasync(IdentityMessage message)
        {
            var myMessage = new SendGridMessage();
            myMessage.AddTo(message.Destination);
            myMessage.From = new MailAddress("admin@portal.com.br", "Admin do Portal");
            myMessage.Subject = message.Subject;
            myMessage.Html = message.Body;

            Web transportWeb;

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["SendGrid:ApiKey"]))
            {
                var credentials = new NetworkCredential(ConfigurationManager.AppSettings["SendGrid:Account"],
                                                    ConfigurationManager.AppSettings["SendGrid:Password"]);
                // Criar um transport web para envio de e-mail
                transportWeb = new Web(credentials);
            }
            else
            {
                // Criar um transport web para envio de e-mail
                transportWeb = new Web(ConfigurationManager.AppSettings["SendGrid:ApiKey"]);
            }

            // Enviar o e-mail
            if (transportWeb != null)
            {
                var x = transportWeb.DeliverAsync(myMessage);
                return x;
            }
            else
            {
                return Task.FromResult(0);
            }
        }

        // Implementação de e-mail manual
        private Task SendMail(IdentityMessage message)
        {
            if (ConfigurationManager.AppSettings["Internet"] == "true")
            {
                var text = HttpUtility.HtmlEncode(message.Body);

                var msg = new MailMessage();
                msg.From = new MailAddress("admin@portal.com.br", "Admin do Portal");
                msg.To.Add(new MailAddress(message.Destination));
                msg.Subject = message.Subject;
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Html));

                var smtpClient = new SmtpClient("smtp.provedor.com", Convert.ToInt32(587));
                var credentials = new NetworkCredential(ConfigurationManager.AppSettings["ContaDeEmail"],
                    ConfigurationManager.AppSettings["SenhaEmail"]);
                smtpClient.Credentials = credentials;
                smtpClient.EnableSsl = true;
                smtpClient.Send(msg);
            }

            return Task.FromResult(0);
        }
    }
}