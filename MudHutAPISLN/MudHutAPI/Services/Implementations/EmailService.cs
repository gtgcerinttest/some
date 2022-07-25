using MudHutAPI.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MudHutAPI.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly ApiSettings _apiSettings;
        public EmailService(ApiSettings apiSettings)
        {
            _apiSettings = apiSettings;
        }

        public async Task<bool> SendRegistrationVerificationEmail(string to, string subject, string txtContent, string htmlContent)
        {
            try
            {                
                var toEmail = new EmailAddress(to, "Email to validate");
                await SendAPIEmail(toEmail, subject, txtContent, htmlContent);             
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region PrivateMethods

        private async Task<bool> SendAPIEmail(EmailAddress toEmail, string emailSubject, string txtContent = "", string htmlContent = "")
        {
            try
            {                
                SendGridClient _sendGridClient = new SendGridClient(_apiSettings.SendGridApiKey);
                var from = new EmailAddress(_apiSettings.NoReplyEmail, "MLT non-reply email");
                var to = toEmail;
                var subject = emailSubject;
                var msg = MailHelper.CreateSingleEmail(from, to, subject, txtContent, htmlContent);
                var response = await _sendGridClient.SendEmailAsync(msg);                

                if (response.IsSuccessStatusCode)
                {                 
                    return true;
                }
                else
                {             
                    throw new ApplicationException("Error Sending Email message");
                }
            }
            catch (Exception ex)
            {                
                throw;
            }
        }

        #endregion PrivateMethods
    }
}
