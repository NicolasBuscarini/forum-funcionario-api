using System.Net.Mail;
using System.Net;
using ForumFuncionario.Api.Service.Interface;

namespace ForumFuncionario.Api.Service
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Sends an email using SMTP.
        /// </summary>
        /// <param name="to">The email address of the recipient.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body of the email.</param>
        /// <returns>True if the email was successfully sent; otherwise, false.</returns>
        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                // Retrieve SMTP settings from appsettings.json
                var smtpUser = _configuration["SmtpSettings:User"];
                var smtpPass = _configuration["SmtpSettings:Password"];
                var smtpServer = _configuration["SmtpSettings:Server"];
                var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]!);
                var enableSsl = bool.Parse(_configuration["SmtpSettings:EnableSsl"]!);

                // Email message configuration
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                // SMTP client configuration
                var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = enableSsl,
                    Timeout = 60000, // 60 seconds timeout
                    UseDefaultCredentials = false
                };

                // Send the email
                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent to: {to}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to: {to}");
                return false;
            }
        }
    }
}
