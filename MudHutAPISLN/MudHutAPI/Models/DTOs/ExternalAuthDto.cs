namespace MudHutAPI.Models.DTOs
{
    public class ExternalAuthDto
    {
        public string Provider { get; set; } = String.Empty;
        public string IdToken { get; set; } = String.Empty;
    }
}
