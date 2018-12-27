using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace TechDevs.Mail
{
    public class SMTPSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public interface IEmailer
    {
        Task SendSecurityEmail(string toAddress, string subject, string body, bool isBodyHtml);
        Task SendSupportEmail(string toAddress, string subject, string body, bool isBodyHtml);
    }

    public class DotNetEmailer: IEmailer
    {
        private SMTPSettings _settings;

        public DotNetEmailer(IOptions<SMTPSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendSecurityEmail(string toAddress, string subject, string body, bool isBodyHtml)
        {
            var securityAdd = new MailAddress("security@techdevs.net", "TechDevs Security");
            await SendEmail(securityAdd, toAddress, subject, body, isBodyHtml);
        }

        public async Task SendSupportEmail(string toAddress, string subject, string body, bool isBodyHtml)
        {
            var supportAdd = new MailAddress("support@techdevs.net", "TechDevs Support");
            await SendEmail(supportAdd, toAddress, subject, body, isBodyHtml);
        }

        private async Task SendEmail(MailAddress from, string toAddress, string subject, string body, bool isBodyHtml)
        {
            var creds = new NetworkCredential(_settings.Username, _settings.Password);
            using (var client = new SmtpClient(_settings.Host, _settings.Port))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = creds;
                client.EnableSsl = true;

                var msg = new MailMessage();
                msg.To.Add(new MailAddress(toAddress));
                msg.From = from;
                msg.Subject = subject;
                msg.Body = body;
                msg.IsBodyHtml = isBodyHtml;

                try
                {
                    await client.SendMailAsync(msg);
                }   
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }
    }
}
