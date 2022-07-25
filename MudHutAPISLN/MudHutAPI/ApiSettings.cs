namespace MudHutAPI
{
    public class ApiSettings
    {
        public string Secret { get; set; } = string.Empty;
        public bool UseCache { get; set; }
        public string SendGridApiKey { get; set; } = string.Empty;
        public string ConfirmEmailPageUrl { get; set; } = string.Empty;
        public string NoReplyEmail { get; set; } = string.Empty;
        

    }
}
