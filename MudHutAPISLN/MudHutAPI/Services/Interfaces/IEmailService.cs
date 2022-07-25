namespace MudHutAPI.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendRegistrationVerificationEmail(string to, string subject, string txtContent, string htmlContent);
    }
}
