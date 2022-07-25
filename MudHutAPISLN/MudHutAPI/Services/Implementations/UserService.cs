using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using Google.Apis.Auth;
using System.Web;
using Microsoft.EntityFrameworkCore;
using MudHutAPI.Services.Interfaces;
using MudHutAPI.Models.DTOs;
using MudHutAPI.Models.Requests;

namespace MudHutAPI.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ApiSettings _apiSettings;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;



        public UserService(ApiSettings apiSettings, 
            UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager, IEmailService emailService)
        {
            _apiSettings = apiSettings;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }
        public async Task<UserDTO> RegisterUser(string Email, string Password)
        {

            try
            {
                var user = new UserDTO();
                var existinUser = await _userManager.FindByEmailAsync(Email);
                if (existinUser != null)
                {

                    throw new InvalidOperationException("Username already taken.");
                }

                var newUser = new IdentityUser
                {
                    Email = Email,
                    UserName = Email
                };
                var createdUser = await _userManager.CreateAsync(newUser, Password);
                if (!createdUser.Succeeded)
                {
                    var xptnMessage = "";
                    if (createdUser.Errors.Any())
                    {
                        xptnMessage = createdUser.Errors?.FirstOrDefault()?.Description;
                    }

                    throw new Exception("Failed to create user " + xptnMessage);
                }

                existinUser = await _userManager.FindByEmailAsync(Email);
                await _userManager.AddToRoleAsync(existinUser, "User");


                var code = await _userManager.GenerateEmailConfirmationTokenAsync(existinUser);
                var plainTextBytes = System.Text.Encoding.Default.GetBytes(code);
                var code64 = System.Convert.ToBase64String(plainTextBytes);


                var callbackUrl = _apiSettings.ConfirmEmailPageUrl + "?userId=" + existinUser.Id + "&code=" + code64;


                await _emailService.SendRegistrationVerificationEmail(
                    Email,
                    "Confirm your email",
                    $"Please confirm your account by {HtmlEncoder.Default.Encode(callbackUrl)}",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                user = new UserDTO()
                {
                    Id = existinUser.Id,
                    Username = existinUser.UserName,
                    FirstName = "",
                    LastName = "",
                    Role = "Teacher"
                };
               

                return user;
            }
            catch (Exception)
            {               
                throw;
            }
        }

        public async Task<bool> ConfirmEmailAccount(string userId, string code)
        {
            try
            {                
                var user = await _userManager.FindByIdAsync(userId);


                var base64EncodedBytes = System.Convert.FromBase64String(code);
                var decoded64 = System.Text.Encoding.Default.GetString(base64EncodedBytes);
                var result = await _userManager.ConfirmEmailAsync(user, decoded64);
             
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<UserDTO> Authenticate(AuthenticateRequest model)
        {
            try
            {
                var user = new UserDTO();                
                var existingUser = await _userManager.FindByEmailAsync(model.Username);
                // return null if user not found
                if (existingUser == null)
                {                 
                    return user;
                }

                if (existingUser.EmailConfirmed == false)
                {                   
                    return user;
                }

                var userHasValidPassword = await _userManager.CheckPasswordAsync(existingUser, model.Password);
                if (!userHasValidPassword)
                {                   
                    return user;
                }
                
                var Role = await _userManager.GetRolesAsync(existingUser);
                user = new UserDTO()
                {
                    Id = existingUser.Id,
                    Username = existingUser.UserName,
                    Role = (Role == null) ? "Not assigned" : Role.First()
                    //TODO: Load Additional user information
                };                
                return user;
            }
            catch (Exception ex)
            {                
                throw;
            }

        }

        public async Task<UserDTO> GetById(string id)
        {
            try
            {
                var user = new UserDTO();                
                var existinUser = await _userManager.FindByIdAsync(id);
                if (existinUser == null)
                {
                    return user;
                }
                
                var Role = await _userManager.GetRolesAsync(existinUser);
                user = new UserDTO()
                {
                    Id = existinUser.Id,
                    Username = existinUser.UserName,
                    //TODO: Load Additional user information
                    Role = (Role == null) ? "Not assigned" : Role.First()                  

                };                
                return user;
            }
            catch (Exception ex)
            {                
                throw;
            }
        }

    }
}
