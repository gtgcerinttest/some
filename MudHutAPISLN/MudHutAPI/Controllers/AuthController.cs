using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MudHutAPI.Helpers;
using MudHutAPI.Models.DTOs;
using MudHutAPI.Models.Requests;
using MudHutAPI.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace MudHutAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        private readonly ApiSettings _apiSettings;

        public AuthController(IUserService userService, ApiSettings apiSettings)
        {
            _userService = userService;
            _apiSettings = apiSettings;
        }


        [HttpPost("authenticate")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Authorizes the user", Description = "Authorizes the user and returns the JWT")]
        [SwaggerResponse(200, "Auth Success")]
        [SwaggerResponse(400, "Bad Request or invalid auth")]
        public async Task<IActionResult> Authenticate(AuthenticateRequest model)
        {            
            var userDTO = new UserDTO();
            try
            {
                userDTO = await _userService.Authenticate(model);
                if (userDTO != null && !string.IsNullOrEmpty(userDTO.Id))
                {
                    userDTO.Token = AuthHelper.GenerateJwtToken(userDTO.Id, _apiSettings.Secret, userDTO.Role);                    
                    return Ok(userDTO);
                }
                else
                {
                    return BadRequest("User Not Found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RegisterUser")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Register User", Description = "Registers a new User")]
        [SwaggerResponse(200, "Register Success")]
        [SwaggerResponse(400, "Bad Request or invalid registration")]
        public async Task<IActionResult> RegisterUser(string email, string pwd)
        {
            var userDTO = new UserDTO();
            try
            {
                userDTO = await _userService.RegisterUser(email, pwd);
                userDTO.Token = AuthHelper.GenerateJwtToken(userDTO.Id, _apiSettings.Secret, userDTO.Role);
                return Ok(userDTO);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpPost("ConfirmEmailAccount")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Confirm Email Account", Description = "Confirm Email Account")]
        [SwaggerResponse(200, "Confirm Email Account Success")]
        [SwaggerResponse(400, "Bad Request")]
        public async Task<IActionResult> ConfirmEmailAccount([FromBody] ConfirmAccountRequest model)
        {

            var userDto = new UserDTO();
            try
            {                
                if (model.userId == null || model.code == null)
                {
             
                    return BadRequest("Bad Request");
                }

                var user = await _userService.GetById(model.userId);
                if (user == null)
                {
                    
                    return BadRequest("Unable to load user.");
                }

                var result = await _userService.ConfirmEmailAccount(model.userId, model.code);
                if (result)
                {                    
                    return Ok(result);
                }
                else
                {                 
                    return BadRequest("Unable Confirm email.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }


}
