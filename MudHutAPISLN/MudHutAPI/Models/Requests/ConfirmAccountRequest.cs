namespace MudHutAPI.Models.Requests
{
    public class ConfirmAccountRequest
    {
        public string userId { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
    }
}
