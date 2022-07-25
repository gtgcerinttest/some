namespace MudHutAPI.Models.DTOs;
using System.Text.Json.Serialization;

public class UserDTO
{

    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; }   = string.Empty;
    

    [JsonIgnore]
    public string Password { get; set; } = string.Empty;
}