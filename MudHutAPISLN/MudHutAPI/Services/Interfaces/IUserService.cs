using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using MudHutAPI.Models.DTOs;
using MudHutAPI.Models.Requests;

namespace MudHutAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> Authenticate(AuthenticateRequest model);        
        Task<UserDTO> GetById(string id);
        Task<UserDTO> RegisterUser(string Email, string Password);
        Task<bool> ConfirmEmailAccount(string userId, string code);
    }
}
